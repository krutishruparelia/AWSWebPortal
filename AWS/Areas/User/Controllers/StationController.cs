using AWS.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace AWS.Areas.User.Controllers
{
    public class StationController : Controller
    {
        private clsDatabase adDB = new clsDatabase();
        private AWSDatabaseContext db = new AWSDatabaseContext();
        private string json = "";
        private string yesjson = "";
        private string weeklyjson = "";
        private string monthlyjson = "";
        private string colName = "";
        private string AliasColumnName = "";
        // GET: User/StationView
        public ActionResult Index()
        {
            if (Convert.ToString(Session["U_userid"]) != "")
            {
                int userid = Convert.ToInt32(Session["U_userid"]);
                List<userMap> lstusermap = new List<userMap>();
                //var query = db.tbl_StationMaster.ToList();
                var chkPermission = db.tbl_Permission.Where(x => x.UserID == userid).ToList();
                foreach (var item in chkPermission)
                {
                    var getLatLog = db.tbl_StationMaster.Where(x => x.ID.ToString() == item.StationID).ToList();

                    foreach (var data in getLatLog)
                    {
                        lstusermap.Add(new userMap
                        {
                            location = data.Latitude + "," + data.Longitude,
                            tooltip = data.Name + "<br />" + "<a href = 'Station/StationView/" + data.StationID + "' >View Details</a >"
                        });
                    }
                }

                ViewBag.UserMapData = lstusermap;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login", new { area = "" });
            }

        }
        public ActionResult StationView(string id)
        {
            if (Convert.ToString(Session["U_userid"]) != "")
            {
                int uid = Convert.ToInt32(Session["U_userid"]);
                Session.Add("StationID", id);
                var imagesql = db.tbl_StationMaster.Where(x => x.StationID == id).FirstOrDefault();
                var imagePath = imagesql.Image;
                ViewBag.ImagePath = "../../../Images/" + imagePath + "";
                List<ColumnNames> lstcol = new List<ColumnNames>();
                var tableName = "tbl_StationData_" + id;
                ///Fetching CoulmnName to display on grid header
                DataSet columnDataset = adDB.FetchData_SP_columnName("getColumnNameForGrid", tableName, "WEB");
                DataTable columnDataTable = new DataTable();
                string columnnames = "";
                string graphcols = "";
                if (columnDataset.Tables.Count != 0)
                {
                    columnDataTable = columnDataset.Tables[0];
                    for (int i = 0; i < columnDataTable.Rows.Count - 1; i++)
                    {

                        columnnames += "\"" + columnDataTable.Rows[i][0] + "\"" + ",";
                        graphcols += columnDataTable.Rows[i][0] + ",";


                    }

                }
                var colnames = "[" + columnnames.TrimEnd(',') + "]";
                var trimStart = colnames.TrimStart('\"');
                var trimEnd = trimStart.TrimEnd('\"');
                ///Fetching Today's Data to display in grid
                ColumnName();
                var col = "Date,Time," + colName.TrimEnd(',');
                DataSet TodayData = adDB.SP_ColName(Sp_list.FetchTodayData.ToString(), tableName, col, uid, "WEB");
                DataTable tdata = new DataTable();
                if (TodayData.Tables.Count != 0)
                {
                    tdata = TodayData.Tables[0];
                    json = JsonConvert.SerializeObject(tdata);
                }
                ///Fetching Yesterday's Data to display in grid

                DataSet yesterdayDataset = adDB.SP_ColName(Sp_list.FetchYesterDayData.ToString(), tableName, col, uid, "WEB");
                DataTable yesdata = new DataTable();
                if (yesterdayDataset.Tables[0].Rows.Count != 0)
                {
                    yesdata = yesterdayDataset.Tables[0];
                    yesjson = JsonConvert.SerializeObject(yesdata);
                }
                ///Fetch Latitude and Longitude for map
                List<map> lstmap = new List<map>();
                var query = db.tbl_StationMaster.Where(x => x.StationID == id).ToList();
                foreach (var item in query)
                {
                    lstmap.Add(new map
                    {
                        location = item.Latitude + "," + item.Longitude,
                        tooltip = item.Name
                    });

                }
                ViewBag.MapData = lstmap;
                ViewBag.StationGrid = json;
                ViewBag.YesterdayGrid = yesjson;
                ViewBag.ColumnNamesGrid = trimEnd;
                var removebracesstart = trimEnd.TrimStart('[');
                var removebracesend = removebracesstart.TrimEnd(']');
                ViewBag.graphColnames = graphcols.TrimEnd(',');
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        public ActionResult TodayView()
        {
            var id = Session["StationID"].ToString();
            var tableName = "tbl_StationData_" + id;
            DataSet columnDataset = adDB.FetchData_SP_columnName(Sp_list.getColumnName.ToString(), tableName, "WEB");
            DataTable columnDataTable = new DataTable();
            if (columnDataset.Tables.Count != 0)
            {
                columnDataTable = columnDataset.Tables[0];
                ViewBag.ColumnNames = columnDataTable;
            }

            return View();
        }
        public ActionResult TodayGraph(string data, string Position, string Unit)
        {
            var id = Session["StationID"].ToString();
            int uid = Convert.ToInt32(Session["U_userid"]);
            var tableName = "tbl_StationData_" + id;
            ViewBag.TodaygUnit = Unit;
            ViewBag.GraphName = data;
            List<graph> lstgraph = new List<graph>();
            ColumnName();
            var grpahcolname = "ID,StationID,Date,Time," + colName.TrimEnd(',');
            DataSet TodayData = adDB.SP_ColName(Sp_list.FetchTodayData.ToString(), tableName, grpahcolname, uid, "WEB");
            DataTable tdata = new DataTable();
            int pos = Convert.ToInt32(Position);
            if (TodayData.Tables.Count != 0)
            {
                tdata = TodayData.Tables[0];
                for (int i = 0; i < tdata.Rows.Count; i++)
                {
                    lstgraph.Add(new graph
                    {
                        Time = tdata.Rows[i][3].ToString(),
                        ColumnName = data,
                        value = tdata.Rows[i][pos].ToString()
                    });

                }

            }

            ViewBag.TodayData = lstgraph;
            return View();
        }
        public ActionResult YesterdayView()
        {
            var id = Session["StationID"].ToString();
            var tableName = "tbl_StationData_" + id;
            DataSet columnDatasetyesterday = adDB.FetchData_SP_columnName(Sp_list.getColumnName.ToString(), tableName, "WEB");
            DataTable columnDataTableyesterday = new DataTable();
            if (columnDatasetyesterday.Tables.Count != 0)
            {
                columnDataTableyesterday = columnDatasetyesterday.Tables[0];
                ViewBag.YesterdayColumnNames = columnDataTableyesterday;
            }
            return View();
        }
        public ActionResult YesterdayGraph(string data, string Position, string Unit)
        {
            var id = Session["StationID"].ToString();
            int uid = Convert.ToInt32(Session["U_userid"]);
            var tableName = "tbl_StationData_" + id;
            ViewBag.YesterdayGraphName = data;
            ViewBag.YesgUnit = Unit;
            List<yesterdaygraph> lstyesterdaygraph = new List<yesterdaygraph>();
            ColumnName();
            var grpahcolname = "ID,StationID,Date,Time," + colName.TrimEnd(',');
            DataSet yesterdayData = adDB.SP_ColName(Sp_list.FetchYesterDayData.ToString(), tableName, grpahcolname, uid, "WEB");
            DataTable ydata = new DataTable();
            int pos = Convert.ToInt32(Position);
            if (yesterdayData.Tables.Count != 0)
            {
                ydata = yesterdayData.Tables[0];
                for (int i = 0; i < ydata.Rows.Count - 1; i++)
                {
                    lstyesterdaygraph.Add(new yesterdaygraph
                    {
                        Time = ydata.Rows[i][3].ToString(),
                        ColumnName = data,
                        value = ydata.Rows[i][pos].ToString()
                    });
                }
            }

            ViewBag.yesData = lstyesterdaygraph;
            return View();
        }
        public ActionResult WeeklyGrid()
        {
            var id = Session["StationID"].ToString();
            string dynamiccol = "";
            int uid = Convert.ToInt32(Session["U_userid"]);
            var tableName = "tbl_StationData_" + id;
            //Fetch Column Name
            DataSet columnDataset = adDB.FetchData_SP_columnName(Sp_list.getColumnName.ToString(), tableName, "WEB");
            DataTable columnDataTable = new DataTable();
            if (columnDataset.Tables.Count != 0)
            {
                columnDataTable = columnDataset.Tables[0];
                ViewBag.ColumnNames = columnDataTable;
            }
            ///Create Weekly column name
            DataSet weeklycolumnanedataset = adDB.FetchData_SP_columnName("getColumnName", tableName, "WEB");
            DataTable weeklycolumndatatable = new DataTable();
            if (weeklycolumnanedataset.Tables.Count != 0)
            {
                weeklycolumndatatable = weeklycolumnanedataset.Tables[0];
                for (int i = 0; i < weeklycolumndatatable.Rows.Count - 1; i++)
                {
                    dynamiccol += "min([" + weeklycolumndatatable.Rows[i][0].ToString() + "]) as [Min " + weeklycolumndatatable.Rows[i][0].ToString() + "],max([" + weeklycolumndatatable.Rows[i][0].ToString() + "]) as [Max " + weeklycolumndatatable.Rows[i][0].ToString() + "],CEILING(avg(Try_convert(float,[" + weeklycolumndatatable.Rows[i][0].ToString() + "]))) as [avg " + weeklycolumndatatable.Rows[i][0].ToString() + "],";
                }
            }
            var trimDynamicCol = dynamiccol.TrimEnd(',');
            ///Fetching Weekly Data to display in grid
            DataSet weeklyDataset = adDB.FetchData_SP_MonthlyWeeklyData("getWeeklyMonthlyData", tableName, trimDynamicCol, uid, "WEB");
            DataTable wdata = new DataTable();
            if (weeklyDataset.Tables[0].Rows.Count != 0)
            {
                wdata = weeklyDataset.Tables[0];
                weeklyjson = JsonConvert.SerializeObject(wdata);
            }
            ViewBag.week = wdata;
            return View();
        }
        public ActionResult TodayOverviewView()
        {
            int uid = Convert.ToInt32(Session["U_userid"]);
            var id = Session["StationID"].ToString();
            var tableName = "tbl_StationData_" + id;
            DataSet columnDataset = adDB.FetchData_SP_columnName(Sp_list.getColumnName.ToString(), tableName, "WEB");
            DataTable columnDataTable = new DataTable();
            if (columnDataset.Tables.Count != 0)
            {
                ColumnName();
                var trimDynamicCol = colName.TrimEnd(',');
                DataSet ds2 = adDB.SP_ColName_Alias(Sp_list.getTodayOverview.ToString(), tableName, trimDynamicCol, AliasColumnName.TrimEnd(','), uid, "WEB");
                DataTable dt1 = new DataTable();
                if (ds2.Tables[0].Rows.Count != 0)
                {
                    dt1 = ds2.Tables[0];
                }
                ViewBag.TodayOverview = dt1;
            }
            return View();
        }
        public ActionResult CurrentViewGauge()
        {
            var id = Session["StationID"].ToString();
            var tableName = "tbl_StationData_" + id;
            DataSet getMaxBatteryVoltageDataset = adDB.FetchData_SP_columnName("getMaxbatteryVoltage", tableName, "WEB");
            DataTable getMaxBatteryVoltagDataTable = new DataTable();
            if (getMaxBatteryVoltageDataset.Tables[0].Rows.Count != 0)
            {
                getMaxBatteryVoltagDataTable = getMaxBatteryVoltageDataset.Tables[0];
                ViewBag.MaxBattery = getMaxBatteryVoltagDataTable.Rows[0][0].ToString();
            }
            return View();
        }
        public ActionResult YesterdayOverviewView()
        {
            var id = Session["StationID"].ToString();
            int uid = Convert.ToInt32(Session["U_userid"]);
            var tableName = "tbl_StationData_" + id;
            DataSet yesterdaycolumnDataset = adDB.FetchData_SP_columnName(Sp_list.getColumnName.ToString(), tableName, "WEB");
            DataTable yesterdaycolumnDataTable = new DataTable();
            if (yesterdaycolumnDataset.Tables.Count != 0)
            {
                ColumnName();
                var trimDynamicCol = colName.TrimEnd(',');
                DataSet yesterdaydataset = adDB.SP_ColName_Alias("getYesterdayOverview", tableName, trimDynamicCol, AliasColumnName.TrimEnd(','), uid, "WEB");
                DataTable yesterdaydatatable = new DataTable();
                if (yesterdaydataset.Tables[0].Rows.Count != 0)
                {
                    yesterdaydatatable = yesterdaydataset.Tables[0];
                }
                ViewBag.yesterdayOverview = yesterdaydatatable;
            }
            return View();
        }
        public ActionResult WeeklyOverviewView()
        {
            int uid = Convert.ToInt32(Session["U_userid"]);
            var id = Session["StationID"].ToString();
            var tableName = "tbl_StationData_" + id;
            DataSet WeeklycolumnDataset = adDB.FetchData_SP_columnName(Sp_list.getColumnName.ToString(), tableName, "WEB");
            DataTable WeeklycolumnDataTable = new DataTable();
            if (WeeklycolumnDataset.Tables.Count != 0)
            {
                WeeklycolumnDataTable = WeeklycolumnDataset.Tables[0];
                ViewBag.ColumnNames = WeeklycolumnDataTable;
                ColumnName();
                var trimDynamicCol = colName.TrimEnd(',');
                DataSet Weeklydataset = adDB.SP_ColName_Alias("getWeeklyOverview", tableName, trimDynamicCol, AliasColumnName.TrimEnd(','), uid, "WEB");
                DataTable Weeklydatatable = new DataTable();
                if (Weeklydataset.Tables.Count != 0)
                {
                    Weeklydatatable = Weeklydataset.Tables[0];
                }
                ViewBag.weeklyOverview = Weeklydatatable;
            }
            return View();
        }
        public ActionResult WeeklyView()
        {
            var id = Session["StationID"].ToString();
            var tableName = "tbl_StationData_" + id;
            DataSet columnDatasetWeekly = adDB.FetchData_SP_columnName(Sp_list.getColumnName.ToString(), tableName, "WEB");
            DataTable columnDataTableweekly = new DataTable();
            if (columnDatasetWeekly.Tables.Count != 0)
            {
                columnDataTableweekly = columnDatasetWeekly.Tables[0];
                ColumnName();
                ViewBag.weeklyColumnNames = colName.TrimEnd(',');
            }
            return View();
        }
        public ActionResult WeeklyGraph(string data, string Position, string Unit)
        {
            var id = Session["StationID"].ToString();
            int uid = Convert.ToInt32(Session["U_userid"]);
            var tableName = "tbl_StationData_" + id;
            ViewBag.WeeklyGraphName = data;
            ViewBag.gUnit = Unit;
            string dynamiccol = "";
            DataSet ds1 = adDB.FetchData_SP_columnName("getColumnName", tableName, "WEB");
            DataTable dt = new DataTable();
            List<Weeklygraph> lstweklygraph = new List<Weeklygraph>();
            int pos = Convert.ToInt32(Position);
            dt = ds1.Tables[0];

            ViewBag.weeklycolumnName = dt;
            for (int i = 0; i < dt.Rows.Count; i++)
            {

                dynamiccol += "min([" + dt.Rows[i][0].ToString() + "]) as [Min " + dt.Rows[i][0].ToString() + "],max([" + dt.Rows[i][0].ToString() + "]) as [Max " + dt.Rows[i][0].ToString() + "],CEILING(avg(Try_convert(float,[" + dt.Rows[i][0].ToString() + "] ))) as [avg " + dt.Rows[i][0].ToString() + "],";
            }
            var trimDynamicCol = dynamiccol.TrimEnd(',');
            DataSet ds2 = adDB.FetchData_SP_MonthlyWeeklyData("getWeeklyMonthlyData", tableName, trimDynamicCol, uid, "WEB");
            DataTable dt1 = new DataTable();
            if (ds2.Tables.Count != 0)
            {
                dt1 = ds2.Tables[0];
                for (int i = 0; i < dt1.Rows.Count; i++)
                {

                    lstweklygraph.Add(new Weeklygraph
                    {
                        Time = dt1.Rows[i][0].ToString(),
                        ColumnName = data,
                        min = dt1.Rows[i][pos].ToString(),
                        max = dt1.Rows[i][pos + 1].ToString(),
                        avg = dt1.Rows[i][pos + 2].ToString(),

                    });
                }
            }
            ViewBag.weekData = lstweklygraph;
            return View();
        }
        public ActionResult MonthlyOverviewView()
        {
            int uid = Convert.ToInt32(Session["U_userid"]);
            var id = Session["StationID"].ToString();
            var tableName = "tbl_StationData_" + id;
            DataSet MonthlycolumnDataset = adDB.FetchData_SP_columnName(Sp_list.getColumnName.ToString(), tableName, "WEB");
            DataTable MonthlycolumnDataTable = new DataTable();
            if (MonthlycolumnDataset.Tables.Count != 0)
            {
                MonthlycolumnDataTable = MonthlycolumnDataset.Tables[0];
                ColumnName();
                var trimDynamicCol = colName.TrimEnd(',');
                DataSet Monthlydataset = adDB.SP_ColName_Alias("getMonthlyOverview", tableName, trimDynamicCol, AliasColumnName.TrimEnd(','), uid, "WEB");
                DataTable Monthlydatatable = new DataTable();
                if (Monthlydataset.Tables.Count != 0)
                {
                    Monthlydatatable = Monthlydataset.Tables[0];
                }
                ViewBag.monthlyOverview = Monthlydatatable;
            }
            return View();
        }
        public ActionResult MonthlyView()
        {
            var id = Session["StationID"].ToString();
            var tableName = "tbl_StationData_" + id;
            DataSet columnDatasetMonthly = adDB.FetchData_SP_columnName(Sp_list.getColumnName.ToString(), tableName, "WEB");
            DataTable columnDataTableMonthly = new DataTable();
            if (columnDatasetMonthly.Tables.Count != 0)
            {
                columnDataTableMonthly = columnDatasetMonthly.Tables[0];
                ViewBag.MonthlyColumnNames = columnDataTableMonthly;
            }
            return View();
        }
        public ActionResult MonthlyGraph(string data, string Position,string Unit)
        {
            var id = Session["StationID"].ToString();
            int uid = Convert.ToInt32(Session["U_userid"]);
            var tableName = "tbl_StationData_" + id;
            ViewBag.MonthlyGraphName = data;
            ViewBag.MonthlygUnit = Unit;
            string dynamiccol = "";
            DataSet ds1 = adDB.FetchData_SP_columnName("getColumnName", tableName, "WEB");
            DataTable dt = new DataTable();
            List<monthlygraph> lstmonthlygraph = new List<monthlygraph>();
            int pos = Convert.ToInt32(Position);
            dt = ds1.Tables[0];

            ViewBag.monthlycolumnName = dt;
            for (int i = 0; i < dt.Rows.Count - 1; i++)
            {

                dynamiccol += "min([" + dt.Rows[i][0].ToString() + "]) as [Min " + dt.Rows[i][0].ToString() + "],max([" + dt.Rows[i][0].ToString() + "]) as [Max " + dt.Rows[i][0].ToString() + "],CEILING(avg(Try_convert(float,[" + dt.Rows[i][0].ToString() + "]))) as [avg " + dt.Rows[i][0].ToString() + "],";
            }
            var trimDynamicCol = dynamiccol.TrimEnd(',');
            DataSet ds2 = adDB.FetchData_SP_MonthlyWeeklyData("getMonthlyData", tableName, trimDynamicCol, uid, "WEB");
            DataTable dt1 = new DataTable();
            if (ds2.Tables.Count != 0)
            {
                dt1 = ds2.Tables[0];
                for (int i = 0; i < dt1.Rows.Count; i++)
                {

                    lstmonthlygraph.Add(new monthlygraph
                    {
                        Time = dt1.Rows[i][0].ToString(),
                        ColumnName = data,
                        min = dt1.Rows[i][pos].ToString(),
                        max = dt1.Rows[i][pos + 1].ToString(),
                        avg = dt1.Rows[i][pos + 2].ToString(),

                    });
                }
            }
            ViewBag.montlyData = lstmonthlygraph;
            return View();
        }
        public ActionResult MonthlyGrid()
        {
            int uid = Convert.ToInt32(Session["U_userid"]);
            var id = Session["StationID"].ToString();
            string dynamiccol = "";
            var tableName = "tbl_StationData_" + id;
            //Fetch Column Name
            DataSet columnDataset = adDB.FetchData_SP_columnName(Sp_list.getColumnName.ToString(), tableName, "WEB");
            DataTable columnDataTable = new DataTable();
            if (columnDataset.Tables.Count != 0)
            {
                columnDataTable = columnDataset.Tables[0];
                ViewBag.ColumnNames = columnDataTable;
            }
            ///Create Weekly column name
            DataSet Monthlycolumnanedataset = adDB.FetchData_SP_columnName("getColumnName", tableName, "WEB");
            DataTable Monthlycolumndatatable = new DataTable();
            if (Monthlycolumnanedataset.Tables.Count != 0)
            {
                Monthlycolumndatatable = Monthlycolumnanedataset.Tables[0];
                for (int i = 0; i < Monthlycolumndatatable.Rows.Count - 1; i++)
                {
                    dynamiccol += "min([" + Monthlycolumndatatable.Rows[i][0].ToString() + "]) as [Min " + Monthlycolumndatatable.Rows[i][0].ToString() + "],max([" + Monthlycolumndatatable.Rows[i][0].ToString() + "]) as [Max " + Monthlycolumndatatable.Rows[i][0].ToString() + "],CEILING(avg(Try_convert(float,[" + Monthlycolumndatatable.Rows[i][0].ToString() + "]))) as [avg " + Monthlycolumndatatable.Rows[i][0].ToString() + "],";
                }
            }
            var trimDynamicCol = dynamiccol.TrimEnd(',');
            ///Fetching Weekly Data to display in grid
            DataSet MonthlyDataset = adDB.FetchData_SP_MonthlyWeeklyData("getMonthlyData", tableName, trimDynamicCol, uid, "WEB");
            DataTable mdata = new DataTable();
            if (MonthlyDataset.Tables[0].Rows.Count != 0)
            {
                mdata = MonthlyDataset.Tables[0];
                monthlyjson = JsonConvert.SerializeObject(mdata);
            }
            ViewBag.month = mdata;
            return View();
        }
        public void ColumnName()

        {

            var id = Session["StationID"].ToString();
            var tableName = "tbl_StationData_" + id;
            DataSet columnDataset = adDB.FetchData_SP_columnName(Sp_list.getColumnName.ToString(), tableName, "WEB");
            DataTable columnDataTable = new DataTable();
            if (columnDataset.Tables.Count != 0)
            {
                columnDataTable = columnDataset.Tables[0];
                ViewBag.gpColname = columnDataset.Tables[0];
                for (int i = 0; i < columnDataTable.Rows.Count - 1; i++)
                {
                    var sensorName = columnDataTable.Rows[i][0].ToString();
                    var SensorIDsql = db.tbl_SensorMaster.Where(x => x.Name == sensorName).FirstOrDefault();
                    int sensorID = Convert.ToInt32(SensorIDsql.ID);
                    var getUnit = db.tbl_ParameterMaster.Where(x => x.SensorID == sensorID).FirstOrDefault();
                    if (getUnit == null)
                    {
                        colName += "[" + columnDataTable.Rows[i][0].ToString() + "],";
                        AliasColumnName += "[" + columnDataTable.Rows[i][0].ToString() + "],";
                    }
                    else
                    {
                        colName += "[" + columnDataTable.Rows[i][0].ToString() + "] as [" + columnDataTable.Rows[i][0].ToString() + "(" + getUnit.Unit + ")]" + ",";
                        AliasColumnName += "[" + columnDataTable.Rows[i][0].ToString() + "(" + getUnit.Unit + ")],";
                    }
                }
                ViewBag.gpYesterdayColumnNames = colName.Trim(',');
            }
        }

        public ActionResult CustomRangeOverview()
        {
            int uid = Convert.ToInt32(Session["U_userid"]);
            var id = Session["StationID"].ToString();
            var tableName = "tbl_StationData_" + id;
            var fromdate = Convert.ToString(Session["fromDate"]);
            var todate = Convert.ToString(Session["toDate"]);
            ColumnName();
            DataSet customoverviewset = adDB.SP_CustomRange("getCustomDateOverview", tableName, colName.TrimEnd(','), AliasColumnName.TrimEnd(','), uid, fromdate, todate, "WEB");
            DataTable customoverviewdata = new DataTable();
            if (customoverviewset.Tables.Count != 0)
            {
                customoverviewdata = customoverviewset.Tables[0];
                ViewBag.customoverviewdata = customoverviewdata;
            }
            return View();
        }
        public JsonResult CustomRangeOverviewData(string fromdate, string toDate)
        {
            var id = Session["StationID"].ToString();
            var tableName = "tbl_StationData_" + id;
            DataSet columnDataset = adDB.FetchData_SP_columnName(Sp_list.getColumnName.ToString(), tableName, "WEB");
            DataTable columnDataTable = new DataTable();
            if (columnDataset.Tables.Count != 0)
            {
                columnDataTable = columnDataset.Tables[0];
                ViewBag.ColumnNames = columnDataTable;
            }
            Session.Add("fromDate", fromdate);
            Session.Add("toDate", toDate);
            return Json("", JsonRequestBehavior.AllowGet);
        }
    }
}