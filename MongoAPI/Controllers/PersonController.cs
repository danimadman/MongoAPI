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
    public class PersonController : Controller
    {
        private readonly PersonService _personService;
        
        public PersonController(PersonService personService)
        {
            _personService = personService;
        }
        
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(Person), 200)]
        [ProducesResponseType(typeof(ErrorMsg), 400)]
        public async Task<IActionResult> GetOne(string id)
        {
            try
            {
                Person res = await _personService.GetAsync(id);
                return Json(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMsg(false, ex.Message));
            }
        }
        
        [HttpGet]
        [ProducesResponseType(typeof(List<PersonDto>), 200)]
        [ProducesResponseType(typeof(ErrorMsg), 400)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var res = await _personService.GetAsync();
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
        public async Task<IActionResult> Post([FromForm] Person data)
        {
            try
            {
                ModelIsValid();
                
                await _personService.CreateAsync(data);
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
        public async Task<IActionResult> Update([FromForm] Person data)
        {
            try
            {
                ModelIsValid();
                
                await _personService.UpdateAsync(data);
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
                await _personService.RemoveAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMsg(false, ex.Message));
            }
        }
        
        private void ModelIsValid()
        {
            if (!ModelState.IsValid)
                throw new Exception(string.Join("; ", ModelState
                    .Where(x => x.Value.ValidationState == ModelValidationState.Invalid)
                    .SelectMany(x => x.Value.Errors)
                    .Select(x => x.ErrorMessage)));
        }
    }
}