// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Health.Fhir.Core.Messages.Get;

namespace Microsoft.Health.Fhir.R4.Functions.Features.Functions.FhirRoutes
{
    public class CapabilityStatementRoutes : IFhirRoute
    {
        private readonly IMediator _mediator;

        public CapabilityStatementRoutes(IMediator mediator)
        {
            _mediator = mediator;
        }

        public IReadOnlyCollection<(string template, string verb)> RouteTemplates { get; } = new List<(string template, string verb)>
        {
            ("metadata", "GET"),
        };

        public async Task<IActionResult> Execute(HttpRequest request, HttpResponse response, RouteData routeData)
        {
            var result = await _mediator.Send(new GetCapabilitiesRequest());

            return FunctionsFhirResult.Create(result.CapabilityStatement);
        }
    }
}