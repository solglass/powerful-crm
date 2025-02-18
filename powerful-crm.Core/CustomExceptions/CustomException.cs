﻿using System;

namespace powerful_crm.Core.CustomExceptions
{
   public abstract class CustomException : Exception
    {
        public int StatusCode { get; protected set; }
        public string ErrorMessage { get; protected set; }
    }
}
