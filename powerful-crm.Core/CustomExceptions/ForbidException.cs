using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace powerful_crm.Core.CustomExceptions
{
   public class ForbidException:CustomException
    {
        public ForbidException(string message)
        {
            ErrorMessage = message;
            StatusCode = StatusCodes.Status403Forbidden;
        }
    }
}
