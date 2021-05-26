using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace powerful_crm.Core.CustomExceptions
{
    public class PayPalException:CustomException
    {
        public PayPalException(string errorMessage) {
            StatusCode = (int)HttpStatusCode.BadRequest;
            ErrorMessage = errorMessage;
        }
    }
}
