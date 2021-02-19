using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebApi.DAL.Entities;

namespace WebApi.DAL.Interfaces
{
    public interface IDBServices
    {
        Task SaveMaterial(Material material, MaterialVersion materialVersion);
        Task SaveMaterialVersion(MaterialVersion materialVersion);
        void UpdateMaterial(Material material);

        IEnumerable<Material> GetMaterialsByTheFilters(MaterialCategories category, double minSize, double maxSize);
        MaterialCategories GetCategoryOfMaterial(string fileName);
        int GetActualVersion(string fileName);
        public Material GetMaterialByName(string fileName);
        IEnumerable<Material> GetListOfMaterials();
#nullable enable
        bool CheckFilesInDB(string? fileName);
        bool ValidateOfCategory(MaterialCategories category);
        string GetPathOfMaterialByTheVersionAndName(string fileName, int version);
    }
}
