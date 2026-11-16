using Npgsql;

using SDPlusReport.Entity;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDPlusReport.Data
{
    public class TimesheetData
    {
        public DataTable GetTimesheetData(string dbConnection, string filterString2, DateTime startDate, DateTime endDate)
        {
            DataTable dt = new();
            try
            {
                StringBuilder strQuery = new(string.Empty);
                strQuery.Append("WITH PARAMS AS (SELECT DATE '{START_DATE}' AS RangeStart, DATE '{END_DATE}' AS RangeEnd), ");
                strQuery.Append("WEEKS AS (SELECT gs::date AS WeekStartDate, gs::date + 6 AS WeekEndDate ");
                strQuery.Append("FROM PARAMS P, generate_series( (P.RangeStart - EXTRACT(DOW FROM ");
                strQuery.Append(" P.RangeStart)::int),P.RangeEnd,INTERVAL '7 day' ) gs ), ");
                strQuery.Append("TECHS AS ( SELECT DISTINCT ON (U.USERID) ");
                strQuery.Append("U.USERID AS TECHID,A.FIRST_NAME AS TECHNICIANNAME,DEPT.DEPTNAME AS DEPARTMENT, ");
                strQuery.Append("ORG.NAME AS EMPLOCATION, U.EMPLOYEEID AS EMPLOYEETYPE, ");
                strQuery.Append("U.REPORTINGTO AS MANAGERID,MGR.FIRST_NAME AS MANAGERNAME ");
                strQuery.Append("FROM SDUSER U ");
                strQuery.Append(" JOIN AAAUSER A ON A.USER_ID = U.USERID ");
                strQuery.Append("LEFT JOIN USERDEPARTMENT UD ON UD.USERID = U.USERID ");
                strQuery.Append("LEFT JOIN DEPARTMENTDEFINITION DEPT ON DEPT.DEPTID = UD.DEPTID ");
                strQuery.Append("LEFT JOIN SITEDEFINITION SITE ON SITE.SITEID = DEPT.SITEID ");
                strQuery.Append("LEFT JOIN SDORGANIZATION ORG ON ORG.ORG_ID = SITE.SITEID ");
                strQuery.Append("LEFT JOIN AAAUSER MGR ON MGR.USER_ID = U.REPORTINGTO ");
                strQuery.Append("WHERE U.STATUS = 'ACTIVE' ");
                strQuery.Append("AND (EXISTS ( SELECT 1 FROM TIMESHEET T3 WHERE T3.TECHNICIANID = U.USERID) ");
                strQuery.Append("OR EXISTS (SELECT 1 FROM CHARGESTABLE C3 WHERE C3.TECHNICIANID = U.USERID)) ");
                strQuery.Append("ORDER BY U.USERID, DEPT.DEPTNAME NULLS LAST), ");
                strQuery.Append("TS_DATA AS ( SELECT T.TECHNICIANID AS TECHID, ");
                strQuery.Append("CASE WHEN TO_CHAR( TO_TIMESTAMP(T.STARTTIME / 1000.0),'HH24:MI:SS') = '00:00:00' ");
                strQuery.Append("THEN TO_TIMESTAMP(T.STARTTIME / 1000.0)::date ");
                strQuery.Append("ELSE TO_TIMESTAMP(T.STARTTIME / 1000.0)::date + 1 ");
                strQuery.Append("END AS WeekStartDate, T.ID AS TIMESHEETID, T.ID AS T_ID,");
                strQuery.Append("ROUND((T.TOTALTIME / 60000.0) / 60.0, 2) AS TotalTimeSpent, ");
                strQuery.Append("TS.NAME AS TimeSheetStatus, T.STATUSID, T.APPROVERID, AA.FIRST_NAME AS APPROVEDBY ");
                strQuery.Append("FROM TIMESHEET T ");
                strQuery.Append("JOIN TIMESHEETSTATUSDEFINITION TS ON TS.ID = T.STATUSID ");
                strQuery.Append("LEFT JOIN SDUSER UU ON UU.USERID = T.APPROVERID ");
                strQuery.Append("LEFT JOIN AAAUSER AA ON AA.USER_ID = UU.USERID), ");
                strQuery.Append("TS_WEEK AS (SELECT DISTINCT ON (TECHID, WeekStartDate) ");
                strQuery.Append("TECHID, WeekStartDate, TIMESHEETID, T_ID, ");
                strQuery.Append("SUM(TotalTimeSpent) OVER (PARTITION BY TECHID, WeekStartDate) AS TotalTimeSpent, ");
                strQuery.Append("TimeSheetStatus,STATUSID, APPROVERID, APPROVEDBY ");
                strQuery.Append("FROM TS_DATA ORDER BY TECHID, WeekStartDate, T_ID DESC),");
                strQuery.Append("CHARGE_TOTALS AS (SELECT C.TECHNICIANID AS TECHID, ");
                strQuery.Append("(TO_TIMESTAMP(C.TS_STARTTIME / 1000.0)::date ");
                strQuery.Append("- EXTRACT ( DOW FROM TO_TIMESTAMP(C.TS_STARTTIME / 1000.0)::date )::int ) AS WeekStartDate, ");
                strQuery.Append("ROUND(SUM(C.TIMESPENT) / 3600000.0, 2) AS ChargeHours ");
                strQuery.Append("FROM CHARGESTABLE C ");
                strQuery.Append("GROUP BY C.TECHNICIANID, ( TO_TIMESTAMP(C.TS_STARTTIME / 1000.0)::date ");
                strQuery.Append("- EXTRACT ( DOW FROM TO_TIMESTAMP(C.TS_STARTTIME / 1000.0)::date )::int )), ");
                strQuery.Append("COMBINED AS (SELECT TE.TECHID, TW.TIMESHEETID, TE.TECHNICIANNAME,  TE.DEPARTMENT, ");
                strQuery.Append("W.WeekStartDate AS TimesheetStartDate, W.WeekEndDate AS TimesheetEndDate, ");
                strQuery.Append("COALESCE(TW.TotalTimeSpent,CT.ChargeHours,0)::numeric AS TotalTimeSpent,");
                strQuery.Append("COALESCE(TW.TimeSheetStatus, CASE WHEN CT.ChargeHours IS NOT NULL THEN 'Not Created' ");
                strQuery.Append("ELSE 'No Time Logged' END) AS TimeSheetStatus, ");
                strQuery.Append("TW.APPROVERID,TW.APPROVEDBY, ");
                strQuery.Append("TE.EMPLOCATION,TE.EMPLOYEETYPE,TW.T_ID, ");
                strQuery.Append("COALESCE(TW.STATUSID,0) AS STATUSID, TE.MANAGERID, TE.MANAGERNAME ");
                strQuery.Append("FROM TECHS TE CROSS JOIN WEEKS W ");
                strQuery.Append("LEFT JOIN TS_WEEK TW ON TW.TECHID = TE.TECHID AND TW.WeekStartDate = W.WeekStartDate ");
                strQuery.Append("LEFT JOIN CHARGE_TOTALS CT ON CT.TECHID = TE.TECHID AND CT.WeekStartDate = W.WeekStartDate) ");
                strQuery.Append("SELECT COMBINED.TIMESHEETID,COMBINED.TECHID AS TECHNICIANID, COMBINED.TECHNICIANNAME, COMBINED.DEPARTMENT,");
                strQuery.Append("TO_CHAR(COMBINED.TimesheetStartDate, 'YYYY-Mon-DD') AS TimesheetStartDate, ");
                strQuery.Append("TO_CHAR(COMBINED.TimesheetEndDate, 'YYYY-Mon-DD') AS TimesheetEndDate, ");
                strQuery.Append("COMBINED.TotalTimeSpent, COMBINED.TimeSheetStatus,COMBINED.MANAGERNAME AS APPROVEDBY,");
                strQuery.Append(" COMBINED.EMPLOCATION, COMBINED.EMPLOYEETYPE, ");
                strQuery.Append("(SELECT TO_CHAR(TO_TIMESTAMP(TAH.TIME / 1000.0),'YYYY-Mon-DD') ");
                strQuery.Append("FROM TIMESHEETAPPROVALHISTORY TAH ");
                strQuery.Append("WHERE TAH.TIMESHEETID = COMBINED.T_ID AND TAH.TOSTATUS = 5 ");
                strQuery.Append("ORDER BY TAH.TIME DESC LIMIT 1 ) AS ApprovedDate ");
                strQuery.Append("FROM COMBINED ");
                strQuery.Append(filterString2);
                strQuery.Append(" ORDER BY COMBINED.TECHNICIANNAME, COMBINED.TimesheetStartDate DESC;");
                strQuery.Replace("{START_DATE}", startDate.ToString("yyyy-MM-dd"));
                strQuery.Replace("{END_DATE}", endDate.ToString("yyyy-MM-dd"));

                using var connection = new NpgsqlConnection(dbConnection);
                using var dataAdapter = new NpgsqlDataAdapter(strQuery.ToString(), connection);
                {
                    connection.Open();
                    dataAdapter.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetTimesheetData(): " + ex.ToString());
            }

            return dt;
        }

        public DataTable GetTimesheetCount(string dbConnection, string filterStrings)
        {
            DataTable dt = new();
            try
            {
                using NpgsqlConnection connection = new NpgsqlConnection(dbConnection);
                StringBuilder strQuery = new StringBuilder(string.Empty);

                strQuery.Append("WITH TimesheetData AS (");
                strQuery.Append("SELECT T.ID,T.TECHNICIANID,A.CREATEDTIME, T.STARTTIME,");
                strQuery.Append("T.ENDTIME,T.STATUSID,T.APPROVERID,");
                strQuery.Append("COALESCE(U.FIRSTNAME,'') || ' ' || COALESCE(U.MIDDLENAME,'') || ' ' || COALESCE(U.LASTNAME,'') AS TechnicianName,");
                strQuery.Append("COALESCE(DEPT.DEPTNAME, DEPT.DEPTNAME) AS Department,");
                strQuery.Append("U.EMPLOYEEID AS EMPLOYEETYPE,CISITE.NAME AS EMPLOCATION,");
                strQuery.Append("COALESCE(UA.FIRSTNAME,'') || ' ' || COALESCE(UA.MIDDLENAME,'') || ' ' || COALESCE(UA.LASTNAME,'') AS ApproverName ");
                strQuery.Append("FROM TIMESHEET T ");
                strQuery.Append("LEFT JOIN SDUSER U ON T.TECHNICIANID = U.USERID ");
                strQuery.Append("LEFT JOIN AAAUser A ON A.USER_ID = T.TECHNICIANID ");
               // strQuery.Append("LEFT JOIN PORTALTECHNICIANS POTECH ON U.USERID = POTECH.USERID ");
                strQuery.Append("LEFT JOIN USERDEPARTMENT USERDEPT ON USERDEPT.USERID = U.USERID ");
                strQuery.Append("LEFT JOIN DEPARTMENTDEFINITION DEPT ON DEPT.DEPTID = USERDEPT.DEPTID ");
                strQuery.Append("LEFT JOIN TIMESHEETSTATUSDEFINITION TS ON TS.ID = T.STATUSID ");
                strQuery.Append("LEFT JOIN SDUSER UA ON UA.USERID = T.APPROVERID ");
                strQuery.Append("LEFT JOIN SDOrganization CISITE ON DEPT.SITEID = CISITE.ORG_ID ");
                strQuery.Append(filterStrings);
                strQuery.Append(" ),");
                strQuery.Append("LastTimesheet AS (");
                strQuery.Append("SELECT *, ROW_NUMBER() OVER (PARTITION BY TECHNICIANID ORDER BY ENDTIME DESC) AS rn ");
                strQuery.Append("FROM TimesheetData ),");
                strQuery.Append("CountSince AS (");
                strQuery.Append("SELECT TECHNICIANID, COUNT(*) AS TimesheetCount ");
                strQuery.Append("FROM TimesheetData ");
                strQuery.Append("WHERE TO_TIMESTAMP(CAST(STARTTIME AS bigint)/1000)::date >= DATE '2026-07-01' ");
                strQuery.Append("GROUP BY TECHNICIANID),");
                strQuery.Append("FirstLastRange AS (");
                strQuery.Append("SELECT TECHNICIANID,");
                strQuery.Append("MIN(STARTTIME) AS FirstStartTime,");
                strQuery.Append("MAX(ENDTIME)   AS LastEndTime ");
                strQuery.Append("FROM TimesheetData ");
                strQuery.Append("GROUP BY TECHNICIANID ) ");
                strQuery.Append("SELECT LT.TechnicianName,");
                strQuery.Append("TO_CHAR(TO_TIMESTAMP(LT.CREATEDTIME/1000), 'YYYY-Mon-DD') AS CREATEDATE,");
                strQuery.Append("LT.Department, LT.EMPLOYEETYPE,");
                strQuery.Append("LT.EMPLOCATION,LT.ApproverName AS Approver, ");
                strQuery.Append("CASE WHEN LT.EMPLOCATION = 'India' ");
                strQuery.Append("THEN TO_CHAR((to_timestamp(FLR.FirstStartTime / 1000.0) AT TIME ZONE 'Asia/Kolkata'), 'YYYY-Mon-DD') ");
                strQuery.Append("ELSE TO_CHAR((to_timestamp(FLR.FirstStartTime / 1000.0)), 'YYYY-Mon-DD') END AS TimesheetStartDate, ");
                strQuery.Append("CASE WHEN LT.EMPLOCATION = 'India' THEN ");
                strQuery.Append("TO_CHAR((to_timestamp(FLR.LastEndTime / 1000.0)), 'YYYY-MM-DD') ");
                strQuery.Append("ELSE TO_CHAR((to_timestamp(FLR.LastEndTime / 1000.0) AT TIME ZONE 'America/Chicago'), 'YYYY-MM-DD') ");
                strQuery.Append("END AS TimesheetEndDate, ");
                strQuery.Append("(CASE WHEN LT.EMPLOCATION = 'India' THEN ");
                strQuery.Append("TO_CHAR((to_timestamp(LT.STARTTIME / 1000.0) AT TIME ZONE 'Asia/Kolkata'),'FMMM/FMDD') ");
                strQuery.Append("ELSE TO_CHAR((to_timestamp(LT.STARTTIME / 1000.0)),'FMMM/FMDD') END ");
                strQuery.Append("|| '-' || CASE WHEN LT.EMPLOCATION = 'India' THEN ");
                strQuery.Append("TO_CHAR((to_timestamp(LT.ENDTIME / 1000.0)),'FMMM/FMDD') ELSE ");
                strQuery.Append(" TO_CHAR((to_timestamp(LT.ENDTIME / 1000.0) AT TIME ZONE 'America/Chicago'), 'FMMM/FMDD') END ");
                strQuery.Append(") AS lasttimesheetdate,");
                strQuery.Append("COALESCE(CS.TimesheetCount, 0) AS TimeSheetCount ");
                strQuery.Append("FROM LastTimesheet LT ");
                strQuery.Append("LEFT JOIN CountSince CS ON CS.TECHNICIANID = LT.TECHNICIANID ");
                strQuery.Append("LEFT JOIN FirstLastRange FLR ON FLR.TECHNICIANID = LT.TECHNICIANID ");
                strQuery.Append("WHERE LT.rn = 1 ");
                strQuery.Append("ORDER BY LT.TechnicianName;");

                using var dataAdapter = new NpgsqlDataAdapter(strQuery.ToString(), connection);
                {
                    connection.Open();
                    dataAdapter.Fill(dt);
                }
              
            }
            catch (Exception ex)
            {
                throw new Exception("GetTimesheetCounts(): " + ex.ToString());
            }
            return dt;
        }

        public List<ClsStatus> GetTimesheetStatus(string dbConnection)
        {

            List<ClsStatus> lstTimesheetStatus = new List<ClsStatus>();
            try
            {
                using NpgsqlConnection connection = new NpgsqlConnection(dbConnection);
                connection.Open();
                string strQuery = "SELECT Id, name AS StatusName FROM TIMESHEETSTATUSDEFINITION ORDER BY Id";
                using NpgsqlCommand cmd = new NpgsqlCommand(strQuery, connection);
                using NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var objStatus = new ClsStatus();
                    objStatus.Id = Convert.ToInt32(reader["Id"]);
                    objStatus.StatusName = (string)reader["StatusName"];
                    lstTimesheetStatus.Add(objStatus);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetTimesheetStatus(): " + ex.ToString());
            }
            return [.. lstTimesheetStatus];
        }
        public List<TimesheetApprover> GetTimesheetApprovers(string dbConnection)
        {
            List<TimesheetApprover> lstTimesheetApprovers = new List<TimesheetApprover>();
            try
            {
                using NpgsqlConnection connection = new NpgsqlConnection(dbConnection);
                connection.Open();
                //string strQuery = "SELECT DISTINCT T.APPROVERID,AU.FIRST_NAME AS APPROVERNAME FROM TIMESHEET T LEFT JOIN AAAUSER AU ON AU.USER_ID =T.APPROVERID WHERE T.APPROVERID IS NOT NULL ORDER BY AU.FIRST_NAME";
                StringBuilder strQuery = new StringBuilder(string.Empty);
                strQuery.Append("SELECT DISTINCT SU.USERID AS ApproverId,AU.FIRST_NAME AS APPROVERNAME FROM SDUser SU ");
                strQuery.Append("LEFT JOIN AAAUSER AU ON AU.USER_ID = SU.USERID ");
                strQuery.Append("WHERE SU.USERID IN (SELECT DISTINCT ReportingTo FROM SDUSER WHERE ReportingTo IS NOT NULL) ");
                strQuery.Append("AND SU.Status = 'Active' ");
                strQuery.Append("UNION ");
                strQuery.Append("SELECT DISTINCT T.APPROVERID, U.FIRST_NAME AS APPROVERNAME FROM TIMESHEET T ");
                strQuery.Append("LEFT JOIN AAAuser U ON U.USER_ID = T.APPROVERID ");
                strQuery.Append("WHERE T.APPROVERID IS NOT NULL ORDER BY APPROVERNAME");

                using NpgsqlCommand cmd = new NpgsqlCommand(strQuery.ToString(), connection);
                using NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var objApprover = new TimesheetApprover(){
                        Id = Convert.ToInt32(reader["APPROVERID"]),
                        ApproverName = (string)reader["APPROVERNAME"]
                    };
                    lstTimesheetApprovers.Add(objApprover);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetTimesheetApprovers(): " + ex.ToString());
            }
            return [..lstTimesheetApprovers];
        }
    }
}
