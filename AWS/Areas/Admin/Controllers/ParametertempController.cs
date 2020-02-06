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
    public class ParametertempController : Controller
    {
        // GET: Admin/Parametertemp
        AWSDatabaseContext db = new AWSDatabaseContext();
        public ActionResult Index()
        {
            int userid = Convert.ToInt32(Session["userid"]);
            List<tbl_parametertemp> lstParameter = new List<tbl_parametertemp>();
            var query = db.tbl_parametertemp.ToList();
            foreach (var item in query)
            {
                lstParameter.Add(new tbl_parametertemp
                {
                    ID = item.ID,
                   ParameterName = item.ParameterName
                });
            }
            ViewBag.stparam = lstParameter;
            return View();
        }
        public void Insert(List<String> value)
        {
            int userid = Convert.ToInt32(Session["userid"]);
            tbl_parametertemp parameter = new tbl_parametertemp();
            string newcombindedString = string.Join(",", value.ToArray());
            dynamic data = JObject.Parse(newcombindedString);
            parameter.ParameterName = data.ParameterName;
            parameter.CreatedBy = userid;
            parameter.IsDeleted = false;
            parameter.UpdatedBy = userid;
            parameter.CreatedDate = DateTime.Now;
            parameter.UpdatedDate = DateTime.Now;
            var model = db.tbl_parametertemp;
            model.Add(parameter);
            db.SaveChanges();
        }
        public ActionResult Update(List<String> newdata, List<String> olddata, tbl_parametertemp parameter1)
        {
            int userid = Convert.ToInt32(Session["userid"]);
            tbl_parametertemp parameter = new tbl_parametertemp();
            string newcombindedString = string.Join(",", newdata.ToArray());
            dynamic data = JObject.Parse(newcombindedString);
            string oldcombindedString = string.Join(",", olddata.ToArray());
            dynamic Olddata = JObject.Parse(oldcombindedString);
            if (newcombindedString.Contains("ParameterName"))
            {
                parameter.ParameterName = data.ParameterName;
            }
            else
            {
                parameter.ParameterName = Olddata.ParameterName;
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
            var model = db.tbl_parametertemp;
            var item1 = model.FirstOrDefault(it => it.ID == ID);
            model.Remove(item1);
            db.SaveChanges();
        }
    }
}