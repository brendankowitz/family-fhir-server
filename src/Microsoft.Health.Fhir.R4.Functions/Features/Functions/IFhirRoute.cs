// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.Health.Fhir.R4.Functions.Features.Functions
{
    public interface IFhirRoute
    {
        IReadOnlyCollection<(string template, string verb)> RouteTemplates { get; }

        Task<IFhirRestResult> Execute(HttpRequestMessage request, IDictionary<string, object> routeData);
    }
}