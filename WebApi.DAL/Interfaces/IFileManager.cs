using System.Threading.Tasks;

namespace WebApi.DAL.Interfaces
{
    public interface IFileManager
    {
        string GetPath(string hash);
        Task<string> SaveFile(byte[] fileBytes, string fileName);
    }
}
