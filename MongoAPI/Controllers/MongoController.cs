using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoAPI.Models;
using MongoAPI.Services;
using MongoDB.Bson;

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

        [HttpGet]
        [Route("[action]/{dbName}/{collectionName}/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorMsg), 400)]
        public async Task<IActionResult> GetOneAsync(string dbName, string collectionName, ObjectId id)
        {
            try
            {
                var res = await _mongoService.GetOneAsync<object>(dbName, collectionName, id);

                if (res == null)
                    return NotFound();
                
                return Json(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMsg(false, ex.Message));
            }
        }
        
        [HttpGet]
        [Route("[action]/{dbName}/{collectionName}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorMsg), 400)]
        public async Task<IActionResult> GetAsync(string dbName, string collectionName)
        {
            try
            {
                var res = await _mongoService.GetAsync<object>(dbName, collectionName);

                if (res == null || res.Count == 0)
                    return NotFound();
                
                return Json(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMsg(false, ex.Message));
            }
        }
        
        [HttpPost]
        [Route("[action]/{dbName}/{collectionName}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorMsg), 400)]
        public async Task<IActionResult> PostAsync(string dbName, string collectionName, [FromBody] object data)
        {
            try
            {
                await _mongoService.CreateAsync(dbName, collectionName, data);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMsg(false, ex.Message));
            }
        }
        
        [HttpPut]
        [Route("[action]/{dbName}/{collectionName}/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorMsg), 400)]
        public async Task<IActionResult> UpdateAsync(string dbName, string collectionName, ObjectId id, [FromBody] object data)
        {
            try
            {
                await _mongoService.UpdateAsync(dbName, collectionName, data, id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMsg(false, ex.Message));
            }
        }
        
        [HttpDelete]
        [Route("[action]/{dbName}/{collectionName}/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorMsg), 400)]
        public async Task<IActionResult> DeleteAsync(string dbName, string collectionName, ObjectId id)
        {
            try
            {
                await _mongoService.RemoveAsync<object>(dbName, collectionName, id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMsg(false, ex.Message));
            }
        }
    }
}