using System;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using powerful_crm.Core.Settings;
using System.Data;
using powerful_crm.Core.Models;
using System.Linq;
using System.Collections.Generic;
using SqlKata.Execution;
using SqlKata.Compilers;
using powerful_crm.Core.Enums;
using System.Threading.Tasks;
using powerful_crm.Core;

namespace powerful_crm.Data
{
    public class LeadRepository : BaseRepository, ILeadRepository
    {
        private readonly QueryFactory _db;
        private SqlServerCompiler _compiler;

        public LeadRepository(IOptions<AppSettings> options) : base(options)
        {
            _compiler = new SqlServerCompiler();
            _connection = new SqlConnection(_connectionString);
            _db = new QueryFactory(_connection, _compiler);
        }

        public async Task<int> AddUpdateLeadAsync(LeadDto dto)
        {
            return await _connection.QuerySingleOrDefaultAsync<int>(
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
                    dto.BirthDate,
                    roleId=(int)dto.Role
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteOrRecoverLeadAsync(int id, bool isDeleted)
        {
            return await _connection
                .ExecuteAsync("dbo.Lead_DeleteOrRecover",
                new
                {
                    id,
                    isDeleted
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> ChangePasswordLeadAsync(int id, string oldPassword, string newPassword)
        {
            return (await _connection
               .ExecuteAsync("dbo.Lead_ChangePassword", new
               {
                   id,
                   oldPassword,
                   newPassword
               },
               commandType: CommandType.StoredProcedure)) == Constants.EXPECTED_CHANGED_ROWS_COUNT;
        }

        public async Task<LeadDto> GetLeadByIdAsync(int id)
        {
            return (await _connection.QueryAsync<LeadDto, CityDto, LeadDto>(
                "dbo.Lead_SelectById", (lead, city) =>
                {
                    lead.City = city;
                    return lead;
                },
                new { id },
                splitOn: "Id",
                commandType: CommandType.StoredProcedure)).FirstOrDefault();
        }

        public async Task<LeadDto> GetLeadCredentialsAsync(int? id, string login)
        {
            return (await _connection.QueryAsync<LeadDto, int, LeadDto>(
                "dbo.Lead_GetCredentials",(lead, role)=>
                {
                    lead.Role = (Role)role;
                    return lead;
                }, 
                new { id, login },
                splitOn: "Id",
                commandType: CommandType.StoredProcedure)).FirstOrDefault();
        }

        public async Task<int> UpdateLeadRoleAsync(int leadId, int roleId)
        {
            return await _connection.ExecuteAsync("dbo.Lead_UpdateRole", new
            {
                leadId,
                roleId
            },
               commandType: CommandType.StoredProcedure);
        }
        public async Task<List<LeadDto>> SearchLeadsAsync(SearchLeadDto leadDto)
        {
            if (leadDto.FirstName == null && leadDto.LastName == null && leadDto.Email == null && leadDto.Login == null
                && leadDto.Phone == null && leadDto.StartBirthDate == null && leadDto.City.Name == null)
                throw new ArgumentNullException();

            var query =  _db.Query("Lead as l").Join("City as c", "c.Id","l.CityId").Select("l.Id",
                                                "l.FirstName",
                                                "l.LastName",
                                                "l.Login",
                                                "l.Email",
                                                "l.Phone",
                                                "l.BirthDate",
                                                "c.Id",
                                                "c.Name").Where("IsDeleted", "0");
            if(leadDto.FirstName != null)
            {
                query = query.WhereLike("l.FirstName",leadDto.FirstName);
            }
            if (leadDto.LastName != null)
            {
                query = query.WhereLike("l.LastName",leadDto.LastName);
            }
            if (leadDto.Email != null)
            {
                query = query.WhereLike("l.Email", leadDto.Email);
            }
            if (leadDto.Login != null)
            {
                query = query.WhereLike("l.Login",leadDto.Login);
            }
            if (leadDto.Phone != null)
            {
                query = query.WhereLike("l.Phone", leadDto.Phone);
            }
            if (leadDto.City.Name != null)
            {
                query = query.WhereLike("c.Name", leadDto.City.Name);
            }
            if (leadDto.StartBirthDate != null && leadDto.EndBirthDate !=null)
            {
                query = query.WhereBetween("l.BirthDate",leadDto.StartBirthDate, leadDto.EndBirthDate);
            }
            if (leadDto.StartBirthDate != null && leadDto.EndBirthDate == null)
            {
                query = query.WhereDate("l.BirthDate", ">",leadDto.StartBirthDate);
            }
            if (leadDto.StartBirthDate == null && leadDto.EndBirthDate != null)
            {
                query = query.WhereDate("l.BirthDate", "<", leadDto.EndBirthDate);
            }

            var sql =  _compiler.Compile(query).ToString();
            return (await _connection.QueryAsync<LeadDto, CityDto, LeadDto>(
                sql, (lead, city) =>
                {
                    lead.City = city;
                    return lead;
                }, splitOn: "Id"))
                .Distinct()
                .ToList();
        }
       
    }
}
