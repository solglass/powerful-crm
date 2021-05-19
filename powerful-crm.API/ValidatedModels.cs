using powerful_crm.API.Models.InputModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace powerful_crm.API
{
    public class ValidatedModels
    {
        public Dictionary<int, TransactionInputModel> ValidModels { get; set; }
        private static ValidatedModels _instance;

        private ValidatedModels()
        {
            ValidModels = new Dictionary<int, TransactionInputModel>();
        }

        public static ValidatedModels GetValidatedModelInstance()
        {
            if (_instance == null)
            {
                _instance = new ValidatedModels();
            }
            return _instance;
        }
    }
}
