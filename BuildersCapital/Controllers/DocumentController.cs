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

                return Json(documents, JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                //EmailHelper.SendEmailNotice("PriceClaro: Errors found during price file upload", "Errors found during price upload:", ex.Message);
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
    }
}