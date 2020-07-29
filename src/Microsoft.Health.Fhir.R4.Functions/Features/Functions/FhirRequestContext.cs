// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using Microsoft.Health.Fhir.Core.Models;

namespace Microsoft.Health.Fhir.R4.Functions.Features.Functions
{
    public class FhirRequestContext
    {
        public Uri Uri { get; set; }

        public IDictionary<string, StringValues> Headers { get; } = new Dictionary<string, StringValues>();

        public ResourceElement Resource { get; set; }
    }
}