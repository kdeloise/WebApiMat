using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;
using WebApi.DAL.Interfaces;

namespace WebApi.DAL.Services
{
    public class FileManager : IFileManager
    {
        private string filesDir { get; set; }

        public FileManager(IConfiguration configuration)
        {
            filesDir = configuration["FilesDir"];
            Directory.CreateDirectory(filesDir);
        }

        public async Task<string> SaveFile(byte[] fileBytes, string fileName)
        {
            var path = filesDir + fileName;
            await File.WriteAllBytesAsync(path, fileBytes);
            return path;
        }
    }
}
