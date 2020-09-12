using Ionic.Zip;
using System.IO;
using System.Threading.Tasks;

namespace BuildersCapital.DataProvider
{
    public class ZipProvider
    {
        public ZipFile ZipFile(string inputFilePath, string outputFilePath, string password)
        {
            ZipFile zip = new ZipFile();
            using (zip)
            {
                zip.Password = password;
                zip.AddDirectory(inputFilePath);
                zip.Save(outputFilePath);
            }

            return zip;
        }

        public void WriteByteArrayToFile(string path, byte[] bytes)
        {
            File.WriteAllBytes(path, bytes);
        }

        public async Task DownloadZipFile(string path, byte[] bytes)
        {
            using(FileStream stream = new FileStream(path, FileMode.OpenOrCreate))
            using(StreamWriter writer = new StreamWriter(stream))
            {
                await writer.WriteAsync(System.Text.Encoding.ASCII.GetChars(bytes));
            }
        }
    }
}