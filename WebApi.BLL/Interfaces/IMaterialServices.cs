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
        IQueryable<Material> GetMaterialsByTheFilters(MaterialCategories category, double minSize, double maxSize);
        MaterialCategories GetCategoryOfMaterial(string fileName);
        IEnumerable<Material> GetMaterials(string? fileName);
        int GetCountOfMaterials(string? fileName);
        bool ValidateOfCategory(MaterialCategories category);
        int GetActualVersion(string fileName);
        string GetPathOfMaterialByTheVersionAndName(string fileName, int version);
        Task AddNewMaterialToDB(Material material, MaterialFileBM fileMaterialBM);
        Task AddNewMaterialVersionToDb(MaterialFileBM fileMaterialBM, int version);
        IEnumerable<Material> GetInfoByTheFiltersFromDb(MaterialCategories category, double minSize, double maxSize);
        FileStream DownloadMaterialByName(string fileName);
        FileStream DownloadMaterialByNameAndVersion(string fileName, int version);
        void ChangeCategoryOfFile(string fileName, MaterialCategories category);
    }
}
