using powerful_crm.Core.Models;
using System.Threading.Tasks;

namespace powerful_crm.Business
{
    public interface ICityService
    {
        Task<int> AddCityAsync(CityDto city);
        Task<int> DeleteCityAsync(int id);
        Task<CityDto> GetCityByIdAsync(int id);
    }
}
