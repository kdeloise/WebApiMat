using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApi.BLL.BM;
using WebApi.DAL.Entities;

namespace WebApi.BLL.Interfaces
{
    public interface IMaterialServices
    {        
        Task AddNewMaterialToDB(Material material, MaterialFileBM fileMaterialBM);
        Task AddNewMaterialVersionToDb(MaterialFileBM fileMaterialBM, int version);
        IEnumerable<Material> GetInfoByTheFiltersFromDb(MaterialCategories category, double minSize, double maxSize);
        FileStream DownloadMaterialByName(string fileName);
        FileStream DownloadMaterialByNameAndVersion(string fileName, int version);
        void ChangeCategoryOfFile(string fileName, MaterialCategories category);
    }
}
