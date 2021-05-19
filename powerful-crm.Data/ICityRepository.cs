using powerful_crm.Core.Models;
using System.Threading.Tasks;

namespace powerful_crm.Data
{
    public interface ICityRepository
    {
        Task<int> AddCityAsync(CityDto city);
        Task<int> DeleteCityAsync(int id);
        Task<CityDto> GetCityByIdAsync(int id);
    }
}
