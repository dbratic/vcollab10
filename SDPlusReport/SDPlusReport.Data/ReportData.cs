using Npgsql;

using SDPlusReport.Entity;

using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SDPlusReport.Data
{
    public class ReportData
    {
        public List<Report> GetReportData(string dbConnection, ReportFilter objRpt, string loginName, string fullName)
        {
            List<Report> lstReportData = new List<Report>();

            using NpgsqlConnection connection = new NpgsqlConnection(dbConnection);
            try
            {
                connection.Open();
                StringBuilder strQuery = new StringBuilder();

                strQuery.Append("SELECT CHARGESTABLE.CHARGEID AS WorkLogId,CHARGESTABLE.TECHNICIANID AS UserId,TECH.FIRST_NAME AS UserName,");
                strQuery.Append("ACC.EMAILID AS UserEmail,SD.EMPLOYEEID AS EmploymentType,DEPTTABLE.DEPTNAME AS DepartmentName,CISITE.Name AS Location,NBRC.ISBILLABLEWORKLOG AS IsBillable,BS.BILLINGSTATUSNAME AS BillingStatus,");
                strQuery.Append("TO_TIMESTAMP(CHARGESTABLE.TS_STARTTIME / 1000) AS RecordStartDate,");
                strQuery.Append("TO_TIMESTAMP(CHARGESTABLE.TS_ENDTIME / 1000) AS RecordEndDate,");
                strQuery.Append("TO_TIMESTAMP(CHARGESTABLE.CREATEDTIME / 1000) AS WorkLogCreatedTime,");
                strQuery.Append("ROUND((CHARGESTABLE.TIMESPENT / 60000) / 60.0, 2) AS TimeSpentHrs,");
                strQuery.Append("CASE WHEN PAD.ORG_ID IS NOT NULL THEN PAD.ORG_ID WHEN WAD.ORG_ID IS NOT NULL THEN WAD.ORG_ID WHEN CAD.ORG_ID IS NOT NULL THEN CAD.ORG_ID WHEN PBAD.ORG_ID IS NOT NULL THEN PBAD.ORG_ID WHEN TAD.ORG_ID IS NOT NULL THEN TAD.ORG_ID END AS AccountId,");
                strQuery.Append("CASE WHEN PAD.ORG_ID IS NOT NULL THEN PAD.ORG_NAME WHEN WAD.ORG_ID IS NOT NULL THEN WAD.ORG_NAME WHEN CAD.ORG_ID IS NOT NULL THEN CAD.ORG_NAME WHEN PBAD.ORG_ID IS NOT NULL THEN PBAD.ORG_NAME WHEN TAD.ORG_ID IS NOT NULL THEN TAD.ORG_NAME END AS AccountName,");
                strQuery.Append("CASE WHEN PAD.ORG_ID IS NOT NULL THEN PACI.udf_long2 WHEN WAD.ORG_ID IS NOT NULL THEN WACI.udf_long2 WHEN CAD.ORG_ID IS NOT NULL THEN CACI.udf_long2 WHEN PBAD.ORG_ID IS NOT NULL THEN PBACI.udf_long2 WHEN TAD.ORG_ID IS NOT NULL THEN TACI.udf_long2 END AS ClientId,");
                strQuery.Append("CASE WHEN PAD.ORG_ID IS NOT NULL THEN PACI.udf_char5 WHEN WAD.ORG_ID IS NOT NULL THEN WACI.udf_char5 WHEN CAD.ORG_ID IS NOT NULL THEN CACI.udf_char5 WHEN PBAD.ORG_ID IS NOT NULL THEN PBACI.udf_char5 WHEN TAD.ORG_ID IS NOT NULL THEN TACI.udf_char5 END AS ClientName,");
                strQuery.Append("CASE WHEN PAD.ORG_ID IS NOT NULL THEN PACI.udf_long1 WHEN WAD.ORG_ID IS NOT NULL THEN WACI.udf_long1 WHEN CAD.ORG_ID IS NOT NULL THEN CACI.udf_long1 WHEN PBAD.ORG_ID IS NOT NULL THEN PBACI.udf_long1 WHEN TAD.ORG_ID IS NOT NULL THEN TACI.udf_long1 END AS VCOldProjectId,");
                if (objRpt.ReportType == "DetailedM")
                {
                    strQuery.Append("CASE WHEN PD.PROJECTID IS NOT NULL THEN 'Project' WHEN WO.WORKORDERID IS NOT NULL THEN 'Request' WHEN CD.CHANGEID IS NOT NULL THEN 'Change' WHEN P.PROBLEMID IS NOT NULL THEN 'Problem' ELSE 'Task' END AS Module,");
                    strQuery.Append("CASE WHEN PD.PROJECTID IS NOT NULL THEN PD.PROJECTID WHEN CD.CHANGEID IS NOT NULL THEN CD.CHANGEID WHEN P.PROBLEMID IS NOT NULL THEN P.PROBLEMID ELSE COALESCE(WO.WORKORDERID,TD.TASKID) END AS ModuleId,");
                    strQuery.Append("CASE WHEN PD.PROJECTID IS NOT NULL THEN PD.TITLE WHEN CD.CHANGEID IS NOT NULL THEN CD.TITLE WHEN P.PROBLEMID IS NOT NULL THEN P.TITLE ELSE WO.TITLE END AS ModuleTitleOrSubject,");
                    strQuery.Append("CASE WHEN WO.WORKORDERID IS NOT NULL THEN RTD.NAME ELSE 'NA' END AS RequestType,");
                    strQuery.Append("COALESCE(TD.TASKID,WO.WORKORDERID) AS TaskId,");
                    strQuery.Append("COALESCE(TD.TITLE) AS TaskTitle,");
                }
                else
                {
                    strQuery.Append("CASE WHEN PD.PROJECTID IS NOT NULL THEN 'Project' WHEN WO.WORKORDERID IS NOT NULL THEN 'Request' WHEN CD.CHANGEID IS NOT NULL THEN 'Change' WHEN P.PROBLEMID IS NOT NULL THEN 'Problem' ELSE 'Task' END AS Module,");
                    strQuery.Append("CASE WHEN PD.PROJECTID IS NOT NULL THEN PD.PROJECTID WHEN CD.CHANGEID IS NOT NULL THEN CD.CHANGEID WHEN P.PROBLEMID IS NOT NULL THEN P.PROBLEMID ELSE TD.TASKID END AS ModuleId,");
                    strQuery.Append("CASE WHEN PD.PROJECTID IS NOT NULL THEN PD.TITLE WHEN CD.CHANGEID IS NOT NULL THEN CD.TITLE WHEN P.PROBLEMID IS NOT NULL THEN P.TITLE WHEN TD.TITLE IS NOT NULL THEN TD.TITLE ELSE 'NA' END AS ModuleTitleOrSubject,");
                    strQuery.Append("CASE WHEN WO.WORKORDERID IS NOT NULL THEN RTD.NAME ELSE 'NA' END AS RequestType,");
                    strQuery.Append("CASE WHEN WO.WORKORDERID IS NOT NULL THEN WO.WORKORDERID WHEN TTC.TASKID IS NOT NULL THEN TTC.TASKID WHEN PTC.PROBLEMID IS NOT NULL THEN PTC.PROBLEMID ELSE TD.TASKID END AS TaskId,");
                    strQuery.Append("CASE WHEN WO.WORKORDERID IS NOT NULL THEN WO.TITLE WHEN TTC.TASKID IS NOT NULL THEN TD.TITLE WHEN P.TITLE IS NOT NULL THEN P.TITLE ELSE TD.TITLE END AS TaskTitle,");
                }
                strQuery.Append("CHARGESTABLE.SHORT_DESCRIPTION AS WorkLogDescription, CREATOR.FIRST_NAME AS CreatedBy  ");
                strQuery.Append("FROM CHARGESTABLE CHARGESTABLE ");
                strQuery.Append("LEFT JOIN WORKLOGBILLINGSTATUS WBS ON WBS.CHARGEID = CHARGESTABLE.CHARGEID ");
                strQuery.Append("LEFT JOIN NOBILLREQUESTCHARGES NBRC ON NBRC.REQUESTCHARGEID = CHARGESTABLE.CHARGEID ");
                strQuery.Append("LEFT JOIN BILLINGSTATUS BS ON BS.BILLINGSTATUSID = WBS.BILLINGSTATUS ");
                strQuery.Append("LEFT JOIN AAAUSER TECH ON TECH.USER_ID = CHARGESTABLE.TECHNICIANID ");
                strQuery.Append("LEFT JOIN AAAUSERCONTACTINFO AUCC ON AUCC.USER_ID = TECH.USER_ID ");
                strQuery.Append("LEFT JOIN AAACONTACTINFO ACC ON ACC.CONTACTINFO_ID = AUCC.CONTACTINFO_ID ");
                strQuery.Append("LEFT JOIN TASKTOCHARGE TTC ON TTC.CHARGEID = CHARGESTABLE.CHARGEID ");
                strQuery.Append("LEFT JOIN TASKTOPROJECTS TTP ON TTP.TASKID = TTC.TASKID ");
                strQuery.Append("LEFT JOIN TASKDETAILS TD ON TD.TASKID = TTC.TASKID ");
                strQuery.Append("LEFT JOIN PROJECTDETAILS PD ON PD.PROJECTID = TTP.PROJECTID ");
                strQuery.Append("LEFT JOIN PROJECTACCMAPPING PAM ON PAM.PROJECTID = PD.PROJECTID ");
                strQuery.Append("LEFT JOIN ACCOUNTDEFINITION PAD ON PAD.ORG_ID = PAM.ACCOUNTID ");
                strQuery.Append("LEFT JOIN PORTALACCOUNTS PPA ON PAD.ORG_ID = PPA.ACCOUNTID ");
                strQuery.Append("LEFT JOIN ACCOUNTADDITIONALFIELDS PACI ON PPA.ACCOUNTID = PACI.Instance_Id ");
                strQuery.Append("LEFT JOIN WORKORDERTOCHARGE WTC ON WTC.CHARGEID = CHARGESTABLE.CHARGEID ");
                strQuery.Append("LEFT JOIN WORKORDER WO ON WO.WORKORDERID = WTC.WORKORDERID ");
                strQuery.Append("LEFT JOIN WORKORDERSTATES WOS ON WO.WORKORDERID = WOS.WORKORDERID ");
                strQuery.Append("LEFT JOIN REQUESTTYPEDEFINITION RTD ON WOS.REQUESTTYPEID = RTD.REQUESTTYPEID ");
                strQuery.Append("LEFT JOIN WORKORDERACCOUNTMAPPING WAM ON WAM.WORKORDERID = WO.WORKORDERID ");
                strQuery.Append("LEFT JOIN ACCOUNTDEFINITION WAD ON WAD.ORG_ID = WAM.ACCOUNTID ");
                strQuery.Append("LEFT JOIN PORTALACCOUNTS WPA ON WAD.ORG_ID = WPA.ACCOUNTID ");
                strQuery.Append("LEFT JOIN ACCOUNTADDITIONALFIELDS WACI ON WPA.ACCOUNTID = WACI.Instance_Id ");
                strQuery.Append("LEFT JOIN CHANGETOCHARGE CTC ON CTC.CHARGEID = CHARGESTABLE.CHARGEID ");
                strQuery.Append("LEFT JOIN CHANGEDETAILS CD ON CD.CHANGEID = CTC.CHANGEID ");
                strQuery.Append("LEFT JOIN CHANGEACCOUNTMAPPING CAM ON CAM.CHANGEID = CD.CHANGEID ");
                strQuery.Append("LEFT JOIN ACCOUNTDEFINITION CAD ON CAD.ORG_ID = CAM.ACCOUNTID ");
                strQuery.Append("LEFT JOIN PORTALACCOUNTS CPA ON CAD.ORG_ID = CPA.ACCOUNTID ");
                strQuery.Append("LEFT JOIN ACCOUNTADDITIONALFIELDS CACI ON CPA.ACCOUNTID = CACI.Instance_Id ");
                strQuery.Append("LEFT JOIN PROBLEMTOCHARGE PTC ON PTC.CHARGEID = CHARGESTABLE.CHARGEID ");
                strQuery.Append("LEFT JOIN PROBLEM P ON P.PROBLEMID = PTC.PROBLEMID ");
                strQuery.Append("LEFT JOIN PROBLEMACCOUNTMAPPING PBAM ON PBAM.PROBLEMID = P.PROBLEMID ");
                strQuery.Append("LEFT JOIN ACCOUNTDEFINITION PBAD ON PBAD.ORG_ID = PBAM.ACCOUNTID ");
                strQuery.Append("LEFT JOIN PORTALACCOUNTS PBPA ON PBAD.ORG_ID = PBPA.ACCOUNTID ");
                strQuery.Append("LEFT JOIN ACCOUNTADDITIONALFIELDS PBACI ON PBPA.ACCOUNTID = PBACI.Instance_Id ");
                strQuery.Append("LEFT JOIN TASKACCOUNTMAPPING TAM ON TAM.TASKID = TD.TASKID ");
                strQuery.Append("LEFT JOIN ACCOUNTDEFINITION TAD ON TAD.ORG_ID = TAM.ACCOUNTID ");
                strQuery.Append("LEFT JOIN PORTALACCOUNTS TAPA ON TAPA.ACCOUNTID = TAD.ORG_ID ");
                strQuery.Append("LEFT JOIN ACCOUNTADDITIONALFIELDS TACI ON TACI.Instance_Id = TAPA.ACCOUNTID ");
                strQuery.Append("LEFT JOIN USERDEPARTMENT USERDEPT ON TECH.USER_ID = USERDEPT.USERID  ");
                strQuery.Append("LEFT JOIN DEPARTMENTDEFINITION DEPTTABLE ON USERDEPT.DEPTID = DEPTTABLE.DEPTID ");
                strQuery.Append("LEFT JOIN SDOrganization CISITE ON DEPTTABLE.SITEID = CISITE.ORG_ID ");
                strQuery.Append("LEFT JOIN SDUser SD ON SD.USERID = CHARGESTABLE.TECHNICIANID ");
                strQuery.Append("LEFT JOIN AAAUSER CREATOR ON CREATOR.USER_ID = CHARGESTABLE.CREATEDBY ");

                strQuery.Append("WHERE '[" + objRpt.RecordStartDate.ToString("yyyy-MM-dd") + ", " + objRpt.RecordEndDate.ToString("yyyy-MM-dd") + "]'::daterange @> to_timestamp(CAST(chargesTable.ts_endtime as bigint)/1000)::date");
                //-------------------------
                //Add Filters
                //-------------------------
                strQuery.Append(StandardQueryFilter(dbConnection, objRpt, loginName, fullName));

                strQuery.Append(" ORDER BY chargesTable.ts_starttime");
                //var txtQuery= strQuery.ToString();

                using NpgsqlCommand cmd = new NpgsqlCommand(strQuery.ToString(), connection);
                using NpgsqlDataReader reader = cmd.ExecuteReader();

                TimeZoneInfo easternTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

                while (reader.Read())
                {
                    var objReport = new Report();
                    objReport.WorkLogId = Convert.ToInt32(reader["WorkLogId"]);
                    objReport.UserId = Convert.ToInt32(reader["UserId"]);
                    objReport.UserName = (string)reader["UserName"];
                    objReport.UserEmail = (string)(reader["UserEmail"] is DBNull ? "" : reader["UserEmail"]);
                    objReport.EmploymentType = (string)(reader["EmploymentType"] is DBNull ? "" : reader["EmploymentType"]);
                    objReport.DepartmentName = (string)(reader["DepartmentName"] is DBNull ? "" : reader["DepartmentName"]);
                    objReport.Location = (string)(reader["Location"] is DBNull ? "" : reader["Location"]);
                    objReport.IsBillable = (bool)reader["IsBillable"];
                    objReport.BillingStatus = (string)reader["BillingStatus"];
                    objReport.RecordStartDate = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(reader["RecordStartDate"]), easternTimeZone).ToString("yyyy'-'MMM'-'dd").Replace(".", "");
                    objReport.RecordEndDate = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(reader["RecordEndDate"]), easternTimeZone).ToString("yyyy'-'MMM'-'dd").Replace(".", "");
                    objReport.WorkLogCreatedDate = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(reader["WorkLogCreatedTime"]), easternTimeZone).ToString("yyyy'-'MMM'-'dd").Replace(".", "");
                    objReport.WorkLogCreatedTime = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(reader["WorkLogCreatedTime"]), easternTimeZone).ToString("HH:mm:ss");
                    objReport.TimeSpentHrs = Convert.ToDecimal(reader["TimeSpentHrs"]);
                    objReport.AccountId = (reader["AccountId"] is DBNull ? "" : reader["AccountId"].ToString());
                    objReport.AccountName = (string)(reader["AccountName"] is DBNull ? "" : reader["AccountName"]);
                    objReport.ClientId = (reader["ClientId"] is DBNull ? "" : reader["ClientId"].ToString());
                    objReport.ClientName = (string)(reader["ClientName"] is DBNull ? "" : reader["ClientName"]);
                    objReport.VcOldProjectId = (reader["VcOldProjectId"] is DBNull ? "" : reader["VcOldProjectId"].ToString());
                    objReport.Module = (string)(reader["Module"] is DBNull ? "" : reader["Module"]);
                    objReport.ModuleId = (reader["ModuleId"] is DBNull ? "" : reader["ModuleId"].ToString());
                    objReport.ModuleTitle = (string)(reader["ModuleTitleOrSubject"] is DBNull ? "" : reader["ModuleTitleOrSubject"]);
                    objReport.RequestType = (string)(reader["RequestType"] is DBNull ? "" : reader["RequestType"]);
                    objReport.TaskId = (reader["TaskId"] is DBNull ? "" : reader["TaskId"].ToString());
                    objReport.TaskTitle = (string)(reader["TaskTitle"] is DBNull ? "" : reader["TaskTitle"]);
                    objReport.WorkLogDescription = (string)(reader["WorkLogDescription"] is DBNull ? "" : reader["WorkLogDescription"]);
                    objReport.CreatedBy = (string)(reader["CreatedBy"] is DBNull ? "" : reader["CreatedBy"]);
                    lstReportData.Add(objReport);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetReportData(): " + ex.ToString());
            }
            finally
            {
                connection.Close();
            }
            return [.. lstReportData];
        }

        public List<HoursByDay> GetReportDataHoursByDay(string dbConnection, ReportFilter objRpt, string loginName, string fullName)
        {
            List<HoursByDay> lstReportData = new List<HoursByDay>();

            using NpgsqlConnection connection = new NpgsqlConnection(dbConnection);
            try
            {
                connection.Open();
                StringBuilder strQuery = new StringBuilder();
                //strQuery.Append("SELECT to_timestamp(CAST(chargesTable.ts_endtime as bigint)/1000)::date AS RecordDate, SUM(ROUND((CHARGESTABLE.TIMESPENT / 60000) / 60.0, 2)) AS TotalHours ");
                //strQuery.Append("FROM CHARGESTABLE CHARGESTABLE ");
                //strQuery.Append("LEFT JOIN WORKLOGBILLINGSTATUS WBS ON WBS.CHARGEID = CHARGESTABLE.CHARGEID ");
                //strQuery.Append("LEFT JOIN NOBILLREQUESTCHARGES NBRC ON NBRC.REQUESTCHARGEID = CHARGESTABLE.CHARGEID ");
                //strQuery.Append("LEFT JOIN BILLINGSTATUS BS ON BS.BILLINGSTATUSID = WBS.BILLINGSTATUS ");
                //strQuery.Append("LEFT JOIN AAAUSER TECH ON TECH.USER_ID = CHARGESTABLE.TECHNICIANID ");
                //strQuery.Append("LEFT JOIN AAAUSERCONTACTINFO AUCC ON AUCC.USER_ID = TECH.USER_ID ");
                //strQuery.Append("LEFT JOIN AAACONTACTINFO ACC ON ACC.CONTACTINFO_ID = AUCC.CONTACTINFO_ID ");
                //strQuery.Append("LEFT JOIN TASKTOCHARGE TTC ON TTC.CHARGEID = CHARGESTABLE.CHARGEID ");
                //strQuery.Append("LEFT JOIN TASKTOPROJECTS TTP ON TTP.TASKID = TTC.TASKID ");
                //strQuery.Append("LEFT JOIN TASKDETAILS TD ON TD.TASKID = TTC.TASKID ");
                //strQuery.Append("LEFT JOIN PROJECTDETAILS PD ON PD.PROJECTID = TTP.PROJECTID ");
                //strQuery.Append("LEFT JOIN PROJECTACCMAPPING PAM ON PAM.PROJECTID = PD.PROJECTID ");
                //strQuery.Append("LEFT JOIN ACCOUNTDEFINITION PAD ON PAD.ORG_ID = PAM.ACCOUNTID ");
                //strQuery.Append("LEFT JOIN PORTALACCOUNTS PPA ON PAD.ORG_ID = PPA.ACCOUNTID ");
                //strQuery.Append("LEFT JOIN ACCOUNTCI PACI ON PPA.CIID = PACI.CIID ");
                //strQuery.Append("LEFT JOIN WORKORDERTOCHARGE WTC ON WTC.CHARGEID = CHARGESTABLE.CHARGEID ");
                //strQuery.Append("LEFT JOIN WORKORDER WO ON WO.WORKORDERID = WTC.WORKORDERID ");
                //strQuery.Append("LEFT JOIN WORKORDERSTATES WOS ON WO.WORKORDERID = WOS.WORKORDERID ");
                //strQuery.Append("LEFT JOIN REQUESTTYPEDEFINITION RTD ON WOS.REQUESTTYPEID = RTD.REQUESTTYPEID ");
                //strQuery.Append("LEFT JOIN WORKORDERACCOUNTMAPPING WAM ON WAM.WORKORDERID = WO.WORKORDERID ");
                //strQuery.Append("LEFT JOIN ACCOUNTDEFINITION WAD ON WAD.ORG_ID = WAM.ACCOUNTID ");
                //strQuery.Append("LEFT JOIN PORTALACCOUNTS WPA ON WAD.ORG_ID = WPA.ACCOUNTID ");
                //strQuery.Append("LEFT JOIN ACCOUNTCI WACI ON WPA.CIID = WACI.CIID ");
                //strQuery.Append("LEFT JOIN CHANGETOCHARGE CTC ON CTC.CHARGEID = CHARGESTABLE.CHARGEID ");
                //strQuery.Append("LEFT JOIN CHANGEDETAILS CD ON CD.CHANGEID = CTC.CHANGEID ");
                //strQuery.Append("LEFT JOIN CHANGEACCOUNTMAPPING CAM ON CAM.CHANGEID = CD.CHANGEID ");
                //strQuery.Append("LEFT JOIN ACCOUNTDEFINITION CAD ON CAD.ORG_ID = CAM.ACCOUNTID ");
                //strQuery.Append("LEFT JOIN PORTALACCOUNTS CPA ON CAD.ORG_ID = CPA.ACCOUNTID ");
                //strQuery.Append("LEFT JOIN ACCOUNTCI CACI ON CPA.CIID = CACI.CIID ");
                //strQuery.Append("LEFT JOIN PROBLEMTOCHARGE PTC ON PTC.CHARGEID = CHARGESTABLE.CHARGEID ");
                //strQuery.Append("LEFT JOIN PROBLEM P ON P.PROBLEMID = PTC.PROBLEMID ");
                //strQuery.Append("LEFT JOIN PROBLEMACCOUNTMAPPING PBAM ON PBAM.PROBLEMID = P.PROBLEMID ");
                //strQuery.Append("LEFT JOIN ACCOUNTDEFINITION PBAD ON PBAD.ORG_ID = PBAM.ACCOUNTID ");
                //strQuery.Append("LEFT JOIN PORTALACCOUNTS PBPA ON PBAD.ORG_ID = PBPA.ACCOUNTID ");
                //strQuery.Append("LEFT JOIN ACCOUNTCI PBACI ON PBPA.CIID = PBACI.CIID ");
                //strQuery.Append("LEFT JOIN TASKACCOUNTMAPPING TAM ON TAM.TASKID = TD.TASKID ");
                //strQuery.Append("LEFT JOIN ACCOUNTDEFINITION TAD ON TAD.ORG_ID = TAM.ACCOUNTID ");
                //strQuery.Append("LEFT JOIN PORTALACCOUNTS TAPA ON TAPA.ACCOUNTID = TAD.ORG_ID ");
                //strQuery.Append("LEFT JOIN ACCOUNTCI TACI ON TACI.CIID = TAPA.CIID ");
                //strQuery.Append("LEFT JOIN USERDEPARTMENT USERDEPT ON TECH.USER_ID = USERDEPT.USERID  ");
                //strQuery.Append("LEFT JOIN DEPARTMENTDEFINITION DEPTTABLE ON USERDEPT.DEPTID = DEPTTABLE.DEPTID ");
                //strQuery.Append("LEFT JOIN SDOrganization CISITE ON DEPTTABLE.SITEID = CISITE.ORG_ID ");
                //strQuery.Append("LEFT JOIN SDUser SD ON SD.USERID = CHARGESTABLE.TECHNICIANID ");
                //strQuery.Append("LEFT JOIN AAAUSER CREATOR ON CREATOR.USER_ID = CHARGESTABLE.CREATEDBY ");

                strQuery.Append("SELECT to_timestamp(CAST(chargesTable.ts_endtime as bigint)/1000)::date AS RecordDate, SUM(ROUND((CHARGESTABLE.TIMESPENT / 60000) / 60.0, 2)) AS TotalHours ");
                strQuery.Append("FROM CHARGESTABLE CHARGESTABLE ");
                strQuery.Append("LEFT JOIN WORKLOGBILLINGSTATUS WBS ON WBS.CHARGEID = CHARGESTABLE.CHARGEID ");
                strQuery.Append("LEFT JOIN NOBILLREQUESTCHARGES NBRC ON NBRC.REQUESTCHARGEID = CHARGESTABLE.CHARGEID ");
                strQuery.Append("LEFT JOIN BILLINGSTATUS BS ON BS.BILLINGSTATUSID = WBS.BILLINGSTATUS ");
                strQuery.Append("LEFT JOIN AAAUSER TECH ON TECH.USER_ID = CHARGESTABLE.TECHNICIANID ");
                strQuery.Append("LEFT JOIN AAAUSERCONTACTINFO AUCC ON AUCC.USER_ID = TECH.USER_ID ");
                strQuery.Append("LEFT JOIN AAACONTACTINFO ACC ON ACC.CONTACTINFO_ID = AUCC.CONTACTINFO_ID ");
                strQuery.Append("LEFT JOIN TASKTOCHARGE TTC ON TTC.CHARGEID = CHARGESTABLE.CHARGEID ");
                strQuery.Append("LEFT JOIN TASKTOPROJECTS TTP ON TTP.TASKID = TTC.TASKID ");
                strQuery.Append("LEFT JOIN TASKDETAILS TD ON TD.TASKID = TTC.TASKID ");
                strQuery.Append("LEFT JOIN PROJECTDETAILS PD ON PD.PROJECTID = TTP.PROJECTID ");
                strQuery.Append("LEFT JOIN PROJECTACCMAPPING PAM ON PAM.PROJECTID = PD.PROJECTID ");
                strQuery.Append("LEFT JOIN ACCOUNTDEFINITION PAD ON PAD.ORG_ID = PAM.ACCOUNTID ");
                strQuery.Append("LEFT JOIN PORTALACCOUNTS PPA ON PAD.ORG_ID = PPA.ACCOUNTID ");
                strQuery.Append("LEFT JOIN ACCOUNTADDITIONALFIELDS PACI ON PPA.ACCOUNTID = PACI.Instance_Id ");
                strQuery.Append("LEFT JOIN WORKORDERTOCHARGE WTC ON WTC.CHARGEID = CHARGESTABLE.CHARGEID ");
                strQuery.Append("LEFT JOIN WORKORDER WO ON WO.WORKORDERID = WTC.WORKORDERID ");
                strQuery.Append("LEFT JOIN WORKORDERSTATES WOS ON WO.WORKORDERID = WOS.WORKORDERID ");
                strQuery.Append("LEFT JOIN REQUESTTYPEDEFINITION RTD ON WOS.REQUESTTYPEID = RTD.REQUESTTYPEID ");
                strQuery.Append("LEFT JOIN WORKORDERACCOUNTMAPPING WAM ON WAM.WORKORDERID = WO.WORKORDERID ");
                strQuery.Append("LEFT JOIN ACCOUNTDEFINITION WAD ON WAD.ORG_ID = WAM.ACCOUNTID ");
                strQuery.Append("LEFT JOIN PORTALACCOUNTS WPA ON WAD.ORG_ID = WPA.ACCOUNTID ");
                strQuery.Append("LEFT JOIN ACCOUNTADDITIONALFIELDS WACI ON WPA.ACCOUNTID = WACI.Instance_Id ");
                strQuery.Append("LEFT JOIN CHANGETOCHARGE CTC ON CTC.CHARGEID = CHARGESTABLE.CHARGEID ");
                strQuery.Append("LEFT JOIN CHANGEDETAILS CD ON CD.CHANGEID = CTC.CHANGEID ");
                strQuery.Append("LEFT JOIN CHANGEACCOUNTMAPPING CAM ON CAM.CHANGEID = CD.CHANGEID ");
                strQuery.Append("LEFT JOIN ACCOUNTDEFINITION CAD ON CAD.ORG_ID = CAM.ACCOUNTID ");
                strQuery.Append("LEFT JOIN PORTALACCOUNTS CPA ON CAD.ORG_ID = CPA.ACCOUNTID ");
                strQuery.Append("LEFT JOIN ACCOUNTADDITIONALFIELDS CACI ON CPA.ACCOUNTID = CACI.Instance_Id ");
                strQuery.Append("LEFT JOIN PROBLEMTOCHARGE PTC ON PTC.CHARGEID = CHARGESTABLE.CHARGEID ");
                strQuery.Append("LEFT JOIN PROBLEM P ON P.PROBLEMID = PTC.PROBLEMID ");
                strQuery.Append("LEFT JOIN PROBLEMACCOUNTMAPPING PBAM ON PBAM.PROBLEMID = P.PROBLEMID ");
                strQuery.Append("LEFT JOIN ACCOUNTDEFINITION PBAD ON PBAD.ORG_ID = PBAM.ACCOUNTID ");
                strQuery.Append("LEFT JOIN PORTALACCOUNTS PBPA ON PBAD.ORG_ID = PBPA.ACCOUNTID ");
                strQuery.Append("LEFT JOIN ACCOUNTADDITIONALFIELDS PBACI ON PBPA.ACCOUNTID = PBACI.Instance_Id ");
                strQuery.Append("LEFT JOIN TASKACCOUNTMAPPING TAM ON TAM.TASKID = TD.TASKID ");
                strQuery.Append("LEFT JOIN ACCOUNTDEFINITION TAD ON TAD.ORG_ID = TAM.ACCOUNTID ");
                strQuery.Append("LEFT JOIN PORTALACCOUNTS TAPA ON TAPA.ACCOUNTID = TAD.ORG_ID ");
                strQuery.Append("LEFT JOIN ACCOUNTADDITIONALFIELDS TACI ON TACI.Instance_Id = TAPA.ACCOUNTID ");
                strQuery.Append("LEFT JOIN USERDEPARTMENT USERDEPT ON TECH.USER_ID = USERDEPT.USERID  ");
                strQuery.Append("LEFT JOIN DEPARTMENTDEFINITION DEPTTABLE ON USERDEPT.DEPTID = DEPTTABLE.DEPTID ");
                strQuery.Append("LEFT JOIN SDOrganization CISITE ON DEPTTABLE.SITEID = CISITE.ORG_ID ");
                strQuery.Append("LEFT JOIN SDUser SD ON SD.USERID = CHARGESTABLE.TECHNICIANID ");
                strQuery.Append("LEFT JOIN AAAUSER CREATOR ON CREATOR.USER_ID = CHARGESTABLE.CREATEDBY ");

                strQuery.Append("WHERE '[" + objRpt.RecordStartDate.ToString("yyyy-MM-dd") + ", " + objRpt.RecordEndDate.ToString("yyyy-MM-dd") + "]'::daterange @> to_timestamp(CAST(chargesTable.ts_endtime as bigint)/1000)::date");
                //-------------------------
                //Add Filters
                //-------------------------
                strQuery.Append(StandardQueryFilter(dbConnection, objRpt, loginName, fullName));

                strQuery.Append(" GROUP BY RecordDate ORDER BY RecordDate");
                //var txtQuery= strQuery.ToString();

                using NpgsqlCommand cmd = new NpgsqlCommand(strQuery.ToString(), connection);
                using NpgsqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var objReport = new HoursByDay();
                    objReport.RecordDate = Convert.ToDateTime(reader["RecordDate"]).ToString("yyyy'-'MMM'-'dd").Replace(".", "");
                    objReport.TotalHours = Convert.ToDecimal(reader["TotalHours"]);
                    lstReportData.Add(objReport);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetReportData(): " + ex.ToString());
            }
            finally
            {
                connection.Close();
            }
            return [.. lstReportData];
        }

        public List<HoursByPerson> GetReportDataHoursByPerson(string dbConnection, ReportFilter objRpt, string loginName, string fullName)
        {
            List<HoursByPerson> lstReportData = new List<HoursByPerson>();

            using NpgsqlConnection connection = new NpgsqlConnection(dbConnection);
            try
            {
                connection.Open();
                StringBuilder strQuery = new StringBuilder();
                //strQuery.Append("SELECT TECH.FIRST_NAME AS UserName, SUM(ROUND((CHARGESTABLE.TIMESPENT / 60000) / 60.0, 2)) AS TotalHours ");
                //strQuery.Append("FROM CHARGESTABLE CHARGESTABLE ");
                //strQuery.Append("LEFT JOIN WORKLOGBILLINGSTATUS WBS ON WBS.CHARGEID = CHARGESTABLE.CHARGEID ");
                //strQuery.Append("LEFT JOIN NOBILLREQUESTCHARGES NBRC ON NBRC.REQUESTCHARGEID = CHARGESTABLE.CHARGEID ");
                //strQuery.Append("LEFT JOIN BILLINGSTATUS BS ON BS.BILLINGSTATUSID = WBS.BILLINGSTATUS ");
                //strQuery.Append("LEFT JOIN AAAUSER TECH ON TECH.USER_ID = CHARGESTABLE.TECHNICIANID ");
                //strQuery.Append("LEFT JOIN AAAUSERCONTACTINFO AUCC ON AUCC.USER_ID = TECH.USER_ID ");
                //strQuery.Append("LEFT JOIN AAACONTACTINFO ACC ON ACC.CONTACTINFO_ID = AUCC.CONTACTINFO_ID ");
                //strQuery.Append("LEFT JOIN TASKTOCHARGE TTC ON TTC.CHARGEID = CHARGESTABLE.CHARGEID ");
                //strQuery.Append("LEFT JOIN TASKTOPROJECTS TTP ON TTP.TASKID = TTC.TASKID ");
                //strQuery.Append("LEFT JOIN TASKDETAILS TD ON TD.TASKID = TTC.TASKID ");
                //strQuery.Append("LEFT JOIN PROJECTDETAILS PD ON PD.PROJECTID = TTP.PROJECTID ");
                //strQuery.Append("LEFT JOIN PROJECTACCMAPPING PAM ON PAM.PROJECTID = PD.PROJECTID ");
                //strQuery.Append("LEFT JOIN ACCOUNTDEFINITION PAD ON PAD.ORG_ID = PAM.ACCOUNTID ");
                //strQuery.Append("LEFT JOIN PORTALACCOUNTS PPA ON PAD.ORG_ID = PPA.ACCOUNTID ");
                //strQuery.Append("LEFT JOIN ACCOUNTCI PACI ON PPA.CIID = PACI.CIID ");
                //strQuery.Append("LEFT JOIN WORKORDERTOCHARGE WTC ON WTC.CHARGEID = CHARGESTABLE.CHARGEID ");
                //strQuery.Append("LEFT JOIN WORKORDER WO ON WO.WORKORDERID = WTC.WORKORDERID ");
                //strQuery.Append("LEFT JOIN WORKORDERSTATES WOS ON WO.WORKORDERID = WOS.WORKORDERID ");
                //strQuery.Append("LEFT JOIN REQUESTTYPEDEFINITION RTD ON WOS.REQUESTTYPEID = RTD.REQUESTTYPEID ");
                //strQuery.Append("LEFT JOIN WORKORDERACCOUNTMAPPING WAM ON WAM.WORKORDERID = WO.WORKORDERID ");
                //strQuery.Append("LEFT JOIN ACCOUNTDEFINITION WAD ON WAD.ORG_ID = WAM.ACCOUNTID ");
                //strQuery.Append("LEFT JOIN PORTALACCOUNTS WPA ON WAD.ORG_ID = WPA.ACCOUNTID ");
                //strQuery.Append("LEFT JOIN ACCOUNTCI WACI ON WPA.CIID = WACI.CIID ");
                //strQuery.Append("LEFT JOIN CHANGETOCHARGE CTC ON CTC.CHARGEID = CHARGESTABLE.CHARGEID ");
                //strQuery.Append("LEFT JOIN CHANGEDETAILS CD ON CD.CHANGEID = CTC.CHANGEID ");
                //strQuery.Append("LEFT JOIN CHANGEACCOUNTMAPPING CAM ON CAM.CHANGEID = CD.CHANGEID ");
                //strQuery.Append("LEFT JOIN ACCOUNTDEFINITION CAD ON CAD.ORG_ID = CAM.ACCOUNTID ");
                //strQuery.Append("LEFT JOIN PORTALACCOUNTS CPA ON CAD.ORG_ID = CPA.ACCOUNTID ");
                //strQuery.Append("LEFT JOIN ACCOUNTCI CACI ON CPA.CIID = CACI.CIID ");
                //strQuery.Append("LEFT JOIN PROBLEMTOCHARGE PTC ON PTC.CHARGEID = CHARGESTABLE.CHARGEID ");
                //strQuery.Append("LEFT JOIN PROBLEM P ON P.PROBLEMID = PTC.PROBLEMID ");
                //strQuery.Append("LEFT JOIN PROBLEMACCOUNTMAPPING PBAM ON PBAM.PROBLEMID = P.PROBLEMID ");
                //strQuery.Append("LEFT JOIN ACCOUNTDEFINITION PBAD ON PBAD.ORG_ID = PBAM.ACCOUNTID ");
                //strQuery.Append("LEFT JOIN PORTALACCOUNTS PBPA ON PBAD.ORG_ID = PBPA.ACCOUNTID ");
                //strQuery.Append("LEFT JOIN ACCOUNTCI PBACI ON PBPA.CIID = PBACI.CIID ");
                //strQuery.Append("LEFT JOIN TASKACCOUNTMAPPING TAM ON TAM.TASKID = TD.TASKID ");
                //strQuery.Append("LEFT JOIN ACCOUNTDEFINITION TAD ON TAD.ORG_ID = TAM.ACCOUNTID ");
                //strQuery.Append("LEFT JOIN PORTALACCOUNTS TAPA ON TAPA.ACCOUNTID = TAD.ORG_ID ");
                //strQuery.Append("LEFT JOIN ACCOUNTCI TACI ON TACI.CIID = TAPA.CIID ");
                //strQuery.Append("LEFT JOIN USERDEPARTMENT USERDEPT ON TECH.USER_ID = USERDEPT.USERID  ");
                //strQuery.Append("LEFT JOIN DEPARTMENTDEFINITION DEPTTABLE ON USERDEPT.DEPTID = DEPTTABLE.DEPTID ");
                //strQuery.Append("LEFT JOIN SDOrganization CISITE ON DEPTTABLE.SITEID = CISITE.ORG_ID ");
                //strQuery.Append("LEFT JOIN SDUser SD ON SD.USERID = CHARGESTABLE.TECHNICIANID ");
                //strQuery.Append("LEFT JOIN AAAUSER CREATOR ON CREATOR.USER_ID = CHARGESTABLE.CREATEDBY ");

                strQuery.Append("SELECT TECH.FIRST_NAME AS UserName, SUM(ROUND((CHARGESTABLE.TIMESPENT / 60000) / 60.0, 2)) AS TotalHours ");
                strQuery.Append("FROM CHARGESTABLE CHARGESTABLE ");
                strQuery.Append("LEFT JOIN WORKLOGBILLINGSTATUS WBS ON WBS.CHARGEID = CHARGESTABLE.CHARGEID ");
                strQuery.Append("LEFT JOIN NOBILLREQUESTCHARGES NBRC ON NBRC.REQUESTCHARGEID = CHARGESTABLE.CHARGEID ");
                strQuery.Append("LEFT JOIN BILLINGSTATUS BS ON BS.BILLINGSTATUSID = WBS.BILLINGSTATUS ");
                strQuery.Append("LEFT JOIN AAAUSER TECH ON TECH.USER_ID = CHARGESTABLE.TECHNICIANID ");
                strQuery.Append("LEFT JOIN AAAUSERCONTACTINFO AUCC ON AUCC.USER_ID = TECH.USER_ID ");
                strQuery.Append("LEFT JOIN AAACONTACTINFO ACC ON ACC.CONTACTINFO_ID = AUCC.CONTACTINFO_ID ");
                strQuery.Append("LEFT JOIN TASKTOCHARGE TTC ON TTC.CHARGEID = CHARGESTABLE.CHARGEID ");
                strQuery.Append("LEFT JOIN TASKTOPROJECTS TTP ON TTP.TASKID = TTC.TASKID ");
                strQuery.Append("LEFT JOIN TASKDETAILS TD ON TD.TASKID = TTC.TASKID ");
                strQuery.Append("LEFT JOIN PROJECTDETAILS PD ON PD.PROJECTID = TTP.PROJECTID ");
                strQuery.Append("LEFT JOIN PROJECTACCMAPPING PAM ON PAM.PROJECTID = PD.PROJECTID ");
                strQuery.Append("LEFT JOIN ACCOUNTDEFINITION PAD ON PAD.ORG_ID = PAM.ACCOUNTID ");
                strQuery.Append("LEFT JOIN PORTALACCOUNTS PPA ON PAD.ORG_ID = PPA.ACCOUNTID ");
                strQuery.Append("LEFT JOIN ACCOUNTADDITIONALFIELDS PACI ON PPA.ACCOUNTID = PACI.Instance_Id ");
                strQuery.Append("LEFT JOIN WORKORDERTOCHARGE WTC ON WTC.CHARGEID = CHARGESTABLE.CHARGEID ");
                strQuery.Append("LEFT JOIN WORKORDER WO ON WO.WORKORDERID = WTC.WORKORDERID ");
                strQuery.Append("LEFT JOIN WORKORDERSTATES WOS ON WO.WORKORDERID = WOS.WORKORDERID ");
                strQuery.Append("LEFT JOIN REQUESTTYPEDEFINITION RTD ON WOS.REQUESTTYPEID = RTD.REQUESTTYPEID ");
                strQuery.Append("LEFT JOIN WORKORDERACCOUNTMAPPING WAM ON WAM.WORKORDERID = WO.WORKORDERID ");
                strQuery.Append("LEFT JOIN ACCOUNTDEFINITION WAD ON WAD.ORG_ID = WAM.ACCOUNTID ");
                strQuery.Append("LEFT JOIN PORTALACCOUNTS WPA ON WAD.ORG_ID = WPA.ACCOUNTID ");
                strQuery.Append("LEFT JOIN ACCOUNTADDITIONALFIELDS WACI ON WPA.ACCOUNTID = WACI.Instance_Id ");
                strQuery.Append("LEFT JOIN CHANGETOCHARGE CTC ON CTC.CHARGEID = CHARGESTABLE.CHARGEID ");
                strQuery.Append("LEFT JOIN CHANGEDETAILS CD ON CD.CHANGEID = CTC.CHANGEID ");
                strQuery.Append("LEFT JOIN CHANGEACCOUNTMAPPING CAM ON CAM.CHANGEID = CD.CHANGEID ");
                strQuery.Append("LEFT JOIN ACCOUNTDEFINITION CAD ON CAD.ORG_ID = CAM.ACCOUNTID ");
                strQuery.Append("LEFT JOIN PORTALACCOUNTS CPA ON CAD.ORG_ID = CPA.ACCOUNTID ");
                strQuery.Append("LEFT JOIN ACCOUNTADDITIONALFIELDS CACI ON CPA.ACCOUNTID = CACI.Instance_Id ");
                strQuery.Append("LEFT JOIN PROBLEMTOCHARGE PTC ON PTC.CHARGEID = CHARGESTABLE.CHARGEID ");
                strQuery.Append("LEFT JOIN PROBLEM P ON P.PROBLEMID = PTC.PROBLEMID ");
                strQuery.Append("LEFT JOIN PROBLEMACCOUNTMAPPING PBAM ON PBAM.PROBLEMID = P.PROBLEMID ");
                strQuery.Append("LEFT JOIN ACCOUNTDEFINITION PBAD ON PBAD.ORG_ID = PBAM.ACCOUNTID ");
                strQuery.Append("LEFT JOIN PORTALACCOUNTS PBPA ON PBAD.ORG_ID = PBPA.ACCOUNTID ");
                strQuery.Append("LEFT JOIN ACCOUNTADDITIONALFIELDS PBACI ON PBPA.ACCOUNTID = PBACI.Instance_Id ");
                strQuery.Append("LEFT JOIN TASKACCOUNTMAPPING TAM ON TAM.TASKID = TD.TASKID ");
                strQuery.Append("LEFT JOIN ACCOUNTDEFINITION TAD ON TAD.ORG_ID = TAM.ACCOUNTID ");
                strQuery.Append("LEFT JOIN PORTALACCOUNTS TAPA ON TAPA.ACCOUNTID = TAD.ORG_ID ");
                strQuery.Append("LEFT JOIN ACCOUNTADDITIONALFIELDS TACI ON TACI.Instance_Id = TAPA.ACCOUNTID ");
                strQuery.Append("LEFT JOIN USERDEPARTMENT USERDEPT ON TECH.USER_ID = USERDEPT.USERID  ");
                strQuery.Append("LEFT JOIN DEPARTMENTDEFINITION DEPTTABLE ON USERDEPT.DEPTID = DEPTTABLE.DEPTID ");
                strQuery.Append("LEFT JOIN SDOrganization CISITE ON DEPTTABLE.SITEID = CISITE.ORG_ID ");
                strQuery.Append("LEFT JOIN SDUser SD ON SD.USERID = CHARGESTABLE.TECHNICIANID ");
                strQuery.Append("LEFT JOIN AAAUSER CREATOR ON CREATOR.USER_ID = CHARGESTABLE.CREATEDBY ");

                strQuery.Append("WHERE '[" + objRpt.RecordStartDate.ToString("yyyy-MM-dd") + ", " + objRpt.RecordEndDate.ToString("yyyy-MM-dd") + "]'::daterange @> to_timestamp(CAST(chargesTable.ts_endtime as bigint)/1000)::date");
                //-------------------------
                //Add Filters
                //-------------------------
                strQuery.Append(StandardQueryFilter(dbConnection, objRpt, loginName, fullName));

                strQuery.Append(" GROUP BY UserName ORDER BY UserName");
                //var txtQuery= strQuery.ToString();

                using NpgsqlCommand cmd = new NpgsqlCommand(strQuery.ToString(), connection);
                using NpgsqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var objReport = new HoursByPerson();
                    objReport.PersonName = reader["UserName"].ToString();
                    objReport.TotalHours = Convert.ToDecimal(reader["TotalHours"]);
                    lstReportData.Add(objReport);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetReportData(): " + ex.ToString());
            }
            finally
            {
                connection.Close();
            }
            return [.. lstReportData];
        }

        public List<Compliance> GetReportDataCompliance(string dbConnection, ReportFilter objRpt, string loginName, string fullName)
        {
            List<Compliance> lstReportData = new List<Compliance>();

            using NpgsqlConnection connection = new NpgsqlConnection(dbConnection);
            try
            {
                connection.Open();
                StringBuilder strQuery = new StringBuilder();
                strQuery.Append("SELECT UserID, UserName, EmploymentType, DepartmentName, Location, COALESCE(SUM(ROUND((CHARGESTABLE.TIMESPENT / 60000) / 60.0, 2)),0) AS TotalHours FROM ( ");
                strQuery.Append("SELECT USR.USER_ID As UserID, USR.FIRST_NAME AS UserName, SD.EMPLOYEEID AS EmploymentType, DEPTTABLE.DEPTNAME AS DepartmentName, CISITE.Name AS Location ");
                strQuery.Append("FROM AAAUSER USR ");
                strQuery.Append("JOIN AAALOGIN LOG ON USR.USER_ID = LOG.USER_ID ");
                strQuery.Append("JOIN requesteraccountmapping MAPP ON USR.USER_ID = MAPP.USERID ");
                strQuery.Append("JOIN aaausercontactinfo UCI ON UCI.USER_ID = USR.USER_ID ");
                strQuery.Append("JOIN portaltechnicians PT ON PT.USERID = USR.USER_ID ");
                strQuery.Append("LEFT JOIN accountdefinition ACC ON MAPP.ACCOUNTID = ACC.ORG_ID ");
                strQuery.Append("LEFT JOIN aaacontactinfo ACI ON ACI.contactinfo_id = UCI.contactinfo_id ");
                strQuery.Append("LEFT JOIN USERDEPARTMENT USERDEPT ON USR.USER_ID = USERDEPT.USERID  ");
                strQuery.Append("LEFT JOIN DEPARTMENTDEFINITION DEPTTABLE ON USERDEPT.DEPTID = DEPTTABLE.DEPTID ");
                strQuery.Append("LEFT JOIN SDOrganization CISITE ON DEPTTABLE.SITEID = CISITE.ORG_ID ");
                strQuery.Append("LEFT JOIN SDUser SD ON SD.USERID = USR.USER_ID ");
                strQuery.Append("WHERE FIRST_NAME <> 'administrator' AND ORG_NAME = 'Vigilant Technologies' AND CISITE.Name <> 'Client' AND SD.Status = 'ACTIVE' ");
                strQuery.Append(") t ");
                strQuery.Append("LEFT JOIN CHARGESTABLE CHARGESTABLE ON CHARGESTABLE.TECHNICIANID = t.UserId AND '[" + objRpt.RecordStartDate.ToString("yyyy-MM-dd") + ", " + objRpt.RecordEndDate.ToString("yyyy-MM-dd") + "]'::daterange @> to_timestamp(CAST(chargesTable.ts_endtime as bigint)/1000)::date ");
                strQuery.Append("WHERE 1 = 1 ");
                if (objRpt.Department != null)
                {
                    var strDepartments = "'" + objRpt.Department.Replace("'", "''").Replace(",", "','") + "'";
                    strQuery.Append("AND (DepartmentName IN (" + strDepartments + ")) ");
                }
                if (objRpt.Location != null)
                {
                    var strLocations = "'" + objRpt.Location.Replace("'", "''").Replace(",", "','") + "'";
                    strQuery.Append("AND (Location IN (" + strLocations + ")) ");
                }
                if (objRpt.EmploymentType != null)
                {
                    var strEmploymentTypes = "'" + objRpt.EmploymentType.Replace("'", "''").Replace(",", "','") + "'";
                    strQuery.Append("AND (EmploymentType IN (" + strEmploymentTypes + ")) ");
                }
                strQuery.Append("GROUP BY USERID,UserName,EmploymentType,DepartmentName,Location ORDER BY TotalHours, UserName");
                //var txtQuery= strQuery.ToString();

                using NpgsqlCommand cmd = new NpgsqlCommand(strQuery.ToString(), connection);
                using NpgsqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var objReport = new Compliance();
                    objReport.UserId = Convert.ToInt32(reader["UserId"]);
                    objReport.UserName = (string)reader["UserName"];
                    objReport.EmploymentType = (string)(reader["EmploymentType"] is DBNull ? "" : reader["EmploymentType"]);
                    objReport.DepartmentName = (string)(reader["DepartmentName"] is DBNull ? "" : reader["DepartmentName"]);
                    objReport.Location = (string)(reader["Location"] is DBNull ? "" : reader["Location"]);
                    objReport.TotalHours = Convert.ToDecimal(reader["TotalHours"]);
                    lstReportData.Add(objReport);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetReportData(): " + ex.ToString());
            }
            finally
            {
                connection.Close();
            }
            return [.. lstReportData];
        }

        public List<Utilization> GetReportDataUtilizationByPerson(string dbConnection, ReportFilter objRpt, string loginName, string fullName)
        {
            List<Utilization> lstReportData = new List<Utilization>();
            DateTime startDate = objRpt.RecordStartDate;
            DateTime endDate = objRpt.RecordEndDate;
            DateTime currentDate = DateTime.Now;
            if (endDate.ToString("yyyy-MM-dd") != currentDate.ToString("yyyy-MM-dd"))
            {
                currentDate = currentDate.AddDays(-1);
                if (endDate > currentDate)
                {
                    endDate = currentDate;
                }
                if (startDate > endDate)
                {
                    startDate = endDate;
                }
            }

            using NpgsqlConnection connection = new NpgsqlConnection(dbConnection);
            try
            {
                connection.Open();
                StringBuilder strQuery = new StringBuilder();

                //strQuery.Append("SELECT USR.USER_ID As UserID, USR.FIRST_NAME AS UserName, ACI.EMAILID AS UserEmail, DEPTTABLE.DEPTNAME AS DepartmentName, CISITE.Name AS Location, COALESCE(StartDate,'" + startDate.ToString("yyyy-MM-dd") + "') As StartDate, COALESCE(EndDate,'" + endDate.ToString("yyyy-MM-dd") + "') As EndDate, COALESCE(BusinessDays,(1 + '" + endDate.ToString("yyyy-MM-dd") + "'::date - '" + startDate.ToString("yyyy-MM-dd") + "'::date - (SELECT COUNT(*) FROM generate_series('" + startDate.ToString("yyyy-MM-dd") + "', '" + endDate.ToString("yyyy-MM-dd") + "', '1 day'::interval) AS g(d) WHERE EXTRACT(DOW FROM d) IN (0, 6)))) As BusinessDays, COALESCE(BillableHours,0) As BillableHours, COALESCE(TotalHours,COALESCE((SELECT SUM(ROUND((CT2.TIMESPENT / 60000) / 60.0, 2)) As Total FROM ChargesTable CT2 WHERE '[" + startDate.ToString("yyyy-MM-dd") + ", " + endDate.ToString("yyyy-MM-dd") + "]'::daterange @> to_timestamp(CAST(CT2.ts_endtime as bigint)/1000)::date AND CT2.Technicianid = USR.USER_ID GROUP BY TECHNICIANID),0)) As TotalHours, COALESCE(UtilizationPercentage,0) As UtilizationPercentage ");
                //strQuery.Append("FROM AAAUSER USR JOIN AAALOGIN LOG ON USR.USER_ID = LOG.USER_ID JOIN requesteraccountmapping MAPP ON USR.USER_ID = MAPP.USERID JOIN aaausercontactinfo UCI ON UCI.USER_ID = USR.USER_ID JOIN portaltechnicians PT ON PT.USERID = USR.USER_ID LEFT JOIN accountdefinition ACC ON MAPP.ACCOUNTID = ACC.ORG_ID LEFT JOIN aaacontactinfo ACI ON ACI.contactinfo_id = UCI.contactinfo_id LEFT JOIN USERDEPARTMENT USERDEPT ON USR.USER_ID = USERDEPT.USERID LEFT JOIN DEPARTMENTDEFINITION DEPTTABLE ON USERDEPT.DEPTID = DEPTTABLE.DEPTID LEFT JOIN SDOrganization CISITE ON DEPTTABLE.SITEID = CISITE.ORG_ID LEFT JOIN SDUser SD ON SD.USERID = USR.USER_ID ");
                //strQuery.Append("LEFT JOIN (");
                //strQuery.Append("SELECT UserId, UserName, StartDate, '" + endDate.ToString("yyyy-MM-dd") + "' As EndDate, (1 + '" + endDate.ToString("yyyy-MM-dd") + "'::date - StartDate - (SELECT COUNT(*) FROM generate_series(StartDate, '" + endDate.ToString("yyyy-MM-dd") + "', '1 day'::interval) AS g(d) WHERE EXTRACT(DOW FROM d) IN (0, 6))) AS BusinessDays, SUM(TimeSpentHrs) As BillableHours, (SELECT SUM(ROUND((CT2.TIMESPENT / 60000) / 60.0, 2)) As Total FROM ChargesTable CT2 WHERE '[" + startDate.ToString("yyyy-MM-dd") + ", " + endDate.ToString("yyyy-MM-dd") + "]'::daterange @> to_timestamp(CAST(CT2.ts_endtime as bigint)/1000)::date AND CT2.Technicianid = UserID GROUP BY TECHNICIANID) AS TotalHours, (SUM(TimeSpentHrs)/((CASE WHEN UserID=2057 THEN 4 ELSE 8 END)*(1 + '" + endDate.ToString("yyyy-MM-dd") + "'::date - StartDate - (SELECT COUNT(*) FROM generate_series(StartDate, '" + endDate.ToString("yyyy-MM-dd") + "', '1 day'::interval) AS g(d) WHERE EXTRACT(DOW FROM d) IN (0, 6))))) As UtilizationPercentage FROM ( ");
                //strQuery.Append("SELECT (select CASE WHEN MIN(to_timestamp(CAST(CT.ts_endtime as bigint)/1000)::date)>'" + startDate.ToString("yyyy-MM-dd") + "'::date THEN MIN(to_timestamp(CAST(CT.ts_endtime as bigint)/1000)::date) ELSE '" + startDate.ToString("yyyy-MM-dd") + "' END from chargestable CT where CT.Technicianid = CHARGESTABLE.TECHNICIANID) AS StartDate, CHARGESTABLE.TECHNICIANID AS UserId,TECH.FIRST_NAME AS UserName,ACC.EMAILID AS UserEmail,DEPTTABLE.DEPTNAME AS DepartmentName,CISITE.Name AS Location,TO_TIMESTAMP(CHARGESTABLE.TS_ENDTIME / 1000) AS RecordEndDate,ROUND((CHARGESTABLE.TIMESPENT / 60000) / 60.0, 2) AS TimeSpentHrs,CASE WHEN PAD.ORG_ID IS NOT NULL THEN PACI.ATTRIBUTE_302 WHEN WAD.ORG_ID IS NOT NULL THEN WACI.ATTRIBUTE_302 WHEN CAD.ORG_ID IS NOT NULL THEN CACI.ATTRIBUTE_302 WHEN PBAD.ORG_ID IS NOT NULL THEN PBACI.ATTRIBUTE_302 WHEN TAD.ORG_ID IS NOT NULL THEN TACI.ATTRIBUTE_302 END AS ClientId ");
                //strQuery.Append("FROM CHARGESTABLE CHARGESTABLE ");
                //strQuery.Append("LEFT JOIN WORKLOGBILLINGSTATUS WBS ON WBS.CHARGEID = CHARGESTABLE.CHARGEID ");
                //strQuery.Append("LEFT JOIN NOBILLREQUESTCHARGES NBRC ON NBRC.REQUESTCHARGEID = CHARGESTABLE.CHARGEID ");
                //strQuery.Append("LEFT JOIN AAAUSER TECH ON TECH.USER_ID = CHARGESTABLE.TECHNICIANID ");
                //strQuery.Append("LEFT JOIN AAAUSERCONTACTINFO AUCC ON AUCC.USER_ID = TECH.USER_ID ");
                //strQuery.Append("LEFT JOIN AAACONTACTINFO ACC ON ACC.CONTACTINFO_ID = AUCC.CONTACTINFO_ID ");
                //strQuery.Append("LEFT JOIN TASKTOCHARGE TTC ON TTC.CHARGEID = CHARGESTABLE.CHARGEID ");
                //strQuery.Append("LEFT JOIN TASKTOPROJECTS TTP ON TTP.TASKID = TTC.TASKID ");
                //strQuery.Append("LEFT JOIN TASKDETAILS TD ON TD.TASKID = TTC.TASKID ");
                //strQuery.Append("LEFT JOIN PROJECTDETAILS PD ON PD.PROJECTID = TTP.PROJECTID ");
                //strQuery.Append("LEFT JOIN PROJECTACCMAPPING PAM ON PAM.PROJECTID = PD.PROJECTID ");
                //strQuery.Append("LEFT JOIN ACCOUNTDEFINITION PAD ON PAD.ORG_ID = PAM.ACCOUNTID ");
                //strQuery.Append("LEFT JOIN PORTALACCOUNTS PPA ON PAD.ORG_ID = PPA.ACCOUNTID ");
                //strQuery.Append("LEFT JOIN ACCOUNTCI PACI ON PPA.CIID = PACI.CIID ");
                //strQuery.Append("LEFT JOIN WORKORDERTOCHARGE WTC ON WTC.CHARGEID = CHARGESTABLE.CHARGEID ");
                //strQuery.Append("LEFT JOIN WORKORDER WO ON WO.WORKORDERID = WTC.WORKORDERID ");
                //strQuery.Append("LEFT JOIN WORKORDERSTATES WOS ON WO.WORKORDERID = WOS.WORKORDERID ");
                //strQuery.Append("LEFT JOIN REQUESTTYPEDEFINITION RTD ON WOS.REQUESTTYPEID = RTD.REQUESTTYPEID ");
                //strQuery.Append("LEFT JOIN WORKORDERACCOUNTMAPPING WAM ON WAM.WORKORDERID = WO.WORKORDERID ");
                //strQuery.Append("LEFT JOIN ACCOUNTDEFINITION WAD ON WAD.ORG_ID = WAM.ACCOUNTID ");
                //strQuery.Append("LEFT JOIN PORTALACCOUNTS WPA ON WAD.ORG_ID = WPA.ACCOUNTID ");
                //strQuery.Append("LEFT JOIN ACCOUNTCI WACI ON WPA.CIID = WACI.CIID ");
                //strQuery.Append("LEFT JOIN CHANGETOCHARGE CTC ON CTC.CHARGEID = CHARGESTABLE.CHARGEID ");
                //strQuery.Append("LEFT JOIN CHANGEDETAILS CD ON CD.CHANGEID = CTC.CHANGEID ");
                //strQuery.Append("LEFT JOIN CHANGEACCOUNTMAPPING CAM ON CAM.CHANGEID = CD.CHANGEID ");
                //strQuery.Append("LEFT JOIN ACCOUNTDEFINITION CAD ON CAD.ORG_ID = CAM.ACCOUNTID ");
                //strQuery.Append("LEFT JOIN PORTALACCOUNTS CPA ON CAD.ORG_ID = CPA.ACCOUNTID ");
                //strQuery.Append("LEFT JOIN ACCOUNTCI CACI ON CPA.CIID = CACI.CIID ");
                //strQuery.Append("LEFT JOIN PROBLEMTOCHARGE PTC ON PTC.CHARGEID = CHARGESTABLE.CHARGEID ");
                //strQuery.Append("LEFT JOIN PROBLEM P ON P.PROBLEMID = PTC.PROBLEMID ");
                //strQuery.Append("LEFT JOIN PROBLEMACCOUNTMAPPING PBAM ON PBAM.PROBLEMID = P.PROBLEMID ");
                //strQuery.Append("LEFT JOIN ACCOUNTDEFINITION PBAD ON PBAD.ORG_ID = PBAM.ACCOUNTID ");
                //strQuery.Append("LEFT JOIN PORTALACCOUNTS PBPA ON PBAD.ORG_ID = PBPA.ACCOUNTID ");
                //strQuery.Append("LEFT JOIN ACCOUNTCI PBACI ON PBPA.CIID = PBACI.CIID ");
                //strQuery.Append("LEFT JOIN TASKACCOUNTMAPPING TAM ON TAM.TASKID = TD.TASKID ");
                //strQuery.Append("LEFT JOIN ACCOUNTDEFINITION TAD ON TAD.ORG_ID = TAM.ACCOUNTID ");
                //strQuery.Append("LEFT JOIN PORTALACCOUNTS TAPA ON TAPA.ACCOUNTID = TAD.ORG_ID ");
                //strQuery.Append("LEFT JOIN ACCOUNTCI TACI ON TACI.CIID = TAPA.CIID ");
                //strQuery.Append("LEFT JOIN USERDEPARTMENT USERDEPT ON TECH.USER_ID = USERDEPT.USERID  ");
                //strQuery.Append("LEFT JOIN DEPARTMENTDEFINITION DEPTTABLE ON USERDEPT.DEPTID = DEPTTABLE.DEPTID ");
                //strQuery.Append("LEFT JOIN SDOrganization CISITE ON DEPTTABLE.SITEID = CISITE.ORG_ID ");
                //strQuery.Append("LEFT JOIN SDUser SD ON SD.USERID = CHARGESTABLE.TECHNICIANID ");

                strQuery.Append("SELECT USR.USER_ID As UserID, USR.FIRST_NAME AS UserName, ACI.EMAILID AS UserEmail, DEPTTABLE.DEPTNAME AS DepartmentName, CISITE.Name AS Location, COALESCE(StartDate,'" + startDate.ToString("yyyy-MM-dd") + "') As StartDate, COALESCE(EndDate,'" + endDate.ToString("yyyy-MM-dd") + "') As EndDate, COALESCE(BusinessDays,(1 + '" + endDate.ToString("yyyy-MM-dd") + "'::date - '" + startDate.ToString("yyyy-MM-dd") + "'::date - (SELECT COUNT(*) FROM generate_series('" + startDate.ToString("yyyy-MM-dd") + "', '" + endDate.ToString("yyyy-MM-dd") + "', '1 day'::interval) AS g(d) WHERE EXTRACT(DOW FROM d) IN (0, 6)))) As BusinessDays, COALESCE(BillableHours,0) As BillableHours, COALESCE(TotalHours,COALESCE((SELECT SUM(ROUND((CT2.TIMESPENT / 60000) / 60.0, 2)) As Total FROM ChargesTable CT2 WHERE '[" + startDate.ToString("yyyy-MM-dd") + ", " + endDate.ToString("yyyy-MM-dd") + "]'::daterange @> to_timestamp(CAST(CT2.ts_endtime as bigint)/1000)::date AND CT2.Technicianid = USR.USER_ID GROUP BY TECHNICIANID),0)) As TotalHours, COALESCE(UtilizationPercentage,0) As UtilizationPercentage ");
                strQuery.Append("FROM AAAUSER USR JOIN AAALOGIN LOG ON USR.USER_ID = LOG.USER_ID JOIN requesteraccountmapping MAPP ON USR.USER_ID = MAPP.USERID JOIN aaausercontactinfo UCI ON UCI.USER_ID = USR.USER_ID JOIN portaltechnicians PT ON PT.USERID = USR.USER_ID LEFT JOIN accountdefinition ACC ON MAPP.ACCOUNTID = ACC.ORG_ID LEFT JOIN aaacontactinfo ACI ON ACI.contactinfo_id = UCI.contactinfo_id LEFT JOIN USERDEPARTMENT USERDEPT ON USR.USER_ID = USERDEPT.USERID LEFT JOIN DEPARTMENTDEFINITION DEPTTABLE ON USERDEPT.DEPTID = DEPTTABLE.DEPTID LEFT JOIN SDOrganization CISITE ON DEPTTABLE.SITEID = CISITE.ORG_ID LEFT JOIN SDUser SD ON SD.USERID = USR.USER_ID ");
                strQuery.Append("LEFT JOIN (");
                strQuery.Append("SELECT UserId, UserName, StartDate, '" + endDate.ToString("yyyy-MM-dd") + "' As EndDate, (1 + '" + endDate.ToString("yyyy-MM-dd") + "'::date - StartDate - (SELECT COUNT(*) FROM generate_series(StartDate, '" + endDate.ToString("yyyy-MM-dd") + "', '1 day'::interval) AS g(d) WHERE EXTRACT(DOW FROM d) IN (0, 6))) AS BusinessDays, SUM(TimeSpentHrs) As BillableHours, (SELECT SUM(ROUND((CT2.TIMESPENT / 60000) / 60.0, 2)) As Total FROM ChargesTable CT2 WHERE '[" + startDate.ToString("yyyy-MM-dd") + ", " + endDate.ToString("yyyy-MM-dd") + "]'::daterange @> to_timestamp(CAST(CT2.ts_endtime as bigint)/1000)::date AND CT2.Technicianid = UserID GROUP BY TECHNICIANID) AS TotalHours, (SUM(TimeSpentHrs)/((CASE WHEN UserID=2057 THEN 4 ELSE 8 END)*(1 + '" + endDate.ToString("yyyy-MM-dd") + "'::date - StartDate - (SELECT COUNT(*) FROM generate_series(StartDate, '" + endDate.ToString("yyyy-MM-dd") + "', '1 day'::interval) AS g(d) WHERE EXTRACT(DOW FROM d) IN (0, 6))))) As UtilizationPercentage FROM ( ");
                strQuery.Append("SELECT (select CASE WHEN MIN(to_timestamp(CAST(CT.ts_endtime as bigint)/1000)::date)>'" + startDate.ToString("yyyy-MM-dd") + "'::date THEN MIN(to_timestamp(CAST(CT.ts_endtime as bigint)/1000)::date) ELSE '" + startDate.ToString("yyyy-MM-dd") + "' END from chargestable CT where CT.Technicianid = CHARGESTABLE.TECHNICIANID) AS StartDate, CHARGESTABLE.TECHNICIANID AS UserId,TECH.FIRST_NAME AS UserName,ACC.EMAILID AS UserEmail,DEPTTABLE.DEPTNAME AS DepartmentName,CISITE.Name AS Location,TO_TIMESTAMP(CHARGESTABLE.TS_ENDTIME / 1000) AS RecordEndDate,ROUND((CHARGESTABLE.TIMESPENT / 60000) / 60.0, 2) AS TimeSpentHrs,CASE WHEN PAD.ORG_ID IS NOT NULL THEN PACI.udf_long2 WHEN WAD.ORG_ID IS NOT NULL THEN WACI.udf_long2 WHEN CAD.ORG_ID IS NOT NULL THEN CACI.udf_long2 WHEN PBAD.ORG_ID IS NOT NULL THEN PBACI.udf_long2 WHEN TAD.ORG_ID IS NOT NULL THEN TACI.udf_long2 END AS ClientId ");
                strQuery.Append("FROM CHARGESTABLE CHARGESTABLE ");
                strQuery.Append("LEFT JOIN WORKLOGBILLINGSTATUS WBS ON WBS.CHARGEID = CHARGESTABLE.CHARGEID ");
                strQuery.Append("LEFT JOIN NOBILLREQUESTCHARGES NBRC ON NBRC.REQUESTCHARGEID = CHARGESTABLE.CHARGEID ");
                strQuery.Append("LEFT JOIN AAAUSER TECH ON TECH.USER_ID = CHARGESTABLE.TECHNICIANID ");
                strQuery.Append("LEFT JOIN AAAUSERCONTACTINFO AUCC ON AUCC.USER_ID = TECH.USER_ID ");
                strQuery.Append("LEFT JOIN AAACONTACTINFO ACC ON ACC.CONTACTINFO_ID = AUCC.CONTACTINFO_ID ");
                strQuery.Append("LEFT JOIN TASKTOCHARGE TTC ON TTC.CHARGEID = CHARGESTABLE.CHARGEID ");
                strQuery.Append("LEFT JOIN TASKTOPROJECTS TTP ON TTP.TASKID = TTC.TASKID ");
                strQuery.Append("LEFT JOIN TASKDETAILS TD ON TD.TASKID = TTC.TASKID ");
                strQuery.Append("LEFT JOIN PROJECTDETAILS PD ON PD.PROJECTID = TTP.PROJECTID ");
                strQuery.Append("LEFT JOIN PROJECTACCMAPPING PAM ON PAM.PROJECTID = PD.PROJECTID ");
                strQuery.Append("LEFT JOIN ACCOUNTDEFINITION PAD ON PAD.ORG_ID = PAM.ACCOUNTID ");
                strQuery.Append("LEFT JOIN PORTALACCOUNTS PPA ON PAD.ORG_ID = PPA.ACCOUNTID ");
                strQuery.Append("LEFT JOIN ACCOUNTADDITIONALFIELDS PACI ON PPA.ACCOUNTID = PACI.Instance_Id ");
                strQuery.Append("LEFT JOIN WORKORDERTOCHARGE WTC ON WTC.CHARGEID = CHARGESTABLE.CHARGEID ");
                strQuery.Append("LEFT JOIN WORKORDER WO ON WO.WORKORDERID = WTC.WORKORDERID ");
                strQuery.Append("LEFT JOIN WORKORDERSTATES WOS ON WO.WORKORDERID = WOS.WORKORDERID ");
                strQuery.Append("LEFT JOIN REQUESTTYPEDEFINITION RTD ON WOS.REQUESTTYPEID = RTD.REQUESTTYPEID ");
                strQuery.Append("LEFT JOIN WORKORDERACCOUNTMAPPING WAM ON WAM.WORKORDERID = WO.WORKORDERID ");
                strQuery.Append("LEFT JOIN ACCOUNTDEFINITION WAD ON WAD.ORG_ID = WAM.ACCOUNTID ");
                strQuery.Append("LEFT JOIN PORTALACCOUNTS WPA ON WAD.ORG_ID = WPA.ACCOUNTID ");
                strQuery.Append("LEFT JOIN ACCOUNTADDITIONALFIELDS WACI ON WPA.ACCOUNTID = WACI.Instance_Id ");
                strQuery.Append("LEFT JOIN CHANGETOCHARGE CTC ON CTC.CHARGEID = CHARGESTABLE.CHARGEID ");
                strQuery.Append("LEFT JOIN CHANGEDETAILS CD ON CD.CHANGEID = CTC.CHANGEID ");
                strQuery.Append("LEFT JOIN CHANGEACCOUNTMAPPING CAM ON CAM.CHANGEID = CD.CHANGEID ");
                strQuery.Append("LEFT JOIN ACCOUNTDEFINITION CAD ON CAD.ORG_ID = CAM.ACCOUNTID ");
                strQuery.Append("LEFT JOIN PORTALACCOUNTS CPA ON CAD.ORG_ID = CPA.ACCOUNTID ");
                strQuery.Append("LEFT JOIN ACCOUNTADDITIONALFIELDS CACI ON CPA.ACCOUNTID = CACI.Instance_Id ");
                strQuery.Append("LEFT JOIN PROBLEMTOCHARGE PTC ON PTC.CHARGEID = CHARGESTABLE.CHARGEID ");
                strQuery.Append("LEFT JOIN PROBLEM P ON P.PROBLEMID = PTC.PROBLEMID ");
                strQuery.Append("LEFT JOIN PROBLEMACCOUNTMAPPING PBAM ON PBAM.PROBLEMID = P.PROBLEMID ");
                strQuery.Append("LEFT JOIN ACCOUNTDEFINITION PBAD ON PBAD.ORG_ID = PBAM.ACCOUNTID ");
                strQuery.Append("LEFT JOIN PORTALACCOUNTS PBPA ON PBAD.ORG_ID = PBPA.ACCOUNTID ");
                strQuery.Append("LEFT JOIN ACCOUNTADDITIONALFIELDS PBACI ON PBPA.ACCOUNTID = PBACI.Instance_Id ");
                strQuery.Append("LEFT JOIN TASKACCOUNTMAPPING TAM ON TAM.TASKID = TD.TASKID ");
                strQuery.Append("LEFT JOIN ACCOUNTDEFINITION TAD ON TAD.ORG_ID = TAM.ACCOUNTID ");
                strQuery.Append("LEFT JOIN PORTALACCOUNTS TAPA ON TAPA.ACCOUNTID = TAD.ORG_ID ");
                strQuery.Append("LEFT JOIN ACCOUNTADDITIONALFIELDS TACI ON TACI.Instance_Id = TAPA.ACCOUNTID ");
                strQuery.Append("LEFT JOIN USERDEPARTMENT USERDEPT ON TECH.USER_ID = USERDEPT.USERID  ");
                strQuery.Append("LEFT JOIN DEPARTMENTDEFINITION DEPTTABLE ON USERDEPT.DEPTID = DEPTTABLE.DEPTID ");
                strQuery.Append("LEFT JOIN SDOrganization CISITE ON DEPTTABLE.SITEID = CISITE.ORG_ID ");
                strQuery.Append("LEFT JOIN SDUser SD ON SD.USERID = CHARGESTABLE.TECHNICIANID ");


                strQuery.Append("WHERE '[" + startDate.ToString("yyyy-MM-dd") + ", " + endDate.ToString("yyyy-MM-dd") + "]'::daterange @> to_timestamp(CAST(chargesTable.ts_endtime as bigint)/1000)::date AND CISITE.Name <> 'Client' AND SD.EmployeeId = 'Emp' AND NBRC.ISBILLABLEWORKLOG = true ");
                strQuery.Append(") t WHERE ClientId <> '6' ");
                strQuery.Append("GROUP BY UserId, UserName, UserEmail, DepartmentName, Location, StartDate ORDER BY UserName) AS S ");
                strQuery.Append("ON USR.USER_ID = S.UserId AND USR.FIRST_NAME = S.UserName ");
                strQuery.Append("WHERE FIRST_NAME <> 'administrator' AND ORG_NAME = 'Vigilant Technologies' AND CISITE.Name <> 'Client' AND SD.EmployeeId = 'Emp' AND SD.Status = 'ACTIVE' ");
                if (objRpt.Department != null)
                {
                    var strDepartments = "'" + objRpt.Department.Replace("'", "''").Replace(",", "','") + "'";
                    strQuery.Append("AND (DEPTTABLE.DEPTNAME IN (" + strDepartments + ")) ");
                }
                if (objRpt.Location != null)
                {
                    var strLocations = "'" + objRpt.Location.Replace("'", "''").Replace(",", "','") + "'";
                    strQuery.Append("AND (CISITE.Name IN (" + strLocations + ")) ");
                }
                strQuery.Append("ORDER BY UserName");
                //var txtQuery= strQuery.ToString();

                using NpgsqlCommand cmd = new NpgsqlCommand(strQuery.ToString(), connection);
                using NpgsqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var objReport = new Utilization();
                    objReport.UserId = Convert.ToInt32(reader["UserId"]);
                    objReport.UserName = (string)reader["UserName"];
                    objReport.UserEmail = (string)(reader["UserEmail"] is DBNull ? "" : reader["UserEmail"]);
                    objReport.DepartmentName = (string)(reader["DepartmentName"] is DBNull ? "" : reader["DepartmentName"]);
                    objReport.Location = (string)(reader["Location"] is DBNull ? "" : reader["Location"]);
                    objReport.StartDate = Convert.ToDateTime(reader["StartDate"]).ToString("yyyy'-'MMM'-'dd").Replace(".", "");
                    objReport.EndDate = Convert.ToDateTime(reader["EndDate"]).ToString("yyyy'-'MMM'-'dd").Replace(".", "");
                    objReport.BusinessDays = Convert.ToInt32(reader["BusinessDays"]);
                    objReport.BillableHours = Convert.ToDecimal(reader["BillableHours"]);
                    objReport.NonBillableHours = Convert.ToDecimal(reader["TotalHours"]) - Convert.ToDecimal(reader["BillableHours"]);
                    objReport.UtilizationPercentage = Convert.ToDecimal(reader["UtilizationPercentage"]).ToString("P2");
                    lstReportData.Add(objReport);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetReportData(): " + ex.ToString());
            }
            finally
            {
                connection.Close();
            }
            return [.. lstReportData];
        }

        public List<Utilization> GetReportDataUtilizationByDepartment(string dbConnection, ReportFilter objRpt, string loginName, string fullName)
        {
            List<Utilization> lstReportData = new List<Utilization>();
            DateTime startDate = objRpt.RecordStartDate;
            DateTime endDate = objRpt.RecordEndDate;
            DateTime currentDate = DateTime.Now;
            if (endDate.ToString("yyyy-MM-dd") != currentDate.ToString("yyyy-MM-dd"))
            {
                currentDate = currentDate.AddDays(-1);
                if (endDate > currentDate)
                {
                    endDate = currentDate;
                }
                if (startDate > endDate)
                {
                    startDate = endDate;
                }
            }

            using NpgsqlConnection connection = new NpgsqlConnection(dbConnection);
            try
            {
                connection.Open();
                StringBuilder strQuery = new StringBuilder();
                //strQuery.Append("SELECT DepartmentName, MIN(StartDate) AS StartDate, MAX(EndDate) AS EndDate, MAX(BusinessDays) AS BusinessDays, SUM(BillableHours) AS BillableHours, SUM(TotalHours) AS TotalHours, (SUM(BillableHours)/SUM(AvailableHours)) AS UtilizationPercentage FROM ( ");
                //strQuery.Append("SELECT DepartmentName, StartDate, EndDate, BusinessDays, SUM(BillableHours) AS BillableHours, SUM(TotalHours) AS TotalHours, (CASE WHEN DepartmentName = 'PMO' THEN (COUNT(*) * BusinessDays * 8)-(BusinessDays * 4) ELSE (COUNT(*) * BusinessDays * 8) END) AS AvailableHours FROM ( ");
                //strQuery.Append("SELECT USR.USER_ID As UserID, USR.FIRST_NAME AS UserName, ACI.EMAILID AS UserEmail, DEPTTABLE.DEPTNAME AS DepartmentName, CISITE.Name AS Location, COALESCE(StartDate,'" + startDate.ToString("yyyy-MM-dd") + "') As StartDate, COALESCE(EndDate,'" + endDate.ToString("yyyy-MM-dd") + "') As EndDate, COALESCE(BusinessDays,(1 + '" + endDate.ToString("yyyy-MM-dd") + "'::date - '" + startDate.ToString("yyyy-MM-dd") + "'::date - (SELECT COUNT(*) FROM generate_series('" + startDate.ToString("yyyy-MM-dd") + "', '" + endDate.ToString("yyyy-MM-dd") + "', '1 day'::interval) AS g(d) WHERE EXTRACT(DOW FROM d) IN (0, 6)))) As BusinessDays, COALESCE(BillableHours,0) As BillableHours, COALESCE(TotalHours,COALESCE((SELECT SUM(ROUND((CT2.TIMESPENT / 60000) / 60.0, 2)) As Total FROM ChargesTable CT2 WHERE '[" + startDate.ToString("yyyy-MM-dd") + ", " + endDate.ToString("yyyy-MM-dd") + "]'::daterange @> to_timestamp(CAST(CT2.ts_endtime as bigint)/1000)::date AND CT2.Technicianid = USR.USER_ID GROUP BY TECHNICIANID),0)) As TotalHours, COALESCE(UtilizationPercentage,0) As UtilizationPercentage ");
                //strQuery.Append("FROM AAAUSER USR JOIN AAALOGIN LOG ON USR.USER_ID = LOG.USER_ID JOIN requesteraccountmapping MAPP ON USR.USER_ID = MAPP.USERID JOIN aaausercontactinfo UCI ON UCI.USER_ID = USR.USER_ID JOIN portaltechnicians PT ON PT.USERID = USR.USER_ID LEFT JOIN accountdefinition ACC ON MAPP.ACCOUNTID = ACC.ORG_ID LEFT JOIN aaacontactinfo ACI ON ACI.contactinfo_id = UCI.contactinfo_id LEFT JOIN USERDEPARTMENT USERDEPT ON USR.USER_ID = USERDEPT.USERID LEFT JOIN DEPARTMENTDEFINITION DEPTTABLE ON USERDEPT.DEPTID = DEPTTABLE.DEPTID LEFT JOIN SDOrganization CISITE ON DEPTTABLE.SITEID = CISITE.ORG_ID LEFT JOIN SDUser SD ON SD.USERID = USR.USER_ID ");
                //strQuery.Append("LEFT JOIN (");
                //strQuery.Append("SELECT UserId, UserName, StartDate, '" + endDate.ToString("yyyy-MM-dd") + "' As EndDate, (1 + '" + endDate.ToString("yyyy-MM-dd") + "'::date - StartDate - (SELECT COUNT(*) FROM generate_series(StartDate, '" + endDate.ToString("yyyy-MM-dd") + "', '1 day'::interval) AS g(d) WHERE EXTRACT(DOW FROM d) IN (0, 6))) AS BusinessDays, SUM(TimeSpentHrs) As BillableHours, (SELECT SUM(ROUND((CT2.TIMESPENT / 60000) / 60.0, 2)) As Total FROM ChargesTable CT2 WHERE '[" + startDate.ToString("yyyy-MM-dd") + ", " + endDate.ToString("yyyy-MM-dd") + "]'::daterange @> to_timestamp(CAST(CT2.ts_endtime as bigint)/1000)::date AND CT2.Technicianid = UserID GROUP BY TECHNICIANID) AS TotalHours, (SUM(TimeSpentHrs)/((CASE WHEN UserID=2057 THEN 4 ELSE 8 END)*(1 + '" + endDate.ToString("yyyy-MM-dd") + "'::date - StartDate - (SELECT COUNT(*) FROM generate_series(StartDate, '" + endDate.ToString("yyyy-MM-dd") + "', '1 day'::interval) AS g(d) WHERE EXTRACT(DOW FROM d) IN (0, 6))))) As UtilizationPercentage FROM ( ");
                //strQuery.Append("SELECT (select CASE WHEN MIN(to_timestamp(CAST(CT.ts_endtime as bigint)/1000)::date)>'" + startDate.ToString("yyyy-MM-dd") + "'::date THEN MIN(to_timestamp(CAST(CT.ts_endtime as bigint)/1000)::date) ELSE '" + startDate.ToString("yyyy-MM-dd") + "' END from chargestable CT where CT.Technicianid = CHARGESTABLE.TECHNICIANID) AS StartDate, CHARGESTABLE.TECHNICIANID AS UserId,TECH.FIRST_NAME AS UserName,ACC.EMAILID AS UserEmail,DEPTTABLE.DEPTNAME AS DepartmentName,CISITE.Name AS Location,TO_TIMESTAMP(CHARGESTABLE.TS_ENDTIME / 1000) AS RecordEndDate,ROUND((CHARGESTABLE.TIMESPENT / 60000) / 60.0, 2) AS TimeSpentHrs,CASE WHEN PAD.ORG_ID IS NOT NULL THEN PACI.udf_long2 WHEN WAD.ORG_ID IS NOT NULL THEN WACI.udf_long2 WHEN CAD.ORG_ID IS NOT NULL THEN CACI.udf_long2 WHEN PBAD.ORG_ID IS NOT NULL THEN PBACI.udf_long2 WHEN TAD.ORG_ID IS NOT NULL THEN TACI.udf_long2 END AS ClientId ");
                //strQuery.Append("FROM CHARGESTABLE CHARGESTABLE ");
                //strQuery.Append("LEFT JOIN WORKLOGBILLINGSTATUS WBS ON WBS.CHARGEID = CHARGESTABLE.CHARGEID ");
                //strQuery.Append("LEFT JOIN NOBILLREQUESTCHARGES NBRC ON NBRC.REQUESTCHARGEID = CHARGESTABLE.CHARGEID ");
                //strQuery.Append("LEFT JOIN AAAUSER TECH ON TECH.USER_ID = CHARGESTABLE.TECHNICIANID ");
                //strQuery.Append("LEFT JOIN AAAUSERCONTACTINFO AUCC ON AUCC.USER_ID = TECH.USER_ID ");
                //strQuery.Append("LEFT JOIN AAACONTACTINFO ACC ON ACC.CONTACTINFO_ID = AUCC.CONTACTINFO_ID ");
                //strQuery.Append("LEFT JOIN TASKTOCHARGE TTC ON TTC.CHARGEID = CHARGESTABLE.CHARGEID ");
                //strQuery.Append("LEFT JOIN TASKTOPROJECTS TTP ON TTP.TASKID = TTC.TASKID ");
                //strQuery.Append("LEFT JOIN TASKDETAILS TD ON TD.TASKID = TTC.TASKID ");
                //strQuery.Append("LEFT JOIN PROJECTDETAILS PD ON PD.PROJECTID = TTP.PROJECTID ");
                //strQuery.Append("LEFT JOIN PROJECTACCMAPPING PAM ON PAM.PROJECTID = PD.PROJECTID ");
                //strQuery.Append("LEFT JOIN ACCOUNTDEFINITION PAD ON PAD.ORG_ID = PAM.ACCOUNTID ");
                //strQuery.Append("LEFT JOIN PORTALACCOUNTS PPA ON PAD.ORG_ID = PPA.ACCOUNTID ");
                //strQuery.Append("LEFT JOIN ACCOUNTADDITIONALFIELDS PACI ON PPA.ACCOUNTID = PACI.Instance_Id ");
                //strQuery.Append("LEFT JOIN WORKORDERTOCHARGE WTC ON WTC.CHARGEID = CHARGESTABLE.CHARGEID ");
                //strQuery.Append("LEFT JOIN WORKORDER WO ON WO.WORKORDERID = WTC.WORKORDERID ");
                //strQuery.Append("LEFT JOIN WORKORDERSTATES WOS ON WO.WORKORDERID = WOS.WORKORDERID ");
                //strQuery.Append("LEFT JOIN REQUESTTYPEDEFINITION RTD ON WOS.REQUESTTYPEID = RTD.REQUESTTYPEID ");
                //strQuery.Append("LEFT JOIN WORKORDERACCOUNTMAPPING WAM ON WAM.WORKORDERID = WO.WORKORDERID ");
                //strQuery.Append("LEFT JOIN ACCOUNTDEFINITION WAD ON WAD.ORG_ID = WAM.ACCOUNTID ");
                //strQuery.Append("LEFT JOIN PORTALACCOUNTS WPA ON WAD.ORG_ID = WPA.ACCOUNTID ");
                //strQuery.Append("LEFT JOIN ACCOUNTADDITIONALFIELDS WACI ON WPA.ACCOUNTID = WACI.Instance_Id ");
                //strQuery.Append("LEFT JOIN CHANGETOCHARGE CTC ON CTC.CHARGEID = CHARGESTABLE.CHARGEID ");
                //strQuery.Append("LEFT JOIN CHANGEDETAILS CD ON CD.CHANGEID = CTC.CHANGEID ");
                //strQuery.Append("LEFT JOIN CHANGEACCOUNTMAPPING CAM ON CAM.CHANGEID = CD.CHANGEID ");
                //strQuery.Append("LEFT JOIN ACCOUNTDEFINITION CAD ON CAD.ORG_ID = CAM.ACCOUNTID ");
                //strQuery.Append("LEFT JOIN PORTALACCOUNTS CPA ON CAD.ORG_ID = CPA.ACCOUNTID ");
                //strQuery.Append("LEFT JOIN ACCOUNTADDITIONALFIELDS CACI ON CPA.ACCOUNTID = CACI.Instance_Id ");
                //strQuery.Append("LEFT JOIN PROBLEMTOCHARGE PTC ON PTC.CHARGEID = CHARGESTABLE.CHARGEID ");
                //strQuery.Append("LEFT JOIN PROBLEM P ON P.PROBLEMID = PTC.PROBLEMID ");
                //strQuery.Append("LEFT JOIN PROBLEMACCOUNTMAPPING PBAM ON PBAM.PROBLEMID = P.PROBLEMID ");
                //strQuery.Append("LEFT JOIN ACCOUNTDEFINITION PBAD ON PBAD.ORG_ID = PBAM.ACCOUNTID ");
                //strQuery.Append("LEFT JOIN PORTALACCOUNTS PBPA ON PBAD.ORG_ID = PBPA.ACCOUNTID ");
                //strQuery.Append("LEFT JOIN ACCOUNTADDITIONALFIELDS PBACI ON PBPA.ACCOUNTID = PBACI.Instance_Id ");
                //strQuery.Append("LEFT JOIN TASKACCOUNTMAPPING TAM ON TAM.TASKID = TD.TASKID ");
                //strQuery.Append("LEFT JOIN ACCOUNTDEFINITION TAD ON TAD.ORG_ID = TAM.ACCOUNTID ");
                //strQuery.Append("LEFT JOIN PORTALACCOUNTS TAPA ON TAPA.ACCOUNTID = TAD.ORG_ID ");
                //strQuery.Append("LEFT JOIN ACCOUNTADDITIONALFIELDS TACI ON TACI.Instance_Id = TAPA.ACCOUNTID ");
                //strQuery.Append("LEFT JOIN USERDEPARTMENT USERDEPT ON TECH.USER_ID = USERDEPT.USERID  ");
                //strQuery.Append("LEFT JOIN DEPARTMENTDEFINITION DEPTTABLE ON USERDEPT.DEPTID = DEPTTABLE.DEPTID ");
                //strQuery.Append("LEFT JOIN SDOrganization CISITE ON DEPTTABLE.SITEID = CISITE.ORG_ID ");
                //strQuery.Append("LEFT JOIN SDUser SD ON SD.USERID = CHARGESTABLE.TECHNICIANID ");
                //strQuery.Append("WHERE '[" + startDate.ToString("yyyy-MM-dd") + ", " + endDate.ToString("yyyy-MM-dd") + "]'::daterange @> to_timestamp(CAST(chargesTable.ts_endtime as bigint)/1000)::date AND CISITE.Name <> 'Client' AND SD.EmployeeId = 'Emp' AND NBRC.ISBILLABLEWORKLOG = true ");
                //strQuery.Append(") t WHERE ClientId <> '6' ");
                //strQuery.Append("GROUP BY UserId, UserName, UserEmail, DepartmentName, Location, StartDate ORDER BY UserName) AS S ");
                //strQuery.Append("ON USR.USER_ID = S.UserId AND USR.FIRST_NAME = S.UserName ");
                //strQuery.Append("WHERE FIRST_NAME <> 'administrator' AND ORG_NAME = 'Vigilant Technologies' AND CISITE.Name <> 'Client' AND SD.EmployeeId = 'Emp' AND SD.Status = 'ACTIVE') x ");
                //strQuery.Append("GROUP BY DepartmentName, StartDate, EndDate, BusinessDays) y ");
                //if (objRpt.Department != null)
                //{
                //    var strDepartments = "'" + objRpt.Department.Replace("'", "''").Replace(",", "','") + "'";
                //    strQuery.Append("WHERE (DepartmentName IN (" + strDepartments + ")) ");
                //}
                //strQuery.Append("GROUP BY DepartmentName ORDER BY DepartmentName");
                ////var txtQuery = strQuery.ToString();

                strQuery.Append("WITH params AS ( ");
                strQuery.Append("SELECT '" + startDate + "'::date AS period_start, '" + endDate + "'::date AS period_end, ");
                strQuery.Append("(1 + '" + endDate + "'::date - '" + startDate + "'::date ");
                strQuery.Append("- (SELECT COUNT(*) ");
                strQuery.Append("FROM generate_series('" + startDate + "', '" + endDate + "', '1 day'::interval) g(d) ");
                strQuery.Append(" WHERE EXTRACT(DOW FROM d) IN(0, 6)) ");
                strQuery.Append(") AS biz_days ");
                strQuery.Append("), ");
                strQuery.Append("billable AS ( ");
                strQuery.Append("SELECT CT.TECHNICIANID AS UserId, TECH.FIRST_NAME AS UserName,  ACC.EMAILID AS UserEmail, ");
                strQuery.Append("DEPTTABLE.DEPTNAME AS DepartmentName, CISITE.Name AS Location, ");
                strQuery.Append("CASE WHEN MIN(to_timestamp(CT.TS_ENDTIME / 1000)::date) > (SELECT period_start FROM params) ");
                strQuery.Append("THEN MIN(to_timestamp(CT.TS_ENDTIME/ 1000)::date) ");
                strQuery.Append("ELSE (SELECT period_start FROM params) ");
                strQuery.Append("END AS StartDate, ");
                strQuery.Append("(SELECT period_end FROM params) AS EndDate,");
                strQuery.Append("SUM(ROUND((CT.TIMESPENT / 60000) / 60.0, 2)) AS BillableHours,");
                strQuery.Append("CASE WHEN CT.TECHNICIANID = 2057 THEN 4 ELSE 8 END AS DailyHrs ");
                strQuery.Append("FROM CHARGESTABLE CT ");
                strQuery.Append("LEFT JOIN NOBILLREQUESTCHARGES NBRC ON NBRC.REQUESTCHARGEID = CT.CHARGEID ");
                strQuery.Append("LEFT JOIN AAAUSER TECH ON TECH.USER_ID = CT.TECHNICIANID ");
                strQuery.Append("LEFT JOIN AAAUSERCONTACTINFO AUCC ON AUCC.USER_ID = TECH.USER_ID ");
                strQuery.Append("LEFT JOIN AAACONTACTINFO ACC  ON ACC.CONTACTINFO_ID = AUCC.CONTACTINFO_ID ");
                strQuery.Append("LEFT JOIN TASKTOCHARGE TTC ON TTC.CHARGEID = CT.CHARGEID ");
                strQuery.Append("LEFT JOIN TASKTOPROJECTS TTP ON TTP.TASKID = TTC.TASKID ");
                strQuery.Append("LEFT JOIN TASKDETAILS TD ON TD.TASKID = TTC.TASKID ");
                strQuery.Append("LEFT JOIN PROJECTDETAILS PD ON PD.PROJECTID = TTP.PROJECTID ");
                strQuery.Append("LEFT JOIN PROJECTACCMAPPING PAM ON PAM.PROJECTID = PD.PROJECTID ");
                strQuery.Append("LEFT JOIN ACCOUNTDEFINITION PAD ON PAD.ORG_ID = PAM.ACCOUNTID ");
                strQuery.Append("LEFT JOIN PORTALACCOUNTS PPA ON PAD.ORG_ID = PPA.ACCOUNTID ");
                strQuery.Append("LEFT JOIN ACCOUNTADDITIONALFIELDS PACI ON PPA.ACCOUNTID = PACI.Instance_Id ");
                strQuery.Append("LEFT JOIN WORKORDERTOCHARGE WTC ON WTC.CHARGEID = CT.CHARGEID ");
                strQuery.Append("LEFT JOIN WORKORDER WO ON WO.WORKORDERID = WTC.WORKORDERID ");
                strQuery.Append("LEFT JOIN WORKORDERSTATES WOS ON WO.WORKORDERID = WOS.WORKORDERID ");
                strQuery.Append("LEFT JOIN REQUESTTYPEDEFINITION RTD ON WOS.REQUESTTYPEID = RTD.REQUESTTYPEID ");
                strQuery.Append("LEFT JOIN WORKORDERACCOUNTMAPPING WAM ON WAM.WORKORDERID = WO.WORKORDERID ");
                strQuery.Append("LEFT JOIN ACCOUNTDEFINITION WAD ON WAD.ORG_ID = WAM.ACCOUNTID ");
                strQuery.Append("LEFT JOIN PORTALACCOUNTS WPA ON WAD.ORG_ID = WPA.ACCOUNTID ");
                strQuery.Append("LEFT JOIN ACCOUNTADDITIONALFIELDS WACI ON WPA.ACCOUNTID = WACI.Instance_Id ");
                strQuery.Append("LEFT JOIN CHANGETOCHARGE CTC ON CTC.CHARGEID = CT.CHARGEID ");
                strQuery.Append("LEFT JOIN CHANGEDETAILS CD ON CD.CHANGEID = CTC.CHANGEID ");
                strQuery.Append("LEFT JOIN CHANGEACCOUNTMAPPING CAM ON CAM.CHANGEID = CD.CHANGEID ");
                strQuery.Append("LEFT JOIN ACCOUNTDEFINITION CAD ON CAD.ORG_ID = CAM.ACCOUNTID ");
                strQuery.Append("LEFT JOIN PORTALACCOUNTS CPA ON CAD.ORG_ID = CPA.ACCOUNTID ");
                strQuery.Append("LEFT JOIN ACCOUNTADDITIONALFIELDS CACI ON CPA.ACCOUNTID = CACI.Instance_Id ");
                strQuery.Append("LEFT JOIN PROBLEMTOCHARGE PTC ON PTC.CHARGEID = CT.CHARGEID ");
                strQuery.Append("LEFT JOIN PROBLEM P ON P.PROBLEMID = PTC.PROBLEMID ");
                strQuery.Append("LEFT JOIN PROBLEMACCOUNTMAPPING PBAM ON PBAM.PROBLEMID = P.PROBLEMID ");
                strQuery.Append("LEFT JOIN ACCOUNTDEFINITION PBAD ON PBAD.ORG_ID = PBAM.ACCOUNTID ");
                strQuery.Append("LEFT JOIN PORTALACCOUNTS PBPA ON PBAD.ORG_ID = PBPA.ACCOUNTID ");
                strQuery.Append("LEFT JOIN ACCOUNTADDITIONALFIELDS PBACI ON PBPA.ACCOUNTID = PBACI.Instance_Id ");
                strQuery.Append("LEFT JOIN TASKACCOUNTMAPPING TAM ON TAM.TASKID = TD.TASKID ");
                strQuery.Append("LEFT JOIN ACCOUNTDEFINITION TAD ON TAD.ORG_ID = TAM.ACCOUNTID ");
                strQuery.Append("LEFT JOIN PORTALACCOUNTS TAPA ON TAPA.ACCOUNTID = TAD.ORG_ID ");
                strQuery.Append("LEFT JOIN ACCOUNTADDITIONALFIELDS TACI ON TACI.Instance_Id = TAPA.ACCOUNTID ");
                strQuery.Append("LEFT JOIN USERDEPARTMENT USERDEPT ON TECH.USER_ID = USERDEPT.USERID ");
                strQuery.Append("LEFT JOIN DEPARTMENTDEFINITION DEPTTABLE ON USERDEPT.DEPTID = DEPTTABLE.DEPTID ");
                strQuery.Append("LEFT JOIN SDOrganization CISITE ON DEPTTABLE.SITEID = CISITE.ORG_ID ");
                strQuery.Append("LEFT JOIN SDUser SD ON SD.USERID = CT.TECHNICIANID ");
                strQuery.Append(" WHERE ");
                strQuery.Append("(SELECT '[' || period_start || ',' || period_end || ']' ");
                strQuery.Append("FROM params)::daterange ");
                strQuery.Append(" @> to_timestamp(CT.TS_ENDTIME / 1000)::date ");
                strQuery.Append("AND CISITE.Name<> 'Client' ");
                strQuery.Append("AND SD.EmployeeId = 'Emp' ");
                strQuery.Append("AND NBRC.ISBILLABLEWORKLOG = true ");
                strQuery.Append("AND CASE ");
                strQuery.Append("WHEN PAD.ORG_ID IS NOT NULL THEN PACI.udf_long2 ");
                strQuery.Append("WHEN WAD.ORG_ID  IS NOT NULL THEN WACI.udf_long2 ");
                strQuery.Append("WHEN CAD.ORG_ID IS NOT NULL THEN CACI.udf_long2 ");
                strQuery.Append("WHEN PBAD.ORG_ID IS NOT NULL THEN PBACI.udf_long2 ");
                strQuery.Append("WHEN TAD.ORG_ID IS NOT NULL THEN TACI.udf_long2 ");
                strQuery.Append("END<> '6' ");
                strQuery.Append("GROUP BY ");
                strQuery.Append("CT.TECHNICIANID, TECH.FIRST_NAME, ACC.EMAILID, ");
                strQuery.Append("DEPTTABLE.DEPTNAME, CISITE.Name, ");
                strQuery.Append("CASE WHEN CT.TECHNICIANID = 2057 THEN 4 ELSE 8 END ), ");
                strQuery.Append("total_hrs AS ( ");
                strQuery.Append("SELECT CT2.TECHNICIANID, SUM(ROUND((CT2.TIMESPENT / 60000) / 60.0, 2)) AS TotalHours ");
                strQuery.Append("FROM ChargesTable CT2 ");
                strQuery.Append("WHERE(SELECT '[' || period_start || ',' || period_end || ']' ");
                strQuery.Append("FROM params)::daterange ");
                strQuery.Append("@> to_timestamp(CT2.TS_ENDTIME / 1000)::date ");
                strQuery.Append("GROUP BY CT2.TECHNICIANID ),");
                strQuery.Append("tech_metrics AS ( ");
                strQuery.Append("SELECT USR.USER_ID, USR.FIRST_NAME, ACI.EMAILID, DEPTTABLE.DEPTNAME AS DepartmentName, ");
                strQuery.Append("CISITE.Name AS Location, COALESCE(B.StartDate, params.period_start) AS StartDate, ");
                strQuery.Append("params.period_end  AS EndDate, params.biz_days AS BusinessDays,");
                strQuery.Append("COALESCE(B.BillableHours, 0) AS BillableHours, ");
                strQuery.Append("COALESCE(TH.TotalHours, 0) AS TotalHours, ");
                strQuery.Append("CASE WHEN DEPTTABLE.DEPTNAME = 'PMO' ");
                strQuery.Append("THEN(params.biz_days * 8) - (params.biz_days * 4) ");
                strQuery.Append("ELSE  params.biz_days * 8 END AS AvailableHours ");
                strQuery.Append("FROM AAAUSER USR ");
                strQuery.Append("CROSS JOIN params ");
                strQuery.Append("JOIN AAALOGIN LOG ON LOG.USER_ID = USR.USER_ID ");
                strQuery.Append("JOIN requesteraccountmapping MAPP ON MAPP.USERID = USR.USER_ID ");
                strQuery.Append("JOIN aaausercontactinfo UCI ON UCI.USER_ID = USR.USER_ID ");
                strQuery.Append("JOIN portaltechnicians PT ON PT.USERID = USR.USER_ID ");
                strQuery.Append("LEFT JOIN accountdefinition ORGACC ON ORGACC.ORG_ID = MAPP.ACCOUNTID ");
                strQuery.Append("LEFT JOIN aaacontactinfo ACI ON ACI.contactinfo_id = UCI.contactinfo_id ");
                strQuery.Append("LEFT JOIN USERDEPARTMENT USERDEPT ON USERDEPT.USERID = USR.USER_ID ");
                strQuery.Append("LEFT JOIN DEPARTMENTDEFINITION DEPTTABLE ON DEPTTABLE.DEPTID = USERDEPT.DEPTID ");
                strQuery.Append("LEFT JOIN SDOrganization CISITE ON CISITE.ORG_ID = DEPTTABLE.SITEID ");
                strQuery.Append("LEFT JOIN SDUser SD ON SD.USERID = USR.USER_ID ");
                strQuery.Append("LEFT JOIN billable B ON B.UserId = USR.USER_ID ");
                strQuery.Append("LEFT JOIN total_hrs TH ON TH.TECHNICIANID = USR.USER_ID ");
                strQuery.Append("WHERE USR.FIRST_NAME<> 'administrator' AND ORGACC.ORG_NAME = 'Vigilant Technologies' ");
                strQuery.Append("AND CISITE.Name<> 'Client' AND SD.EmployeeId = 'Emp' AND SD.Status = 'ACTIVE' ) ");
                strQuery.Append("SELECT DepartmentName, MIN(StartDate) AS StartDate, MAX(EndDate) AS EndDate, ");
                strQuery.Append(" MAX(BusinessDays) AS BusinessDays, SUM(BillableHours) AS BillableHours, ");
                strQuery.Append("SUM(TotalHours) AS TotalHours, ");
                strQuery.Append("SUM(BillableHours) / NULLIF(SUM(AvailableHours), 0) AS UtilizationPercentage ");
                strQuery.Append("FROM tech_metrics ");
                if (objRpt.Department != null)
                {
                    var strDepartments = "'" + objRpt.Department.Replace("'", "''").Replace(",", "','") + "'";
                    strQuery.Append("WHERE (DepartmentName IN (" + strDepartments + ")) ");
                }
                strQuery.Append("GROUP BY DepartmentName ");
                strQuery.Append("ORDER BY DepartmentName;");

                using NpgsqlCommand cmd = new NpgsqlCommand(strQuery.ToString(), connection);
                using NpgsqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var objReport = new Utilization();
                    objReport.DepartmentName = (string)(reader["DepartmentName"] is DBNull ? "" : reader["DepartmentName"]);
                    objReport.StartDate = Convert.ToDateTime(reader["StartDate"]).ToString("yyyy'-'MMM'-'dd").Replace(".", "");
                    objReport.EndDate = Convert.ToDateTime(reader["EndDate"]).ToString("yyyy'-'MMM'-'dd").Replace(".", "");
                    objReport.BusinessDays = Convert.ToInt32(reader["BusinessDays"]);
                    objReport.BillableHours = Convert.ToDecimal(reader["BillableHours"]);
                    objReport.NonBillableHours = Convert.ToDecimal(reader["TotalHours"]) - Convert.ToDecimal(reader["BillableHours"]);
                    objReport.UtilizationPercentage = Convert.ToDecimal(reader["UtilizationPercentage"]).ToString("P2");
                    lstReportData.Add(objReport);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetReportData(): " + ex.ToString());
            }
            finally
            {
                connection.Close();
            }
            return [.. lstReportData];
        }

        public List<Inventory> GetReportDataInventory(string dbConnection)
        {
            List<Inventory> lstReportData = new List<Inventory>();

            using NpgsqlConnection connection = new NpgsqlConnection(dbConnection);
            try
            {
                connection.Open();
                StringBuilder strQuery = new StringBuilder();
                //strQuery.Append("SELECT attribute_302 AS ClientID, attribute_604 AS ClientName, attribute_303 AS VCollabID, org_id AS AccountID, org_name AS AccountName, PD.projectid AS ProjectID, PD.title AS ProjectName ");
                //strQuery.Append("FROM accountdefinition AD ");
                //strQuery.Append("JOIN portalaccounts PA ON PA.accountid = AD.org_id ");
                //strQuery.Append("JOIN accountci CI ON CI.ciid = PA.ciid ");
                //strQuery.Append("LEFT JOIN PROJECTACCMAPPING PAM ON PAM.accountid = PA.accountid ");
                //strQuery.Append("LEFT JOIN PROJECTDETAILS PD ON PD.projectid = PAM.projectid ");
                //strQuery.Append("ORDER BY ClientName, AccountName, ProjectName");
                //Existing Code Start
                //strQuery.Append("SELECT udf_long2 AS ClientID, udf_char5 AS ClientName, udf_long1 AS VCollabID, org_id AS AccountID, org_name AS AccountName, PD.projectid AS ProjectID, PD.title AS ProjectName ");
                //strQuery.Append("FROM accountdefinition AD ");
                //strQuery.Append("JOIN portalaccounts PA ON PA.accountid = AD.org_id ");
                //strQuery.Append("JOIN ACCOUNTADDITIONALFIELDS CI ON CI.Instance_Id = PA.ACCOUNTID ");
                //strQuery.Append("LEFT JOIN PROJECTACCMAPPING PAM ON PAM.accountid = PA.accountid ");
                //strQuery.Append("LEFT JOIN PROJECTDETAILS PD ON PD.projectid = PAM.projectid ");
                // strQuery.Append("ORDER BY ClientName, AccountName, ProjectName");
                //var txtQuery = strQuery.ToString();
                //Existing Code End
                //New Code Start
                //------------------------
                strQuery.Append("SELECT CI.udf_long2 AS ClientID, CI.udf_char5 AS ClientName, CI.udf_long1 AS VCollabID,AD.org_id AS AccountID,");
                strQuery.Append("AD.org_name AS AccountName, PACI.UDF_CHAR4 AS AccountActivity,");
                strQuery.Append("(SELECT TO_CHAR(TO_TIMESTAMP(MAX(PD2.createdtime) / 1000), 'YYYY-Mon-DD') ");
                strQuery.Append("FROM PROJECTDETAILS PD2 ");
                strQuery.Append("JOIN PROJECTACCMAPPING PAM2 ON PAM2.PROJECTID = PD2.PROJECTID ");
                strQuery.Append("WHERE PAM2.ACCOUNTID = AD.ORG_ID ) AS CreatedDate, ");
                strQuery.Append("(SELECT TO_CHAR(TO_TIMESTAMP(MAX(CH.ts_endtime) / 1000),'YYYY-Mon-DD') ");
                strQuery.Append("FROM CHARGESTABLE CH ");
                strQuery.Append("JOIN TASKTOCHARGE TTC ON TTC.CHARGEID = CH.CHARGEID ");
                strQuery.Append("JOIN TASKTOPROJECTS TTP ON TTP.TASKID = TTC.TASKID ");
                strQuery.Append("JOIN PROJECTDETAILS PD3 ON PD3.PROJECTID = TTP.PROJECTID ");
                strQuery.Append("JOIN PROJECTACCMAPPING PAM3 ON PAM3.PROJECTID = PD3.PROJECTID ");
                strQuery.Append("WHERE PAM3.ACCOUNTID = AD.ORG_ID) AS LastWorkLog, ");
                strQuery.Append("PD.ProjectID AS ProjectID, PD.Title AS ProjectName ");
                strQuery.Append("FROM ACCOUNTDEFINITION AD ");
                strQuery.Append("JOIN PORTALACCOUNTS PA ON PA.AccountID = AD.ORG_ID ");
                strQuery.Append("JOIN ACCOUNTADDITIONALFIELDS CI ON CI.Instance_Id = PA.AccountID ");
                strQuery.Append("LEFT JOIN PROJECTACCMAPPING PAM ON PAM.AccountID = PA.AccountID ");
                strQuery.Append("LEFT JOIN PROJECTDETAILS PD ON PD.ProjectID = PAM.ProjectID ");
                strQuery.Append("LEFT JOIN ACCOUNTADDITIONALFIELDS PACI ON PACI.Instance_Id = PA.AccountID ");
                strQuery.Append("ORDER BY ClientName, AccountName, ProjectName;");
                //New Code End
                //------------------------
                using NpgsqlCommand cmd = new NpgsqlCommand(strQuery.ToString(), connection);
                using NpgsqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var objReport = new Inventory()
                    {
                        AccountId = (reader["AccountId"] is DBNull ? "" : reader["AccountId"].ToString()),
                        AccountName = (string)(reader["AccountName"] is DBNull ? "" : reader["AccountName"]),
                        ClientId = (reader["ClientId"] is DBNull ? "" : reader["ClientId"].ToString()),
                        ClientName = (string)(reader["ClientName"] is DBNull ? "" : reader["ClientName"]),
                        ProjectId = (reader["ProjectId"] is DBNull ? "" : reader["ProjectId"].ToString()),
                        ProjectName = (string)(reader["ProjectName"] is DBNull ? "" : reader["ProjectName"]),
                        VCollabId = (reader["VCollabID"] is DBNull ? "" : reader["VCollabID"].ToString()),
                        CreatedDate = (reader["CreatedDate"] is DBNull ? "" : reader["CreatedDate"].ToString()),
                        AccountActivity = (reader["AccountActivity"] is DBNull ? "" : reader["AccountActivity"].ToString()),
                        LastWorkLog = (reader["LastWorkLog"] is DBNull ? "" : reader["LastWorkLog"].ToString())
                    };

                    lstReportData.Add(objReport);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetReportData(): " + ex.ToString());
            }
            finally
            {
                connection.Close();
            }
            return [.. lstReportData];
        }

        public string StandardQueryFilter(string dbConnection, ReportFilter objRpt, string loginName, string fullName)
        {
            StringBuilder strFilters = new StringBuilder();
            bool isAdmin = false;
            bool isAppManager = false;
            string path = "wwwroot/Temp/admins.txt";
            using (StreamReader readers = new StreamReader(path))
            {
                while (!readers.EndOfStream)
                {
                    if (readers.ReadLine() == loginName)
                    {
                        isAdmin = true;
                    }
                }
            }

            if (!isAdmin)
            {
                string sql = "SELECT AAAUSER.FIRST_NAME FROM AAAUSER AAAUSER LEFT JOIN SDUSER SDUSER ON AAAUSER.USER_ID = SDUSER.USERID INNER JOIN PORTALTECHNICIANS POTECH ON SDUSER.USERID = POTECH.USERID LEFT JOIN AAALOGIN AAL ON POTECH.USERID = AAL.USER_ID LEFT JOIN AAAACCOUNT ACC ON AAL.LOGIN_ID = ACC.LOGIN_ID LEFT JOIN SDAUTHORIZEDROLE AUTH ON ACC.ACCOUNT_ID = AUTH.ACCOUNT_ID LEFT JOIN AAAROLE AROLE ON AUTH.ROLE_ID = AROLE.ROLE_ID LEFT JOIN AAAUSERCONTACTINFO AAAUSERCONTACT ON AAAUSER.USER_ID = AAAUSERCONTACT.USER_ID LEFT JOIN AAACONTACTINFO AAACONTACT ON AAAUSERCONTACT.CONTACTINFO_ID = AAACONTACT.CONTACTINFO_ID LEFT JOIN USERDEPARTMENT USERDEPT ON POTECH.USERID = USERDEPT.USERID LEFT JOIN DEPARTMENTDEFINITION DEPTTABLE ON USERDEPT.DEPTID = DEPTTABLE.DEPTID WHERE AROLE.ROLE_ID = 2102 AND AAL.NAME = '" + loginName + "'";

                using NpgsqlConnection conn = new NpgsqlConnection(dbConnection);
                conn.Open();
                using NpgsqlCommand cmdAdmin = new NpgsqlCommand(sql, conn);
                using NpgsqlDataReader readerAdmin = cmdAdmin.ExecuteReader();

                while (readerAdmin.Read())
                {
                    isAdmin = true;
                }
            }

            // Check if the user is an Application Manager
            if (!isAdmin)
            {
                string sql = "SELECT AAAUSER.FIRST_NAME FROM AAAUSER AAAUSER LEFT JOIN SDUSER SDUSER ON AAAUSER.USER_ID = SDUSER.USERID INNER JOIN PORTALTECHNICIANS POTECH ON SDUSER.USERID = POTECH.USERID LEFT JOIN AAALOGIN AAL ON POTECH.USERID = AAL.USER_ID LEFT JOIN AAAACCOUNT ACC ON AAL.LOGIN_ID = ACC.LOGIN_ID LEFT JOIN SDAUTHORIZEDROLE AUTH ON ACC.ACCOUNT_ID = AUTH.ACCOUNT_ID LEFT JOIN AAAROLE AROLE ON AUTH.ROLE_ID = AROLE.ROLE_ID LEFT JOIN AAAUSERCONTACTINFO AAAUSERCONTACT ON AAAUSER.USER_ID = AAAUSERCONTACT.USER_ID LEFT JOIN AAACONTACTINFO AAACONTACT ON AAAUSERCONTACT.CONTACTINFO_ID = AAACONTACT.CONTACTINFO_ID LEFT JOIN USERDEPARTMENT USERDEPT ON POTECH.USERID = USERDEPT.USERID LEFT JOIN DEPARTMENTDEFINITION DEPTTABLE ON USERDEPT.DEPTID = DEPTTABLE.DEPTID WHERE AROLE.ROLE_ID = 2701 AND AAL.NAME = '" + loginName + "'";
                using NpgsqlConnection conn = new NpgsqlConnection(dbConnection);
                conn.Open();
                using NpgsqlCommand cmdAdmin = new NpgsqlCommand(sql, conn);
                using NpgsqlDataReader readerAppManager = cmdAdmin.ExecuteReader();

                while (readerAppManager.Read())
                {
                    isAppManager = true;
                }
            }

            if (isAdmin)
            {
                if (objRpt.UserId != null)
                {
                    strFilters.Append(" AND TECH.USER_ID IN(" + objRpt.UserId + ")");
                }
            }
            else if (isAppManager)
            {
                if (!string.IsNullOrEmpty(objRpt.UserId))
                {

                    UserData objUser = new UserData();
                    int userId = objUser.GetUserId(dbConnection, loginName);
                    string accountIds = objUser.GetAccountIds(dbConnection, userId);
                    int countUserId = objRpt.UserId.Split(',').Length;
                    int currentUserId = 0;
                    if (countUserId == 1)
                    {
                        currentUserId = int.Parse(objRpt.UserId);
                    }
                    if (userId == currentUserId && string.IsNullOrEmpty(objRpt.AccountId))
                    {
                        strFilters.Append(" AND TECH.FIRST_NAME = '" + fullName + "'");
                    }
                    else
                    {
                        strFilters.Append(" AND TECH.USER_ID IN(" + objRpt.UserId + ")");
                        strFilters.Append(" AND (PAD.ORG_ID IN (" + accountIds + ") OR WAD.ORG_ID IN (" + accountIds + ") OR CAD.ORG_ID IN (" + accountIds + ") OR PBAD.ORG_ID IN (" + accountIds + ") OR TAD.ORG_ID IN (" + accountIds + "))");
                    }
                }
            }
            else
            {
                strFilters.Append(" AND TECH.FIRST_NAME = '" + fullName + "'");
            }

            if (objRpt.ClientId != null)
            {
                var strClients = "'" + objRpt.ClientId.Replace("'", "''").Replace(",", "','") + "'";
                strFilters.Append(" AND (PBACI.udf_char5 IN (" + strClients + ") OR CACI.udf_char5 IN(" + strClients + ") OR WACI.udf_char5 IN (" + strClients + ") OR PACI.udf_char5 IN (" + strClients + ") OR TACI.udf_char5 IN (" + strClients + "))");
            }
            if (objRpt.AccountId != null)
            {
                strFilters.Append(" AND (PAD.ORG_ID IN (" + objRpt.AccountId + ") OR WAD.ORG_ID IN (" + objRpt.AccountId + ") OR CAD.ORG_ID IN (" + objRpt.AccountId + ") OR PBAD.ORG_ID IN (" + objRpt.AccountId + ") OR TAD.ORG_ID IN (" + objRpt.AccountId + "))");
            }
            else
            {
                if (isAppManager && objRpt.UserId == null)
                {
                    UserData objUser = new UserData();
                    int userId = objUser.GetUserId(dbConnection, loginName);
                    string accountIds = objUser.GetAccountIds(dbConnection, userId);

                    strFilters.Append(" AND (PAD.ORG_ID IN (" + accountIds + ") OR WAD.ORG_ID IN (" + accountIds + ") OR CAD.ORG_ID IN (" + accountIds + ") OR PBAD.ORG_ID IN (" + accountIds + ") OR TAD.ORG_ID IN (" + accountIds + "))");
                }

            }
            if (objRpt.Module != null)
            {
                if (objRpt.Module == "Project")
                {
                    strFilters.Append(" AND PD.PROJECTID IS NOT NULL");
                }
                if (objRpt.Module == "Request")
                {
                    strFilters.Append(" AND WO.WORKORDERID IS NOT NULL");
                }
                if (objRpt.Module == "Change")
                {
                    strFilters.Append(" AND CD.CHANGEID IS NOT NULL");
                }
                if (objRpt.Module == "Problem")
                {
                    strFilters.Append(" AND P.PROBLEMID IS NOT NULL");
                }
                if (objRpt.Module == "Change,Problem,Project,Request")
                {
                    strFilters.Append(" AND (CD.CHANGEID IS NOT NULL OR P.PROBLEMID IS NOT NULL OR PD.PROJECTID IS NOT NULL OR WO.WORKORDERID IS NOT NULL) ");
                }
                if (objRpt.Module == "Change,Problem,Project")
                {
                    strFilters.Append(" AND (CD.CHANGEID IS NOT NULL OR P.PROBLEMID IS NOT NULL OR PD.PROJECTID IS NOT NULL)");
                }
                if (objRpt.Module == "Change,Problem,Request")
                {
                    strFilters.Append(" AND (CD.CHANGEID IS NOT NULL OR P.PROBLEMID IS NOT NULL OR WO.WORKORDERID IS NOT NULL)");
                }
                if (objRpt.Module == "Change,Project,Request")
                {
                    strFilters.Append(" AND (CD.CHANGEID IS NOT NULL OR PD.PROJECTID IS NOT NULL OR WO.WORKORDERID IS NOT NULL)");
                }
                if (objRpt.Module == "Change,Project")
                {
                    strFilters.Append(" AND (CD.CHANGEID IS NOT NULL OR PD.PROJECTID IS NOT NULL)");
                }
                if (objRpt.Module == "Change,Problem")
                {
                    strFilters.Append(" AND (CD.CHANGEID IS NOT NULL OR P.PROBLEMID IS NOT NULL)");
                }
                if (objRpt.Module == "Change,Request")
                {
                    strFilters.Append(" AND (CD.CHANGEID IS NOT NULL OR WO.WORKORDERID IS NOT NULL)");
                }
                if (objRpt.Module == "Problem,Project,Request")
                {
                    strFilters.Append(" AND (P.PROBLEMID IS NOT NULL OR PD.PROJECTID IS NOT NULL OR WO.WORKORDERID IS NOT NULL)");
                }
                if (objRpt.Module == "Problem,Project")
                {
                    strFilters.Append(" AND (P.PROBLEMID IS NOT NULL OR PD.PROJECTID IS NOT NULL)");
                }
                if (objRpt.Module == "Problem,Request")
                {
                    strFilters.Append(" AND (P.PROBLEMID IS NOT NULL OR WO.WORKORDERID IS NOT NULL)");
                }
                if (objRpt.Module == "Project,Request")
                {
                    strFilters.Append(" AND (PD.PROJECTID IS NOT NULL OR WO.WORKORDERID IS NOT NULL)");
                }
            }
            if (objRpt.RequestType != null)
            {
                strFilters.Append(" AND (RTD.NAME = '" + objRpt.RequestType + "')");
            }
            if (objRpt.IsBillable != null)
            {
                strFilters.Append(" AND (NBRC.ISBILLABLEWORKLOG = " + objRpt.IsBillable + ")");
            }
            if (objRpt.Department != null)
            {
                var strDepartments = "'" + objRpt.Department.Replace("'", "''").Replace(",", "','") + "'";
                strFilters.Append(" AND (DEPTTABLE.DEPTNAME IN (" + strDepartments + "))");
            }
            if (objRpt.Location != null)
            {
                var strLocations = "'" + objRpt.Location.Replace("'", "''").Replace(",", "','") + "'";
                strFilters.Append(" AND (CISITE.Name IN (" + strLocations + "))");
            }
            if (objRpt.EmploymentType != null)
            {
                var strEmploymentTypes = "'" + objRpt.EmploymentType.Replace("'", "''").Replace(",", "','") + "'";
                strFilters.Append(" AND (SD.EmployeeId IN (" + strEmploymentTypes + "))");
            }

            return strFilters.ToString();
        }

        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        private string GetAccountId(string dbConnection, string userId)
        {
            string accountIds = string.Empty;
            try
            {
                string strQuery = "SELECT AcctDef.Org_id AS Id FROM TechnicianAccountMapping TAM LEFT JOIN ACCOUNTDEFINITION AcctDef ON TAM.accountid = AcctDef.org_id" +
                       " WHERE TAM.technicianid IN(" + userId + ")";

                using NpgsqlConnection conn = new NpgsqlConnection(dbConnection);
                conn.Open();
                using NpgsqlCommand cmdUser = new NpgsqlCommand(strQuery, conn);
                using NpgsqlDataReader readerUser = cmdUser.ExecuteReader();
                while (readerUser.Read())
                {
                    accountIds += Convert.ToString(readerUser["Id"]) + ",";
                }
                accountIds = accountIds.TrimEnd(',');
            }
            catch (Exception ex)
            {

                throw new Exception(ex.ToString());
            }

            return accountIds;
        }
    }
}