using AWS.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AWS.Areas.Admin.Controllers
{
    public class ParameterInSensorController : Controller
    {
        // GET: Admin/ParameterInSensor
        AWSDatabaseContext db = new AWSDatabaseContext();
        public ActionResult Index()
        {
            int userid = Convert.ToInt32(Session["userid"]);
            List<tbl_ParameterInSensor> lstParameter = new List<tbl_ParameterInSensor>();
            List<tbl_SensorMaster> lstSensor = new List<tbl_SensorMaster>();
            List<tbl_parametertemp> lstParameter1 = new List<tbl_parametertemp>();
            

            var query = db.tbl_ParameterInSensor.ToList();
            foreach (var item in query)
            {
                lstParameter.Add(new tbl_ParameterInSensor
                {
                    ID = item.ID,
                    SensorID = item.SensorID,
                    ParameterID = item.ParameterID,
                    Value = item.Value
                });
            }
            ViewBag.stparam = lstParameter;

            // for the Sensor list
            var sqlsensor = db.tbl_SensorMaster.ToList();
            foreach (var item in sqlsensor)
            {
                lstSensor.Add(new tbl_SensorMaster
                {
                    ID = item.ID,
                    Name = item.Name
                });
            }
            ViewBag.stsensor = lstSensor;

            // for Parameter value
            var sqlparameter = db.tbl_parametertemp.ToList();
            foreach (var item in sqlparameter)
            {
                lstParameter1.Add(new tbl_parametertemp
                {
                    ID = item.ID,
                    ParameterName = item.ParameterName
                });
            }
            ViewBag.stparam1 = lstParameter1;

            return View();
        }
        public void Insert(List<String> value)
        {
            int userid = Convert.ToInt32(Session["userid"]);
            tbl_ParameterInSensor parameter = new tbl_ParameterInSensor();
            string newcombindedString = string.Join(",", value.ToArray());
            dynamic data = JObject.Parse(newcombindedString);
            parameter.SensorID = data.SensorID;
            parameter.ParameterID = data.ParameterID;
            parameter.Value = data.Value;
            parameter.CreatedBy = userid;
            parameter.IsDeleted = false;
            parameter.UpdatedBy = userid;
            parameter.CreatedDate = DateTime.Now;
            parameter.UpdatedDate = DateTime.Now;
            var model = db.tbl_ParameterInSensor;
            model.Add(parameter);
            db.SaveChanges();
        }
        public ActionResult Update(List<String> newdata, List<String> olddata, tbl_ParameterInSensor parameter1)
        {
            int userid = Convert.ToInt32(Session["userid"]);
            tbl_ParameterInSensor parameter = new tbl_ParameterInSensor();
            string newcombindedString = string.Join(",", newdata.ToArray());
            dynamic data = JObject.Parse(newcombindedString);
            string oldcombindedString = string.Join(",", olddata.ToArray());
            dynamic Olddata = JObject.Parse(oldcombindedString);
            if (newcombindedString.Contains("SensorID"))
            {
                parameter.SensorID = data.SensorID;
            }
            else
            {
                parameter.SensorID = Olddata.SensorID;
            }

            if (newcombindedString.Contains("ParameterID"))
            {
                parameter.ParameterID = data.ParameterID;
            }
            else
            {
                parameter.ParameterID = Olddata.ParameterID;
            }

            if (newcombindedString.Contains("Value"))
            {
                parameter.Value = data.Value;
            }
            else
            {
                parameter.Value = Olddata.Value;
            }

            parameter.ID = Olddata.ID;
            parameter.CreatedBy = userid;
            parameter.IsDeleted = false;
            parameter.UpdatedBy = userid;
            parameter.CreatedDate = DateTime.Now;
            parameter.UpdatedDate = DateTime.Now;
            db.Entry(parameter).State = EntityState.Modified;
            //this.UpdateModel(parameter);
            db.SaveChanges();
            return View();
        }
        public void Remove(int ID)
        {
            var model = db.tbl_ParameterInSensor;
            var item1 = model.FirstOrDefault(it => it.ID == ID);
            model.Remove(item1);
            db.SaveChanges();
        }
    }
}