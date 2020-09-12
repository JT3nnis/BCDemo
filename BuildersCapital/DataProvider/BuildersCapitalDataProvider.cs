using BuildersCapitalDataAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BuildersCapital.Models;
using Newtonsoft.Json;
using Ionic.Zip;

namespace BuildersCapital.DataProvider
{
    public class BuildersCapitalDataProvider
    {
        private BuildersCapitalDBEntities BuildersCapitalDBEntities;

        public BuildersCapitalDataProvider()
        {
            BuildersCapitalDBEntities = new BuildersCapitalDBEntities();
        }

        /// <summary>
        /// Finds all documents in a document database.
        /// </summary>
        public IEnumerable<Document> RetrieveDocuments()
        {
            return BuildersCapitalDBEntities.Documents.ToList();
        }

        public IEnumerable<DocStatusView> RetrieveDocStatusViews()
        {
            return BuildersCapitalDBEntities.DocStatusViews.ToList();
        }

        public Document CreateDocument(Guid propertyId, string docType, ZipFile zipFile)
        {
            try
            {
                Document newDocument = new Document()
                {
                    Id = Guid.NewGuid(),
                    PropertyId = propertyId,
                    DocType = docType,
                    FileName = "test.docx",
                    DocBlob = File.ReadAllBytes(zipFile.Name)
                };

                BuildersCapitalDBEntities.Documents.Add(newDocument);
                BuildersCapitalDBEntities.SaveChanges();
                return newDocument;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void VerifyDocuments(IList<Guid> documentIds, string filePath)
        {
            ZipProvider zipProvider = new ZipProvider();
            IEnumerable<DocStatusView> foundViews = RetrieveDocStatusViews().Where(x => documentIds.Any(y => y == x.PropertyId));
            foreach (DocStatusView docStatusView in foundViews)
            {
                if (!docStatusView.Agreement)
                {

                }
                if (!docStatusView.Appraisal)
                {
                    // Create Agreement Document
                    ZipFile zip = zipProvider.ZipFile(filePath, String.Concat(docStatusView.PropertyId.ToString(), GetLast8Characters("Appraisal")));
                    CreateDocument(docStatusView.PropertyId, "Appraisal", zip);

                }
                if (!docStatusView.SiteMap)
                {
                    // Create Agreement Document
                }
                if (!docStatusView.Resume)
                {
                    // Create Agreement Document
                }
                if (!docStatusView.Paperwork)
                {
                    // Create Agreement Document
                }
            }
        }

        public IList<Guid> UploadDataModel(Stream jsonData)
        {
            IList<Guid> uploadData = new List<Guid>();
            using (StreamReader r = new StreamReader(jsonData))
            {
                string json = r.ReadToEnd();
                InputDataModel inputDataModel = JsonConvert.DeserializeObject<InputDataModel>(json);
                uploadData = inputDataModel.data;
            }

            return uploadData;
        }

        private string GetLast8Characters(string name)
        {
            int start = (Math.Max(0, name.Length - 8));
            return name.Substring(start, name.Length - start);
        }
    }
}