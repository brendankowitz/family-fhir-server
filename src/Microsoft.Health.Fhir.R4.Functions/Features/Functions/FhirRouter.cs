// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Hl7.Fhir.Model;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Health.Fhir.Core.Extensions;
using Microsoft.Health.Fhir.Core.Models;
using Task = System.Threading.Tasks.Task;

namespace Microsoft.Health.Fhir.R4.Functions.Features.Functions
{
    public sealed class FhirRouter
    {
        private readonly IMediator _mediator;
        private readonly IRouter _router;

        public FhirRouter(IServiceProvider provider, IMediator mediator)
        {
            _mediator = mediator;

            var routeBuilder = new RouteBuilder(new ApplicationBuilder(provider));

            foreach (var route in provider.GetServices<IFhirRoute>().OrderBy(x => x is IOrderedFhirRoute ordered ? ordered.Order : 100))
            {
                var currentRoute = route;
                foreach (var template in route.RouteTemplates)
                {
                    routeBuilder.Routes.Add(
                        new Route(
                            new RouteHandler(context => ExecuteRoute(currentRoute, context.Request, context.Response, new RouteData())),
                            Guid.NewGuid().ToString(),
                            $"api/{template.template}",
                            new RouteValueDictionary(),
                            new RouteValueDictionary(new
                            {
                                httpVerb = new HttpMethodRouteConstraint(template.verb),
                            }),
                            new RouteValueDictionary(),
                            provider.GetRequiredService<IInlineConstraintResolver>()));
                }
            }

            _router = routeBuilder.Build();
        }

        public async Task RouteRequest(HttpContext context)
        {
            var routeContext = new RouteContext(context);

            await _router.RouteAsync(routeContext);

            if (routeContext.Handler != null)
            {
                await routeContext.Handler.Invoke(context);
            }
            else
            {
                await NotFound(context);
            }
        }

        private async Task ExecuteRoute(IFhirRoute route, HttpRequest httpRequest, HttpResponse httpResponse, RouteData routeData)
        {
            var result = await route.Execute(httpRequest, httpResponse, routeData);
            await result.ExecuteResultAsync(new ActionContext(httpRequest.HttpContext, routeData, new ActionDescriptor()));
        }

        private async Task NotFound(HttpContext context)
        {
            var outcomeIssue = new OperationOutcomeIssue(
                OperationOutcome.IssueSeverity.Information.ToString(),
                OperationOutcome.IssueType.NotSupported.ToString(),
                "This action isn't supported yet :)");

            var outcome = new OperationOutcome
            {
                Issue = new List<OperationOutcome.IssueComponent>
                {
                    outcomeIssue.ToPoco(),
                },
            };

            await FunctionsFhirResult.Create(outcome.ToResourceElement(), HttpStatusCode.NotFound)
                .ExecuteResultAsync(new ActionContext(context, new RouteData(), new ActionDescriptor()));
        }
    }
}