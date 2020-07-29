// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Health.Fhir.R4.Functions.Features.Functions.RouteSegments
{
    public class TemplateRouteSegment : RouteSegmentBase
    {
        private readonly string _routeDataName;
        private readonly string _regexConstraint;

        public TemplateRouteSegment(string routeDataName, string regexConstraint = null)
        {
            _routeDataName = routeDataName;
            _regexConstraint = regexConstraint;
        }

        public override bool IsMatch(string inputSegment, out IDictionary<string, StringValues> routeData)
        {
            if (!string.IsNullOrEmpty(inputSegment) &&
                (string.IsNullOrEmpty(_regexConstraint) || Regex.IsMatch(inputSegment, _regexConstraint)))
            {
                routeData = new Dictionary<string, StringValues>
                {
                    { _routeDataName, inputSegment },
                };

                return true;
            }

            routeData = null;
            return false;
        }
    }
}