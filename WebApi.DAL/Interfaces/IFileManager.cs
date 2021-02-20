using System.Threading.Tasks;

namespace WebApi.DAL.Interfaces
{
    public interface IFileManager
    {
        Task<string> SaveFile(byte[] fileBytes, string fileName);
    }
}
