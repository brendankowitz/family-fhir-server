// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Health.Fhir.R4.Functions.Features.Functions
{
    public interface IFhirRestResult
    {
        object Model { get; }

        HttpStatusCode? StatusCode { get; }

        IDictionary<string, StringValues> Headers { get; }
    }
}