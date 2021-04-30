using powerful_crm.Core.Models;

namespace powerful_crm.Business
{
    public interface ICityService
    {
        int AddCity(CityDto city);
        int DeleteCity(int id);
        CityDto GetCityById(int id);
    }
}
