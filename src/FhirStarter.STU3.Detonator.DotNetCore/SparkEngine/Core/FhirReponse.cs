﻿using System.Net;
using Hl7.Fhir.Model;

namespace FhirStarter.STU3.Detonator.DotNetCore.SparkEngine.Core
{
    public class FhirResponse
    {
        public HttpStatusCode StatusCode;
        public IKey Key;
        public Resource Resource;

        public FhirResponse(HttpStatusCode code)
        {
            StatusCode = code;
            Key = null;
            Resource = null;
        }

        public bool HasBody => Resource != null;
    }
}
