using powerful_crm.Core.Models;
using powerful_crm.Data;
using System.Threading.Tasks;

namespace powerful_crm.Business
{
   public class CityService:ICityService
    {
        private ICityRepository _cityRepository;
        public CityService(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }
        public async Task<int> AddCityAsync(CityDto city) => await _cityRepository.AddCityAsync(city);
        public async Task<int> DeleteCityAsync(int id) => await _cityRepository.DeleteCityAsync(id);
        public async Task<CityDto> GetCityByIdAsync(int id) => await _cityRepository.GetCityByIdAsync(id);
    }
}
