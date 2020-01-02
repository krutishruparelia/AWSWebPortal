using AWS.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AWS.Areas.Admin.Controllers
{
    public class PermissionMasterController : Controller
    {
        private AWSDatabaseContext db = new AWSDatabaseContext();
        // GET: Admin/PermissionMaster
        public ActionResult Index()
        {
            try
            {

                var StationAllData = db.tbl_StationMaster.ToList();
                ViewBag.stationalldata = StationAllData;
                var alluser = db.tbl_User.Select(x => x.EmailID).ToList();
                ViewBag.alluser = alluser;
                return View();
            }

            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return View();
            }
        }
     
        public JsonResult Insert(string ID,string user)
        {
            try
            {
                //int uid = Convert.ToInt32(Session["userid"]);
                tbl_Permission permission = new tbl_Permission();
                var getUserID = db.tbl_User.Where(x => x.EmailID == user).FirstOrDefault();
                int userID = getUserID.ID;
                permission.UserID = userID;
                IEnumerable<tbl_Permission> list = db.tbl_Permission.Where(x => x.UserID == userID).ToList();
                db.tbl_Permission.RemoveRange(list);
                db.SaveChanges();
                string[] splitID = ID.Split(',');
                foreach (var item in splitID)
                {
                    int stID = Convert.ToInt32(item);
                    var stationsql = db.tbl_StationMaster.Where(x => x.ID == stID).FirstOrDefault();

                    permission.StationID = stID.ToString();
                    permission.District = stationsql.District;
                    var profilesql = db.tbl_ProfileMaster.Where(x => x.Name == stationsql.Profile).FirstOrDefault();
                    permission.ProfileID = profilesql.ID.ToString();
                    permission.CreatedDate = DateTime.Now;
                    permission.CreatedBy = 1;
                    var model = db.tbl_Permission;
                    model.Add(permission);
                    db.SaveChanges();
                }
                return Json(permission, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult PermissionMaster(string value)
        {
            string userids = "";
         
            var getuserid = db.tbl_User.Where(x => x.EmailID == value).FirstOrDefault();
            var getassingeduser = db.tbl_Permission.Where(x => x.UserID == getuserid.ID).ToList();
            foreach(var ids in getassingeduser)
            {
                userids += "\""+ids.StationID+"\""+",";
                //
            }
           
            ViewData["selectedRows"] = new string[] { userids.TrimEnd(',') };
            
            //Response.Conte("<script>$('#hidden11').val(" + userids + ")</script>");
            return Json(userids.TrimEnd(','), JsonRequestBehavior.AllowGet);
        }
    }
}