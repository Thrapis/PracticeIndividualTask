using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MaterailReport.Models;
using MaterailReport.Models.ExcelEditing;
using MaterailReport.Models.ImageTools;
using System.IO;

namespace MaterailReport.Controllers
{
    public class HomeController : Controller
    {
        SqlConnection connection = MRConnection.GetConnection();

        public ActionResult Index()
        {
            AuthenticationProvider authProvider = new AuthenticationProvider(connection);

            if (User.Identity.IsAuthenticated)
            {
                string saved_name = User.Identity.Name;
                UserInfo ui = authProvider.GetUserInfo(saved_name);
                ViewBag.Login = ui.Login;
                ViewBag.Name = ui.Full_Name;
                ViewBag.Level = ui.Access_Level;
                ViewBag.UserPhoto = ui.GetPhotoForPage();
            }
            else
            {
                ViewBag.Name = "NonAuthorized";
            }

            ViewBag.Logo = Image.FromFile(Server.MapPath("~/Content/Images/app_ico_blue.png")).GetImageForPage();

            if (User.Identity.IsAuthenticated)
            {
                WarehouseProvider warehoseProvider = new WarehouseProvider(connection);

                ViewBag.Parts = 0;
                ViewBag.BParts = 0;
                ViewBag.DParts = 0;
                ViewBag.Parts = warehoseProvider.GetContentPartsCount() + 1;
                ViewBag.BParts = warehoseProvider.GetBrokenContentPartsCount() + 1;
                ViewBag.DParts = warehoseProvider.GetDecommissionedContentPartsCount() + 1;
            }
            

            return View();
        }
        
        [HttpPost]
        public ActionResult FromSimpleToBroken(string[] nums, string restorationCoast, string deprecation)
        {
            if (User.Identity.IsAuthenticated)
            {
                WarehouseProvider warehoseProvider = new WarehouseProvider(connection);

                double restor = Double.Parse(restorationCoast.Replace(".", ","));
                double depr = Double.Parse(deprecation.Replace(".", ","));

                for (int i = 0; i < nums.Length; i++)
                {
                    warehoseProvider.FromSimpleToBroken(int.Parse(nums[i]), restor, depr);
                }
            }

            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        public ActionResult FromBrokenToSimple(string[] nums)
        {
            if (User.Identity.IsAuthenticated)
            {
                WarehouseProvider warehoseProvider = new WarehouseProvider(connection);

                for (int i = 0; i < nums.Length; i++)
                {
                    warehoseProvider.FromBrokenToSimple(int.Parse(nums[i]));
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult FromBrokenToDecommissioned(string[] nums, string decommissionDate)
        {
            if (User.Identity.IsAuthenticated)
            {
                WarehouseProvider warehoseProvider = new WarehouseProvider(connection);

                DateTime dateTime = DateTime.Parse(decommissionDate);

                for (int i = 0; i < nums.Length; i++)
                {
                    warehoseProvider.FromBrokenToDecommissioned(int.Parse(nums[i]), dateTime);
                }
            }

            return RedirectToAction("Index", "Home");
        }
        
        [HttpPost]
        public ActionResult AddContentToWarehouse(string objectName, string commissioningDate)
        {
            if (User.Identity.IsAuthenticated)
            {
                MaterialsAndComponentsProvider macProvider = new MaterialsAndComponentsProvider(connection);
                WarehouseProvider warehoseProvider = new WarehouseProvider(connection);

                DateTime dateTime = DateTime.Parse(commissioningDate);

                if (macProvider.GetMaterialObjectByName(objectName) != null)
                {
                    warehoseProvider.AddContentToWarehouse(objectName, dateTime);
                }
                
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public bool HasMaterial(string objectName)
        {
            if (User.Identity.IsAuthenticated)
            {
                MaterialsAndComponentsProvider macProvider = new MaterialsAndComponentsProvider(connection);

                return macProvider.GetMaterialObjectByName(objectName) != null;
            }

            return false;
        }

        [HttpGet]
        public FileResult MakeReport(string[] nums, string post, string fullName, string fileName)
        {
            if (User.Identity.IsAuthenticated)
            {
                string fileNamePlus = fileName + " (" + DateTime.Now.ToString("dd.MM.yyyy HH-mm-ss") + ")";

                List<int> idsList = new List<int>();

                foreach (string num in nums)
                {
                    idsList.Add(int.Parse(num));
                }

                idsList.Sort();

                string file_path = ReportExcelFile.CreateFile(post, fullName, fileNamePlus, idsList);

                //string file_path = System.IO.Path.GetFullPath(fileNamePlus + ".xlsx"); ;
                string file_type = "application/xlsx";
                string file_name = fileNamePlus + ".xlsx";
                return File(file_path, file_type, file_name);
            }

            return null;
        }

        public ActionResult ContentOfWarehousePart(int part)
        {
            if (User.Identity.IsAuthenticated)
            {
                WarehouseProvider warehoseProvider = new WarehouseProvider(connection);

                var cow = warehoseProvider.GetContentOfWarehouseByIdRange(part - 1).ToList();

                return PartialView(cow);
            }

            return null;
        }

        public ActionResult BrokenContentOfWarehousePart(int part)
        {
            if (User.Identity.IsAuthenticated)
            {
                WarehouseProvider warehoseProvider = new WarehouseProvider(connection);

                var cow = warehoseProvider.GetBrokenContentOfWarehouseByIdRange(part - 1).ToList();

                return PartialView(cow);
            }

            return null;
        }

        public ActionResult DecommissionedContentOfWarehousePart(int part)
        {
            if (User.Identity.IsAuthenticated)
            {
                WarehouseProvider warehoseProvider = new WarehouseProvider(connection);

                var cow = warehoseProvider.GetDecommissionedContentOfWarehouseByIdRange(part - 1).ToList();

                return PartialView(cow);
            }

            return null;
        }
    }
}