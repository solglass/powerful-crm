using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace powerful_crm.Core.CustomExceptions
{
    public class WrongCredentialsException:Exception
    {
        public int StatusCode { get; private set; }
        public string ErrorMessage { get; private set; }

        public WrongCredentialsException(string message)
        {
            StatusCode = (int)HttpStatusCode.Conflict;
            ErrorMessage = message;
        }
    }
}
