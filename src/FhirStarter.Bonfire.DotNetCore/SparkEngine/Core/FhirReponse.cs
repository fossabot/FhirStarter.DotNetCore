﻿/* 
 * Copyright (c) 2018, Firely (info@furore.com) and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the BSD 3-Clause license
 * available at https://raw.github.com/furore-fhir/spark/master/LICENSE
 */


using System.Net;
using Hl7.Fhir.Model;

namespace FhirStarter.Bonfire.DotNetCore.SparkEngine.Core
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
