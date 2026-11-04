using Microsoft.AspNetCore.Mvc;
using SDPlusReport.Data;
using SDPlusReport.Entity;
using System.Data;
using System.Text;

namespace SDPlusReport.Web.Controllers
{
    public class AnnualReportController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public AnnualReportController(ILogger<HomeController> logger, IConfiguration configuration, IWebHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _configuration = configuration;
            _env = hostEnvironment;
        }
        public IActionResult Index()
        {
            string connectionString = _configuration.GetConnectionString("DbConnection").ToString();
            HoursByAccountData objData=new HoursByAccountData();
            DataTable dt = new DataTable();
            string strDepartment = string.Empty;
            dt = objData.GetHoursByAccount(connectionString);

            var objAccountList = dt.AsEnumerable().Select(row => row[0]).Distinct().ToList();
            List<HoursByAccount> objReportData=new List<HoursByAccount>();            

            decimal augTotal = 0.0m;
            decimal sepTotal = 0.0m;
            decimal octTotal = 0.0m;
            decimal novTotal = 0.0m;
            decimal decTotal = 0.0m;                     
            
            foreach (string objAccount in objAccountList)
            {

                string filterExpression = "accountname = '" + objAccount + "'";
                DataRow[] filteredRows = dt.Select(filterExpression);

                HoursByAccount objReportRow= new HoursByAccount();
                objReportRow.AccountName = objAccount;
                
                decimal grandTotal = 0.0m;

                foreach (DataRow row in filteredRows)
                {
                    grandTotal += (decimal)row[1];

                    if (row[0].ToString() == "Aug")
                    {
                        augTotal += (decimal)row[1];
                        objReportRow.August = (decimal)row[1];
                    }
                    else if (row[0].ToString() == "Sep")
                    {
                        sepTotal += (decimal)row[1];
                        objReportRow.September = (decimal)row[1];
                    }
                    else if (row[0].ToString() == "Oct")
                    {
                        octTotal += (decimal)row[1];
                        objReportRow.October = (decimal)row[1];
                    }
                    else if (row[0].ToString() == "Nov")
                    {
                        novTotal += (decimal)row[1];
                        objReportRow.November = (decimal)row[1];
                    }
                    else if (row[0].ToString() == "Dec")
                    {
                        decTotal += (decimal)row[1];
                        objReportRow.December = (decimal)row[1];
                    }     
                }

                objReportRow.YearTotal = grandTotal;
                objReportData.Add(objReportRow);
            }           
            HoursByAccount objRpt=new HoursByAccount();
            objRpt.AccountName = "TOTAL";
            objRpt.August = augTotal;
            objRpt.September = sepTotal;
            objRpt.October = octTotal;
            objRpt.November=novTotal;
            objRpt.December = decTotal;
            objReportData.Add(objRpt);

            ViewBag.Content= objReportData;
            return View();
        }
    }
}
