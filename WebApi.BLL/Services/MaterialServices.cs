using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.BLL.BM;
using WebApi.BLL.Interfaces;
using WebApi.BLL.Services;
using WebApi.DAL.EF;
using WebApi.DAL.Entities;
using WebApi.DAL.Interfaces;

namespace WebApi.BLL.Services
{
    public class MaterialServices : IMaterialServices
    {
        private readonly IDBServices _dbServices;
        private readonly IFileManager _fileManager;

        public MaterialServices(IDBServices dbServices, IFileManager fileManager)
        {
            _dbServices = dbServices;
            _fileManager = fileManager;
        }        

        public async Task AddNewMaterialToDB(Material material, MaterialFileBM fileMaterialBM)
        {
            var path = await _fileManager.SaveFile(fileMaterialBM.FileBytes, fileMaterialBM.FileName);

            var materialVersion = new MaterialVersion
            {
                Material = material,
                MetaDateTime = DateTime.Now,
                MetaFileSize = fileMaterialBM.FileSize,
                VersionNumber = 1, 
                Path = path
            };

            material.ActualVersion = 1;
            material.Versions.Add(materialVersion);

            await _dbServices.SaveMaterial(material, materialVersion);
        }

        public async Task AddNewMaterialVersionToDb(MaterialFileBM fileMaterialBM, int version)
        {
            var path = await _fileManager.SaveFile(fileMaterialBM.FileBytes, fileMaterialBM.FileName);

            Material newFile = _dbServices.GetMaterialByName(fileMaterialBM.FileName);

            var materialVersion = new MaterialVersion
            {
                Material = newFile,
                MetaDateTime = DateTime.Now,
                MetaFileSize = fileMaterialBM.FileSize,
                VersionNumber = version,
                Path = path
            };

            newFile.ActualVersion = version;
            newFile.Versions.Add(materialVersion);

            await _dbServices.SaveMaterialVersion(materialVersion);
        }

        public IEnumerable<Material> GetInfoByTheFiltersFromDb(MaterialCategories category, double minSize, double maxSize)
        {
            var filtersMat = _dbServices.GetMaterialsByTheFilters(category, minSize, maxSize);
            return filtersMat;
        }

        public FileStream DownloadMaterialByName(string fileName)
        {
            var actualVersion = _dbServices.GetActualVersion(fileName);
            var path = _dbServices.GetPathOfMaterialByTheVersionAndName(fileName, actualVersion);

            return (new FileStream(path, FileMode.Open));
        }

        public FileStream DownloadMaterialByNameAndVersion(string fileName, int version)
        {            
            var path = _dbServices.GetPathOfMaterialByTheVersionAndName(fileName, version);

            return (new FileStream(path, FileMode.Open));
        }

        public void ChangeCategoryOfFile(string fileName, MaterialCategories category)
        {
            var material = _dbServices. GetMaterialByName(fileName);

            material.Category = category;
            _dbServices.UpdateMaterial(material);
        }
    }
}
