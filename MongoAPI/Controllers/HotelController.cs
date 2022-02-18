using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoAPI.Models;
using MongoAPI.Models.Enums;
using MongoAPI.Services;

namespace MongoAPI.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]/[action]")]
    public class HotelController : Controller
    {
        private readonly HotelService _hotelService;
        
        public HotelController(HotelService hotelService)
        {
            _hotelService = hotelService;
        }
        
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(HotelRoom), 200)]
        [ProducesResponseType(typeof(ErrorMsg), 400)]
        public async Task<IActionResult> GetOne(string id)
        {
            try
            {
                var res = await _hotelService.GetAsync(id);
                return Json(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMsg(false, ex.Message));
            }
        }
        
        [HttpGet]
        [ProducesResponseType(typeof(List<HotelRoom>), 200)]
        [ProducesResponseType(typeof(ErrorMsg), 400)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var res = await _hotelService.GetAsync();
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
        public async Task<IActionResult> Post([FromBody] HotelRoom data)
        {
            try
            {
                ModelIsValid(data);
                
                await _hotelService.CreateAsync(data);
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
        public async Task<IActionResult> Update([FromBody] HotelRoom data)
        {
            try
            {
                ModelIsValid(data);
                
                await _hotelService.UpdateAsync(data);
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
                await _hotelService.RemoveAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMsg(false, ex.Message));
            }
        }

        private void ModelIsValid(HotelRoom room)
        {
            if (!Enum.IsDefined(typeof(ComformLevelEnum), room.ComfortLevel))
                throw new Exception("Выберите уровень комфорта");
            //
            // if (room.Cost < 0)
            //     throw new Exception("Недопустимое значение для цены номера отеля");
            //
            // if (room.Number < 0)
            //     throw new Exception("Недопустимое значение для номер отеля");
            //
            // if (room.Seats < 0)
            //     throw new Exception("Недопустимое значение для количества мест в номере отеля");
            
            if (!ModelState.IsValid)
                throw new Exception("Проверьте правильность заполненных данных");
        }
    }
}