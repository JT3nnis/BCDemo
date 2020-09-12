using BuildersCapitalDataAccess;
using BuildersCapital.DataProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using BuildersCapital.Models;
using System.IO;

namespace BuildersCapital.Controllers
{
    public class DocumentController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Upload(UploadFileModel form, HttpPostedFileBase fileToUpload)
        {
            try
            {
                BuildersCapitalDataProvider BuildersCapitalDataProvider = new BuildersCapitalDataProvider();
                IList<Guid> uploadedData = BuildersCapitalDataProvider.UploadDataModel(fileToUpload.InputStream);

                string path = Server.MapPath("~/App_Data");
                IList<Document> documents = BuildersCapitalDataProvider.VerifyDocuments(uploadedData, path);
                var result = new
                {
                    error = 0,
                    data = documents,
                    message = ""
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                var result = new
                {
                    error = 1,
                    data = "",
                    message = ex.Message
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
    }
}