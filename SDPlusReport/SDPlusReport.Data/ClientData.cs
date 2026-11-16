using Npgsql;

using SDPlusReport.Entity;

namespace SDPlusReport.Data
{
    public class ClientData
    {
        public List<ClientMaster> GetAllClient(string dbConnection)
        {
            List<ClientMaster> lstClient = new List<ClientMaster>();
            try
            {
                using NpgsqlConnection connection = new NpgsqlConnection(dbConnection);
                connection.Open();
                string strQuery = "select distinct udf_char5 as ClientName From ACCOUNTADDITIONALFIELDS where udf_char5 is not null ORDER BY udf_char5";
                using NpgsqlCommand cmd = new NpgsqlCommand(strQuery, connection);
                using NpgsqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var objClient = new ClientMaster();
                    objClient.Id = 0;
                    objClient.ClientName = (string)reader["ClientName"];
                    lstClient.Add(objClient);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetAllClient(): " + ex.ToString());
            }
            return [.. lstClient];
        }
        public List<ClientAccount> GetAllAccount(string dbConnection, int userid, bool isAdmin)
        {
            List<ClientAccount> lstAccount = new List<ClientAccount>();
            try
            {
                using NpgsqlConnection connection = new NpgsqlConnection(dbConnection);
                connection.Open();
                string strQuery = "";
                if (isAdmin)
                {
                    strQuery = "Select Org_id As id, org_name as AccountName from ACCOUNTDEFINITION ORDER BY org_name";
                }
                else
                {
                    strQuery = "SELECT AcctDef.Org_id AS Id, AcctDef.org_name AS AccountName " +
                        "FROM TechnicianAccountMapping TAM " +
                        "LEFT JOIN ACCOUNTDEFINITION AcctDef ON TAM.accountid = AcctDef.org_id WHERE TAM.technicianid=" + userid;
                }
                using NpgsqlCommand cmd = new NpgsqlCommand(strQuery, connection);
                using NpgsqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var objAccount = new ClientAccount();
                    objAccount.Id = Convert.ToInt32(reader["Id"]);
                    objAccount.AccountName = (string)reader["AccountName"];
                    lstAccount.Add(objAccount);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetAllAccount(): " + ex.ToString());
            }
            return [.. lstAccount];
        }

        public List<ClientDepartment> GetAllDepartments(string dbConnection, bool isAdmin, string loginName)
        {
            List<ClientDepartment> lstDepartment = new List<ClientDepartment>();
            try
            {
                string strQuery = string.Empty;
                if (isAdmin)
                {
                    strQuery = "SELECT DISTINCT DEPTNAME AS DepartmentName FROM DEPARTMENTDEFINITION WHERE SITEID IN (604,2101) AND isdeleted = false AND is_account_active = true ORDER BY DEPTNAME";
                }
                else
                {
                    strQuery = "SELECT DEPTName AS DepartmentName FROM DEPARTMENTDEFINITION DD INNER JOIN USERDEPARTMENT UD ON UD.DEPTID=DD.DEPTID WHERE UD.USERID = (SELECT USER_ID from AAALOGIN where name='" + loginName + "' )";
                }
                using NpgsqlConnection connection = new NpgsqlConnection(dbConnection);
                connection.Open();

                using NpgsqlCommand cmd = new NpgsqlCommand(strQuery, connection);
                using NpgsqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var objDepartment = new ClientDepartment();
                    objDepartment.DepartmentName = (string)reader["DepartmentName"];
                    lstDepartment.Add(objDepartment);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetAllDepartments(): " + ex.ToString());
            }
            return [.. lstDepartment];
        }
   
          public List<ClsStatus> GetAllStatus(string dbConnection)
        {

            List<ClsStatus> lstStatus = new List<ClsStatus>();
            try
            {
                using NpgsqlConnection connection = new NpgsqlConnection(dbConnection);
                connection.Open();
                string strQuery = "SELECT DISTINCT statusid AS Id, statusname AS StatusName FROM STATUSDEFINITION WHERE isdeleted = false ORDER BY statusname";
                using NpgsqlCommand cmd = new NpgsqlCommand(strQuery, connection);
                using NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var objStatus = new ClsStatus();
                    objStatus.Id = Convert.ToInt32(reader["Id"]);
                    objStatus.StatusName = (string)reader["StatusName"];
                    lstStatus.Add(objStatus);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetAllStatus(): " + ex.ToString());
            }
            return [.. lstStatus];
        }


    }
}