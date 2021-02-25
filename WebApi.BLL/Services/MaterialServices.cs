using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WebApi.BLL.BM;
using WebApi.BLL.Categories;
using WebApi.BLL.Interfaces;
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

        public bool FileExisting(string fileName)
        {
            return _dbServices.CheckFilesInDB(fileName);
        }

        public bool CategoryExisting(MaterialCategories category)
        {
            return Enum.IsDefined(typeof(MaterialCategories), category);
        }

        public int GetActualVersion(string fileName)
        {
            return _dbServices.GetMaterialByName(fileName).ActualVersion;
        }

        public MaterialBM GetMaterialBMbyName(string fileName)
        {
            var material = _dbServices.GetMaterialByName(fileName);
            var materialVersionsBM = new List<MaterialVersionBM>();

            foreach (var matV in material.Versions)
            {
                materialVersionsBM.Add(new MaterialVersionBM
                {
                    Hash = matV.Hash,
                    MetaDateTime = matV.MetaDateTime,
                    MetaFileSize = matV.MetaFileSize,
                    VersionNumber = matV.VersionNumber
                });
            }

            var materialBM = new MaterialBM
            {
                MaterialName = material.MaterialName,
                ActualVersion = material.ActualVersion,
                Category = material.Category,
                Versions = materialVersionsBM
            };

            return materialBM;            
        }

        public IEnumerable<MaterialBM> GetMaterialsBM()
        {
            var materials = _dbServices.GetListOfMaterials();
            var materialsBM = new List<MaterialBM>();

            foreach (var mat in materials)
            {
                var materialVersionsBM = new List<MaterialVersionBM>();

                foreach (var matV in mat.Versions)
                {
                    materialVersionsBM.Add(new MaterialVersionBM
                    {
                        Hash = matV.Hash,
                        MetaDateTime = matV.MetaDateTime,
                        MetaFileSize = matV.MetaFileSize,
                        VersionNumber = matV.VersionNumber
                    });
                }

                materialsBM.Add(new MaterialBM
                {
                    MaterialName = mat.MaterialName,
                    ActualVersion = mat.ActualVersion,
                    Category = mat.Category,
                    Versions = materialVersionsBM
                });
            }
            return materialsBM;
        }


        public async Task CreateMaterial(MaterialBM materialBM, MaterialFileBM fileMaterialBM)
        {
            var hash = HashCalculate.CalculateMd5(fileMaterialBM.FileBytes);
            await _fileManager.SaveFile(fileMaterialBM.FileBytes, hash);

            Material material = new Material
            {
                MaterialName = materialBM.MaterialName,
                Category = materialBM.Category,
                ActualVersion = materialBM.ActualVersion,
                Versions = new List<MaterialVersion>()
            };

            var materialVersion = new MaterialVersion
            {
                Material = material,
                MetaDateTime = DateTime.Now,
                MetaFileSize = fileMaterialBM.FileSize,
                VersionNumber = 1, 
                Hash = hash
            };

            material.Versions.Add(materialVersion);

            await _dbServices.SaveMaterial(material, materialVersion);
        }

        public async Task CreateMaterialVersion(MaterialFileBM fileMaterialBM, int version)
        {
            var hash = HashCalculate.CalculateMd5(fileMaterialBM.FileBytes);
            await _fileManager.SaveFile(fileMaterialBM.FileBytes, hash);

            Material newFile = _dbServices.GetMaterialByName(fileMaterialBM.FileName);

            var materialVersion = new MaterialVersion
            {
                Material = newFile,
                MetaDateTime = DateTime.Now,
                MetaFileSize = fileMaterialBM.FileSize,
                VersionNumber = version,
                Hash = hash
            };

            newFile.ActualVersion = version;
            newFile.Versions.Add(materialVersion);

            await _dbServices.SaveMaterialVersion(materialVersion);
        }

        public IEnumerable<MaterialBM> GetInfoByTheFilters(MaterialCategories category, double minSize, double maxSize)
        {
            var filtersMat = _dbServices.GetMaterialsByTheFilters((int)category, minSize, maxSize);
            var materialsBM = new List<MaterialBM>();

            foreach (var mat in filtersMat)
            {
                var materialVersionsBM = new List<MaterialVersionBM>();

                foreach (var matV in mat.Versions)
                {
                    materialVersionsBM.Add(new MaterialVersionBM
                    {
                        Hash = matV.Hash,
                        MetaDateTime = matV.MetaDateTime,
                        MetaFileSize = matV.MetaFileSize,
                        VersionNumber = matV.VersionNumber
                    });
                }

                materialsBM.Add(new MaterialBM
                {
                    MaterialName = mat.MaterialName,
                    ActualVersion = mat.ActualVersion,
                    Category = mat.Category,
                    Versions = materialVersionsBM
                });
            }

            return materialsBM;
        }

        public FileStream DownloadMaterialByName(string fileName)
        {
            var actualVersion = GetActualVersion(fileName);
            var hash = _dbServices.GetPathOfMaterialByTheVersionAndName(fileName, actualVersion);
            var path = _fileManager.GetPath(hash);

            return (new FileStream(path, FileMode.Open));
        }

        public FileStream DownloadMaterialByNameAndVersion(string fileName, int version)
        {
            var hash = _dbServices.GetPathOfMaterialByTheVersionAndName(fileName, version);
            var path = _fileManager.GetPath(hash);

            return (new FileStream(path, FileMode.Open));
        }

        public void ChangeCategoryOfFile(string fileName, MaterialCategories category)
        {
            var material = _dbServices.GetMaterialByName(fileName);

            material.Category = (int)category;
            _dbServices.UpdateMaterial(material);
        }
    }
}
