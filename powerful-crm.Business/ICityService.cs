using powerful_crm.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace powerful_crm.Business
{
    public interface ICityService
    {
        int AddCity(CityDto city);
        int DeleteCity(int id);
        CityDto GetCityById(int id);
    }
}
