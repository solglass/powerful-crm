using Dapper;
using Microsoft.Extensions.Options;
using powerful_crm.Core.Models;
using powerful_crm.Core.Settings;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace powerful_crm.Data
{
    public class CityRepository : BaseRepository, ICityRepository
    {
        public CityRepository(IOptions<AppSettings> options) : base(options)
        {
            _connection = new SqlConnection(_connectionString);
        }
        public async Task<int> AddCityAsync(CityDto dto)
        {
            return await _connection.QuerySingleOrDefaultAsync<int>(
                "dbo.City_Add",
                param: new
                {
                    dto.Name
                },
                commandType: CommandType.StoredProcedure);
        }
        public async Task<int> DeleteCityAsync(int id)
        {
            return await _connection.ExecuteAsync(
                "dbo.City_Delete",
                param: new
                {
                    id
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<CityDto> GetCityByIdAsync(int id)
        {
            return await _connection.QueryFirstOrDefaultAsync<CityDto>(
                "dbo.City_SelectById",
                new { id },
                commandType: CommandType.StoredProcedure);
        }
    }
}
