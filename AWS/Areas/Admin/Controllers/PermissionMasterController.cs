using AWS.Models;
using System;
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
            var StationAllData = db.tbl_StationMaster.ToList();
            ViewBag.stationalldata = StationAllData;
            var alluser = db.tbl_User.Select(x=>x.EmailID).ToList();
            ViewBag.alluser = alluser;
            return View();
        }
     
        public JsonResult Insert(string ID,string user)
        {
            //int uid = Convert.ToInt32(Session["userid"]);
            tbl_Permission permission = new tbl_Permission();
            var getUserID = db.tbl_User.Where(x => x.Username == user).FirstOrDefault();
            int userID = getUserID.ID;
            permission.UserID = userID;
            string[] splitID = ID.Split(',');
            foreach(var item in splitID)
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
    }
}