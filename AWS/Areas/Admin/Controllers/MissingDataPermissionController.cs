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
    public class MissingDataPermissionController : Controller
    {
        AWSDatabaseContext db = new AWSDatabaseContext();
        // GET: Admin/MissingDataPermission
        public ActionResult Index()
        {
            var datasql = db.tbl_MissingDataPermission.ToList();
            var userlookup = db.tbl_User.ToList();
            ViewBag.UserLookup = userlookup;
            ViewBag.PerData = datasql;
            return View();
        }
        public void Insert(List<String> value)
        {
            int userid = Convert.ToInt32(Session["userid"]);
            tbl_MissingDataPermission missing = new tbl_MissingDataPermission();
            string newcombindedString = string.Join(",", value.ToArray());
            dynamic data = JObject.Parse(newcombindedString);
            missing.UserID = data.UserID;
            missing.ShowMissingData = data.ShowMissingData;
            var model = db.tbl_MissingDataPermission;
            model.Add(missing);
            db.SaveChanges();
        }
        public ActionResult Update(List<String> newdata, List<String> olddata, tbl_Role role1)
        {
            int userid = Convert.ToInt32(Session["userid"]);
            tbl_MissingDataPermission missing = new tbl_MissingDataPermission();
            string newcombindedString = string.Join(",", newdata.ToArray());
            dynamic data = JObject.Parse(newcombindedString);
            string oldcombindedString = string.Join(",", olddata.ToArray());
            dynamic Olddata = JObject.Parse(oldcombindedString);
            missing.ID = Olddata.ID;
            missing.UserID = data.UserID == null ? Olddata.UserID : data.UserID;
            missing.ShowMissingData= data.ShowMissingData == null ? Olddata.ShowMissingData : data.ShowMissingData;
            db.Entry(missing).State = EntityState.Modified;
            db.SaveChanges();
            return View();
        }
        public void Remove(int ID)
        {
            var model = db.tbl_MissingDataPermission;
            var item1 = model.FirstOrDefault(it => it.ID == ID);
            model.Remove(item1);
            db.SaveChanges();
        }
    }
}