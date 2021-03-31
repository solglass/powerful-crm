using System;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using powerful_crm.Core.Settings;
using powerful_crm.Data.Models;
using System.Data;

namespace powerful_crm.Data
{
    public class LeadRepository : BaseRepository, ILeadRepository
    {
        public LeadRepository(IOptions<AppSettings> options) : base(options)
        {
            _connection = new SqlConnection(_connectionString);
        }

        public int AddLead(LeadDto dto)
        {
            return _connection.QuerySingle<int>(
                "dbo.Lead_Add",
                param: new
                {
                    dto.FirstName,
                    dto.LastName,
                    dto.Login,
                    dto.Password,
                    dto.Email,
                    dto.Phone,
                    dto.BirthDate
                },
                commandType: CommandType.StoredProcedure);
        }

        public int UpdateLead(LeadDto dto)
        {
            return _connection.Execute(
                "dbo.Lead_Update",
                param: new
                {
                    dto.Id,
                    dto.FirstName,
                    dto.LastName,
                    dto.Email,
                    dto.Phone,
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

        public int HardDeleteLead(int id)
        {
            return _connection
                .Execute("dbo.Lead_HardDelete",
                new { id },
                commandType: System.Data.CommandType.StoredProcedure);
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
               commandType: System.Data.CommandType.StoredProcedure);
        }

        public LeadDto GetLeadById(int id)
        {
            return _connection.QueryFirstOrDefault<LeadDto>(
                "dbo.Lead_SelectById",
                param: new
                {
                    id
                },
                commandType: CommandType.StoredProcedure);
        }
    }
}
