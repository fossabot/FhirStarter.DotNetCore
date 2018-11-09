﻿/* 
 * Copyright (c) 2014, Furore (info@furore.com) and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the BSD 3-Clause license
 * available at https://raw.github.com/furore-fhir/spark/master/LICENSE
 */

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Hl7.Fhir.Model;

namespace FhirStarter.Bonfire.DotNetCore.SparkEngine.ExceptionHandling
{
    public class FhirErrorMessageHandler : DelegatingHandler
    {

        // the error handling has been changed, look into this
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response =  await base.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                //var content = response.Content as ObjectContent;
                var content = response.Content as ObjectContent;
                if (content != null && content.ObjectType == typeof (HttpError))
                {
                    //  var outcome = new OperationOutcome().AddError(response.ReasonPhrase);
                    var issue = new OperationOutcome.IssueComponent
                    {
                        Details = new CodeableConcept(nameof(HttpError), nameof(HttpError), response.ReasonPhrase)
                    };
                    var outcome = new OperationOutcome();
                    outcome.Issue.Add(issue);
                    
                    return request.CreateResponse(response.StatusCode, outcome);
                }
            }
            return response;
        }
    }
}
