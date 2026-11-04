using Npgsql;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDPlusReport.Data
{
    public class BudgetData
    {
        public DataTable GetBudgetData(string dbConnection, string filterString1, string filterString2)
        {
            DataTable dt = new();
            try
            {
                StringBuilder strQuery = new(string.Empty);
                strQuery.Append("SELECT COALESCE(PAD.ORG_ID, WAD.ORG_ID, CAD.ORG_ID, PBAD.ORG_ID, TAD.ORG_ID) AS AccountId,");
                strQuery.Append(" COALESCE(PAD.ORG_NAME, WAD.ORG_NAME, CAD.ORG_NAME, PBAD.ORG_NAME, TAD.ORG_NAME) AS AccountName,");
                strQuery.Append(" COALESCE(WO.WORKORDERID, WTD.WORKORDERID) AS TicketId,");
                strQuery.Append(" COALESCE(WO.TITLE, WO2.TITLE) AS TicketSubject,TD.TaskId,");
                strQuery.Append("CASE WHEN TRIM(LOWER(TD.Title)) = TRIM(LOWER(COALESCE(WO.TITLE, WO2.TITLE))) THEN NULL ");
                strQuery.Append("ELSE TD.Title END AS TaskTitle,");
                strQuery.Append("TO_CHAR(TO_TIMESTAMP(TD.createddate / 1000), 'YYYY-Mon-DD') AS OpenDate, ");
                strQuery.Append("SD.STATUSNAME AS Status,TD.ESTIMATEDEFFORTHOURS,");
                strQuery.Append("COALESCE(TS.TimeSpentHrs, 0) AS TimeSpentHrs,");
                strQuery.Append("ROUND(COALESCE(TS.BillableHour, 0)-TD.ESTIMATEDEFFORTHOURS, 2) AS TimeDifference,");
                strQuery.Append(" COALESCE(TS.BillableHour, 0) AS BillableHour,");
                strQuery.Append(" COALESCE(TS.NonBillableHour, 0) AS NonBillableHour ");
                strQuery.Append("FROM TASKDETAILS TD ");
                strQuery.Append("LEFT JOIN (SELECT TTC.TASKID,");
                strQuery.Append("ROUND(COALESCE(SUM(CH.TIMESPENT), 0) / (60000.0 * 60), 2) AS TimeSpentHrs,");
                strQuery.Append("ROUND(COALESCE(SUM(CASE WHEN NBR.ISBILLABLEWORKLOG = TRUE ");
                strQuery.Append("THEN CH.TIMESPENT ELSE 0 END), 0) / (60000.0 * 60), 2) AS BillableHour,");
                strQuery.Append("ROUND(COALESCE(SUM(CASE WHEN NBR.ISBILLABLEWORKLOG = FALSE ");
                strQuery.Append("THEN CH.TIMESPENT ELSE 0 END), 0) / (60000.0 * 60), 2) AS NonBillableHour ");
                strQuery.Append("FROM CHARGESTABLE CH ");
                strQuery.Append("JOIN TASKTOCHARGE TTC ON CH.CHARGEID = TTC.CHARGEID ");
                strQuery.Append("LEFT JOIN NOBILLREQUESTCHARGES NBR ON NBR.REQUESTCHARGEID = CH.CHARGEID ");
                strQuery.Append("GROUP BY TTC.TASKID ) TS ON TS.TASKID = TD.TASKID ");
                strQuery.Append("LEFT JOIN TASKTOCHARGE TTC ON TTC.TASKID = TD.TASKID ");
                strQuery.Append("LEFT JOIN CHARGESTABLE CHARGESTABLE ON CHARGESTABLE.CHARGEID = TTC.CHARGEID ");
                strQuery.Append("LEFT JOIN WORKORDERTOCHARGE WTC ON WTC.CHARGEID = CHARGESTABLE.CHARGEID ");
                strQuery.Append("LEFT JOIN WORKORDER WO ON WO.WORKORDERID = WTC.WORKORDERID ");
                strQuery.Append("LEFT JOIN WORKORDERTOTASKDETAILS WTD ON WTD.TASKID = TD.TASKID ");
                strQuery.Append("LEFT JOIN WORKORDER WO2 ON WO2.WORKORDERID = WTD.WORKORDERID ");
                strQuery.Append("LEFT JOIN WORKORDERSTATES WOS ON WOS.WORKORDERID = COALESCE(WO.WORKORDERID, WTD.WORKORDERID) ");
                strQuery.Append("LEFT JOIN STATUSDEFINITION SD ON SD.STATUSID = TD.STATUSID ");
                strQuery.Append("LEFT JOIN REQUESTTYPEDEFINITION RTD ON WOS.REQUESTTYPEID = RTD.REQUESTTYPEID ");
                strQuery.Append("LEFT JOIN WORKORDERACCOUNTMAPPING WAM ON WAM.WORKORDERID = COALESCE(WO.WORKORDERID, WTD.WORKORDERID) ");
                strQuery.Append("LEFT JOIN ACCOUNTDEFINITION WAD ON WAD.ORG_ID = WAM.ACCOUNTID ");
                strQuery.Append("LEFT JOIN PORTALACCOUNTS WPA ON WAD.ORG_ID = WPA.ACCOUNTID ");
                strQuery.Append("LEFT JOIN ACCOUNTADDITIONALFIELDS WACI ON WPA.AccountId = WACI.Instance_id ");
                strQuery.Append("LEFT JOIN TASKTOPROJECTS TTP ON TTP.TASKID = TD.TASKID ");
                strQuery.Append("LEFT JOIN PROJECTDETAILS PD ON PD.PROJECTID = TTP.PROJECTID ");
                strQuery.Append("LEFT JOIN PROJECTACCMAPPING PAM ON PAM.PROJECTID = PD.PROJECTID ");
                strQuery.Append("LEFT JOIN ACCOUNTDEFINITION PAD ON PAD.ORG_ID = PAM.ACCOUNTID ");
                strQuery.Append("LEFT JOIN PORTALACCOUNTS PPA ON PAD.ORG_ID = PPA.ACCOUNTID ");
                strQuery.Append("LEFT JOIN ACCOUNTADDITIONALFIELDS PACI ON PPA.AccountId = PACI.Instance_ID ");
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
                strQuery.Append("LEFT JOIN TASKACCOUNTMAPPING TAM ON TAM.TASKID = TD.TASKID ");
                strQuery.Append("LEFT JOIN ACCOUNTDEFINITION TAD ON TAD.ORG_ID = TAM.ACCOUNTID ");
                strQuery.Append(filterString1);
                strQuery.Append(" GROUP BY ");
                strQuery.Append("COALESCE(PAD.ORG_ID, WAD.ORG_ID, CAD.ORG_ID, PBAD.ORG_ID, TAD.ORG_ID),");
                strQuery.Append("COALESCE(PAD.ORG_NAME, WAD.ORG_NAME, CAD.ORG_NAME, PBAD.ORG_NAME, TAD.ORG_NAME), ");
                strQuery.Append("COALESCE(WO.WORKORDERID, WTD.WORKORDERID), ");
                strQuery.Append("WO.TITLE,WO2.TITLE,TD.TaskId,TD.Title,");
                strQuery.Append("TO_CHAR(TO_TIMESTAMP(TD.createddate / 1000), 'YYYY-Mon-DD'), ");
                strQuery.Append("TD.ESTIMATEDEFFORTHOURS,TS.TimeSpentHrs,");
                strQuery.Append("TS.BillableHour,TS.NonBillableHour, SD.STATUSNAME ");
                strQuery.Append(" UNION ALL ");
                strQuery.Append("SELECT DISTINCT COALESCE(PAD1.ORG_ID, WAD.ORG_ID, CAD.ORG_ID, PBAD.ORG_ID, TAD.ORG_ID) AS AccountId,");
                strQuery.Append("COALESCE(PAD1.ORG_NAME, WAD.ORG_NAME, CAD.ORG_NAME, PBAD.ORG_NAME, TAD.ORG_NAME) AS AccountName,");
                strQuery.Append("WO.WorkOrderId AS TicketId, WO.TITLE AS TicketSubject, NULL::BIGINT AS TaskId,");
                strQuery.Append("CASE WHEN TRIM(LOWER(WO.Title)) = TRIM(LOWER(WO.TITLE)) THEN NULL ");
                strQuery.Append("ELSE WO.Title END AS TaskTitle,");
                strQuery.Append("TO_CHAR(TO_TIMESTAMP(WO.createdtime / 1000), 'YYYY-Mon-DD') AS OpenDate,");
                strQuery.Append("SD.STATUSNAME AS Status, ");
                strQuery.Append("COALESCE(NULLIF(TRIM(RA.UDF_CHAR3), '')::NUMERIC, 0) AS ESTIMATEDEFFORTHOURS,");
                strQuery.Append("ROUND(SUM(CHG.TIMESPENT / 60000) / 60.0, 2) AS TimeSpentHrs,");
                strQuery.Append("ROUND(ROUND(COALESCE(SUM(CASE WHEN NBR.ISBILLABLEWORKLOG = TRUE THEN CHG.TIMESPENT ELSE 0 END) / 60000.0, 0) / 60.0, 2)");
                strQuery.Append("- COALESCE(NULLIF(TRIM(RA.UDF_CHAR3), '')::NUMERIC, 0), 2) AS TimeDifference,");
                strQuery.Append("ROUND(COALESCE(SUM(CASE WHEN NBR.ISBILLABLEWORKLOG = TRUE ");
                strQuery.Append("THEN CHG.TIMESPENT ELSE 0 END) / 60000.0, 0) / 60.0, 2) AS BillableHour,");
                strQuery.Append("ROUND(COALESCE(SUM(CASE WHEN NBR.ISBILLABLEWORKLOG = FALSE THEN ");
                strQuery.Append("CHG.TIMESPENT ELSE 0 END) / 60000.0, 0) / 60.0, 2) AS NonBillableHour ");
                strQuery.Append("FROM CHARGESTABLE CHG ");
                strQuery.Append("LEFT JOIN NOBILLREQUESTCHARGES NBR ON NBR.REQUESTCHARGEID = CHG.CHARGEID ");
                strQuery.Append("LEFT JOIN WORKLOGBILLINGSTATUS WBS ON WBS.CHARGEID = CHG.CHARGEID ");
                strQuery.Append("LEFT JOIN WORKORDERTOCHARGE WTC ON WTC.CHARGEID = CHG.CHARGEID ");
                strQuery.Append("LEFT JOIN WORKORDER WO ON WO.WORKORDERID = WTC.WORKORDERID ");
                strQuery.Append("LEFT JOIN Request_Additional_Fields_1 RA ON RA.WorkorderID = WO.WORKORDERID ");
                strQuery.Append("LEFT JOIN WORKORDERSTATES WOS ON WO.WORKORDERID = WOS.WORKORDERID ");
                strQuery.Append("LEFT JOIN STATUSDEFINITION SD ON SD.STATUSID = WOS.STATUSID ");
                strQuery.Append("LEFT JOIN REQUESTTYPEDEFINITION RTD ON WOS.REQUESTTYPEID = RTD.REQUESTTYPEID ");
                strQuery.Append("LEFT JOIN WORKORDERACCOUNTMAPPING WAM ON WAM.WORKORDERID = WO.WORKORDERID ");
                strQuery.Append("LEFT JOIN ACCOUNTDEFINITION WAD ON WAD.ORG_ID = WAM.ACCOUNTID ");
                strQuery.Append("LEFT JOIN PORTALACCOUNTS WPA ON WAD.ORG_ID = WPA.ACCOUNTID ");
                strQuery.Append("LEFT JOIN CHANGETOCHARGE CTC ON CTC.CHARGEID = CHG.CHARGEID ");
                strQuery.Append("LEFT JOIN CHANGEDETAILS CD ON CD.CHANGEID = CTC.CHANGEID ");
                strQuery.Append("LEFT JOIN CHANGEACCOUNTMAPPING CAM ON CAM.CHANGEID = CD.CHANGEID ");
                strQuery.Append("LEFT JOIN ACCOUNTDEFINITION CAD ON CAD.ORG_ID = CAM.ACCOUNTID ");
                strQuery.Append("LEFT JOIN PORTALACCOUNTS CPA ON CAD.ORG_ID = CPA.ACCOUNTID ");
                strQuery.Append("LEFT JOIN PROBLEMTOCHARGE PTC ON PTC.CHARGEID = CHG.CHARGEID ");
                strQuery.Append("LEFT JOIN PROBLEM P ON P.PROBLEMID = PTC.PROBLEMID ");
                strQuery.Append("LEFT JOIN PROBLEMACCOUNTMAPPING PBAM ON PBAM.PROBLEMID = P.PROBLEMID ");
                strQuery.Append("LEFT JOIN ACCOUNTDEFINITION PBAD ON PBAD.ORG_ID = PBAM.ACCOUNTID ");
                strQuery.Append("LEFT JOIN PORTALACCOUNTS PBPA ON PBAD.ORG_ID = PBPA.ACCOUNTID ");
                strQuery.Append("LEFT JOIN TASKACCOUNTMAPPING TAM ON TAM.TASKID = ( ");
                strQuery.Append("SELECT TASKID FROM TASKTOCHARGE WHERE CHARGEID = CHG.CHARGEID LIMIT 1 ) ");
                strQuery.Append("LEFT JOIN ACCOUNTDEFINITION TAD ON TAD.ORG_ID = TAM.ACCOUNTID ");
                strQuery.Append("LEFT JOIN PORTALACCOUNTS TAPA ON TAPA.ACCOUNTID = TAD.ORG_ID ");
                strQuery.Append("LEFT JOIN PROJECTACCMAPPING PAM ON PAM.PROJECTID = ( ");
                strQuery.Append("SELECT PD.PROJECTID FROM TASKTOCHARGE TTC2 ");
                strQuery.Append("JOIN TASKTOPROJECTS TTP ON TTP.TASKID = TTC2.TASKID ");
                strQuery.Append("JOIN PROJECTDETAILS PD ON PD.PROJECTID = TTP.PROJECTID ");
                strQuery.Append("WHERE TTC2.CHARGEID = CHG.CHARGEID LIMIT 1 ) ");
                strQuery.Append("LEFT JOIN ACCOUNTDEFINITION PAD1 ON PAD1.ORG_ID = PAM.ACCOUNTID ");
                strQuery.Append("LEFT JOIN PORTALACCOUNTS PPA ON PAD1.ORG_ID = PPA.ACCOUNTID ");
                strQuery.Append(filterString2);
                strQuery.Append(" GROUP BY ");
                strQuery.Append("COALESCE(PAD1.ORG_ID, WAD.ORG_ID, CAD.ORG_ID, PBAD.ORG_ID, TAD.ORG_ID),");
                strQuery.Append("COALESCE(PAD1.ORG_NAME, WAD.ORG_NAME, CAD.ORG_NAME, PBAD.ORG_NAME, TAD.ORG_NAME),");
                strQuery.Append("WO.WORKORDERID, WO.Title,WO.createdtime, SD.STATUSNAME, RA.UDF_CHAR3 ");
                strQuery.Append("ORDER BY AccountName, TicketId;");
             
                using var connection = new NpgsqlConnection(dbConnection);
                using var dataAdapter = new NpgsqlDataAdapter(strQuery.ToString(), connection);
                {
                    connection.Open();
                    dataAdapter.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetBudgetData(): " + ex.ToString());
            }

            return dt;
        }
    }
}
