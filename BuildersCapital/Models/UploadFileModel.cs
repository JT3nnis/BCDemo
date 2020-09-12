namespace BuildersCapital.Models
{
    public class UploadFileModel
    {
        public UploadFileModel()
        {
            UploadFile = string.Empty;
        }

        public string FileType { get; set; } = "Json";

        public string UploadFile { get; set; }

    }
}