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

        public IList<Document> VerifyDocuments(IList<Guid> documentIds, string filePath)
        {
            IList<Document> documents = new List<Document>();
            IEnumerable<DocStatusView> foundViews = RetrieveDocStatusViews().Where(x => documentIds.Any(y => y == x.PropertyId));
            foreach (DocStatusView docStatusView in foundViews)
            {
                if (!docStatusView.Agreement)
                {
                    documents.Add(CreateDocType(filePath, docStatusView.PropertyId, DocType.Agreement.ToString()));
                }
                if (!docStatusView.Appraisal)
                {
                    documents.Add(CreateDocType(filePath, docStatusView.PropertyId, DocType.Appraisal.ToString()));
                }
                if (!docStatusView.SiteMap)
                {
                    documents.Add(CreateDocType(filePath, docStatusView.PropertyId, DocType.SiteMap.ToString()));
                }
                if (!docStatusView.Resume)
                {
                    documents.Add(CreateDocType(filePath, docStatusView.PropertyId, DocType.Resume.ToString()));
                }
                if (!docStatusView.Paperwork)
                {
                    documents.Add(CreateDocType(filePath, docStatusView.PropertyId, DocType.Paperwork.ToString()));
                }
            }

            return documents;
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

        private Document CreateDocType(string filePath, Guid propertyId, string docType) {
            ZipProvider zipProvider = new ZipProvider();
            ZipFile zip = zipProvider.ZipFile(filePath, String.Concat(propertyId.ToString(), GetLast8Characters(docType)));
            return CreateDocument(propertyId, docType, zip);
        }
    }
}