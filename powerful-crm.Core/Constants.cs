using System;
using System.Collections.Generic;
using System.Text;

namespace powerful_crm.Core
{
    public static class Constants
    {
        public const string DATE_FORMAT = "dd.MM.yyyy";
        public const string DATE_FORMAT_WITH_TIME = "dd.MM.yyyy HH:mm:ss";

        public const string LOGIN_UNIQUE_CONSTRAINT = "UQLead5E55825B7B2276C4";
        public const string EMAIL_UNIQUE_CONSTRAINT = "UQLeadA9D10534BF185160";

        public const string API_GET_TRANSACTION = "/api/Transaction/{0}";
        public const string API_GET_BALANCE = "/api/Transaction/balance/{0}";
        public const string API_DEPOSIT = "/api/Transaction/deposite";
        public const string API_WITHDRAW = "/api/Transaction/withdraw";
        public const string API_TRANSFER = "/api/Transaction/transfer";

        public const string ERROR_LEAD_NOT_FOUND_BY_ID = "Lead with id {0} is not found";
        public const string ERROR_LEAD_NOT_FOUND_BY_LOGIN = "Lead with login {0} is not found";
        public const string ERROR_LEAD_ALREADY_DELETED = "Lead with id {0} has already been deleted";
        public const string ERROR_LEAD_NOT_DELETED = "Lead with id {0} is not deleted";
        public const string ERROR_CITY_NOT_FOUND = "City with id {0} is not found";
        public const string ERROR_CITY_HAS_DEPENDENCIES = "The city with id {0} can't be deleted because there are some accounts connected with it";
        public const string ERROR_WRONG_PASSWORD = "Entered password doesn't match current password!";
        public const string ERROR_NOT_UNIQUE_EMAIL = "This email is already in use.";
        public const string ERROR_NOT_UNIQUE_LOGIN = "This login is already in use.";
        public const string ERROR_NOT_ALLOWED_ACTIONS_WITH_OTHER_LEAD = "There is no permission for that action with someone else's data.";
        public const string ERROR_NOT_ALLOWED_ACTIONS_WITH_CITY = "There is no permission for that action with city information.";
        public const string GLOBAL_ERROR_MESSAGE = "An error occured while processing the request.";

    }
}
