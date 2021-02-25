using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WebApi.BLL.BM;
using WebApi.BLL.Categories;

namespace WebApi.BLL.Interfaces
{
    public interface IMaterialServices
    {
        bool FileExisting(string fileName);
        bool CategoryExisting(MaterialCategories category);
        int GetActualVersion(string fileName);
        MaterialBM GetMaterialBMbyName(string fileName);
        IEnumerable<MaterialBM> GetMaterialsBM();


        Task CreateMaterial(MaterialBM materialBM, MaterialFileBM fileMaterialBM);
        Task CreateMaterialVersion(MaterialFileBM fileMaterialBM, int version);
        IEnumerable<MaterialBM> GetInfoByTheFilters(MaterialCategories category, double minSize, double maxSize);
        FileStream DownloadMaterialByName(string fileName);
        FileStream DownloadMaterialByNameAndVersion(string fileName, int version);
        void ChangeCategoryOfFile(string fileName, MaterialCategories category);
    }
}
