using AWS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AWS.Areas.Admin.Controllers
{
    public class StationGridDisplayController : Controller
    {
        AWSDatabaseContext db = new AWSDatabaseContext();
        // GET: Admin/StationGridDisplay
        public ActionResult Index()
        {
            var stationsdata = db.tbl_StationMaster.ToList();
            ViewBag.stationdata = stationsdata;
            return View();
        }
        public void Remove(int ID)
        {
            var model = db.tbl_StationMaster;
            var item1 = model.FirstOrDefault(it => it.ID == ID);
            model.Remove(item1);
            db.SaveChanges();
        }
    }
}