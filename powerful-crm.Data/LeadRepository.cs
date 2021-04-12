using System;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using powerful_crm.Core.Settings;
using System.Data;
using powerful_crm.Core.Models;
using System.Linq;
using System.Collections.Generic;
using SqlKata;
using SqlKata.Execution;
using SqlKata.Compilers;

namespace powerful_crm.Data
{
    public class LeadRepository : BaseRepository, ILeadRepository
    {
        private readonly QueryFactory db;
        private SqlServerCompiler _compiler;

        public LeadRepository(IOptions<AppSettings> options) : base(options)
        {
            _compiler = new SqlServerCompiler();
            _connection = new SqlConnection(_connectionString);
            db = new QueryFactory(_connection, _compiler);
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
        public List<LeadDto> SearchLeads(SearchLeadDto leadDto)
        {
            if (leadDto.FirstName == null && leadDto.LastName == null && leadDto.Email == null && leadDto.Login == null
                && leadDto.Phone == null && leadDto.StartBirthDate == null && leadDto.City == null)
                throw new ArgumentNullException();

            var query = db.Query("Lead as l").Join("City as c", "c.Id","l.CityId").Select("l.Id",
                                                "l.FirstName",
                                                "l.LastName",
                                                "l.Login",
                                                "l.Email",
                                                "l.Phone",
                                                "l.BirthDate",
                                                "c.Id",
                                                "c.Name").Where("IsDeleted", "=", 0);
            if(leadDto.FirstName != null)
            {
                query = query.Where("FirstName", "like", leadDto.FirstName);
            }
            if (leadDto.LastName != null)
            {
                query = query.Where("LastName", "like", leadDto.LastName);
            }
            if (leadDto.Email != null)
            {
                query = query.Where("Email", "like", leadDto.Email);
            }
            if (leadDto.Login != null)
            {
                query = query.Where("Login", "like", leadDto.Login);
            }
            if (leadDto.Phone != null)
            {
                query = query.Where("Phone", "like", leadDto.Phone);
            }
            var sql = _compiler.Compile(query).ToString();
            return _connection.Query<LeadDto, CityDto, LeadDto>(
                sql, (lead, city) =>
                {
                    lead.City = city;
                    return lead;
                }, splitOn: "Id")
                .Distinct()
                .ToList();
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
            return _connection.QueryFirstOrDefault<CityDto>("dbo.City_SelectById",
                new { id },
                commandType: CommandType.StoredProcedure);
        }
    }
}
