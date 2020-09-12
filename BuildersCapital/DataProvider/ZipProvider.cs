using Ionic.Zip;
using System.IO;

namespace BuildersCapital.DataProvider
{
    public class ZipProvider
    {
        public ZipFile ZipFile(string path, string password)
        {
            ZipFile zip = new ZipFile();
            using (zip)
            {
                zip.Password = password;
                zip.AddDirectory(Path.Combine(path, "Blobs"));
                zip.Save(Path.Combine(path, "test.zip"));
            }

            return zip;
        }

        public void WriteByteArrayToFile(string path, byte[] bytes)
        {
            string outputPath = Path.Combine(path, "Output/test.zip");
            File.WriteAllBytes(outputPath, bytes);
        }
    }
}