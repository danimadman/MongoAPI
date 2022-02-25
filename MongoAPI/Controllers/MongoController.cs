using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        
        [HttpGet]
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

        [HttpPost]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(ErrorMsg), 400)]
        public IActionResult PostDb([FromBody] MongoDbDto dto)
        {
            try
            {
                _mongoService.CreateDb(dto.connString, dto.dbName);
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMsg(false, ex.Message));
            }
        }
        
        [HttpPost]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(ErrorMsg), 400)]
        public IActionResult PostCollection([FromBody] MongoDbDto dto)
        {
            try
            {
                _mongoService.CreateCollection(dto.connString, dto.dbName, dto.collectionName);
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMsg(false, ex.Message));
            }
        }
        
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorMsg), 400)]
        public async Task<IActionResult> GetContent(string connString, string dbName, string collectionName, int comfortLevel)
        {
            try
            {
                var res = await _mongoService.GetAsync(connString, dbName, collectionName, comfortLevel);
                if (res == null)
                    return Json("");

                //object obj = res.Select(x => BsonTypeMapper.MapToDotNetValue(x)).ToList();

                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMsg(false, ex.Message));
            }
        }
        
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorMsg), 400)]
        public IActionResult CreateDoc(string connString, string dbName, string collectionName)
        {
            try
            {
                _mongoService.CreateDoc(connString, dbName, collectionName, new HotelRoom()
                {
                    Number = 50,
                    Cost = 5000,
                    Seats = 3,
                    ComfortLevel = 1
                });
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMsg(false, ex.Message));
            }
        }
    }
}