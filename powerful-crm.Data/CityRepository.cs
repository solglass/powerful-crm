using Dapper;
using Microsoft.Extensions.Options;
using powerful_crm.Core.Models;
using powerful_crm.Core.Settings;
using System.Data;
using System.Data.SqlClient;

namespace powerful_crm.Data
{
    public class CityRepository : BaseRepository, ICityRepository
    {
        public CityRepository(IOptions<AppSettings> options) : base(options)
        {
            _connection = new SqlConnection(_connectionString);
        }
        public int AddCity(CityDto dto)
        {
            return _connection.QuerySingleOrDefault<int>(
                "dbo.City_Add",
                param: new
                {
                    dto.Name
                },
                commandType: CommandType.StoredProcedure);
        }
        public int DeleteCity(int id)
        {
            return _connection.Execute(
                "dbo.City_Delete",
                param: new
                {
                    id
                },
                commandType: CommandType.StoredProcedure);
        }

        public CityDto GetCityById(int id)
        {
            return _connection.QueryFirstOrDefault<CityDto>(
                "dbo.City_SelectById",
                new { id },
                commandType: CommandType.StoredProcedure);
        }
    }
}
