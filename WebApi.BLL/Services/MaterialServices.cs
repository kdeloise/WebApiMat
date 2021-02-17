using Microsoft.AspNetCore.Http;
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

namespace WebApi.BLL.Services
{
    public class MaterialServices : IMaterialServices
    {
        private readonly MaterialsDbContext _context;
        private readonly IFileManager _fileManager;

        public MaterialServices(MaterialsDbContext context, IFileManager fileManager)
        {            
            _context = context;
            _fileManager = fileManager;
        }

        public IQueryable<Material> GetMaterialsByTheFilters(MaterialCategories category, double minSize, double maxSize)
        {
            return _context.Materialss.Where(x => x.category == category)
                                                .Where(x => x.metaFileSize >= minSize)
                                                .Where(x => x.metaFileSize <= maxSize);
        }

        public MaterialCategories GetCategoryOfMaterial(string fileName)
        {
            return _context.Materialss.Where(x => x.materialName == fileName)
                                                  .First(x => x.category != 0).category;
        }

        #nullable enable
        public IEnumerable<Material> GetMaterials(string? fileName)
        {
            return ((fileName == null)
                ? _context.Materialss.ToList()
                : _context.Materialss.Where(x => x.materialName == fileName).ToList());
        }

        #nullable enable
        public int GetCountOfMaterials(string? fileName)
        {
            return ((fileName != null)
                ? _context.Materialss.Count(x => x.materialName == fileName)
                : _context.Materialss.Count());
        }

        public bool ValidateOfCategory(MaterialCategories category)
        {
            return Enum.IsDefined(typeof(MaterialCategories), category);
        }

        public int GetActualVersion(string fileName)
        {
            return GetMaterials(fileName).Max(x => x.versionNumber);
        }

        public string GetPathOfMaterialByTheVersionAndName(string fileName, int version)
        {
            return _context.Materialss.Where(x => x.materialName == fileName)
                .First(x => x.versionNumber == version).path;
        }

        public async Task AddNewMaterialToDB(Material material, MaterialFileBM fileMaterialBM)
        {
            var path = await _fileManager.SaveFile(fileMaterialBM.FileBytes, fileMaterialBM.FileName);

            material.path = path;

            await _context.AddAsync(material);
            await _context.SaveChangesAsync();
        }

        public async Task AddNewMaterialVersionToDb(MaterialFileBM fileMaterialBM, int version)
        {
            var path = await _fileManager.SaveFile(fileMaterialBM.FileBytes, fileMaterialBM.FileName);
            var category = GetCategoryOfMaterial(fileMaterialBM.FileName);

            Material newFile = new Material
            {
                materialName = fileMaterialBM.FileName,
                path = path,
                metaFileSize = fileMaterialBM.FileSize,
                category = category,
                metaDateTime = DateTime.Now,
                versionNumber = version
            };

            await _context.AddAsync(newFile);
            await _context.SaveChangesAsync();
        }

        public IEnumerable<Material> GetInfoByTheFiltersFromDb(MaterialCategories category, double minSize, double maxSize)
        {
            var materials = new List<Material>();
            var filtersMat = GetMaterialsByTheFilters(category, minSize, maxSize);

            foreach (var mat in filtersMat)
            {
                materials.Add(new Material
                {
                    materialName = mat.materialName,
                    category = mat.category,
                    metaFileSize = mat.metaFileSize,
                    versionNumber = mat.versionNumber,
                    metaDateTime = mat.metaDateTime,
                    path = mat.path
                });
            }

            return materials;
        }

        public FileStream DownloadMaterialByName(string fileName)
        {
            var actualVersion = GetActualVersion(fileName);
            var path = GetPathOfMaterialByTheVersionAndName(fileName, actualVersion);

            return (new FileStream(path, FileMode.Open));
        }

        public FileStream DownloadMaterialByNameAndVersion(string fileName, int version)
        {            
            var path = GetPathOfMaterialByTheVersionAndName(fileName, version);

            return (new FileStream(path, FileMode.Open));
        }

        public void ChangeCategoryOfFile(string fileName, MaterialCategories category)
        {
            var materials = GetMaterials(fileName);

            foreach (var mat in materials)
            {
                mat.category = category;
                _context.Materialss.Update(mat);
            }
            _context.SaveChangesAsync();
        }
    }


}
