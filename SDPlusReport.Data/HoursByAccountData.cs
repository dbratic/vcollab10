using Npgsql;

using SDPlusReport.Entity;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SDPlusReport.Data
{
    public class HoursByAccountData
    {
        public static DateTime EndDate { get; set; } =  DateTime.Today;
        public static DateTime StartDate { get; set; }= EndDate.AddMonths(-11);
        
        public DataTable GetHoursByAccountDetails(string dbConnection,string strDepartment)
        {
            DataTable dt = new DataTable();

            StringBuilder strQuery = new StringBuilder();

            strQuery.Append("WITH ChargeWithAccount AS ( SELECT C.CHARGEID, C.TIMESPENT, C.ts_endtime, ");
            strQuery.Append("COALESCE(PAM.ACCOUNTID, WAM.ACCOUNTID, CAM.ACCOUNTID, PBAM.ACCOUNTID, TAM.ACCOUNTID) AS ACCOUNT_ID, DEPTTABLE.DEPTNAME AS Department, ");
            strQuery.Append("ROW_NUMBER() OVER ( PARTITION BY C.CHARGEID ORDER BY ");
            strQuery.Append("CASE WHEN PAM.ACCOUNTID  IS NOT NULL THEN 1 ");
            strQuery.Append("WHEN WAM.ACCOUNTID  IS NOT NULL THEN 2 ");
            strQuery.Append("WHEN CAM.ACCOUNTID  IS NOT NULL THEN 3 ");
            strQuery.Append("WHEN PBAM.ACCOUNTID IS NOT NULL THEN 4 ");
            strQuery.Append("WHEN TAM.ACCOUNTID  IS NOT NULL THEN 5 ");
            strQuery.Append("ELSE 6 END ) AS RN ");
            strQuery.Append("FROM CHARGESTABLE C ");
            strQuery.Append("LEFT JOIN AAAUSER TECH ON TECH.USER_ID = C.TECHNICIANID ");
            strQuery.Append("LEFT JOIN USERDEPARTMENT UDPT ON UDPT.USERID = TECH.USER_ID ");
            strQuery.Append("LEFT JOIN DEPARTMENTDEFINITION DEPTTABLE ON DEPTTABLE.DEPTID = UDPT.DEPTID ");
            strQuery.Append("LEFT JOIN TASKTOCHARGE TTC ON TTC.CHARGEID = C.CHARGEID ");
            strQuery.Append("LEFT JOIN TASKTOPROJECTS TTP  ON TTP.TASKID = TTC.TASKID ");
            strQuery.Append("LEFT JOIN PROJECTDETAILS PD ON PD.PROJECTID = TTP.PROJECTID ");
            strQuery.Append("LEFT JOIN PROJECTACCMAPPING PAM  ON PAM.PROJECTID = PD.PROJECTID ");
            strQuery.Append("LEFT JOIN WORKORDERTOCHARGE WTC ON WTC.CHARGEID = C.CHARGEID ");
            strQuery.Append("LEFT JOIN WORKORDER WO ON WO.WORKORDERID = WTC.WORKORDERID ");
            strQuery.Append("LEFT JOIN WORKORDERACCOUNTMAPPING WAM ON WAM.WORKORDERID = WO.WORKORDERID ");
            strQuery.Append("LEFT JOIN CHANGETOCHARGE CTC ON CTC.CHARGEID = C.CHARGEID ");
            strQuery.Append("LEFT JOIN CHANGEDETAILS CD  ON CD.CHANGEID = CTC.CHANGEID ");
            strQuery.Append("LEFT JOIN CHANGEACCOUNTMAPPING CAM ON CAM.CHANGEID = CD.CHANGEID ");
            strQuery.Append("LEFT JOIN PROBLEMTOCHARGE PTC  ON PTC.CHARGEID = C.CHARGEID ");
            strQuery.Append("LEFT JOIN PROBLEM P ON P.PROBLEMID = PTC.PROBLEMID ");
            strQuery.Append("LEFT JOIN PROBLEMACCOUNTMAPPING PBAM ON PBAM.PROBLEMID = P.PROBLEMID ");
            strQuery.Append("LEFT JOIN TASKDETAILS TD  ON TD.TASKID = TTC.TASKID ");
            strQuery.Append("LEFT JOIN TASKACCOUNTMAPPING TAM ON TAM.TASKID = TD.TASKID ");
            strQuery.Append("WHERE TO_TIMESTAMP(C.ts_endtime / 1000)::DATE ");
            strQuery.Append("BETWEEN DATE_TRUNC('month', CURRENT_DATE - INTERVAL '11 months')::DATE ");
            strQuery.Append("AND (DATE_TRUNC('month', CURRENT_DATE + INTERVAL '1 month') - INTERVAL '1 day')::DATE ), ");
            strQuery.Append("DeduplicatedCharges AS ( ");
            strQuery.Append("SELECT CHARGEID, TIMESPENT, ts_endtime, ACCOUNT_ID, Department ");
            strQuery.Append("FROM ChargeWithAccount WHERE RN = 1 ), ");
            strQuery.Append("AccountInfo AS ( SELECT DISTINCT ON (AD.ORG_ID) ");
            strQuery.Append("AD.ORG_ID, AD.ORG_NAME, ACI.udf_char1 AS Category, ACI.UDF_CHAR4 AS Activity ");
            strQuery.Append("FROM ACCOUNTDEFINITION AD ");
            strQuery.Append("LEFT JOIN PORTALACCOUNTS PA ON PA.ACCOUNTID = AD.ORG_ID ");
            strQuery.Append("LEFT JOIN ACCOUNTADDITIONALFIELDS ACI ON ACI.Instance_id = PA.AccountId ");
            strQuery.Append("ORDER BY AD.ORG_ID, ACI.Instance_id ) ");
            strQuery.Append("SELECT AI.ORG_NAME AS AccountName, DC.Department, AI.Category, AI.Activity,");
            strQuery.Append("TO_CHAR( DATE_TRUNC('month', TO_TIMESTAMP(DC.ts_endtime / 1000)::DATE), 'Mon-YY') AS RecordMonth, ");
            strQuery.Append("DATE_TRUNC('month', TO_TIMESTAMP(DC.ts_endtime / 1000)::DATE) AS RecordMonthSort, ");
            strQuery.Append("SUM(ROUND((DC.TIMESPENT / 60000.0) / 60.0, 2)) AS TimeSpentHrs ");
            strQuery.Append("FROM DeduplicatedCharges DC ");
            strQuery.Append("LEFT JOIN AccountInfo AI ON AI.ORG_ID = DC.ACCOUNT_ID ");
            if (!string.IsNullOrEmpty(strDepartment))
            {
                strQuery.Append("AND DEPTTABLE.DEPTNAME IN (" + strDepartment + ") ");
            }
            strQuery.Append("GROUP BY AI.ORG_NAME, DC.Department, AI.Category, AI.Activity, ");
            strQuery.Append("DATE_TRUNC('month', TO_TIMESTAMP(DC.ts_endtime / 1000)::DATE) ");
            strQuery.Append("ORDER BY  AccountName, Department,  RecordMonthSort;");
           
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(dbConnection))
                {
                    using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(strQuery.ToString(), connection))
                    {
                        connection.Open();
                        dataAdapter.Fill(dt);
                    }
                }   
            }
            catch (Exception ex)
            {
                throw new Exception("GetHoursByAccountDetails(): " + ex.ToString());
            }

            return dt;
        }

        public DataTable GetHoursByAccount(string dbConnection)
        {
            DataTable dt = new DataTable();            

            StringBuilder strQuery = new StringBuilder();
            //strQuery.Append("SELECT DEPTTABLE.DEPTNAME AS DEPARTMENT,");
            strQuery.Append("SELECT CASE WHEN PAD.ORG_ID IS NOT NULL THEN PAD.ORG_NAME ");
            strQuery.Append("WHEN WAD.ORG_ID IS NOT NULL THEN WAD.ORG_NAME ");
            strQuery.Append("WHEN CAD.ORG_ID IS NOT NULL THEN CAD.ORG_NAME ");
            strQuery.Append("WHEN PBAD.ORG_ID IS NOT NULL THEN PBAD.ORG_NAME ");
            strQuery.Append("WHEN TAD.ORG_ID IS NOT NULL THEN TAD.ORG_NAME END AS AccountName,");
            strQuery.Append("CASE WHEN PAD.ORG_ID IS NOT NULL THEN PACI.udf_char1 ");
            strQuery.Append("WHEN WAD.ORG_ID IS NOT NULL THEN WACI.udf_char1 ");
            strQuery.Append("WHEN CAD.ORG_ID IS NOT NULL THEN CACI.udf_char1 ");
            strQuery.Append("WHEN PBAD.ORG_ID IS NOT NULL THEN PBACI.udf_char1 ");
            strQuery.Append("WHEN TAD.ORG_ID IS NOT NULL  THEN TACI.udf_char1 ");
            strQuery.Append("END AS Category,");
            strQuery.Append("CASE WHEN PACI.UDF_CHAR4 IS NOT NULL AND PACI.Instance_Id =PAD.ORG_ID THEN PACI.UDF_CHAR4 ");
            strQuery.Append("WHEN WACI.UDF_CHAR4 IS NOT NULL AND WACI.Instance_Id =WAD.ORG_ID THEN WACI.UDF_CHAR4 ");
            strQuery.Append("WHEN CACI.UDF_CHAR4 IS NOT NULL AND CACI.Instance_Id =CAD.ORG_ID THEN CACI.UDF_CHAR4 ");
            strQuery.Append("WHEN PBACI.UDF_CHAR4 IS NOT NULL AND PBACI.Instance_Id =PBAD.ORG_ID THEN PBACI.UDF_CHAR4 ");
            strQuery.Append("WHEN TACI.UDF_CHAR4 IS NOT NULL AND TACI.Instance_Id =TAD.ORG_ID THEN TACI.UDF_CHAR4 ");
            strQuery.Append("END AS Activity,");
            // strQuery.Append("PACI.udf_char4 AS Activity,");
            strQuery.Append("TO_CHAR(DATE_TRUNC('month', (TO_TIMESTAMP(CHARGESTABLE.ts_endtime / 1000)::DATE)), 'Mon-YY') AS RecordMonth,");
            strQuery.Append("Sum(ROUND((CHARGESTABLE.TIMESPENT / 60000) / 60.0, 2)) AS TimeSpentHrs ");
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
            strQuery.Append("LEFT JOIN ACCOUNTADDITIONALFIELDS PACI ON PPA.AccountId = PACI.Instance_id ");
            strQuery.Append("LEFT JOIN WORKORDERTOCHARGE WTC ON WTC.CHARGEID = CHARGESTABLE.CHARGEID ");
            strQuery.Append("LEFT JOIN WORKORDER WO ON WO.WORKORDERID = WTC.WORKORDERID ");
            strQuery.Append("LEFT JOIN WORKORDERSTATES WOS ON WO.WORKORDERID = WOS.WORKORDERID ");
            strQuery.Append("LEFT JOIN REQUESTTYPEDEFINITION RTD ON WOS.REQUESTTYPEID = RTD.REQUESTTYPEID ");
            strQuery.Append("LEFT JOIN WORKORDERACCOUNTMAPPING WAM ON WAM.WORKORDERID = WO.WORKORDERID ");
            strQuery.Append("LEFT JOIN ACCOUNTDEFINITION WAD ON WAD.ORG_ID = WAM.ACCOUNTID ");
            strQuery.Append("LEFT JOIN PORTALACCOUNTS WPA ON WAD.ORG_ID = WPA.ACCOUNTID ");
            strQuery.Append("LEFT JOIN ACCOUNTADDITIONALFIELDS WACI ON WPA.AccountId = WACI.Instance_id ");
            strQuery.Append("LEFT JOIN CHANGETOCHARGE CTC ON CTC.CHARGEID = CHARGESTABLE.CHARGEID ");
            strQuery.Append("LEFT JOIN CHANGEDETAILS CD ON CD.CHANGEID = CTC.CHANGEID ");
            strQuery.Append("LEFT JOIN CHANGEACCOUNTMAPPING CAM ON CAM.CHANGEID = CD.CHANGEID ");
            strQuery.Append("LEFT JOIN ACCOUNTDEFINITION CAD ON CAD.ORG_ID = CAM.ACCOUNTID ");
            strQuery.Append("LEFT JOIN PORTALACCOUNTS CPA ON CAD.ORG_ID = CPA.ACCOUNTID ");
            strQuery.Append("LEFT JOIN ACCOUNTADDITIONALFIELDS CACI ON CPA.AccountId = CACI.Instance_id ");
            strQuery.Append("LEFT JOIN PROBLEMTOCHARGE PTC ON PTC.CHARGEID = CHARGESTABLE.CHARGEID ");
            strQuery.Append("LEFT JOIN PROBLEM P ON P.PROBLEMID = PTC.PROBLEMID ");
            strQuery.Append("LEFT JOIN PROBLEMACCOUNTMAPPING PBAM ON PBAM.PROBLEMID = P.PROBLEMID ");
            strQuery.Append("LEFT JOIN ACCOUNTDEFINITION PBAD ON PBAD.ORG_ID = PBAM.ACCOUNTID ");
            strQuery.Append("LEFT JOIN PORTALACCOUNTS PBPA ON PBAD.ORG_ID = PBPA.ACCOUNTID ");
            strQuery.Append("LEFT JOIN ACCOUNTADDITIONALFIELDS PBACI ON PBPA.AccountId = PBACI.Instance_id ");
            strQuery.Append("LEFT JOIN TASKACCOUNTMAPPING TAM ON TAM.TASKID = TD.TASKID ");
            strQuery.Append("LEFT JOIN ACCOUNTDEFINITION TAD ON TAD.ORG_ID = TAM.ACCOUNTID ");
            strQuery.Append("LEFT JOIN PORTALACCOUNTS TAPA ON TAPA.ACCOUNTID = TAD.ORG_ID ");
            strQuery.Append("LEFT JOIN ACCOUNTADDITIONALFIELDS TACI ON TACI.Instance_id = TAPA.AccountId ");
            strQuery.Append("LEFT JOIN USERDEPARTMENT USERDEPT ON TECH.USER_ID = USERDEPT.USERID ");
            strQuery.Append("LEFT JOIN DEPARTMENTDEFINITION DEPTTABLE ON USERDEPT.DEPTID = DEPTTABLE.DEPTID ");
            strQuery.Append("LEFT JOIN SDOrganization CISITE ON DEPTTABLE.SITEID = CISITE.ORG_ID ");
           //strQuery.Append("WHERE NBRC.ISBILLABLEWORKLOG = true ");
            // strQuery.Append("AND TO_TIMESTAMP(CHARGESTABLE.ts_endtime / 1000)::DATE BETWEEN '2025-08-01'::DATE AND '2025-12-31'::DATE ");
            strQuery.Append("WHERE TO_TIMESTAMP(CHARGESTABLE.ts_endtime / 1000)::DATE BETWEEN '" + StartDate +"'::DATE AND '" + EndDate + "'::DATE ");
            strQuery.Append("GROUP BY AccountName,RecordMonth, Category, Activity ");
            strQuery.Append("ORDER BY AccountName,RecordMonth");
            
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(dbConnection))
                {
                    using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(strQuery.ToString(), connection))
                    {
                        connection.Open();
                        dataAdapter.Fill(dt);
                    }
                }       
            }
            catch (Exception ex)
            {
                throw new Exception("GetHoursByAccount(): " + ex.ToString());
            }

            return dt;
        }
    }
}
