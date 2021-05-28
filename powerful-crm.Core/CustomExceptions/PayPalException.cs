using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace powerful_crm.Core.CustomExceptions
{
    public class PayPalException:CustomException
    {
        public PayPalException(string errorMessage) {
            StatusCode = (int)HttpStatusCode.BadGateway;
            ErrorMessage =$"{ string.Format(Constants.ERROR_PAYPAL_SERVICE_ERROR, DateTime.Now)}  {errorMessage}";
        }
    }
}
