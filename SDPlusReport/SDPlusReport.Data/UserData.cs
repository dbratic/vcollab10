using Npgsql;

using SDPlusReport.Entity;

using System.Data.Common;
using System.Net.NetworkInformation;
using System.Text;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SDPlusReport.Data
{
    public class UserData
    {
        public List<User> GetAllUser(string dbConnection, string loginName)
        {
            List<User> users = new List<User>();
            try
            {
                bool isAdmin = false;


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
                    string sql = "SELECT AAAUSER.FIRST_NAME FROM AAAUSER AAAUSER LEFT JOIN SDUSER SDUSER ON AAAUSER.USER_ID = SDUSER.USERID " +
                        "INNER JOIN PORTALTECHNICIANS POTECH ON SDUSER.USERID = POTECH.USERID " +
                        "LEFT JOIN AAALOGIN AAL ON POTECH.USERID = AAL.USER_ID " +
                        "LEFT JOIN AAAACCOUNT ACC ON AAL.LOGIN_ID = ACC.LOGIN_ID " +
                        "LEFT JOIN SDAUTHORIZEDROLE AUTH ON ACC.ACCOUNT_ID = AUTH.ACCOUNT_ID " +
                        "LEFT JOIN AAAROLE AROLE ON AUTH.ROLE_ID = AROLE.ROLE_ID " +
                        "LEFT JOIN AAAUSERCONTACTINFO AAAUSERCONTACT ON AAAUSER.USER_ID = AAAUSERCONTACT.USER_ID " +
                        "LEFT JOIN AAACONTACTINFO AAACONTACT ON AAAUSERCONTACT.CONTACTINFO_ID = AAACONTACT.CONTACTINFO_ID " +
                        "LEFT JOIN USERDEPARTMENT USERDEPT ON POTECH.USERID = USERDEPT.USERID " +
                        "LEFT JOIN DEPARTMENTDEFINITION DEPTTABLE ON USERDEPT.DEPTID = DEPTTABLE.DEPTID " +
                        "WHERE AROLE.ROLE_ID = 2102 AND AAL.NAME = '" + loginName + "'";
                    using NpgsqlConnection conn = new NpgsqlConnection(dbConnection);
                    conn.Open();
                    using NpgsqlCommand cmdAdmin = new NpgsqlCommand(sql, conn);
                    using NpgsqlDataReader readerAdmin = cmdAdmin.ExecuteReader();

                    while (readerAdmin.Read())
                    {
                        isAdmin = true;
                    }
                }

                int userId= GetUserId(dbConnection, loginName); 
                bool isAppManager = false;
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

                string sqlQuery = "SELECT USR.USER_ID AS ID, USR.FIRST_NAME AS USERNAME, COALESCE(SD.Status,'DELETED') AS STATUS FROM AAAUSER USR JOIN AAALOGIN LOG ON USR.USER_ID = LOG.USER_ID JOIN requesteraccountmapping MAPP ON USR.USER_ID = MAPP.USERID JOIN aaausercontactinfo UCI ON UCI.USER_ID = USR.USER_ID JOIN portaltechnicians PT ON PT.USERID = USR.USER_ID LEFT JOIN accountdefinition ACC ON MAPP.ACCOUNTID = ACC.ORG_ID LEFT JOIN aaacontactinfo ACI ON ACI.contactinfo_id = UCI.contactinfo_id LEFT JOIN USERDEPARTMENT USERDEPT ON USR.USER_ID = USERDEPT.USERID LEFT JOIN DEPARTMENTDEFINITION DEPTTABLE ON USERDEPT.DEPTID = DEPTTABLE.DEPTID LEFT JOIN SDOrganization CISITE ON DEPTTABLE.SITEID = CISITE.ORG_ID LEFT JOIN SDUser SD ON SD.USERID = USR.USER_ID WHERE FIRST_NAME <> 'administrator' AND ORG_NAME = 'Vigilant Technologies' AND CISITE.Name <> 'Client' ";
                if (isAdmin)
                {
                    sqlQuery += "UNION SELECT USER_ID, FIRST_NAME||'-'||COALESCE(SD.Status,'DELETED'), COALESCE(SD.Status,'DELETED') FROM AAAUSER AU LEFT JOIN SDUser SD ON SD.USERID = AU.USER_ID WHERE USER_ID NOT IN (SELECT USER_ID FROM AAALOGIN) AND USER_ID IN (SELECT TECHNICIANID FROM CHARGESTABLE) ";
                }
                else if (isAppManager)
                {
                    sqlQuery += "AND SD.REPORTINGTO=(SELECT USER_ID FROM AAALOGIN WHERE NAME='" + loginName + "') OR LOG.NAME = '" + loginName + "' OR USR.USER_ID in (Select distinct technicianid from TechnicianAccountMapping Where accountid in (SELECT AcctDef.Org_id AS Id FROM TechnicianAccountMapping TAM LEFT JOIN ACCOUNTDEFINITION AcctDef ON TAM.accountid = AcctDef.org_id  WHERE TAM.technicianid = "+ userId + "))";
                }
                else
                {
                    sqlQuery += "AND LOG.NAME = '" + loginName + "' ";
                }
                sqlQuery += "ORDER BY STATUS, USERNAME";
                using NpgsqlConnection connection = new NpgsqlConnection(dbConnection);
                connection.Open();
                using NpgsqlCommand cmd = new NpgsqlCommand(sqlQuery, connection);
                using NpgsqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var user = new User();
                    user.Id = Convert.ToInt32(reader["ID"]);
                    user.UserName = (string)reader["USERNAME"];
                    users.Add(user);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            return users.ToList();
        }

        public int GetUserId(string dbConnection, string loginName)
        {
            int userid = 0;
            try
            {
                string strQuery = "Select User_Id from AAALOGIN where name='" + loginName + "'";
                using NpgsqlConnection conn = new NpgsqlConnection(dbConnection);
                conn.Open();
                using NpgsqlCommand cmdUser = new NpgsqlCommand(strQuery, conn);
                using NpgsqlDataReader readerUser = cmdUser.ExecuteReader();
                while (readerUser.Read())
                {
                    userid = Convert.ToInt32(readerUser["User_Id"]);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }


            return userid;
        }

        public string GetAccountIds(string dbConnection, int userId)
        {
            string accountIds = string.Empty;
            try
            {
                string strQuery = "SELECT AcctDef.Org_id AS Id " +
                        "FROM TechnicianAccountMapping TAM " +
                        "LEFT JOIN ACCOUNTDEFINITION AcctDef ON TAM.accountid = AcctDef.org_id WHERE TAM.technicianid=" + userId;
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

        public List<EmployeeList> GetEmployeeList(string dbConnection, string department)
        {
            StringBuilder sqlQuery = new (string.Empty);
            
            sqlQuery.Append("Select U.USERID,");
            sqlQuery.Append("COALESCE(U.FIRSTNAME, '') || ' ' || COALESCE(U.MIDDLENAME, '') || ' ' || COALESCE(U.LASTNAME, '') AS EmployeeName,");
            sqlQuery.Append("D.DEPTNAME AS DEPARTMENT ");
            sqlQuery.Append("FROM SDUser U ");
            sqlQuery.Append("LEFT JOIN USERDEPARTMENT UD on U.USERID = UD.USERID ");
            sqlQuery.Append("LEFT JOIN DEPARTMENTDEFINITION D ON D.DEPTID = UD.DEPTID ");
            sqlQuery.Append("WHERE U.Status = 'ACTIVE' AND D.DEPTNAME !='General' AND U.USERID !=5 " + department );
            sqlQuery.Append(" ORDER BY EMPLOYEENAME");

            List<EmployeeList> employees = new List<EmployeeList>();
            try
            {
                using NpgsqlConnection connection = new NpgsqlConnection(dbConnection);
                connection.Open();
                using NpgsqlCommand cmd = new NpgsqlCommand(sqlQuery.ToString(), connection);
                using NpgsqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var employee = new EmployeeList
                    {
                        EmployeeName = (string)reader["EmployeeName"],
                        Department = (string)reader["DEPARTMENT"]
                    };
                   
                    employees.Add(employee);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            return employees.ToList();
        }
    }
}
