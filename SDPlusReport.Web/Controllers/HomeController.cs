using ClosedXML.Excel;

using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using Npgsql;

using SDPlusReport.Data;
using SDPlusReport.Entity;
using SDPlusReport.Web.Models;

using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SDPlusReport.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, IWebHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _configuration = configuration;
            _env = hostEnvironment;
        }

        public IActionResult Index()
        {
            string connectionString = _configuration.GetConnectionString("DbConnection").ToString();
            string login_full_name = "";
            string login_user_name = "";

            foreach (var item in User.Claims)
            {
                if (item.Type == "preferred_username")
                    login_user_name = item.Value.Split("@")[0];
                if (item.Type == "name")
                    login_full_name = item.Value;
            }
            ViewBag.LoginUserName = login_user_name;
            ViewBag.LoginFullName = login_full_name;

            UserData userData = new();
            List<User> userList = userData.GetAllUser(connectionString, login_user_name).ToList();
            ViewBag.UserList = userList;

            string path = "wwwroot/Temp/" + login_user_name + ".json";
            if (System.IO.File.Exists(path))
            {
                string jsonReports = System.IO.File.ReadAllText(path);
                List<ReportFilter>? filterList = JsonConvert.DeserializeObject<List<ReportFilter>>(jsonReports);
                ViewBag.FilterList = filterList.OrderBy(x => x.ReportName);
            }

            bool isAdmin = false;
            bool isApplicationManager = false;

            path = "wwwroot/Temp/admins.txt";
            using (StreamReader readers = new StreamReader(path))
            {
                while (!readers.EndOfStream)
                {
                    if (readers.ReadLine() == login_user_name)
                    {
                        isAdmin = true;
                    }
                }
            }

            if (!isAdmin)
            {
                string sql = "SELECT AAAUSER.FIRST_NAME FROM AAAUSER AAAUSER LEFT JOIN SDUSER SDUSER ON AAAUSER.USER_ID = SDUSER.USERID INNER JOIN PORTALTECHNICIANS POTECH ON SDUSER.USERID = POTECH.USERID LEFT JOIN AAALOGIN AAL ON POTECH.USERID = AAL.USER_ID LEFT JOIN AAAACCOUNT ACC ON AAL.LOGIN_ID = ACC.LOGIN_ID LEFT JOIN SDAUTHORIZEDROLE AUTH ON ACC.ACCOUNT_ID = AUTH.ACCOUNT_ID LEFT JOIN AAAROLE AROLE ON AUTH.ROLE_ID = AROLE.ROLE_ID LEFT JOIN AAAUSERCONTACTINFO AAAUSERCONTACT ON AAAUSER.USER_ID = AAAUSERCONTACT.USER_ID LEFT JOIN AAACONTACTINFO AAACONTACT ON AAAUSERCONTACT.CONTACTINFO_ID = AAACONTACT.CONTACTINFO_ID LEFT JOIN USERDEPARTMENT USERDEPT ON POTECH.USERID = USERDEPT.USERID LEFT JOIN DEPARTMENTDEFINITION DEPTTABLE ON USERDEPT.DEPTID = DEPTTABLE.DEPTID WHERE AROLE.ROLE_ID = 2102 AND AAL.NAME = '" + login_user_name + "'";
                using NpgsqlConnection conn = new NpgsqlConnection(connectionString);
                conn.Open();
                using NpgsqlCommand cmdAdmin = new NpgsqlCommand(sql, conn);
                using NpgsqlDataReader readerAdmin = cmdAdmin.ExecuteReader();

                while (readerAdmin.Read())
                {
                    isAdmin = true;
                }
            }

            if (isAdmin)
            {
                path = "wwwroot/Temp/public.json";
                if (System.IO.File.Exists(path))
                {
                    string jsonReports = System.IO.File.ReadAllText(path);
                    List<ReportFilter>? filterPublicList = JsonConvert.DeserializeObject<List<ReportFilter>>(jsonReports);
                    ViewBag.FilterPublicList = filterPublicList.OrderBy(x => x.ReportName);
                }
            }

            if (!isAdmin)
            {
                string sql = "SELECT AAAUSER.FIRST_NAME FROM AAAUSER AAAUSER LEFT JOIN SDUSER SDUSER ON AAAUSER.USER_ID = SDUSER.USERID INNER JOIN PORTALTECHNICIANS POTECH ON SDUSER.USERID = POTECH.USERID LEFT JOIN AAALOGIN AAL ON POTECH.USERID = AAL.USER_ID LEFT JOIN AAAACCOUNT ACC ON AAL.LOGIN_ID = ACC.LOGIN_ID LEFT JOIN SDAUTHORIZEDROLE AUTH ON ACC.ACCOUNT_ID = AUTH.ACCOUNT_ID LEFT JOIN AAAROLE AROLE ON AUTH.ROLE_ID = AROLE.ROLE_ID LEFT JOIN AAAUSERCONTACTINFO AAAUSERCONTACT ON AAAUSER.USER_ID = AAAUSERCONTACT.USER_ID LEFT JOIN AAACONTACTINFO AAACONTACT ON AAAUSERCONTACT.CONTACTINFO_ID = AAACONTACT.CONTACTINFO_ID LEFT JOIN USERDEPARTMENT USERDEPT ON POTECH.USERID = USERDEPT.USERID LEFT JOIN DEPARTMENTDEFINITION DEPTTABLE ON USERDEPT.DEPTID = DEPTTABLE.DEPTID WHERE AROLE.ROLE_ID = 2701 AND AAL.NAME = '" + login_user_name + "'";
                using NpgsqlConnection conn = new NpgsqlConnection(connectionString);
                conn.Open();
                using NpgsqlCommand cmdAdmin = new NpgsqlCommand(sql, conn);
                using NpgsqlDataReader readerAppManager = cmdAdmin.ExecuteReader();

                while (readerAppManager.Read())
                {
                    isApplicationManager = true;
                }
            }

            ViewBag.IsAdmin = isAdmin;

            ViewBag.IsAppManager = isApplicationManager;
            var tabDisabled = isAdmin || isApplicationManager ? "" : "d-none";
            ViewBag.TabDisabled = tabDisabled;

            ClientData objClient = new();
            List<ClientMaster> clientList = objClient.GetAllClient(connectionString).ToList();
            ViewBag.ClientList = clientList;


            UserData objUser = new();
            int userId = objUser.GetUserId(connectionString, login_user_name);

            List<ClientAccount> accountList = objClient.GetAllAccount(connectionString, userId, isAdmin).ToList();
            ViewBag.AccountList = accountList;

            List<ClientDepartment> departmentList = objClient.GetAllDepartments(connectionString, isAdmin, login_user_name).ToList();
            ViewBag.DepartmentList = departmentList;


            List<ClsStatus> statusList = objClient.GetAllStatus(connectionString).ToList();
            ViewBag.StatusList = statusList;

            TimesheetData objTimesheetData = new();
            List<ClsStatus> timesheetStautsList = objTimesheetData.GetTimesheetStatus(connectionString).ToList();
            ViewBag.TimesheetStatusList = timesheetStautsList;
            List<TimesheetApprover> timesheetApproverList = objTimesheetData.GetTimesheetApprovers(connectionString).ToList();
            ViewBag.TimesheetApproverList = timesheetApproverList;

            return View();
        }

        [Route("/GetReportData")]
        [HttpPost]
        public IActionResult GetReportData(ReportFilter objRpt)
        {
            string login_full_name = "";
            string login_user_name = "";
            foreach (var item in User.Claims)
            {
                if (item.Type == "preferred_username")
                    login_user_name = item.Value.Split("@")[0];
                if (item.Type == "name")
                    login_full_name = item.Value;
            }
            string connectionString = _configuration.GetConnectionString("DbConnection").ToString();
            ReportData rptData = new();
            List<Report> rptDataList = rptData.GetReportData(connectionString, objRpt, login_user_name, login_full_name).ToList();
            var totalRecord = rptDataList.Count;
            var returnObject = new
            {
                draw = 1,
                recordsTotal = totalRecord,
                recordsFiltered = totalRecord,
                data = rptDataList
            };
            return Json(returnObject);
        }

        [Route("/GetReportDataHoursByDay")]
        [HttpPost]
        public IActionResult GetReportDataHoursByDay(ReportFilter objRpt)
        {
            string login_full_name = "";
            string login_user_name = "";
            foreach (var item in User.Claims)
            {
                if (item.Type == "preferred_username")
                    login_user_name = item.Value.Split("@")[0];
                if (item.Type == "name")
                    login_full_name = item.Value;
            }
            string connectionString = _configuration.GetConnectionString("DbConnection").ToString();
            ReportData rptData = new();
            List<HoursByDay> rptDataList = rptData.GetReportDataHoursByDay(connectionString, objRpt, login_user_name, login_full_name).ToList();
            var totalRecord = rptDataList.Count;
            var returnObject = new
            {
                draw = 1,
                recordsTotal = totalRecord,
                recordsFiltered = totalRecord,
                data = rptDataList
            };
            return Json(returnObject);
        }

        [Route("/GetReportDataHoursByPerson")]
        [HttpPost]
        public IActionResult GetReportDataHoursByPerson(ReportFilter objRpt)
        {
            string login_full_name = "";
            string login_user_name = "";
            foreach (var item in User.Claims)
            {
                if (item.Type == "preferred_username")
                    login_user_name = item.Value.Split("@")[0];
                if (item.Type == "name")
                    login_full_name = item.Value;
            }
            string connectionString = _configuration.GetConnectionString("DbConnection").ToString();
            ReportData rptData = new();
            List<HoursByPerson> rptDataList = rptData.GetReportDataHoursByPerson(connectionString, objRpt, login_user_name, login_full_name).ToList();
            var totalRecord = rptDataList.Count;
            var returnObject = new
            {
                draw = 1,
                recordsTotal = totalRecord,
                recordsFiltered = totalRecord,
                data = rptDataList
            };
            return Json(returnObject);
        }

        [Route("/GetReportDataHourByAccount")]
        [HttpPost]
        public IActionResult GetReportDataHourByAccount()
        {
            string connectionString = _configuration.GetConnectionString("DbConnection").ToString();
            //var objReportData = GetHoursAccount(connectionString);
            //var totalRecord = objReportData.Count;
            //var returnObject = new
            //{
            //    draw = 1,
            //    recordsTotal = totalRecord,
            //    recordsFiltered = totalRecord,
            //    data = objReportData
            //};
            //var jsonData = Json(returnObject);
            //return Json(returnObject);

            var headers = GenerateDynamicHeaders();
            List<HoursByAccount> reportData = GetHoursAccount(connectionString);

            var flattenedData = FlattenReportData(reportData, headers);
            var totalRecord = reportData.Count();
            return Json(new
            {
                draw = 1,
                recordsTotal = totalRecord,
                recordsFiltered = totalRecord,
                headers = headers,
                data = flattenedData
            });
        }

        // Debug endpoint: returns raw rows and computed aggregates for an account + department
        [Route("/DebugHoursByAccountDetails")]
        [HttpGet]
        public IActionResult DebugHoursByAccountDetails(string accountName, string department = "")
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DbConnection").ToString();
                HoursByAccountData objData = new HoursByAccountData();
                DataTable dt = objData.GetHoursByAccountDetails(connectionString, string.Empty);

                string filter = "accountname = '" + accountName + "'";
                if (!string.IsNullOrEmpty(department))
                {
                    filter += " AND department = '" + department + "'";
                }

                DataRow[] rows = dt.Select(filter);

                var raw = new List<object>();
                decimal dbGrand = 0.0m;
                var monthAgg = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

                foreach (DataRow r in rows)
                {
                    var month = Convert.ToString(r[3]) ?? string.Empty;
                    var value = r[4] == DBNull.Value ? 0.0m : Convert.ToDecimal(r[4]);
                    value = decimal.Round(value, 2, MidpointRounding.AwayFromZero);
                    dbGrand += value;

                    string abbr = month.Length >= 3 ? month.Substring(0, 3) : month;
                    if (!monthAgg.ContainsKey(abbr)) monthAgg[abbr] = 0.0m;
                    monthAgg[abbr] += value;

                    raw.Add(new { month = month, monthAbbr = abbr, value = value, rawRow = r.ItemArray });
                }

                return Json(new { account = accountName, department = department, dbGrand = dbGrand, monthAggregates = monthAgg, rows = raw });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DebugHoursByAccountDetails failed for account={account} dept={dept}", accountName, department);
                return BadRequest(ex.Message);
            }
        }


        [Route("/GetBudgetReportData")]
        [HttpPost]
        public IActionResult GetBudgetReportData(BudgetFilter objBudgetRpt)
        {
            string connectionString = _configuration.GetConnectionString("DbConnection").ToString();
            DataTable dt = new DataTable();
            BudgetData obBudgetjData = new BudgetData();
            StringBuilder strBudgetFilter1 = new StringBuilder();
            strBudgetFilter1.Append("");
            StringBuilder strBudgetFilter2 = new StringBuilder();
            strBudgetFilter2.Append(" WHERE NOT EXISTS (SELECT 1 FROM TASKTOCHARGE T WHERE T.CHARGEID = CHG.CHARGEID) ");

            if (!string.IsNullOrEmpty(objBudgetRpt.AccountId))
            {
                strBudgetFilter1.Append(" WHERE COALESCE(PAD.ORG_ID, WAD.ORG_ID, CAD.ORG_ID, PBAD.ORG_ID, TAD.ORG_ID) IN (" + objBudgetRpt.AccountId + ") ");
                strBudgetFilter2.Append(" AND ( COALESCE(PAD1.ORG_ID, WAD.ORG_ID, CAD.ORG_ID, PBAD.ORG_ID, TAD.ORG_ID) IN (" + objBudgetRpt.AccountId + ") OR COALESCE(PAD1.ORG_ID, WAD.ORG_ID, CAD.ORG_ID, PBAD.ORG_ID, TAD.ORG_ID) IS NULL )");
            }

            if (!string.IsNullOrEmpty(objBudgetRpt.StatusId))
            {
                strBudgetFilter1.Append("AND TD.STATUSID IN (" + objBudgetRpt.StatusId + ")");
                strBudgetFilter2.Append("AND WOS.STATUSID IN (" + objBudgetRpt.StatusId + ")");
            }

            dt = obBudgetjData.GetBudgetData(connectionString, strBudgetFilter1.ToString(), strBudgetFilter2.ToString());

            List<Budget> lstBudget = new List<Budget>();
            foreach (DataRow dr in dt.Rows)
            {
                Budget objBudget = new Budget()
                {
                    AccountId = Convert.ToString(dr["AccountId"]),
                    AccountName = Convert.ToString(dr["AccountName"]),
                    OpenDate = dr["OpenDate"] == DBNull.Value ? (string?)null : Convert.ToString(dr["OpenDate"]),
                    // CloseDate = dr["CloseDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["CloseDate"]),
                    // CloseDate=(DateTime?)null,
                    Status = Convert.ToString(dr["Status"]),
                    TicketNumber = Convert.ToString(dr["TicketId"]),
                    TicketSubject = Convert.ToString(dr["TicketSubject"]),
                    TaskNumber = Convert.ToString(dr["TaskId"]),
                    TaskName = Convert.ToString(dr["TaskTitle"]),
                    EstimatedHours = dr["ESTIMATEDEFFORTHOURS"] == DBNull.Value ? 0.0 : Convert.ToDouble(dr["ESTIMATEDEFFORTHOURS"]),
                    ActualHours = dr["TimeSpentHrs"] == DBNull.Value ? 0.0 : Convert.ToDouble(dr["TimeSpentHrs"]),
                    BillableHours = dr["BillableHour"] == DBNull.Value ? 0.0 : Convert.ToDouble(dr["BillableHour"]),
                    NonBillableHours = dr["NonBillableHour"] == DBNull.Value ? 0.0 : Convert.ToDouble(dr["NonBillableHour"]),
                    HourDifference = dr["TimeDifference"] == DBNull.Value ? 0.0 : Convert.ToDouble(dr["TimeDifference"])
                };
                lstBudget.Add(objBudget);
            }
            var totalRecord = lstBudget.Count;
            var returnObject = new
            {
                draw = 1,
                recordsTotal = totalRecord,
                recordsFiltered = totalRecord,
                data = lstBudget.ToList()
            };

            return Json(returnObject);

        }

        [Route("/GetTimesheetData")]
        [HttpPost]
        public IActionResult GetTimesheetData(TimeSheetFilter objTimesheetRpt)
        {
            string connectionString = _configuration.GetConnectionString("DbConnection").ToString();
            DataTable dt = new DataTable();
            TimesheetData obTimesheetData = new TimesheetData();
            StringBuilder strTimesheetFilter = new(string.Empty);

            DateTime countRangeStart = objTimesheetRpt.TimesheetStartDate?.Date ?? new DateTime(2026, 7, 1);
            DateTime countRangeEnd = objTimesheetRpt.TimesheetEndDate?.Date ?? DateTime.Today;

            if (!string.IsNullOrEmpty(objTimesheetRpt.Department) && objTimesheetRpt.Department != "''")
            {
                strTimesheetFilter.Append(" WHERE COMBINED.DEPARTMENT IN (" + objTimesheetRpt.Department + ")");
            }
            if (!string.IsNullOrEmpty(objTimesheetRpt.EmployeeType) && objTimesheetRpt.EmployeeType != "''")
            {
                if (strTimesheetFilter.Length > 0)
                {
                    strTimesheetFilter.Append(" AND COMBINED.EMPLOYEETYPE IN (" + objTimesheetRpt.EmployeeType + ")");
                }
                else
                {
                    strTimesheetFilter.Append(" WHERE COMBINED.EMPLOYEETYPE IN (" + objTimesheetRpt.EmployeeType + ")");
                }
            }
            if (!string.IsNullOrEmpty(objTimesheetRpt.Location) && objTimesheetRpt.Location != "''")
            {
                if (strTimesheetFilter.Length > 0)
                {
                    strTimesheetFilter.Append(" AND COMBINED.EMPLOCATION IN (" + objTimesheetRpt.Location + ")");
                }
                else
                {
                    strTimesheetFilter.Append(" WHERE COMBINED.EMPLOCATION IN (" + objTimesheetRpt.Location + ")");
                }
            }
            if (!string.IsNullOrEmpty(objTimesheetRpt.Approver))
            {
                if (strTimesheetFilter.Length > 0)
                {
                    strTimesheetFilter.Append(" AND COMBINED.MANAGERID IN (" + objTimesheetRpt.Approver + ")");
                }
                else
                {
                    strTimesheetFilter.Append(" WHERE COMBINED.MANAGERID IN (" + objTimesheetRpt.Approver + ")");
                }
            }

            //if (strTimesheetFilter.Length > 0)
            //{
            //    strTimesheetFilter.Append(" AND '[" + countRangeStart.ToString("yyyy-MM-dd") + ", " + countRangeEnd.ToString("yyyy-MM-dd") + "]'::daterange @> daterange(LEAST(COMBINED.TimesheetStartDate, COMBINED.TimesheetEndDate), GREATEST(COMBINED.TimesheetStartDate, COMBINED.TimesheetEndDate), '[]')");
            //}
            //else
            //{
            //    strTimesheetFilter.Append(" WHERE '[" + countRangeStart.ToString("yyyy-MM-dd") + ", " + countRangeEnd.ToString("yyyy-MM-dd") + "]'::daterange@> daterange(LEAST(COMBINED.TimesheetStartDate, COMBINED.TimesheetEndDate), GREATEST(COMBINED.TimesheetStartDate, COMBINED.TimesheetEndDate), '[]')");
            //}
         
            if (!string.IsNullOrEmpty(objTimesheetRpt.Status))
            {
                if (strTimesheetFilter.Length > 0)
                {
                    strTimesheetFilter.Append(" AND COALESCE(COMBINED.STATUSID,0) IN (" + objTimesheetRpt.Status + ")");
                }
                else
                {
                    strTimesheetFilter.Append(" WHERE COALESCE(COMBINED.STATUSID,0) IN (" + objTimesheetRpt.Status + ")");
                } 
            }

            dt = obTimesheetData.GetTimesheetData(connectionString, strTimesheetFilter.ToString(), countRangeStart,  countRangeEnd);

            List<TimeSheet> lstTimesheet = new List<TimeSheet>();
            foreach (DataRow dr in dt.Rows)
            {
                TimeSheet objTimesheetData = new TimeSheet()
                {
                    TimeSheetId = dr["TimeSheetId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TimeSheetId"]),
                    TechnicianId= dr["TechnicianId"] == DBNull.Value?0 : Convert.ToInt32(dr["TechnicianId"]),
                    TechnicianName = Convert.ToString(dr["TechnicianName"]),
                    Department = Convert.ToString(dr["Department"]),
                    TimesheetStartDate = dr["TimesheetStartDate"] == DBNull.Value ? (string?)null : Convert.ToString(dr["TimesheetStartDate"]),
                    TimesheetEndDate = dr["TimesheetEndDate"] == DBNull.Value ? (string?)null : Convert.ToString(dr["TimesheetEndDate"]),
                    TotalTimeSpent = dr["TotalTimeSpent"] == DBNull.Value ? 0.0 : Convert.ToDouble(dr["TotalTimeSpent"]),
                    TimeSheetStatus = Convert.ToString(dr["TimeSheetStatus"]),
                    ApprovedBy = Convert.ToString(dr["ApprovedBy"]),
                    ApprovedDate = dr["ApprovedDate"] == DBNull.Value ? (string?)null : Convert.ToString(dr["ApprovedDate"]),
                    EmpLocation = Convert.ToString(dr["EmpLocation"]),
                    EmploymentType = Convert.ToString(dr["EmployeeType"])
                };
                lstTimesheet.Add(objTimesheetData);
            }
            var totalRecord = lstTimesheet.Count;
            var returnObject = new
            {
                draw = 1,
                recordsTotal = totalRecord,
                recordsFiltered = totalRecord,
                data = lstTimesheet.ToList()
            };

            return Json(returnObject);
        }

        [Route("/GetTimesheetCountData")]
        [HttpPost]
        public IActionResult GetTimesheetCountData(TimeSheetFilter objTimesheetRpt)
        {

            string connectionString = _configuration.GetConnectionString("DbConnection").ToString();

            StringBuilder strTimesheetFilter = new StringBuilder(string.Empty);

            DateTime countRangeStart = objTimesheetRpt.TimesheetStartDate?.Date ?? new DateTime(2026, 7, 1);
            DateTime countRangeEnd = objTimesheetRpt.TimesheetEndDate?.Date ?? DateTime.Today;

            if (!string.IsNullOrEmpty(objTimesheetRpt.Department) && objTimesheetRpt.Department != "''")
            {
                strTimesheetFilter.Append(" WHERE DEPT.DEPTNAME IN (" + objTimesheetRpt.Department + ")");
            }
            if (!string.IsNullOrEmpty(objTimesheetRpt.EmployeeType) && objTimesheetRpt.EmployeeType !="''")
            {
                if (strTimesheetFilter.Length > 0)
                {
                    strTimesheetFilter.Append(" AND U.EMPLOYEEID IN (" + objTimesheetRpt.EmployeeType + ")");
                }
                else
                {
                    strTimesheetFilter.Append(" WHERE U.EMPLOYEEID IN (" + objTimesheetRpt.EmployeeType + ")");
                }
            }
            if (!string.IsNullOrEmpty(objTimesheetRpt.Location) && objTimesheetRpt.Location!="''")
            {
                if (strTimesheetFilter.Length > 0)
                {
                    strTimesheetFilter.Append(" AND CISITE.NAME IN (" + objTimesheetRpt.Location + ")");
                }
                else
                {
                    strTimesheetFilter.Append(" WHERE CISITE.NAME IN (" + objTimesheetRpt.Location + ")");
                }
            }
            if (!string.IsNullOrEmpty(objTimesheetRpt.Approver))
            {
                if (strTimesheetFilter.Length > 0)
                {
                    strTimesheetFilter.Append(" AND T.APPROVERID IN (" + objTimesheetRpt.Approver + ")");
                }
                else
                {
                    strTimesheetFilter.Append(" WHERE T.APPROVERID IN (" + objTimesheetRpt.Approver + ")");
                }
            }

            if (strTimesheetFilter.Length > 0)
            {
                strTimesheetFilter.Append(" AND '[" + countRangeStart.ToString("yyyy-MM-dd") + ", " + countRangeEnd.ToString("yyyy-MM-dd") + "]'::daterange @> daterange(TO_TIMESTAMP(T.STARTTIME / 1000.0)::date,TO_TIMESTAMP(T.ENDTIME / 1000.0)::date,'[]')");
            }
            else
            {
                strTimesheetFilter.Append(" WHERE '[" + countRangeStart.ToString("yyyy-MM-dd") + ", " + countRangeEnd.ToString("yyyy-MM-dd") + "]'::daterange@> daterange(TO_TIMESTAMP(T.STARTTIME / 1000.0)::date,TO_TIMESTAMP(T.ENDTIME / 1000.0)::date,'[]')");
            }


            TimesheetData obTimesheetData = new TimesheetData();
            DataTable dt = obTimesheetData.GetTimesheetCount(connectionString, strTimesheetFilter.ToString());

            List<TimeSheetCount> lstTimesheetCount = new List<TimeSheetCount>();
            foreach (DataRow dr in dt.Rows)
            {
                TimeSheetCount objTimesheetCount = new TimeSheetCount()
                {
                    EmployeeName = Convert.ToString(dr["TechnicianName"]),
                    CreateDate = dr["CreateDate"] == DBNull.Value ? (string?)null : Convert.ToString(dr["CreateDate"]),
                    Department = Convert.ToString(dr["Department"]),
                    EmployeeType = Convert.ToString(dr["EmployeeType"]),
                    EmpLocation = Convert.ToString(dr["EmpLocation"]),
                    Approver = Convert.ToString(dr["Approver"]),

                    TimesheetStartDate = dr["TimesheetStartDate"] == DBNull.Value ? (string?)null : Convert.ToString(dr["TimesheetStartDate"]),
                    TimesheetEndDate = dr["TimesheetEndDate"] == DBNull.Value ? (string?)null : Convert.ToString(dr["TimesheetEndDate"]),

                    LastTimesheetDate = dr["LastTimesheetDate"] == DBNull.Value ? (string?)null : Convert.ToString(dr["LastTimesheetDate"]),
                    EntryCount = dr["TimeSheetCount"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TimeSheetCount"])


                };
                lstTimesheetCount.Add(objTimesheetCount);
            }

            var totalRecord = lstTimesheetCount.Count;
            var returnObject = new
            {
                draw = 1,
                recordsTotal = totalRecord,
                recordsFiltered = totalRecord,
                data = lstTimesheetCount.ToList()
            };
            return Json(returnObject);
        }

        [Route("/GetEmployeeList")]
        [HttpPost]
        public IActionResult GetEmployeeList(string department)
        {
            string dept;
            string connectionString = _configuration.GetConnectionString("DbConnection").ToString();
            if (!string.IsNullOrEmpty(department) && department != "''")
            {
                dept= " AND D.DEPTNAME IN (" + department + ")";
            }
            else
            {
                dept=string.Empty;
            }

            UserData objEmployeeData = new UserData();
            List<EmployeeList> lstEmployee = objEmployeeData.GetEmployeeList(connectionString, dept).ToList();
            var totalRecord = lstEmployee.Count;
            var returnObject = new
            {
                draw = 1,
                recordsTotal = totalRecord,
                recordsFiltered = totalRecord,
                data = lstEmployee.ToList()
            };
            return Json(returnObject);
        }


        [Route("/GetReportDataHourByAccountDetails")]
        [HttpPost]
        public IActionResult GetReportDataHourByAccountDetails(ReportFilter objRpt)
        {
            string connectionString = _configuration.GetConnectionString("DbConnection").ToString();
            HoursByAccountData objData = new HoursByAccountData();
            DataTable dt = new DataTable();

            StringBuilder strDepartment = new StringBuilder();
            strDepartment.Append("");
            if (!string.IsNullOrEmpty(objRpt.Department))
            {
                string[] arrDepartment = objRpt.Department.Split(",");
                for (int i = 0; i < arrDepartment.Length; i++)
                {
                    if (i == 0)
                    {
                        strDepartment.Append("'" + arrDepartment[i] + "'");
                    }
                    else
                    {
                        strDepartment.Append(", '" + arrDepartment[i] + "'");
                    }
                }
            }

            var objAccountsData = GetHoursAccount(connectionString);
            dt = objData.GetHoursByAccountDetails(connectionString, strDepartment.ToString());
            var lstAccount = dt.AsEnumerable().Select(row => row[0]).Distinct().ToList();
            lstAccount = lstAccount.OrderBy(a => a.ToString()).ToList();

            List<HoursByAccountDetails> objReportData = new List<HoursByAccountDetails>();

            decimal janTotal = 0.0m;
            decimal febTotal = 0.0m;
            decimal marTotal = 0.0m;
            decimal aprTotal = 0.0m;
            decimal mayTotal = 0.0m;
            decimal junTotal = 0.0m;
            decimal julTotal = 0.0m;
            decimal augTotal = 0.0m;
            decimal sepTotal = 0.0m;
            decimal octTotal = 0.0m;
            decimal novTotal = 0.0m;
            decimal decTotal = 0.0m;

            foreach (string objAccount in lstAccount)
            {
                string filterExpression = "accountname = '" + objAccount + "'";
                DataTable dtDepartment = dt.Select(filterExpression).CopyToDataTable();
                var lstDepartment = dtDepartment.AsEnumerable().Select(row => row[1]).Distinct().ToList();
                lstDepartment = lstDepartment.OrderBy(a => a.ToString()).ToList();

                var tblAccount = objAccountsData.Where(k => k.AccountName == objAccount)
                                    .Select(a => new
                                    {
                                        accountName = a.AccountName,
                                        category = a.Category,
                                        activity = a.Activity,
                                        january = a.January,
                                        february = a.February,
                                        march = a.March,
                                        april = a.April,
                                        may = a.May,
                                        june = a.June,
                                        july = a.July,
                                        august = a.August,
                                        september = a.September,
                                        october = a.October,
                                        november = a.November,
                                        december = a.December,
                                        yearTotal = a.YearTotal
                                    })
                                    .FirstOrDefault();

                // Protect against missing data from objAccountsData
                if (tblAccount == null)
                {
                    tblAccount = new
                    {
                        accountName = objAccount,
                        category = string.Empty,
                        activity = string.Empty,
                        january = 0.0m,
                        february = 0.0m,
                        march = 0.0m,
                        april = 0.0m,
                        may = 0.0m,
                        june = 0.0m,
                        july = 0.0m,
                        august = 0.0m,
                        september = 0.0m,
                        october = 0.0m,
                        november = 0.0m,
                        december = 0.0m,
                        yearTotal = 0.0m
                    };
                }

                HoursByAccountDetails objReportRow = new HoursByAccountDetails()
                {
                    AccountName = "<b>" + tblAccount.accountName + "</b>",
                    Category = "",
                    Activity = "",
                    Department = "<b>Total</b>",
                    January = tblAccount.january,
                    February = tblAccount.february,
                    March = tblAccount.march,
                    April = tblAccount.april,
                    May = tblAccount.may,
                    June = tblAccount.june,
                    July = tblAccount.july,
                    August = tblAccount.august,
                    September = tblAccount.september,
                    October = tblAccount.october,
                    November = tblAccount.november,
                    December = tblAccount.december,
                    YearTotal = tblAccount.yearTotal
                };

                objReportData.Add(objReportRow);


                foreach (string objDept in lstDepartment)
                {
                    string accountFilter = "accountname= '" + objAccount + "' AND department= '" + objDept + "'";
                    DataRow[] filteredRows = dtDepartment.Select(accountFilter);

                    HoursByAccountDetails objReportRowDetails = new HoursByAccountDetails();
                    objReportRowDetails.AccountName = $"{objDept} - {objAccount}";
                    objReportRowDetails.Department = objDept;
                    objReportRowDetails.Category = Convert.ToString(filteredRows[0][2]);
                    objReportRowDetails.Activity = Convert.ToString(filteredRows[0][3]);
                    decimal grandTotal = 0.0m;

                    foreach (DataRow row in filteredRows)
                    {
                        // Expect month identifier in row[3] and numeric value in row[4]
                        var monthStr = Convert.ToString(row[4]) ?? string.Empty;
                        var value = row[6] == DBNull.Value ? 0.0m : Convert.ToDecimal(row[6]);
                        // Round DB-provided value to 2 decimals to match SQL rounding
                        value = decimal.Round(value, 2, MidpointRounding.AwayFromZero);
                        grandTotal += value;

                        var abbr = monthStr.Length >= 3 ? monthStr.Substring(0, 3) : monthStr;
                        switch (abbr)
                        {
                            case "Jan":
                                janTotal += value;
                                objReportRowDetails.January += value;
                                break;
                            case "Feb":
                                febTotal += value;
                                objReportRowDetails.February += value;
                                break;
                            case "Mar":
                                marTotal += value;
                                objReportRowDetails.March += value;
                                break;
                            case "Apr":
                                aprTotal += value;
                                objReportRowDetails.April += value;
                                break;
                            case "May":
                                mayTotal += value;
                                objReportRowDetails.May += value;
                                break;
                            case "Jun":
                                junTotal += value;
                                objReportRowDetails.June += value;
                                break;
                            case "Jul":
                                julTotal += value;
                                objReportRowDetails.July += value;
                                break;
                            case "Aug":
                                augTotal += value;
                                objReportRowDetails.August += value;
                                break;
                            case "Sep":
                                sepTotal += value;
                                objReportRowDetails.September += value;
                                break;
                            case "Oct":
                                octTotal += value;
                                objReportRowDetails.October += value;
                                break;
                            case "Nov":
                                novTotal += value;
                                objReportRowDetails.November += value;
                                break;
                            case "Dec":
                                decTotal += value;
                                objReportRowDetails.December += value;
                                break;
                            default:
                                // ignore unknown month identifiers
                                break;
                        }
                    }

                    // Compute YearTotal from accumulated month fields to avoid mismatch
                    objReportRowDetails.YearTotal = objReportRowDetails.January
                        + objReportRowDetails.February
                        + objReportRowDetails.March
                        + objReportRowDetails.April
                        + objReportRowDetails.May
                        + objReportRowDetails.June
                        + objReportRowDetails.July
                        + objReportRowDetails.August
                        + objReportRowDetails.September
                        + objReportRowDetails.October
                        + objReportRowDetails.November
                        + objReportRowDetails.December;
                    // Diagnostic: if accumulated month sum doesn't equal the summed grandTotal from DB rows, log details
                    decimal monthSum = objReportRowDetails.January
                        + objReportRowDetails.February
                        + objReportRowDetails.March
                        + objReportRowDetails.April
                        + objReportRowDetails.May
                        + objReportRowDetails.June
                        + objReportRowDetails.July
                        + objReportRowDetails.August
                        + objReportRowDetails.September
                        + objReportRowDetails.October
                        + objReportRowDetails.November
                        + objReportRowDetails.December;

                    var dbGrand = decimal.Round(grandTotal, 2, MidpointRounding.AwayFromZero);
                    if (Math.Abs(monthSum - dbGrand) > 0.01m)
                    {
                        try
                        {
                            var rowsSb = new StringBuilder();
                            foreach (var dr in filteredRows)
                            {
                                rowsSb.AppendFormat("[{0}={1}],", dr[3]?.ToString(), dr[4]?.ToString());
                            }
                            _logger.LogWarning("HoursByAccountDetails mismatch for Account={account} Dept={dept} DBGrand={dbGrand} MonthSum={monthSum} Rows={rows}", objAccount, objDept, dbGrand, monthSum, rowsSb.ToString());
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to log mismatch details for Account={account} Dept={dept}", objAccount, objDept);
                        }
                    }

                    objReportData.Add(objReportRowDetails);
                }
            }

            var headers = GenerateDynamicHeaders1();
            var flattenedData = FlattenReportData1(objReportData, headers);
            var totalRecord = objReportData.Count;
            return Json(new
            {
                draw = 1,
                recordsTotal = totalRecord,
                recordsFiltered = totalRecord,
                headers = headers,
                data = flattenedData
            });
        }

        [Route("/GetReportDataCompliance")]
        [HttpPost]
        public IActionResult GetReportDataCompliance(ReportFilter objRpt)
        {
            string login_full_name = "";
            string login_user_name = "";
            foreach (var item in User.Claims)
            {
                if (item.Type == "preferred_username")
                    login_user_name = item.Value.Split("@")[0];
                if (item.Type == "name")
                    login_full_name = item.Value;
            }
            string connectionString = _configuration.GetConnectionString("DbConnection").ToString();
            ReportData rptData = new();
            List<Compliance> rptDataList = rptData.GetReportDataCompliance(connectionString, objRpt, login_user_name, login_full_name).ToList();
            var totalRecord = rptDataList.Count;
            var returnObject = new
            {
                draw = 1,
                recordsTotal = totalRecord,
                recordsFiltered = totalRecord,
                data = rptDataList
            };
            return Json(returnObject);
        }

        [Route("/GetReportDataUtilizationByPerson")]
        [HttpPost]
        public IActionResult GetReportDataUtilizationByPerson(ReportFilter objRpt)
        {
            string login_full_name = "";
            string login_user_name = "";
            foreach (var item in User.Claims)
            {
                if (item.Type == "preferred_username")
                    login_user_name = item.Value.Split("@")[0];
                if (item.Type == "name")
                    login_full_name = item.Value;
            }
            string connectionString = _configuration.GetConnectionString("DbConnection").ToString();
            ReportData rptData = new();
            List<Utilization> rptDataList = rptData.GetReportDataUtilizationByPerson(connectionString, objRpt, login_user_name, login_full_name).ToList();
            var totalRecord = rptDataList.Count;
            var returnObject = new
            {
                draw = 1,
                recordsTotal = totalRecord,
                recordsFiltered = totalRecord,
                data = rptDataList
            };
            return Json(returnObject);
        }

        [Route("/GetReportDataUtilizationByDepartment")]
        [HttpPost]
        public IActionResult GetReportDataUtilizationByDepartment(ReportFilter objRpt)
        {
            string login_full_name = "";
            string login_user_name = "";
            foreach (var item in User.Claims)
            {
                if (item.Type == "preferred_username")
                    login_user_name = item.Value.Split("@")[0];
                if (item.Type == "name")
                    login_full_name = item.Value;
            }
            string connectionString = _configuration.GetConnectionString("DbConnection").ToString();
            ReportData rptData = new();
            List<Utilization> rptDataList = rptData.GetReportDataUtilizationByDepartment(connectionString, objRpt, login_user_name, login_full_name).ToList();
            var totalRecord = rptDataList.Count;
            var returnObject = new
            {
                draw = 1,
                recordsTotal = totalRecord,
                recordsFiltered = totalRecord,
                data = rptDataList
            };
            return Json(returnObject);
        }

        [Route("/GetReportDataInventory")]
        [HttpPost]
        public IActionResult GetReportDataInventory()
        {
            string connectionString = _configuration.GetConnectionString("DbConnection").ToString();
            ReportData rptData = new();
            List<Inventory> rptDataList = rptData.GetReportDataInventory(connectionString).ToList();
            var totalRecord = rptDataList.Count;
            var returnObject = new
            {
                draw = 1,
                recordsTotal = totalRecord,
                recordsFiltered = totalRecord,
                data = rptDataList
            };
            return Json(returnObject);
        }

        [Route("/GetFiltersByReportName")]
        [HttpPost]
        public IActionResult GetFiltersByReportName(string reportName)
        {
            string jsonBack = "";
            string login_user_name = "";
            if (reportName[^1].ToString() == " ")
            {
                login_user_name = "public";
            }
            else
            {
                foreach (var item in User.Claims)
                {
                    if (item.Type == "preferred_username")
                        login_user_name = item.Value.Split("@")[0];
                }
            }

            string path = "wwwroot/Temp/" + login_user_name + ".json";
            if (System.IO.File.Exists(path))
            {
                string jsonReports = System.IO.File.ReadAllText(path);
                List<ReportFilter>? reports = JsonConvert.DeserializeObject<List<ReportFilter>>(jsonReports);
                if (reports != null)
                {
                    foreach (ReportFilter report in reports)
                    {
                        if (report.ReportName == reportName)
                        {
                            jsonBack = JsonConvert.SerializeObject(report);
                            break;
                        }
                    }
                }
            }
            return Json(jsonBack);
        }

        [Route("/DeleteFilterByReportName")]
        [HttpPost]
        public IActionResult DeleteFilterByReportName(string reportName)
        {
            int removeAt = -1;
            string login_user_name = "";
            if (reportName[^1].ToString() == " ")
            {
                login_user_name = "public";
            }
            else
            {
                foreach (var item in User.Claims)
                {
                    if (item.Type == "preferred_username")
                        login_user_name = item.Value.Split("@")[0];
                }
            }

            string path = "wwwroot/Temp/" + login_user_name + ".json";
            if (System.IO.File.Exists(path))
            {
                int count = 0;
                string jsonReports = System.IO.File.ReadAllText(path);
                List<ReportFilter>? reports = JsonConvert.DeserializeObject<List<ReportFilter>>(jsonReports);
                if (reports != null)
                {
                    foreach (ReportFilter report in reports)
                    {
                        if (report.ReportName == reportName)
                        {
                            removeAt = count;
                        }
                        count++;
                    }
                    if (removeAt >= 0)
                    {
                        reports.RemoveAt(removeAt);
                        string jsonBack = JsonConvert.SerializeObject(reports);
                        System.IO.File.WriteAllText(path, jsonBack);
                    }
                }
            }
            return Content(removeAt.ToString());
        }

        [Route("/SaveFilter")]
        [HttpPost]
        public IActionResult Save(ReportFilter objRpt)
        {
            int removeAt = -1;
            string login_user_name = "";

            if (objRpt.FilterType == "Public")
            {
                login_user_name = "public";
            }
            else
            {
                foreach (var item in User.Claims)
                {
                    if (item.Type == "preferred_username")
                        login_user_name = item.Value.Split("@")[0];
                }
            }

            string path = "wwwroot/Temp/" + login_user_name + ".json";
            if (System.IO.File.Exists(path))
            {
                int count = 0;
                string jsonReports = System.IO.File.ReadAllText(path);
                List<ReportFilter>? reports = JsonConvert.DeserializeObject<List<ReportFilter>>(jsonReports);
                if (reports != null)
                {
                    foreach (ReportFilter report in reports)
                    {
                        if (report.ReportName == objRpt.ReportName)
                        {
                            removeAt = count;
                        }
                        count++;
                    }
                    if (removeAt >= 0)
                    {
                        reports.RemoveAt(removeAt);
                    }
                    reports.Add(objRpt);
                    string jsonBack = JsonConvert.SerializeObject(reports);
                    System.IO.File.WriteAllText(path, jsonBack);
                }
            }
            else
            {
                List<ReportFilter> reports = [];
                reports.Add(objRpt);
                string jsonBack = JsonConvert.SerializeObject(reports);
                System.IO.File.WriteAllText(path, jsonBack);
            }

            return Content(removeAt.ToString());
        }

        [Route("/download")]
        [HttpPost]
        public IActionResult Download(ReportFilter objRpt)
        {
            string json = JsonConvert.SerializeObject(objRpt);
            string id = Guid.NewGuid().ToString();
            string path = Path.Combine(_env.WebRootPath, "Temp/" + id + ".json");
            System.IO.File.WriteAllText(path, json);

            return Content(id);
        }

        [Route("/download")]
        public IActionResult Download(string id)
        {
            string path = Path.Combine(_env.WebRootPath, "Temp/" + id + ".json");
            string json = System.IO.File.ReadAllText(path);
            ReportFilter? objRpt = JsonConvert.DeserializeObject<ReportFilter>(json);
            System.IO.File.Delete(path);

            string strFileName = "SDPlusReport_" + GenerateFileName() + ".xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            try
            {
                string login_full_name = "";
                string login_user_name = "";
                foreach (var item in User.Claims)
                {
                    if (item.Type == "preferred_username")
                        login_user_name = item.Value.Split("@")[0];
                    if (item.Type == "name")
                        login_full_name = item.Value;
                }
                string connectionString = _configuration.GetConnectionString("DbConnection").ToString();
                ReportData rptData = new ReportData();
                List<Report> rptDataList = rptData.GetReportData(connectionString, objRpt, login_user_name, login_full_name).ToList();
                DataTable dt = rptData.ToDataTable(rptDataList.ToList());
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt, "Report");
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content, contentType, strFileName);
                    }
                }
            }
            catch (Exception ex)
            {
                return Error();
            }
        }

        private List<HoursByAccount> GetHoursAccount(string connectionString)
        {
            HoursByAccountData objData = new HoursByAccountData();
            DataTable dt = new DataTable();

            dt = objData.GetHoursByAccount(connectionString);
            var lstAccount = dt.AsEnumerable().Select(row => row[0]).Distinct().ToList();
            lstAccount = lstAccount.OrderBy(a => a.ToString()).ToList();

            List<HoursByAccount> objReportData = new List<HoursByAccount>();
            decimal janTotal = 0.0m;
            decimal febTotal = 0.0m;
            decimal marTotal = 0.0m;
            decimal aprTotal = 0.0m;
            decimal mayTotal = 0.0m;
            decimal junTotal = 0.0m;
            decimal julTotal = 0.0m;
            decimal augTotal = 0.0m;
            decimal sepTotal = 0.0m;
            decimal octTotal = 0.0m;
            decimal novTotal = 0.0m;
            decimal decTotal = 0.0m;

            foreach (string objAccount in lstAccount)
            {
                string accountFilter = "accountname= '" + objAccount + "'";
                DataRow[] filteredRows = dt.Select(accountFilter);
                HoursByAccount objReportRow = new HoursByAccount();
                objReportRow.AccountName = objAccount;
                decimal grandTotal = 0.0m;
                //filteredRows=filteredRows.OrderBy(a => a.).ToList();
                foreach (DataRow row in filteredRows)
                {
                    objReportRow.Category = Convert.ToString(row[1]);
                    objReportRow.Activity = Convert.ToString(row[2]);
                    var value = row[4] == DBNull.Value ? 0.0m : Convert.ToDecimal(row[4]);
                    grandTotal += value;

                    var monthStr = Convert.ToString(row[3]) ?? string.Empty;
                    var abbr = monthStr.Length >= 3 ? monthStr.Substring(0, 3) : monthStr;
                    switch (abbr)
                    {
                        case "Jan":
                            janTotal += value;
                            objReportRow.January += value;
                            break;
                        case "Feb":
                            febTotal += value;
                            objReportRow.February += value;
                            break;
                        case "Mar":
                            marTotal += value;
                            objReportRow.March += value;
                            break;
                        case "Apr":
                            aprTotal += value;
                            objReportRow.April += value;
                            break;
                        case "May":
                            mayTotal += value;
                            objReportRow.May += value;
                            break;
                        case "Jun":
                            junTotal += value;
                            objReportRow.June += value;
                            break;
                        case "Jul":
                            julTotal += value;
                            objReportRow.July += value;
                            break;
                        case "Aug":
                            augTotal += value;
                            objReportRow.August += value;
                            break;
                        case "Sep":
                            sepTotal += value;
                            objReportRow.September += value;
                            break;
                        case "Oct":
                            octTotal += value;
                            objReportRow.October += value;
                            break;
                        case "Nov":
                            novTotal += value;
                            objReportRow.November += value;
                            break;
                        case "Dec":
                            decTotal += value;
                            objReportRow.December += value;
                            break;
                        default:
                            break;
                    }
                }
                objReportRow.YearTotal = grandTotal;
                objReportData.Add(objReportRow);
            }

            HoursByAccount objReport = new HoursByAccount();
            objReport.AccountName = "<b>TOTAL</b>";
            objReport.Category = "";
            objReport.Activity = "";
            objReport.January = janTotal;
            objReport.February = febTotal;
            objReport.March = marTotal;
            objReport.April = aprTotal;
            objReport.May = mayTotal;
            objReport.June = junTotal;
            objReport.July = julTotal;
            objReport.August = augTotal;
            objReport.September = sepTotal;
            objReport.October = octTotal;
            objReport.November = novTotal;
            objReport.December = decTotal;
            objReport.YearTotal = janTotal + febTotal + marTotal + aprTotal + mayTotal + junTotal + julTotal + augTotal + sepTotal + octTotal + novTotal + decTotal;
            objReportData.Add(objReport);
            return objReportData.ToList();
        }

        private List<string> GenerateDynamicHeaders1()
        {
            var headers = new List<string> { "AccountName", "Department", "Category", "Activity" };

            DateTime currentDate = DateTime.Now;
            for (int i = 11; i >= 0; i--)
            {
                DateTime monthDate = currentDate.AddMonths(-i);
                headers.Add(monthDate.ToString("MMM-yy")); // "Mar-25"
            }

            headers.Add("Grand Total");
            return headers;
        }

        private List<Dictionary<string, object>> FlattenReportData1(
            List<HoursByAccountDetails> reportData, List<string> headers)
        {
            var flattened = new List<Dictionary<string, object>>();

            foreach (var item in reportData)
            {
                var row = new Dictionary<string, object>
                {
                    {"AccountName", item.AccountName ?? string.Empty},
                    {"Department", item.Department ?? string.Empty},
                    {"Category", item.Category ?? string.Empty},
                    {"Activity",item.Activity?? string.Empty}
                };

                // Process month headers (skip first 3 and last column)
                for (int i = 4; i < headers.Count - 1; i++)
                {
                    string header = headers[i];  // e.g., "Mar-25"
                    string monthName = GetMonthNameFromHeader(header);
                    decimal value = GetMonthValue1(item, monthName);
                    row[header] = value;
                }

                // Add GrandTotal
                row["Grand Total"] = item.YearTotal;

                flattened.Add(row);
            }

            return flattened;
        }

        private decimal GetMonthValue1(HoursByAccountDetails item, string monthName)
        {
            try
            {
                var property = typeof(HoursByAccountDetails).GetProperty(monthName);
                if (property != null)
                {
                    var value = property.GetValue(item);
                    return value != null ? (decimal)value : 0.0m;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting value for {monthName}: {ex.Message}");
            }

            return 0.0m;
        }

        //FOR HOUR By Account
        private List<string> GenerateDynamicHeaders()
        {
            var headers = new List<string> { "AccountName", "Category", "Activity" };

            DateTime currentDate = DateTime.Now;
            for (int i = 11; i >= 0; i--)
            {
                DateTime monthDate = currentDate.AddMonths(-i);
                headers.Add(monthDate.ToString("MMM-yy")); // "Mar-25"
            }

            headers.Add("Grand Total");
            return headers;
        }
        private List<Dictionary<string, object>> FlattenReportData(
    List<HoursByAccount> reportData,
    List<string> headers)
        {
            var flattened = new List<Dictionary<string, object>>();

            foreach (var item in reportData)
            {
                var row = new Dictionary<string, object>
                {
                    {"AccountName", item.AccountName ?? string.Empty},
                    {"Category", item.Category ?? string.Empty} ,
                    {"Activity", item.Activity ?? string.Empty},
                };

                // Process month headers (skip first 2 and last column)
                for (int i = 3; i < headers.Count - 1; i++)
                {
                    string header = headers[i];  // e.g., "Mar-25"
                    string monthName = GetMonthNameFromHeader(header);  // "March"

                    // Get value from HoursByAccount object using reflection or switch
                    decimal value = GetMonthValue(item, monthName);
                    row[header] = value;
                }

                // Add GrandTotal
                row["Grand Total"] = item.YearTotal;

                flattened.Add(row);
            }

            return flattened;
        }

        private string GetMonthNameFromHeader(string header)
        {
            var monthMap = new Dictionary<string, string>
            {
                {"Jan", "January"},
                {"Feb", "February"},
                {"Mar", "March"},
                {"Apr", "April"},
                {"May", "May"},
                {"Jun", "June"},
                {"Jul", "July"},
                {"Aug", "August"},
                {"Sep", "September"},
                {"Oct", "October"},
                {"Nov", "November"},
                {"Dec", "December"}
             };

            try
            {
                // Handle formats like "Mar-25", "Mar-2025", "March-25"
                string[] parts = header.Split('-');

                if (parts.Length > 0)
                {
                    string monthPart = parts[0].Trim();

                    // Check if it's already a full month name
                    if (monthMap.ContainsValue(monthPart))
                    {
                        return monthPart;
                    }

                    // Check if it's an abbreviation
                    if (monthPart.Length >= 3)
                    {
                        string monthAbbr = monthPart.Substring(0, 3);

                        if (monthMap.ContainsKey(monthAbbr))
                        {
                            return monthMap[monthAbbr];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log exception if needed
                System.Diagnostics.Debug.WriteLine($"Error parsing header '{header}': {ex.Message}");
            }

            // Fallback: return original header
            return header;
        }

        private decimal GetMonthValue(HoursByAccount item, string monthName)
        {
            try
            {
                var property = typeof(HoursByAccount).GetProperty(monthName);
                if (property != null)
                {
                    var value = property.GetValue(item);
                    return value != null ? (decimal)value : 0.0m;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting value for {monthName}: {ex.Message}");
            }

            return 0.0m;
        }

        private string GenerateFileName()
        {
            var result = DateTime.Now.ToString("yyyyMMddHHmmss");
            return result;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
