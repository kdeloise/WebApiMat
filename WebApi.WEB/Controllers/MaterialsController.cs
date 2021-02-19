using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WebApi.BLL.BM;
using WebApi.DAL.Entities;
using WebApi.BLL.Interfaces;
using WebApi.DAL.Interfaces;

namespace WebApi.WEB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialsController : ControllerBase
    {
        private readonly IMaterialServices _materialServices;
        private readonly IDBServices _dbServices;

        public MaterialsController(IMaterialServices materialServices, IDBServices dbServices)
        {
            _dbServices = dbServices;
            _materialServices = materialServices;
        }

        [HttpPost]
        [Authorize(Roles = "admin, writer")]
        public async Task<IActionResult> AddNewMaterial(IFormFile file, MaterialCategories category)
        {
            try
            {
                if (_dbServices.CheckFilesInDB(file.FileName))
                {
                    return BadRequest($"File: {file.FileName} already exists");
                }
                if (_dbServices.ValidateOfCategory(category) == false)
                {
                    return BadRequest($"Error category. (Presentation, Application, Other)");
                }

                byte[] fileBytes;

                await using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    fileBytes = memoryStream.ToArray();
                }

                MaterialFileBM fileMaterialBM = new MaterialFileBM
                {
                    FileName = file.FileName,
                    FileBytes = fileBytes,
                    FileSize = file.Length
                };

                Material material = new Material
                {
                    MaterialName = file.FileName,
                    Category = category,
                    ActualVersion = 1,
                    Versions = new List<MaterialVersion>()
                };

                await _materialServices.AddNewMaterialToDB(material, fileMaterialBM);
                return Ok($"Material {file.FileName} has been added successfully");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("version")]
        [Authorize(Roles = "admin, writer")]
        public async Task<IActionResult> AddNewVersionMaterial(IFormFile file)
        {
            try
            {
                int version;

                if (_dbServices.CheckFilesInDB(file.FileName))
                {
                    version = _dbServices.GetActualVersion(file.FileName) + 1;
                }
                else
                    return BadRequest($"File: {file} don't have in DB yet");

                byte[] fileBytes;

                await using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    fileBytes = memoryStream.ToArray();
                }

                MaterialFileBM fileMaterialBM = new MaterialFileBM
                {
                    FileName = file.FileName,
                    FileBytes = fileBytes,
                    FileSize = file.Length
                };

                await _materialServices.AddNewMaterialVersionToDb(fileMaterialBM, version);
                return Ok($"Material {file.FileName}v.{version} has been added successfully");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("info/{category}/{minSize}/{maxSize}")]
        [Authorize(Roles = "admin, reader")]
        public IEnumerable<Material> GetFiltersInfo(MaterialCategories category, double minSize, double maxSize)
        {
            return _materialServices.GetInfoByTheFiltersFromDb(category, minSize, maxSize);
        }

        [HttpGet]
        [Route("info/all")]
        [Authorize(Roles = "admin, reader")]
        public ActionResult<IEnumerable<Material>> GetAllMaterialsInfo()
        {
            if (_dbServices.CheckFilesInDB(null))
            {
                return Ok(_dbServices.GetListOfMaterials());
            }
            else
                return Ok($"DB is empty");
        }

        [HttpGet]
        [Route("info/{name}")]
        [Authorize(Roles = "admin, reader")]
        public ActionResult<Material> GetInfo(string name)
        {
            if (_dbServices.CheckFilesInDB(name))
            {
                return Ok(_dbServices.GetMaterialByName(name));
            }
            else
                return BadRequest($"File: {name} does not exists in DB");

        }

        [HttpGet]
        [Route("{name}")]
        [Authorize(Roles = "admin, reader")]
        public IActionResult GetActualMaterial(string name)
        {
            try
            {
                return File(_materialServices.DownloadMaterialByName(name), "application/octet-stream", name);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("{name}/{version}")]
        [Authorize(Roles = "admin, reader")]
        public IActionResult GetMaterialByVersion(string name, int version)
        {
            try
            {
                return File(_materialServices.DownloadMaterialByNameAndVersion(name, version), "application/octet-stream", name);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPatch]
        [Authorize(Roles = "admin, writer")]
        public IActionResult ChangeCategory(string name, MaterialCategories category)
        {
            if (_dbServices.CheckFilesInDB(name) && _dbServices.ValidateOfCategory(category) == true)
            {
                _materialServices.ChangeCategoryOfFile(name, category);
                return Ok($"Category of File: {name} has been changed to {category}");
            }
            else
                return BadRequest($"File: {name} does not exists or Error: {category}");
        }
    }
}
