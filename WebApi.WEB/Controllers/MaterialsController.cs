using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApi.DAL.EF;
using WebApi.DAL.Entities;

namespace WebApi.WEB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialsController : ControllerBase
    {
        private readonly MaterialsDbContext _context;

        public MaterialsController(MaterialsDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddNewMaterial(IFormFile file, MaterialCategories category)
        {
            if (_context.Materialss.Count(x => x.materialName == file.FileName) > 0)
            {
                return BadRequest($"File: {file.FileName} already exists");
            }
            if (Enum.IsDefined(typeof(MaterialCategories), category) == false)
            {
                return BadRequest($"Error category. (Presentation, Application, Other)");
            }

            try
            {
                var path = "../WebApi.DAL/Files/" + file.FileName;

                await using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                Material newFile = new Material
                {
                    materialName = file.FileName,
                    path = path,
                    metaFileSize = file.Length,
                    category = category,
                    metaDateTime = DateTime.Now,
                    versionNumber = 1
                };

                _context.Add(newFile);
                _context.SaveChanges();

                return Ok($"Material {file.FileName} has been added successfully");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("version")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddNewVersionMaterial(IFormFile file)
        {
            int count;
            int version;

            if ((count = _context.Materialss.Count(x => x.materialName == file.FileName)) > 0)
            {
                version = count + 1;
            }
            else
                return BadRequest($"File: {file} don't have in DB yet");

            try
            {
                var path = "../WebApi.DAL/Files/" + file.FileName;
                var category = _context.Materialss.Where(x => x.materialName == file.FileName)
                                                  .First(x => x.category != 0).category;

                await using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                Material newFile = new Material
                {
                    materialName = file.FileName,
                    path = path,
                    metaFileSize = file.Length,
                    category = category,
                    metaDateTime = DateTime.Now,
                    versionNumber = version
                };

                _context.Add(newFile);
                _context.SaveChanges();

                return Ok($"Material {file.FileName}v.{version} has been added successfully");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("info/{category}/{minSize}/{maxSize}")]
        [Authorize(Roles = "admin")]
        public ActionResult<IEnumerable<Material>> GetFiltersInfo(MaterialCategories category, double minSize, double maxSize)
        {
            var materials = new List<Material>();
            var filtersMat = _context.Materialss.Where(x => x.category == category)
                                                .Where(x => x.metaFileSize >= minSize)
                                                .Where(x => x.metaFileSize <= maxSize);

            foreach (var mat in filtersMat)
            {
                materials.Add( new Material
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

        [HttpGet]
        [Route("info/all")]
        [Authorize(Roles = "admin")]
        public ActionResult<IEnumerable<Material>> GetAllMaterialsInfo()
        {
            if (_context.Materialss.Count() > 0)
            {
                return _context.Materialss.ToList();
            }
            else
                return Ok($"DB is empty");
        }

        [HttpGet]
        [Route("info/{name}")]
        [Authorize(Roles = "admin")]
        public ActionResult<IEnumerable<Material>> GetInfo(string name)
        {
            if (_context.Materialss.Count(x => x.materialName == name) > 0)
            {
                return _context.Materialss.Where(x => x.materialName == name).ToList();
            }
            else
                return BadRequest($"File: {name} does not exists");

        }

        [HttpGet]
        [Route("{name}")]
        [Authorize(Roles = "admin")]
        public IActionResult GetActualMaterial(string name)
        {
            try
            {
                var actualVersion = _context.Materialss.Max(x => x.versionNumber);
                var path = _context.Materialss.First(x => x.versionNumber == actualVersion).path;

                return File( new FileStream(path, FileMode.Open), "application/octet-stream", name);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("{name}/{version}")]
        [Authorize(Roles = "admin")]
        public IActionResult GetActualMaterial(string name, int version)
        {
            try
            {
                var path = _context.Materialss.First(x => x.versionNumber == version).path;

                return File(new FileStream(path, FileMode.Open), "application/octet-stream", name);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPatch]
        [Authorize(Roles = "admin")]
        public IActionResult ChangeCategory(string name, MaterialCategories category)
        {
            if (_context.Materialss.Count(x => x.materialName == name) > 0 && Enum.IsDefined(typeof(MaterialCategories), category) == true)
            {
                var materials = _context.Materialss.Where(x => x.materialName == name).ToList();

                foreach (var mat in materials)
                {
                    mat.category = category;
                    _context.Materialss.Update(mat);
                }
                _context.SaveChanges();

                return Ok($"Category of File: {name} has been changed to {category}");
            }
            else
                return BadRequest($"File: {name} does not exists or Error: {category}");
        }

    }
}
