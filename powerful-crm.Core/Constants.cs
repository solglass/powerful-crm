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
        public const string CURRENCY_UNIQUE_CONSTRAINT = "UQAccount5E55825B7B2276C4";

        public const string API_GETTRANSACTION = "/api/Transaction/{0}";
        public const string API_GETBALANCE = "/api/Transaction/balance/{0}";
        public const string API_DEPOSIT = "/api/Transaction/deposite";
        public const string API_WITHDRAW = "/api/Transaction/withdraw";
        public const string API_TRANSFER = "/api/Transaction/transfer";

        public const string ERROR_LEADNOTFOUND = "Lead with id {0} is not found";
        public const string ERROR_ACCOUNTNOTFOUND = "Account with id {0} is not found";
        public const string ERROR_ACCOUNTALREADYDELETED = "Account with id {0} has already been deleted";
        public const string ERROR_LEADALREADYDELETED = "Lead with id {0} has already been deleted";
        public const string ERROR_LEADNOTDELETED = "Lead with id {0} is not deleted";
        public const string ERROR_ACCOUNTNOTDELETED = "Account with id {0} is not deleted";
        public const string ERROR_CITYNOTFOUND = "City with id {0} is not found";
        public const string ERROR_CITYHASDEPENDENCIES = "The city with id {0} can't be deleted because there are some accounts connected with it";
        public const string ERROR_WRONG_PASSWORD = "Entered password doesn't match current password!";
        public const string ERROR_NOT_UNIQUE_EMAIL = "This email is already in use.";
        public const string ERROR_NOT_UNIQUE_LOGIN = "This login is already in use.";
        public const string ERROR_NOT_UNIQUE_CURRENCY = "This currency is already in use.";
        public const string GLOBAL_ERROR_MESSAGE = "An error occured while processing the request.";

    }
}
