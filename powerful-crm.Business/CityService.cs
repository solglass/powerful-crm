using powerful_crm.Core.Models;
using powerful_crm.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace powerful_crm.Business
{
   public class CityService:ICityService
    {
        private ICityRepository _cityRepository;
        public CityService(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }
        public int AddCity(CityDto city) => _cityRepository.AddCity(city);
        public int DeleteCity(int id) => _cityRepository.DeleteCity(id);
        public CityDto GetCityById(int id) => _cityRepository.GetCityById(id);
    }
}
