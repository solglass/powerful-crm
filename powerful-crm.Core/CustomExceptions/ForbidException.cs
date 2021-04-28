using Microsoft.AspNetCore.Http;

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
