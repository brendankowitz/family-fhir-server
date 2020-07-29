// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Health.Fhir.Api.Features.Headers;
using Microsoft.Health.Fhir.Core.Extensions;
using Microsoft.Health.Fhir.Core.Features.Routing;
using Microsoft.Health.Fhir.Core.Models;

namespace Microsoft.Health.Fhir.R4.Functions.Features.Functions.FhirRoutes
{
    public class ResourceRoutes : IFhirRoute
    {
        private readonly IMediator _mediator;
        private readonly IQueryStringParser _queryStringParser;

        public ResourceRoutes(
            IMediator mediator,
            IQueryStringParser queryStringParser)
        {
            _mediator = mediator;
            _queryStringParser = queryStringParser;
        }

        public IReadOnlyCollection<(string template, string verb)> RouteTemplates { get; } = new List<(string template, string verb)>
        {
            // ("{model:fhirResource}", "get"),
            ("Observation", "get"),
            ("Observation/{id}", "get"),
        };

        public async Task<IActionResult> Execute(HttpRequest request, HttpResponse response, RouteData routeData)
        {
            var resourceType = routeData.Values["resourceTypeName"] as string;
            var queries = request.Query.Select(x => Tuple.Create(x.Key, string.Join(",", x.Value.ToArray())))
                .ToList();

            var result = await _mediator.SearchResourceAsync(resourceType, queries);

            return FunctionsFhirResult.Create(result);
        }
    }
}