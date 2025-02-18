﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using powerful_crm.API.Models.OutputModels;
using powerful_crm.Business;
using powerful_crm.Core;
using powerful_crm.Core.CustomExceptions;
using powerful_crm.Core.Enums;
using powerful_crm.Core.Models;
using powerful_crm.Core.Settings;
using System.Threading.Tasks;

namespace powerful_crm.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class CityController : ControllerBase
    {
        private ICityService _cityService;
        private IMapper _mapper;
        private ICheckerService _checker;
        public CityController(IOptions<AppSettings> options, IMapper mapper, ICheckerService checker, ICityService cityService)
        {
            _cityService = cityService;
            _checker = checker;
            _mapper = mapper;
        }
        /// <summary>Creates new city</summary>
        /// <param name="city">Information about new city</param>
        /// <returns>Info about created city</returns>
        [ProducesResponseType(typeof(CityOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost]
        public async Task<ActionResult<CityOutputModel>> AddCityAsync([FromBody] CityInputModel city)
        {
            if (!_checker.CheckIfUserIsAllowed(HttpContext))
                throw new ForbidException(Constants.ERROR_NOT_ALLOWED_ACTIONS_WITH_CITY);

            if (!ModelState.IsValid)
                throw new CustomValidationException(ModelState);

            var dto = _mapper.Map<CityDto>(city);
            var addedCityId = await _cityService.AddCityAsync(dto);
            var outputModel = _mapper.Map<CityOutputModel>(await _cityService.GetCityByIdAsync(addedCityId));
            return Ok(outputModel);
        }

        /// <summary>Deletes the city</summary>
        /// <param name="cityId">Id of the city to delete</param>
        /// <returns>NoContent result</returns>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpDelete("{cityId}")]
        public async Task<ActionResult> DeleteCityAsync(int cityId)
        {
            if (!_checker.CheckIfUserIsAllowed(HttpContext))
                throw new ForbidException(Constants.ERROR_NOT_ALLOWED_ACTIONS_WITH_CITY);

            var city = await _cityService.GetCityByIdAsync(cityId);
            if (city == null)
            {
                return NotFound(new CustomExceptionOutputModel
                {
                    Code = StatusCodes.Status404NotFound,
                    Message = string.Format(Constants.ERROR_CITY_NOT_FOUND, cityId)
                });
            }

            if (!await _cityService.DeleteCityAsync(cityId))
                return Conflict(new CustomExceptionOutputModel
                {
                    Code = StatusCodes.Status409Conflict,
                    Message = string.Format(Constants.ERROR_CITY_HAS_DEPENDENCIES, cityId)
                });

            return NoContent();
        }
    }
}
