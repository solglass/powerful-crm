using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace powerful_crm.API.Models.OutputModels
{
    public class CustomExceptionOutputModel
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }
}
