using System.Net;

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
