using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MongoAPI.Models;
using MongoAPI.Models.Dto;
using MongoAPI.Models.Enums;
using MongoAPI.Services;

namespace MongoAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    public class DictController : Controller
    {
        private readonly DictService _dictService;
        
        public DictController(DictService dictService)
        {
            _dictService = dictService;
        }
        
        [HttpGet]
        [ProducesResponseType(typeof(ComfortLevelDto), 200)]
        [ProducesResponseType(typeof(ErrorMsg), 400)]
        public IActionResult GetDictComfortLevel()
        {
            try
            {
                return Json(_dictService.GetDictComfortLevel());
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorMsg(false, e.Message));
            }
        }
    }
}