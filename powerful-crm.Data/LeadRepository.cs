using System;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using powerful_crm.Core.Settings;
using System.Data;
using powerful_crm.Core.Models;
using System.Linq;

namespace powerful_crm.Data
{
    public class LeadRepository : BaseRepository, ILeadRepository
    {
        public LeadRepository(IOptions<AppSettings> options) : base(options)
        {
            _connection = new SqlConnection(_connectionString);
        }

        public int AddUpdateLead(LeadDto dto)
        {
            return _connection.QuerySingleOrDefault<int>(
                "dbo.Lead_AddUpdate",
                param: new
                {
                    dto.Id,
                    dto.FirstName,
                    dto.LastName,
                    dto.Login,
                    dto.Password,
                    dto.Email,
                    dto.Phone,
                    CityId = dto.City.Id,
                    dto.BirthDate
                },
                commandType: CommandType.StoredProcedure);
        }

        public int DeleteOrRecoverLead(int id, bool isDeleted)
        {
            return _connection
                .Execute("dbo.Lead_DeleteOrRecover",
                new
                {
                    id,
                    isDeleted
                },
                commandType: CommandType.StoredProcedure);
        }

        public int ChangePasswordLead(int id, string oldPassword, string newPassword)
        {
            return _connection
               .Execute("dbo.Lead_ChangePassword", new
               {
                   id,
                   oldPassword,
                   newPassword
               },
               commandType: CommandType.StoredProcedure);
        }

        public LeadDto GetLeadById(int id)
        {
            return _connection.Query<LeadDto, CityDto, LeadDto>(
                "dbo.Lead_SelectById", (lead, city) =>
                {
                    lead.City = city;
                    return lead;
                },
                new { id },
                splitOn: "Id",
                commandType: CommandType.StoredProcedure).FirstOrDefault();
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

        public CityDto GetCityById (int id)
        {
            return _connection.QueryFirstOrDefault<CityDto>("dbo.City_SelectById",
                new { id },
                commandType: CommandType.StoredProcedure);
        }
    }
}
