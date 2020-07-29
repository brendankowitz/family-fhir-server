// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using Microsoft.Health.Fhir.Core.Models;

namespace Microsoft.Health.Fhir.R4.Functions.Features.Functions.RouteSegments
{
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