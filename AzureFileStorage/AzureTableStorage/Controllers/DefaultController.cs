using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AzureTableStorage.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult WorkingWithFileStorage()
        {
            Models.MyFileStorage ObjectMyFileStorage = new Models.MyFileStorage();

            ObjectMyFileStorage.Access_the_file_share_programmatically();
            ObjectMyFileStorage.Set_the_maximum_size_for_a_file_share();
            ObjectMyFileStorage.Generate_a_shared_access_signature_for_a_file_or_file_share();
            ObjectMyFileStorage.Copy_file_to_another_file();
            ObjectMyFileStorage.Copy_a_file_to_a_blob();
            ObjectMyFileStorage.Troubleshooting_File_storage_using_metrics();

            return View("DefaultView");
        }
    }
}