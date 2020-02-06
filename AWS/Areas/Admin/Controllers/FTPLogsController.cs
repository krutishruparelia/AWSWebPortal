using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AWS.Models;
using Newtonsoft.Json;

namespace AWS.Areas.Admin.Controllers
{
    public class FTPLogsController : Controller
    {
        // GET: Admin/FTPLogs
        AWSDatabaseContext db = new AWSDatabaseContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AuditLogs()
        {
            List<tbl_AuditLog> lstAudit = new List<tbl_AuditLog>();
            var auditLogs = db.tbl_AuditLog.ToList().OrderByDescending(x=>x.ID);
            foreach (var item in auditLogs)
            {
                lstAudit.Add(new tbl_AuditLog
                {
                    ID = item.ID,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    Message=item.Message
                });
            }
            var LastRowID = db.tbl_AuditLog.OrderByDescending(jx => jx.ID).FirstOrDefault();
            ViewBag.LastRowID = LastRowID.ID;
            ViewBag.AuditLogs = JsonConvert.SerializeObject(lstAudit);
            return View();
        }
        public JsonResult realtimegrid()
        {
            List<tbl_AuditLog> lstAudit = new List<tbl_AuditLog>();
            var auditLogs = db.tbl_AuditLog.ToList().OrderByDescending(x => x.ID);
            foreach (var item in auditLogs)
            {
                lstAudit.Add(new tbl_AuditLog
                {
                    ID = item.ID,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    Message = item.Message
                });
            }
            return Json(JsonConvert.SerializeObject(lstAudit), JsonRequestBehavior.AllowGet);
        }
    }
}