// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Primitives;
using Microsoft.Health.Fhir.Core.Models;

namespace Microsoft.Health.Fhir.R4.Functions.Features.Functions.RouteSegments
{
    public abstract class RouteSegmentBase
    {
        public abstract bool IsMatch(string inputSegment, out IDictionary<string, StringValues> routeData);
    }

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

    public class ResourceTypeSegment : RouteSegmentBase
    {
        private readonly IModelInfoProvider _modelInfoProvider;

        public ResourceTypeSegment(IModelInfoProvider modelInfoProvider)
        {
            _modelInfoProvider = modelInfoProvider;
        }

        public override bool IsMatch(string inputSegment, out IDictionary<string, StringValues> routeData)
        {
            if (!_modelInfoProvider.IsKnownResource(inputSegment))
            {
                routeData = new Dictionary<string, StringValues>
                {
                    { "resourceType", inputSegment },
                };

                return true;
            }

            routeData = null;
            return false;
        }
    }
}