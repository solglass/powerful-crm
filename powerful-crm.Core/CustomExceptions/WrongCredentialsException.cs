using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace powerful_crm.Core.CustomExceptions
{
    public class WrongCredentialsException:CustomException
    {
        public WrongCredentialsException()
        {
            StatusCode = (int)HttpStatusCode.Conflict;
            ErrorMessage = Constants.ERROR_WRONG_PASSWORD;
        }
    }
}
