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

        IEnumerable<Material> GetMaterialsByTheFilters(int category, double minSize, double maxSize);
        int GetCategoryOfMaterial(string fileName);
        
        public Material GetMaterialByName(string fileName);
        IEnumerable<Material> GetListOfMaterials();
#nullable enable
        bool CheckFilesInDB(string? fileName);
        string GetPathOfMaterialByTheVersionAndName(string fileName, int version);
    }
}
