using AWS.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AWS.Areas.Admin.Controllers
{
    public class StationController : Controller
    {
        private AWSDatabaseContext db = new AWSDatabaseContext();
        private clsDatabase dbs = new clsDatabase();
        // GET: Admin/Station
        public ActionResult Index(string ID)
        {
            int userid = Convert.ToInt32(Session["userid"]);
            //Clear all data from temp table 
            IEnumerable<tbl_StationConfigTemp> list = db.tbl_StationConfigTemp.Where(x => x.UserID == userid.ToString()).ToList();
            db.tbl_StationConfigTemp.RemoveRange(list);
            db.SaveChanges();
            //Check Insert and update
            if (ID == null)
            {
                List<stationprofile> lstprof = new List<stationprofile>();
                var query = db.tbl_ProfileMaster.ToList();
                foreach (var item in query)
                {
                    lstprof.Add(new stationprofile
                    {
                        name = item.Name,
                        value = item.Name
                    });
                }
                ViewBag.stprofile = lstprof;
            }
            else
            {
                Session.Add("UpdateID", ID);
                tbl_StationConfigTemp sttemp = new tbl_StationConfigTemp();
                List<stationprofile> lstprof = new List<stationprofile>();
                var query = db.tbl_ProfileMaster.ToList();
                foreach (var item in query)
                {
                    lstprof.Add(new stationprofile
                    {
                        name = item.Name,
                        value = item.Name
                    });
                }
                ViewBag.stprofile = lstprof;
                int id = Convert.ToInt32(ID);
                var getStationSql = db.tbl_StationMaster.Where(x => x.ID == id).FirstOrDefault();
                ViewBag.sname = getStationSql.Name;
                Session.Add("dropdownvalue", getStationSql.Profile);
                var getProfileid = db.tbl_ProfileMaster.Where(x => x.Name == getStationSql.Profile).FirstOrDefault();
                ViewBag.ProfileID = getProfileid.ID;
                ViewBag.stID = getStationSql.StationID;
                ViewBag.Lat = getStationSql.Latitude;
                ViewBag.Long = getStationSql.Longitude;
                ViewBag.City = getStationSql.City;
                ViewBag.District = getStationSql.District;
                ViewBag.State = getStationSql.State;
                ViewBag.Tahesil = getStationSql.TehsilTaluk;
                ViewBag.Block = getStationSql.Block;
                ViewBag.Village = getStationSql.Village;
                ViewBag.Address = getStationSql.Address;
                ViewBag.Bank = getStationSql.Bank;
                ViewBag.BusStand = getStationSql.BusStand;
                ViewBag.RailwayStation = getStationSql.RailwayStation;
                ViewBag.Airport = getStationSql.Airport;
                ViewBag.other = getStationSql.OtherInformation;
                var getSensordetailsSQL = getProfileid.SensorID;
                string[] splitsensor = getSensordetailsSQL.Split(',');
                for (int i = 0; i < splitsensor.Length; i++)
                {
                    if (getStationSql.Gain != null)
                    {
                        string[] gainValue = getStationSql.Gain.Split(',');
                        if (i >= gainValue.Length)
                        {
                            sttemp.Gain = null;
                        }
                        else
                        {
                            sttemp.Gain = gainValue[i];
                        }
                    }
                    else
                    {
                        sttemp.Gain = null;
                    }
                    if (getStationSql.Offset != null)
                    {
                        string[] OffsetValue = getStationSql.Offset.Split(',');
                        if (i >= OffsetValue.Length)
                        {
                            sttemp.Offset = null;
                        }
                        else
                        {
                            sttemp.Offset = OffsetValue[i];
                        }
                    }
                    else
                    {
                        sttemp.Offset = null;
                    }
                    if (getStationSql.SerialNumber != null)
                    {
                        string[] SerialNumberValue = getStationSql.SerialNumber.Split(',');
                        if (i >= SerialNumberValue.Length)
                        {
                            sttemp.SerialNumber = null;
                        }
                        else
                        {
                            sttemp.SerialNumber = SerialNumberValue[i];
                        }
                    }
                    else
                    {
                        sttemp.SerialNumber = null;
                    }
                    sttemp.SensorName = splitsensor[i];
                    sttemp.UserID = userid.ToString();
                    var model = db.tbl_StationConfigTemp;
                    model.Add(sttemp);
                    db.SaveChanges();
                }
            }
            return View();
        }
        [HttpPost]
        public ActionResult Index(string stationid, String stationname, string latitude, string longnitude, string city, string state, string district, string tahesil, string Block, string village, string address, string bank, string busstand, string airport, string otherinfo, string image, HttpPostedFileBase photo)
        {

            int userid = Convert.ToInt32(Session["userid"]);
            string createQuery = string.Empty;
            string getgain = "";
            string getoffset = "";
            string getShowInGraph = "";
            string getShowInGrid = "";

            string getserialNumber = "";
            ViewBag.Image = "no-image.png";
            if (photo != null)
            {
                SaveFile(photo);
            }
            tbl_StationMaster stationmaster = new tbl_StationMaster();
            var profile =Session["dropdownvalue"].ToString();
            stationmaster.StationID = stationid;
            stationmaster.Name = stationname;
            stationmaster.Latitude = latitude;
            stationmaster.Longitude = longnitude;
            stationmaster.City = city;
            stationmaster.State = state;
            stationmaster.TehsilTaluk = tahesil;
            stationmaster.Block = Block;
            stationmaster.Village = village;
            stationmaster.Address = address;
            stationmaster.Bank = bank;
            stationmaster.District = district;
            stationmaster.BusStand = busstand;
            stationmaster.Airport = airport;
            stationmaster.OtherInformation = otherinfo;
            stationmaster.Image = ViewBag.Image;
            stationmaster.Profile = profile;
            var sql = db.tbl_StationConfigTemp.Where(x => x.UserID == userid.ToString()).ToList();
            ///Get Gain of all sensors
            foreach (var gainitem in sql)
            {
                if (gainitem.Gain == null)
                {
                    getgain += null+",";
                }
                else
                {
                    getgain += gainitem.Gain + ",";
                }
            }
            var gain = getgain.TrimEnd(',');
            ///Get offset of all sensors
            foreach (var offsetitem in sql)
            {
                if (offsetitem.Offset == null)
                {
                    getoffset += null + ",";
                }
                else
                {
                    getoffset += offsetitem.Offset + ",";
                }
            }
            var offset = getoffset.TrimEnd(',');
            foreach (var showingraph in sql)
            {
                if (showingraph.ShowInGraph == null)
                {
                    getShowInGraph += "0,";
                }
                else
                {
                    getShowInGraph += showingraph.ShowInGraph + ",";

                }
            }
            var ShowInGraph = getShowInGraph.TrimEnd(',');
            foreach (var showingrid in sql)
            {
                if (showingrid.ShowInGrid == null)
                {
                    getShowInGrid += "0,";
                }
                else
                {
                    getShowInGrid += showingrid.ShowInGrid + ",";
                }
            }
            var ShowInGrid = getShowInGrid.TrimEnd(',');
            ///Get serialnumber of all sensors
            foreach (var serialnumber in sql)
            {
                if (serialnumber.SerialNumber == null)
                {
                    getserialNumber += null + ",";
                }
                else
                {
                    getserialNumber += serialnumber.SerialNumber + ",";
                }
            }
            var serialNumber = getserialNumber.TrimEnd(',');
            stationmaster.Gain = gain;
            stationmaster.Offset = offset;
            stationmaster.SerialNumber = serialNumber;
            stationmaster.ShowInGraph = ShowInGraph;
            stationmaster.ShowInGrid = ShowInGrid;
            if (Convert.ToString(Session["UpdateID"]) == "")
            {
                var modelSation = db.tbl_StationMaster;
                modelSation.Add(stationmaster);
                db.SaveChanges();
            }
            else
            {
                int id = Convert.ToInt32(Session["UpdateID"]);
                stationmaster.ID = id;
                db.Entry(stationmaster).State = EntityState.Modified;
                db.SaveChanges();
            }
            var sensorNamesql = db.tbl_ProfileMaster.Where(x => x.Name == profile).FirstOrDefault();
            string[] splitSensornameSql = sensorNamesql.SensorID.Split(',');
            foreach (var data in splitSensornameSql)
            {
                if (data.Contains("DateTime"))
                {
                    createQuery += "Date varchar(500),Time varchar(500),";
                }
                else
                {
                    createQuery += "[" + data + "]" + " varchar(500),";
                }

            }
            if (Convert.ToString(Session["UpdateID"]) == "")
            {
                var finalParameterquery = createQuery.TrimEnd(',');
                var querySql = "Create table tbl_StationData_" + stationid + "(ID int not null identity(1,1)," + finalParameterquery + ")";
                dbs.CreateTable(querySql, "WEB");
            }

            var getTempModel = db.tbl_StationConfigTemp;
            var getDeletefile = db.tbl_StationConfigTemp.Where(x => x.UserID == userid.ToString()).ToList();
            foreach (var dfiles in getDeletefile)
            {
                var getData = getTempModel.FirstOrDefault(it => it.ID == dfiles.ID);
                getTempModel.Remove(getData);
                db.SaveChanges();
            }
          
            TempData["Added"] = "Added";
            return RedirectToAction("Index","StationGridDisplay");
        }

        private void SaveFile(HttpPostedFileBase file)
        {
            var targetLocation = Server.MapPath("~/Images/");

            try
            {
                var path = Path.Combine(targetLocation, file.FileName);
                ViewBag.Image = file.FileName;
                //Uncomment to save the file
                file.SaveAs(path);
            }
            catch
            {
                Response.StatusCode = 400;
            }
        }
        public JsonResult getDrodownvalue(string value)
        {
            int userid = Convert.ToInt32(Session["userid"]);
            var checktemp = db.tbl_StationConfigTemp.Where(x => x.UserID == userid.ToString());
            if (checktemp.Count() == 0)
            {
                var data = "";
                var profile = db.tbl_ProfileMaster.Where(X => X.Name == value).ToList();
                foreach (var item in profile)
                {
                    data = item.SensorID;
                }
                string[] final_data = data.Split(',');
                tbl_StationConfigTemp stTemp = new tbl_StationConfigTemp();
                List<tbl_StationConfigTemp> lstSttiontemp = new List<tbl_StationConfigTemp>();
                for (int i = 0; i <= final_data.Length - 1; i++)
                {
                    stTemp.SensorName = final_data[i];
                    stTemp.Gain = null;
                    stTemp.Offset = null;
                    stTemp.SerialNumber = null;
                    stTemp.ShowInGraph = null;
                    stTemp.ShowInGrid = null;
                    stTemp.UserID = userid.ToString();
                    var model = db.tbl_StationConfigTemp;
                    model.Add(stTemp);
                    db.SaveChanges();
                }

                Session.Add("gridvalue", data);
                Session.Add("dropdownvalue", value);
                return Json(stTemp, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var model = db.tbl_StationConfigTemp;
                var item1 = model.Where(x => x.UserID == userid.ToString()).ToList();
                foreach (var deleteitem in item1)
                {
                    var getData = model.FirstOrDefault(it => it.ID == deleteitem.ID);
                    model.Remove(getData);
                    db.SaveChanges();
                }
                var data = "";
                var profile = db.tbl_ProfileMaster.Where(X => X.Name == value).ToList();
                foreach (var item in profile)
                {
                    data = item.SensorID;
                }
                string[] final_data = data.Split(',');
                tbl_StationConfigTemp stTemp = new tbl_StationConfigTemp();
                List<tbl_StationConfigTemp> lstSttiontemp = new List<tbl_StationConfigTemp>();
                for (int i = 0; i <= final_data.Length - 1; i++)
                {
                    stTemp.SensorName = final_data[i];
                    stTemp.Gain = null;
                    stTemp.Offset = null;
                    stTemp.SerialNumber = null;
                    stTemp.ShowInGraph = null;
                    stTemp.ShowInGrid = null;
                    stTemp.UserID = userid.ToString();
                    var model1 = db.tbl_StationConfigTemp;
                    model1.Add(stTemp);
                    db.SaveChanges();
                }

                Session.Add("gridvalue", data);
                Session.Add("dropdownvalue", value);
                return Json(model, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult GridViewPartial()
        {
            int userid = Convert.ToInt32(Session["userid"]);
            List<tbl_StationConfigTemp> lstSttiontemp = new List<tbl_StationConfigTemp>();
            var query = db.tbl_StationConfigTemp.OrderBy(t => t.ID);
            var allButTheLastTwoElements = query.Take(query.Count() - 1);
            //var stParam = allButTheLastTwoElements.Skip(3).ToList();
            var stParam = allButTheLastTwoElements.ToList();

            foreach (var item in query)
            {
                lstSttiontemp.Add(new tbl_StationConfigTemp
                {
                    ID = item.ID,
                    SensorName = item.SensorName,
                    Gain = item.Gain,
                    Offset = item.Offset,
                    SerialNumber = item.SerialNumber,
                    UserID = item.UserID,
                    ShowInGraph = item.ShowInGraph,
                    ShowInGrid = item.ShowInGrid
                });
            }
            ViewBag.stparam = lstSttiontemp;
            return PartialView("GridViewPartial");
        }
        public void Update(List<String> newdata, List<String> olddata)
        {
            int userid = Convert.ToInt32(Session["userid"]);
            tbl_StationConfigTemp stTemp = new tbl_StationConfigTemp();
            string newcombindedString = string.Join(",", newdata.ToArray());
            dynamic data = JObject.Parse(newcombindedString);
            string oldcombindedString = string.Join(",", olddata.ToArray());
            dynamic Olddata = JObject.Parse(oldcombindedString);
            stTemp.Gain = data.Gain == null ? Olddata.Gain : data.Gain;
            stTemp.Offset = data.Offset == null ? Olddata.Offset : data.Offset;
            stTemp.SerialNumber = data.SerialNumber == null ? Olddata.SerialNumber : data.SerialNumber;
            stTemp.ID = Olddata.ID;
            stTemp.UserID = userid.ToString();
            stTemp.ShowInGrid = data.ShowInGrid == null ? Olddata.ShowInGrid : data.ShowInGrid;
            stTemp.ShowInGraph = data.ShowInGraph == null ? Olddata.ShowInGraph : data.ShowInGraph;
            stTemp.SensorName = Olddata.SensorName;
            db.Entry(stTemp).State = EntityState.Modified;
            // this.UpdateModel(modelItem);
            db.SaveChanges();
        }
    }
}