using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AWS.Models;
using System.Data;

namespace AWS.Areas.Admin.Controllers
{
    public class PermissionController : Controller
    {
        string strFunctionName = "";
        clsDatabase objDB = new clsDatabase();
        AWSDatabaseContext db = new AWSDatabaseContext();
        // GET: Admin/Permission
        public ActionResult Index()
        {
            List<tbl_StationMaster> stationDetailist = new List<tbl_StationMaster>();
            strFunctionName = "DashBoard";
            try
            {

                ////////////Fetch Station details//////////
                DataSet ds1 = objDB.FetchDataset("select Name,ID from tbl_StationMaster", "Web");

                ViewBag.countst = ds1.Tables[0].Rows.Count.ToString();

                List<SelectListItem> ObjstList = new List<SelectListItem>();

                foreach (DataRow dr in ds1.Tables[0].Rows)
                {
                    {
                        ObjstList.Add(new SelectListItem
                        {
                            Text = dr["Name"].ToString(),
                            Value = dr["ID"].ToString()
                        });

                    };
                }
                ViewBag.allst = ObjstList;


                ////////////Fetch UserName//////////
                DataSet ds = objDB.FetchDataset("select ID,FirstName from tbl_User", "Web");


                List<SelectListItem> ObjList = new List<SelectListItem>();
                ObjList.Add(new SelectListItem { Text = "Select", Value = "0" });
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    {
                        ObjList.Add(new SelectListItem
                        {
                            Text = dr["FirstName"].ToString(),
                            Value = dr["ID"].ToString()
                        });

                    };
                }
                ViewBag.user = ObjList;

                return View();
            }
            catch (Exception Ex)
            {
                //strExceptionMessage = Ex.Message;

                //string tmpExData = "";
                //tmpExData = strModuleName + "#" + strFunctionName + "#" + strExceptionMessage + "#" + clsGlobalData.strExceptionFileName + "#" + clsGlobalData.strLoc_ExceptionFile;

                //bool ExcpMesg = clsExp.WriteIntoExceptionFile(tmpExData);

                return View(stationDetailist);
            }
        }
        [HttpPost]
        public JsonResult insertuserpermission(string[] permissiondata, string id)
        {

            ////////////Insert Permited Station user wise//////////
            if (id != "")
            {

                DataSet ds1 = objDB.FetchDataset("select * from tbl_Permission where UserID='" + id + "' ", "Web");

                for (int s = 0; s < ds1.Tables[0].Rows.Count; s++)
                {
                    if (ds1.Tables[0].Rows[s]["StationID"].ToString() == permissiondata[0].ToString())
                    {

                    }
                    else
                    {
                        objDB.DeleteFromDb("delete from tbl_Permission where UserID='" + id + "' and StationID='" + ds1.Tables[0].Rows[s]["StationID"].ToString() + "'", "Web");
                    }
                }
                foreach (string num in permissiondata)
                {

                    DataSet ds = objDB.FetchDataset("select * from tbl_Permission where UserID='" + id + "' and StationID='" + num + "'", "Web");
                    if (ds.Tables[0].Rows.Count > 0)
                    {

                    }
                    else
                    {
                        var stid = Convert.ToInt32(num);
                        var getstationsql = db.tbl_StationMaster.Where(x => x.ID == stid).FirstOrDefault();
                        var profileID = db.tbl_ProfileMaster.Where(j => j.Name == getstationsql.Profile).FirstOrDefault();
                        string InsQry = "'" + id + "','" + num + "','"+profileID.ID+"','"+ getstationsql.District+ "',null,null,null,null,null";
                        objDB.InsertIntoDb(InsQry, "tbl_Permission", "Web");
                    }
                }

                return Json("Successfully Station Permited");

            }
            else
            {
                return Json("Enter Valid Data");
            }
        }

        [HttpPost]
       public JsonResult UserwiseStation(string UId)
              {

            ////////////Fetch User Wise Station//////////
            List<SelectListItem> items = new List<SelectListItem>();
            DataSet ds = objDB.FetchDataset(@"SELECT  tbl_Permission.StationID  as id,tbl_StationMaster.Name as name
FROM            tbl_User INNER JOIN
                       tbl_Permission ON tbl_User.ID = tbl_Permission.UserID INNER JOIN
                        tbl_StationMaster ON tbl_Permission.StationID = tbl_StationMaster.ID where tbl_Permission.UserID='" + UId + "'", "Web");
           
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                {
                    items.Add(new SelectListItem
                    {
                        Text = dr["Name"].ToString(),
                        Value = dr["ID"].ToString()
                    });
                };
            }
            return Json(items, JsonRequestBehavior.AllowGet);
        }
    }
}