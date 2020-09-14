using BuildersCapital.DataProvider;
using BuildersCapital.Models;
using BuildersCapitalDataAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BuildersCapital.Controllers
{
    public class DocumentController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Uploads a json file containing with the id of the documents and checks each of their doc type status.
        /// </summary>
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

        [HttpPost]
        public JsonResult Download(string id)
        {
            try
            {
                BuildersCapitalDataProvider BuildersCapitalDataProvider = new BuildersCapitalDataProvider();
                Document foundDocument = BuildersCapitalDataProvider.RetrieveDocuments().ToList().Find(x => x.Id == new Guid(id));
                ZipProvider ZipProvider = new ZipProvider();
                string fileName = $"{id}.zip";
                string folderName = "/Output/";
                string serverPath = Path.Combine(Server.MapPath("~" + folderName), fileName);
                ZipProvider.WriteByteArrayToFile(serverPath, foundDocument.DocBlob);

                var result = new
                {
                    error = 0,
                    data = folderName + fileName,
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

        [HttpPost]
        public async Task<JsonResult> DownloadAsync(string id)
        {
            try
            {
                BuildersCapitalDataProvider BuildersCapitalDataProvider = new BuildersCapitalDataProvider();
                Document foundDocument = BuildersCapitalDataProvider.RetrieveDocuments().ToList().Find(x => x.Id == new Guid(id));
                ZipProvider ZipProvider = new ZipProvider();
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads/" + id + ".zip");
                await ZipProvider.DownloadZipFile(path, foundDocument.DocBlob);

                var result = new
                {
                    error = 0,
                    data = path,
                    message = ""
                };
                return await Task.FromResult(Json(result, JsonRequestBehavior.AllowGet));
            }

            catch (Exception ex)
            {
                var result = new
                {
                    error = 1,
                    data = "",
                    message = ex.Message
                };
                return await Task.FromResult(Json(result, JsonRequestBehavior.AllowGet));
            }
        }
    }
}