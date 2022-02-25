using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoAPI.Models;
using MongoAPI.Models.Dto;
using MongoAPI.Services;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace MongoAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    public class MongoController : Controller
    {
        private readonly MongoService _mongoService;
        
        public MongoController(MongoService mongoService)
        {
            _mongoService = mongoService;
        }
        
        [HttpGet("{connString}")]
        [ProducesResponseType(typeof(List<DdlDto>), 200)]
        [ProducesResponseType(typeof(ErrorMsg), 400)]
        public async Task<IActionResult> GetDataBases(string connString)
        {
            try
            {
                var res = await _mongoService.GetDataBases(connString);
                return Json(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMsg(false, ex.Message));
            }
        }
        
        [HttpGet]
        [Route("{connString}/{dbName}")]
        [ProducesResponseType(typeof(List<DdlDto>), 200)]
        [ProducesResponseType(typeof(ErrorMsg), 400)]
        public async Task<IActionResult> GetCollections(string connString, string dbName)
        {
            try
            {
                var res = await _mongoService.GetCollections(connString, dbName);
                return Json(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMsg(false, ex.Message));
            }
        }

        [HttpGet]
        [Route("{dbName}/{collectionName}/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorMsg), 400)]
        public async Task<IActionResult> GetOneAsync(string dbName, string collectionName, string id)
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
        [Route("{dbName}/{collectionName}")]
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
        [Route("{dbName}/{collectionName}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorMsg), 400)]
        public async Task<IActionResult> PostAsync(string dbName, string collectionName, [FromBody] dynamic value)
        {
            try
            {
                await _mongoService.CreateAsync(dbName, collectionName, value);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMsg(false, ex.Message));
            }
        }
        
        [HttpPut]
        [Route("{dbName}/{collectionName}/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorMsg), 400)]
        public async Task<IActionResult> UpdateAsync(string dbName, string collectionName, string id, [FromBody] dynamic value)
        {
            try
            {
                await _mongoService.UpdateAsync(dbName, collectionName, value, id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMsg(false, ex.Message));
            }
        }
        
        [HttpDelete]
        [Route("{dbName}/{collectionName}/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorMsg), 400)]
        public async Task<IActionResult> DeleteAsync(string dbName, string collectionName, string id)
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

        [HttpGet]
        [Route("{dbName}/{collectionName}")]
        [ProducesResponseType(typeof(FileStreamResult), 200)]
        [ProducesResponseType(typeof(ErrorMsg), 400)]
        public async Task<IActionResult> UploadCollection(string dbName, string collectionName)
        {
            try
            {
                (byte[] bytes, string mimeType) = await _mongoService.UploadCollection(dbName, collectionName);
                
                HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
                return File(bytes, mimeType, collectionName);
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorMsg() { Message = e.Message, Status = false });
            }
        }
    }
}