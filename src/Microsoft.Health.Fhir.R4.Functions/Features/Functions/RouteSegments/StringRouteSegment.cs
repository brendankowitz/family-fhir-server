// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Health.Fhir.R4.Functions.Features.Functions.RouteSegments
{
    public class StringRouteSegment : RouteSegmentBase
    {
        private readonly string _segment;

        public StringRouteSegment(string segment)
        {
            _segment = segment;
        }

        public override bool IsMatch(string inputSegment, out IDictionary<string, StringValues> routeData)
        {
            routeData = null;

            return string.Equals(_segment, inputSegment, StringComparison.OrdinalIgnoreCase);
        }
    }
}