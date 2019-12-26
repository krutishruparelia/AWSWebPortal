using AWS.Models;
using LinqToExcel;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web.Mvc;
namespace AWS.Areas.Admin.Controllers
{
    public class ImportStationController : Controller
    {
        AWSDatabaseContext db = new AWSDatabaseContext();
        clsDatabase dbs = new clsDatabase();
        // GET: Admin/ImportStation
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(ImportExcel importExcel)
        {
            if (ModelState.IsValid)
            {
                
                tbl_StationMaster stm = new tbl_StationMaster();
                string path = Server.MapPath("~/Content/Upload/" + importExcel.file.FileName);
                importExcel.file.SaveAs(path);
                var filename = importExcel.file.FileName;
                string excelConnectionString = @"Provider='Microsoft.ACE.OLEDB.12.0';Data Source='" + path + "';Extended Properties='Excel 12.0 Xml;IMEX=1'";
                var adapter = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", excelConnectionString);
                var ds = new DataSet();
                adapter.Fill(ds, "ExcelTable");

                DataTable dtable = ds.Tables["ExcelTable"];

                string sheetName = "Sheet1";
                var excelFile = new ExcelQueryFactory(path);
                var model = db.tbl_StationMaster;
                var artistAlbums = from a in excelFile.Worksheet<tbl_StationMaster>(sheetName) select a;
                foreach (var a in artistAlbums)
                {
                    string createQuery = "";
                    stm.Name = a.Name;
                    stm.Latitude = a.Latitude;
                    stm.Longitude = a.Longitude;
                    stm.City = a.City;
                    stm.District = a.District;
                    stm.State = a.State;
                    stm.TehsilTaluk = a.TehsilTaluk;
                    stm.Block = a.Block;
                    stm.Village = a.Village;
                    stm.Image = a.Image;
                    stm.Address = a.Address;
                    stm.Bank = a.Bank;
                    stm.BusStand = a.BusStand;
                    stm.RailwayStation = a.RailwayStation;
                    stm.Airport = a.Airport;
                    stm.OtherInformation = a.OtherInformation;
                    stm.StationCategoryID = a.StationCategoryID;
                    stm.Profile = a.Profile;
                    stm.Gain = a.Gain;
                    stm.Offset = a.Offset;
                    stm.CreatedDate = a.CreatedDate;
                    stm.CreatedBy = a.CreatedBy;
                    stm.UpdatedDate = a.UpdatedDate;
                    stm.UpdatedBy = a.UpdatedBy;
                    stm.IsDeleted = a.IsDeleted;
                    stm.StationID = a.StationID;
                    stm.SerialNumber = a.SerialNumber;
                    stm.ShowInGraph = a.ShowInGraph;
                    stm.ShowInGrid = a.ShowInGrid;
                    model.Add(stm);
                    db.SaveChanges();
                    var StationID = a.StationID;
                    var sensorNamesql = db.tbl_ProfileMaster.Where(x => x.Name == a.Profile).FirstOrDefault();
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
                    var finalParameterquery = createQuery.TrimEnd(',');
                    var querySql = "Create table tbl_StationData_" + StationID + "(ID int not null identity(1,1)," + finalParameterquery + ")";
                    dbs.CreateTable(querySql, "WEB");
                }
                //OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);

                ////Sheet Name
                //excelConnection.Open();
                //string tableName = excelConnection.GetSchema("Tables").Rows[0]["TABLE_NAME"].ToString();
                //excelConnection.Close();
                ////End

                //OleDbCommand cmd = new OleDbCommand("Select * from [" + tableName + "]", excelConnection);

                //excelConnection.Open();

                //OleDbDataReader dReader;
                //dReader = cmd.ExecuteReader();
                //SqlBulkCopy sqlBulk = new SqlBulkCopy(ConfigurationManager.ConnectionStrings["AWSDatabaseContext"].ConnectionString);

                ////Give your Destination table name
                //sqlBulk.DestinationTableName = "tbl_StationMaster";

                ////Mappings
                //sqlBulk.ColumnMappings.Add("Name", "Name");
                //sqlBulk.ColumnMappings.Add("Latitude", "Latitude");
                //sqlBulk.ColumnMappings.Add("Longitude", "Longitude");
                //sqlBulk.ColumnMappings.Add("City", "City");
                //sqlBulk.ColumnMappings.Add("District", "District");
                //sqlBulk.ColumnMappings.Add("TehsilTaluk", "TehsilTaluk");
                //sqlBulk.ColumnMappings.Add("Block", "Block");
                //sqlBulk.ColumnMappings.Add("Village", "Village");
                //sqlBulk.ColumnMappings.Add("Image", "Image");
                //sqlBulk.ColumnMappings.Add("Address", "Address");
                //sqlBulk.ColumnMappings.Add("Bank", "Bank");
                //sqlBulk.ColumnMappings.Add("BusStand", "BusStand");
                //sqlBulk.ColumnMappings.Add("RailwayStation", "RailwayStation");
                //sqlBulk.ColumnMappings.Add("Airport", "Airport");
                //sqlBulk.ColumnMappings.Add("OtherInformation", "OtherInformation");
                //sqlBulk.ColumnMappings.Add("StationCategoryID", "StationCategoryID");
                //sqlBulk.ColumnMappings.Add("Profile", "Profile");
                //sqlBulk.ColumnMappings.Add("Gain", "Gain");
                //sqlBulk.ColumnMappings.Add("Offset", "Offset");
                //sqlBulk.ColumnMappings.Add("CreatedDate", "CreatedDate");
                //sqlBulk.ColumnMappings.Add("CreatedBy", "CreatedBy");
                //sqlBulk.ColumnMappings.Add("UpdatedDate", "UpdatedDate");
                //sqlBulk.ColumnMappings.Add("UpdatedBy", "UpdatedBy");
                //sqlBulk.ColumnMappings.Add("IsDeleted", "IsDeleted");
                //sqlBulk.ColumnMappings.Add("StationID", "StationID");
                //sqlBulk.ColumnMappings.Add("SerialNumber", "SerialNumber");
                //sqlBulk.ColumnMappings.Add("ShowInGraph", "ShowInGraph");
                //sqlBulk.ColumnMappings.Add("ShowInGrid", "ShowInGrid");



                //sqlBulk.WriteToServer(dReader);
                //excelConnection.Close();

                ViewBag.Result = "Successfully Imported";
            }
            return View();
        }
    }
}