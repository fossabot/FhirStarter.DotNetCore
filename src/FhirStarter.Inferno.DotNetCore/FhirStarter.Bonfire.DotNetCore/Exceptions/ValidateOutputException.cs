﻿using System;

namespace FhirStarter.Bonfire.DotNetCore.Exceptions
{
    public class ValidateOutputException: ArgumentException
    {

        public ValidateOutputException(string message) : base(message)
        {
        }

        public ValidateOutputException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
