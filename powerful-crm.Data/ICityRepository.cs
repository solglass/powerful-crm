using powerful_crm.Core.Models;

namespace powerful_crm.Data
{
    public interface ICityRepository
    {
        int AddCity(CityDto city);
        int DeleteCity(int id);
        CityDto GetCityById(int id);
    }
}
