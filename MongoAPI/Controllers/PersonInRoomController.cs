using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using MongoAPI.Models;
using MongoAPI.Models.Dto;
using MongoAPI.Options;
using MongoAPI.Services;
using MongoDB.Bson;

namespace MongoAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    public class PersonInRoomController : Controller
    {
        private readonly PersonInRoomService _personInRoomService;
        
        public PersonInRoomController(PersonInRoomService personInRoomService)
        {
            _personInRoomService = personInRoomService;
        }
        
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(RecordsDto), 200)]
        [ProducesResponseType(typeof(ErrorMsg), 400)]
        public async Task<IActionResult> GetOne(string id)
        {
            try
            {
                var res = await _personInRoomService.GetAsync(id);
                return Json(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMsg(false, ex.Message));
            }
        }
        
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(RecordDetailsDto), 200)]
        [ProducesResponseType(typeof(ErrorMsg), 400)]
        public async Task<IActionResult> GeRecordDetails(string id)
        {
            try
            {
                var res = await _personInRoomService.GetDetailsAsync(id);
                return Json(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMsg(false, ex.Message));
            }
        }
        
        [HttpGet]
        [ProducesResponseType(typeof(List<RecordsDto>), 200)]
        [ProducesResponseType(typeof(ErrorMsg), 400)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var res = await _personInRoomService.GetAsync();
                return Json(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMsg(false, ex.Message));
            }
        }
        
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorMsg), 400)]
        public async Task<IActionResult> Post([FromForm] PersonInRoom data)
        {
            try
            {
                ModelIsValid(data);
                
                await _personInRoomService.CreateAsync(data);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMsg(false, ex.Message));
            }
        }
        
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorMsg), 400)]
        public async Task<IActionResult> Update([FromForm] PersonInRoom data)
        {
            try
            {
                ModelIsValid(data);
                
                await _personInRoomService.UpdateAsync(data);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMsg(false, ex.Message));
            }
        }
        
        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorMsg), 400)]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _personInRoomService.RemoveAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMsg(false, ex.Message));
            }
        }

        private void ModelIsValid(PersonInRoom personInRoom)
        {
            // if (string.IsNullOrEmpty(personInRoom.PersonId))
            //     throw new Exception("Не указан клиент");
            //
            // if (string.IsNullOrEmpty(personInRoom.HotelRoomId))
            //     throw new Exception("Не указан номер отеля");

            if (!ModelState.IsValid)
                throw new Exception(string.Join("; ", ModelState
                    .Where(x => x.Value.ValidationState == ModelValidationState.Invalid)
                    .SelectMany(x => x.Value.Errors)
                    .Select(x => x.ErrorMessage)));
        }
    }
}