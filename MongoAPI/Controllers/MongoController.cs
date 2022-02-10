using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoAPI.Models;
using MongoAPI.Services;

namespace MongoAPI.Controllers
{
    [Route("api/[controller]")]
    public class MongoController : Controller
    {
        private readonly MongoService _mongoService;
        
        public MongoController(MongoService mongoService)
        {
            _mongoService = mongoService;
        }
        
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(List<string>), 200)]
        [ProducesResponseType(typeof(ErrorMsg), 400)]
        public async Task<IActionResult> GetDataBases()
        {
            try
            {
                var res = await _mongoService.GetDataBases();
                return Json(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMsg(false, ex.Message));
            }
        }
        
        [HttpGet]
        [Route("[action]/{dbName}")]
        [ProducesResponseType(typeof(List<string>), 200)]
        [ProducesResponseType(typeof(ErrorMsg), 400)]
        public async Task<IActionResult> GetCollections(string dbName)
        {
            try
            {
                var res = await _mongoService.GetCollections(dbName);
                return Json(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMsg(false, ex.Message));
            }
        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorMsg), 400)]
        public async Task<IActionResult> SavePerson([FromBody] Person data)
        {
            try
            {
                await _mongoService.InsertDocument("local", "person", data);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMsg(false, ex.Message));
            }
        }
        
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorMsg), 400)]
        public async Task<IActionResult> SaveHotelNumber([FromBody] HotelNumber data)
        {
            try
            {
                await _mongoService.InsertDocument("local", "hotel_numbers", data);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMsg(false, ex.Message));
            }
        }
    }
}