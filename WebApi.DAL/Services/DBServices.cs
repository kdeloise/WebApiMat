using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.DAL.EF;
using WebApi.DAL.Entities;
using WebApi.DAL.Interfaces;

namespace WebApi.DAL.Services
{
    public class DBServices : IDBServices
    {
        private readonly MaterialsDbContext _context;

        public DBServices(MaterialsDbContext context)
        {
            _context = context;
        }

        public async Task SaveMaterial(Material material, MaterialVersion materialVersion)
        {
            await _context.MaterialVersions.AddAsync(materialVersion);
            await _context.Materialss.AddAsync(material);
            await _context.SaveChangesAsync();
        }

        public async Task SaveMaterialVersion(MaterialVersion materialVersion)
        {
            await _context.MaterialVersions.AddAsync(materialVersion);
            await _context.SaveChangesAsync();
        }

        public void UpdateMaterial(Material material)
        {
            _context.Materialss.Update(material);
            _context.SaveChanges();
        }


        public IEnumerable<Material> GetMaterialsByTheFilters(MaterialCategories category, double minSize, double maxSize)
        {
            var result = new List<Material>();
            var materials = GetListOfMaterials().Where(x => x.Category == category);

            foreach (var mat in materials)
            {
                if (_context.MaterialVersions.Count(x => x.MaterialId == GetMaterialIdByName(mat.MaterialName)
                                                    && minSize <= x.MetaFileSize
                                                    && x.MetaFileSize <= maxSize) > 0)
                    result.Add(mat);
            }


            return result;
        }

        public MaterialCategories GetCategoryOfMaterial(string fileName)
        {
            return _context.Materialss.Where(x => x.MaterialName == fileName)
                                                  .First(x => x.Category != 0).Category;
        }

        public int GetActualVersion(string fileName)
        {
            return GetMaterialByName(fileName).ActualVersion;
        }

        public Material GetMaterialByName(string fileName)
        {
            var material = _context.Materialss.First(x => x.MaterialName == fileName);
            var Id = GetMaterialIdByName(fileName);
            var materialVersions = _context.MaterialVersions.Where(x => x.MaterialId == Id);
            material.Versions = new List<MaterialVersion>
            { _context.MaterialVersions.First(x => x.MaterialId == GetMaterialIdByName(material.MaterialName)) };
            return material;
        }

        public IEnumerable<Material> GetListOfMaterials()
        {
            int i = 0;
            var materials = _context.Materialss.ToList();
            var materialsVersion = _context.MaterialVersions.ToList();
            foreach (var mat in materials)
            {
                i++;
            }
            while (--i > 0)
            {
                materials[i].Versions = new List<MaterialVersion>
                (_context.MaterialVersions.Where(x => x.MaterialId == GetMaterialIdByName(materials[i].MaterialName)).ToList());
            }
            return materials;
        }

#nullable enable
        public bool CheckFilesInDB(string? fileName)
        {
            return ((fileName != null)
                ? _context.Materialss.Count(x => x.MaterialName == fileName) > 0
                : _context.Materialss.Count() > 0);
        }

        public bool ValidateOfCategory(MaterialCategories category)
        {
            return Enum.IsDefined(typeof(MaterialCategories), category);
        }

        public int GetMaterialIdByName(string fileName)
        {
            return _context.Materialss.First(x => x.MaterialName == fileName).Id;
        }

        public string GetPathOfMaterialByTheVersionAndName(string fileName, int version)
        {
            var material = GetMaterialByName(fileName);
            var Id = GetMaterialIdByName(fileName);
            var materialVersions = _context.MaterialVersions.Where(x => x.MaterialId == Id);
            material.Versions = materialVersions.ToList();
            return material.Versions
                    .First(x => x.VersionNumber == version).Path;
        }
    }
}
