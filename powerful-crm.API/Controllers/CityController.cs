using AutoMapper;
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

namespace powerful_crm.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class CityController : ControllerBase
    {
        private ICityService _cityService;
        private IMapper _mapper;
        public CityController(IOptions<AppSettings> options, IMapper mapper,  ICityService cityService)
        {
            _cityService = cityService;
            _mapper = mapper;
        }
        /// <summary>Creates new city</summary>
        /// <param name="city">Information about new city</param>
        /// <returns>Info about created city</returns>
        [ProducesResponseType(typeof(CityOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost]
        public ActionResult<CityOutputModel> AddCity([FromBody] CityInputModel city)
        {
            if (!CheckIfUserIsAllowed())
                throw new ForbidException(Constants.ERROR_NOT_ALLOWED_ACTIONS_WITH_CITY);

            if (!ModelState.IsValid)
            {
                throw new CustomValidationException(ModelState);
            }
            var dto = _mapper.Map<CityDto>(city);
            var addedCityId = _cityService.AddCity(dto);
            var outputModel = _mapper.Map<CityOutputModel>(_cityService.GetCityById(addedCityId));
            return Ok(outputModel);
        }

        /// <summary>Deletes the city</summary>
        /// <param name="cityId">Id of the city to delete</param>
        /// <returns>NoContent result</returns>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpDelete("{id}")]
        public ActionResult DeleteCity(int cityId)
        {
            if (!CheckIfUserIsAllowed())
                throw new ForbidException(Constants.ERROR_NOT_ALLOWED_ACTIONS_WITH_CITY);

            var city = _cityService.GetCityById(cityId);
            if (city == null)
            {
                return NotFound(string.Format(Constants.ERROR_CITY_NOT_FOUND, cityId));
            }
            if (_cityService.DeleteCity(cityId) == 1)
                return NoContent();
            else
                return Conflict(string.Format(Constants.ERROR_CITY_HAS_DEPENDENCIES, cityId));
        }
        private bool CheckIfUserIsAllowed()
        {
            return HttpContext.User.IsInRole(Role.Administrator.ToString());
        }
    }
}
