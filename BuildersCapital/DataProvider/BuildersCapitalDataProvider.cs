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

        public Document CreateDocument(Guid docId, Guid propertyId, string docType, ZipFile zipFile)
        {
            try
            {
                Document newDocument = new Document()
                {
                    Id = docId,
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

        public DocStatusView UpdateDocStatusView(DocStatusView docStatusView)
        {
            try
            {
                //DocStatusView newDocStatusView = new DocStatusView()
                //{
                //    PropertyId = docStatusView.PropertyId,
                //    Agreement = docStatusView.Agreement,
                //    Appraisal = docStatusView.Appraisal,
                //    SiteMap = docStatusView.SiteMap,
                //    Resume = docStatusView.Resume,
                //    Paperwork = docStatusView.Paperwork
                //};

                //BuildersCapitalDBEntities.Entry(docStatusView).CurrentValues.SetValues(newDocStatusView);
                BuildersCapitalDBEntities.DocStatusViews.Attach(docStatusView);
                BuildersCapitalDBEntities.SaveChanges();
                return docStatusView;
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
                Guid newDocGuid = Guid.NewGuid();
                if (!docStatusView.Agreement)
                {
                    docStatusView.Agreement = true;
                    documents.Add(CreateDocType(newDocGuid, filePath, docStatusView.PropertyId, DocType.Agreement.ToString()));
                }
                if (!docStatusView.Appraisal)
                {
                    //docStatusView.Appraisal = true;
                    documents.Add(CreateDocType(newDocGuid, filePath, docStatusView.PropertyId, DocType.Appraisal.ToString()));
                }
                if (!docStatusView.SiteMap)
                {
                    docStatusView.SiteMap = true;
                    documents.Add(CreateDocType(newDocGuid, filePath, docStatusView.PropertyId, DocType.SiteMap.ToString()));
                }
                if (!docStatusView.Resume)
                {
                    docStatusView.Resume = true;
                    documents.Add(CreateDocType(newDocGuid, filePath, docStatusView.PropertyId, DocType.Resume.ToString()));
                }
                if (!docStatusView.Paperwork)
                {
                    docStatusView.Paperwork = true;
                    documents.Add(CreateDocType(newDocGuid, filePath, docStatusView.PropertyId, DocType.Paperwork.ToString()));
                }

                //UpdateDocStatusView(docStatusView);
            }

            // Dispose
            if (File.Exists(Path.Combine(filePath, "test.zip")))
            {
                File.Delete(Path.Combine(filePath, "test.zip"));
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

        private Document CreateDocType(Guid docId, string filePath, Guid propertyId, string docType) {
            ZipProvider zipProvider = new ZipProvider();
            ZipFile zip = zipProvider.ZipFile(filePath, String.Concat(docId, GetLast8Characters(docType)));
            return CreateDocument(docId, propertyId, docType, zip);
        }
    }
}