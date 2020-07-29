// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using EnsureThat;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Task = System.Threading.Tasks.Task;

namespace Microsoft.Health.Fhir.R4.Functions.Features.Functions
{
    public class FhirFunctions
    {
        private readonly FhirRouter _router;

        public FhirFunctions(FhirRouter router)
        {
            EnsureArg.IsNotNull(router, nameof(router));

            _router = router;
        }

        [FunctionName("FhirRoutes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Functions function")]
        public async Task Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", "patch", "options", Route = "{*catchall}")]
            HttpRequest req)
        {
            EnsureArg.IsNotNull(req);
            await _router.RouteRequest(req.HttpContext);
        }
    }
}