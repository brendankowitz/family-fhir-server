// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using EnsureThat;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Health.Fhir.Core.Extensions;
using Microsoft.Health.Fhir.Core.Messages.Get;
using Microsoft.Health.Fhir.Core.Models;
using Newtonsoft.Json;
using Task = System.Threading.Tasks.Task;

namespace Microsoft.Health.Fhir.R4.Functions.Features.Functions
{
    public class FhirFunctions
    {
        private readonly IMediator _mediator;
        private readonly FhirJsonParser _parser;

        public FhirFunctions(IMediator mediator, FhirJsonParser parser)
        {
            EnsureArg.IsNotNull(mediator, nameof(mediator));
            EnsureArg.IsNotNull(parser, nameof(parser));

            _mediator = mediator;
            _parser = parser;
        }

        [FunctionName("metadata")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Functions function")]
        public async Task<IActionResult> Metadata(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req)
        {
            EnsureArg.IsNotNull(req);

            var response = await _mediator.Send(new GetCapabilitiesRequest());

            return FunctionsFhirResult.Create(response.CapabilityStatement);
        }

        [FunctionName("FhirPostRoutes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Functions function")]
        public Task<IActionResult> Create(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "{*catchall}")]
            HttpRequest req)
        {
            EnsureArg.IsNotNull(req);

            using var streamReader = new StreamReader(req.Body);
            using var jsonReader = new JsonTextReader(streamReader);
            var resource = _parser.Parse<Resource>(jsonReader);

            var outcomeIssue = new OperationOutcomeIssue(
                OperationOutcome.IssueSeverity.Information.ToString(),
                OperationOutcome.IssueType.NotSupported.ToString(),
                $"This action isn't supported yet for {resource.TypeName} :)");

            var outcome = new OperationOutcome
            {
                Issue = new List<OperationOutcome.IssueComponent>
                {
                    outcomeIssue.ToPoco(),
                },
            };

            return Task.FromResult<IActionResult>(FunctionsFhirResult.Create(outcome.ToResourceElement()));
        }

        [FunctionName("FhirGetRoutes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Functions function")]
        public Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{*catchall}")]
            HttpRequest req)
        {
            EnsureArg.IsNotNull(req);

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

            return Task.FromResult<IActionResult>(FunctionsFhirResult.Create(outcome.ToResourceElement()));
        }
    }
}