using AWS.Models;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace AWS.Areas.User.Controllers
{
    public class ReportsController : Controller
    {
        private AWSDatabaseContext db = new AWSDatabaseContext();
        private clsDatabase odb = new clsDatabase();
        private string ReportcolumnName = "";
        // GET: User/Reports
        public ActionResult Index()
        {
            if (Convert.ToString(Session["U_userid"]) != "")
            {
                var stationSql = db.tbl_StationMaster.Select(x => x.Name).ToList();
                ViewBag.stationSql = stationSql;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login", new { area = "" });
            }
         
        }
        public JsonResult GetData(string toDate, string fromDate, string stationName)
        {
            string json = "";
            var toDateArray = DateTime.Parse(new string(toDate.Take(24).ToArray()));
            var toDateString = toDateArray.ToString("dd/MM/yy");
            var fromDateArray = DateTime.Parse(new string(fromDate.Take(24).ToArray()));
            var fromDateString = fromDateArray.ToString("dd/MM/yy");
            var StationSql = db.tbl_StationMaster.Where(x => x.Name == stationName).FirstOrDefault();
            var stationID = StationSql.StationID;
            ColumnName(stationID);
            var tablename = "tbl_StationData_" + stationID;
            var reportSql = "Select StationID,Date,Time," + ReportcolumnName.TrimEnd(',') + " from " + tablename + "   where Convert(date,date,3) between Convert(date,'" + fromDateString + "',3) and Convert(date,'" + toDateString + "',3) order by Date,Time";
            DataSet reportset = odb.FetchData_Table(reportSql, "WEB");
            DataTable reportdata = new DataTable();
            if (reportset.Tables.Count != 0)
            {
                reportdata = reportset.Tables[0];
                json = JsonConvert.SerializeObject(reportdata);
            }
            ViewBag.ReportData = json;
            TempData["Fdate"] = fromDateString;
            //var ReportDataSql = "Select * from "

            return Json(json, JsonRequestBehavior.AllowGet);
        }
        public void ColumnName(string stationID)
        {

            var id = stationID;
            var tableName = "tbl_StationData_" + id;
            DataSet columnDataset = odb.FetchData_SP_columnName(Sp_list.getColumnName.ToString(), tableName, "WEB");
            DataTable columnDataTable = new DataTable();
            if (columnDataset.Tables.Count != 0)
            {
                columnDataTable = columnDataset.Tables[0];
                for (int i = 0; i < columnDataTable.Rows.Count - 1; i++)
                {
                    var sensorName = columnDataTable.Rows[i][0].ToString();
                    var SensorIDsql = db.tbl_SensorMaster.Where(x => x.Name == sensorName).FirstOrDefault();
                    int sensorID = Convert.ToInt32(SensorIDsql.ID);
                    var getUnit = db.tbl_ParameterMaster.Where(x => x.SensorID == sensorID).FirstOrDefault();
                    if (getUnit == null)
                    {
                        ReportcolumnName += "[" + columnDataTable.Rows[i][0].ToString() + "],";
                    }
                    else
                    {
                        ReportcolumnName += "[" + columnDataTable.Rows[i][0].ToString() + "] as [" + columnDataTable.Rows[i][0].ToString() + "(" + getUnit.Unit + ")]" + ",";

                    }
                }
            }
        }
    }
}