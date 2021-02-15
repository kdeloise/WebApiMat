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
using WebApi.DAL.Services;

namespace WebApi.WEB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialsController : ControllerBase
    {
        private readonly DbServices _dbServices;

        public MaterialsController(MaterialsDbContext context)
        {
            _dbServices = new DbServices(context);
        }

        [HttpPost]
        [Authorize(Roles = "admin, writer")]
        public async Task<IActionResult> AddNewMaterial(IFormFile file, MaterialCategories category)
        {
            if (_dbServices.GetCountOfMaterials(file.FileName) > 0)
            {
                return BadRequest($"File: {file.FileName} already exists");
            }
            if (_dbServices.ValidateOfCategory(category) == false)
            {
                return BadRequest($"Error category. (Presentation, Application, Other)");
            }

            try
            {
                await _dbServices.AddNewMaterialToDb(file, category);
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
            int count;
            int version;

            if ((count = _dbServices.GetCountOfMaterials(file.FileName)) > 0)
            {
                version = count + 1;
            }
            else
                return BadRequest($"File: {file} don't have in DB yet");
            try
            {
                await _dbServices.AddNewMaterialVersionToDb(file, version);
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
            return _dbServices.GetInfoByTheFiltersFromDb(category, minSize, maxSize);
        }

        [HttpGet]
        [Route("info/all")]
        [Authorize(Roles = "admin, reader")]
        public ActionResult<IEnumerable<Material>> GetAllMaterialsInfo()
        {
            if (_dbServices.GetCountOfMaterials(null) > 0)
            {
                return Ok(_dbServices.GetMaterials(null));
            }
            else
                return Ok($"DB is empty");
        }

        [HttpGet]
        [Route("info/{name}")]
        [Authorize(Roles = "admin, reader")]
        public ActionResult<IEnumerable<Material>> GetInfo(string name)
        {
            if (_dbServices.GetCountOfMaterials(name) > 0)
            {
                return Ok(_dbServices.GetMaterials(name));
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
                return File(_dbServices.DownloadMaterialByName(name), "application/octet-stream", name);
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
                return File(_dbServices.DownloadMaterialByNameAndVersion(name, version), "application/octet-stream", name);
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
            if (_dbServices.GetCountOfMaterials(name) > 0 && _dbServices.ValidateOfCategory(category) == true)
            {
                _dbServices.ChangeCategoryOfFile(name, category);
                return Ok($"Category of File: {name} has been changed to {category}");
            }
            else
                return BadRequest($"File: {name} does not exists or Error: {category}");
        }

    }
}
