﻿using BuildersCapital.Models;
using BuildersCapitalDataAccess;
using Ionic.Zip;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        /// Finds all documents in a Builders Capital database.
        /// </summary>
        public IEnumerable<Document> RetrieveDocuments()
        {
            return BuildersCapitalDBEntities.Documents.ToList();
        }

        /// <summary>
        /// Finds all status views of documents in a Builders Capital database.
        /// </summary>
        public IEnumerable<DocStatusView> RetrieveDocStatusViews()
        {
            return BuildersCapitalDBEntities.DocStatusViews.ToList();
        }

        /// <summary>
        /// Creates a document in the Builders Capital database.
        /// </summary>
        /// <param name="docId">Guid of the document created.</param>
        /// <param name="propertyId">Guid of the property ID for the document.</param>
        /// <param name="docType">Type of document being created.</param>
        /// <param name="zipFile">Zipfile for the document blob.</param>
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

        /// <summary>
        /// Verifies the status of the property and creates documents for missing document types.
        /// </summary>
        /// <param name="documentIds">Document ids of the properties being verified.</param>
        /// <param name="filePath">File path for the server's App Data.</param>
        public IList<Document> VerifyDocuments(IList<Guid> documentIds, string filePath)
        {
            IList<Document> documents = new List<Document>();
            IEnumerable<DocStatusView> foundViews = RetrieveDocStatusViews().Where(x => documentIds.Any(y => y == x.PropertyId));
            string inputFilePath = Path.Combine(filePath, "Blobs");
            string outputFilePath = Path.Combine(filePath, "temp.zip");
            foreach (DocStatusView docStatusView in foundViews)
            {
                if (!docStatusView.Agreement)
                {
                    docStatusView.Agreement = true;
                    documents.Add(CreateDocType(inputFilePath, outputFilePath, docStatusView.PropertyId, DocType.Agreement.ToString()));
                }
                if (!docStatusView.Appraisal)
                {
                    docStatusView.Appraisal = true;
                    documents.Add(CreateDocType(inputFilePath, outputFilePath, docStatusView.PropertyId, DocType.Appraisal.ToString()));
                }
                if (!docStatusView.SiteMap)
                {
                    docStatusView.SiteMap = true;
                    documents.Add(CreateDocType(inputFilePath, outputFilePath, docStatusView.PropertyId, DocType.SiteMap.ToString()));
                }
                if (!docStatusView.Resume)
                {
                    docStatusView.Resume = true;
                    documents.Add(CreateDocType(inputFilePath, outputFilePath, docStatusView.PropertyId, DocType.Resume.ToString()));
                }
                if (!docStatusView.Paperwork)
                {
                    docStatusView.Paperwork = true;
                    documents.Add(CreateDocType(inputFilePath, outputFilePath, docStatusView.PropertyId, DocType.Paperwork.ToString()));
                }
            }

            // Dispose
            if (File.Exists(outputFilePath))
            {
                File.Delete(outputFilePath);
            }

            return documents;
        }

        /// <summary>
        /// Uploads the inputed json and converts its data into a list of guids.
        /// </summary>
        /// <param name="jsonData">Stream of the uploaded file.</param>
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

        /// <summary>
        /// Gets the last 8 characters of a string, if under 8 characters retrieves the entire string.
        /// </summary>
        /// <param name="name">String being parsed.</param>
        private string GetLast8Characters(string name)
        {
            int start = (Math.Max(0, name.Length - 8));
            return name.Substring(start, name.Length - start);
        }

        /// <summary>
        /// Creates a document for the corresponding document type.
        /// </summary>
        /// <param name="inputFilePath">Path of the folder being zipped.</param>
        /// <param name="outputFilePath">Path to the zip file being outputted.</param>
        /// <param name="propertyId">ID of the property.</param>
        /// <param name="docType">The document type.</param>
        private Document CreateDocType(string inputFilePath, string outputFilePath, Guid propertyId, string docType) {
            Guid newDocGuid = Guid.NewGuid();
            ZipProvider zipProvider = new ZipProvider();
            ZipFile zip = zipProvider.ZipFile(inputFilePath, outputFilePath, String.Concat(newDocGuid, GetLast8Characters(docType)));
            return CreateDocument(newDocGuid, propertyId, docType, zip);
        }
    }
}