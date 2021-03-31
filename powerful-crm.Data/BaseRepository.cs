using Microsoft.Extensions.Options;
using powerful_crm.Core.Settings;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace powerful_crm.Data
{
    public abstract class BaseRepository
    {
        protected SqlConnection _connection;
        protected string _connectionString;
        public BaseRepository(IOptions<AppSettings> options)
        {
            _connectionString = options.Value.CONNECTION_STRING;
        }
    }
}
