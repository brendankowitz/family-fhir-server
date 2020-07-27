// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.Health.Extensions.DependencyInjection;
using Microsoft.Health.Fhir.Api.Configs;
using Microsoft.Health.Fhir.Api.Features.Routing;
using Microsoft.Health.Fhir.Api.Modules;
using Microsoft.Health.Fhir.Core.Features.Routing;
using Microsoft.Health.Fhir.R4.Functions.Features.IoC;
using Microsoft.Health.Fhir.R4.Functions.Features.Routing;
using Microsoft.Health.Fhir.R4.Functions.Modules;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Microsoft.Health.Fhir.R4.Functions.Modules
{
    public class Startup : FunctionsStartup
    {
        private const string FhirServerConfigurationSectionName = "FhirServer";

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;

            /* Hack to get IConfiguration */
            var serviceProvider = builder.Services.BuildServiceProvider();
            var configurationRoot = serviceProvider.GetService<IConfiguration>();

            services.AddOptions();
            var fhirServerConfiguration = new FhirServerConfiguration();

            configurationRoot?.GetSection(FhirServerConfigurationSectionName).Bind(fhirServerConfiguration);

            services.AddSingleton(Options.Create(fhirServerConfiguration));
            services.AddSingleton(Options.Create(fhirServerConfiguration.Security));
            services.AddSingleton(Options.Create(fhirServerConfiguration.Features));
            services.AddSingleton(Options.Create(fhirServerConfiguration.CoreFeatures));
            services.AddSingleton(Options.Create(fhirServerConfiguration.Cors));
            services.AddSingleton(Options.Create(fhirServerConfiguration.Operations));
            services.AddSingleton(Options.Create(fhirServerConfiguration.Operations.Export));
            services.AddSingleton(Options.Create(fhirServerConfiguration.Operations.Reindex));
            services.AddSingleton(Options.Create(fhirServerConfiguration.Audit));
            services.AddSingleton(Options.Create(fhirServerConfiguration.Bundle));
            services.AddSingleton(Options.Create(fhirServerConfiguration.Throttling));

            new FhirModule(fhirServerConfiguration).Load(services);
            services.RemoveAll(typeof(IScoped<>));
            services.AddTransient(typeof(IScoped<>), typeof(FunctionsScoped<>));

            new MediationModule().Load(services);
            new PersistenceModule().Load(services);
            new SearchModule(fhirServerConfiguration).Load(services);
            new ValidationModule().Load(services);

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.Add<QueryStringParser>().Singleton().AsService<IQueryStringParser>();
            services.Add<FunctionsUrlResolver>().Singleton().ReplaceService<IUrlResolver>();

            services.AddHttpClient();

            services.Add<StartupExtension>().Transient().AsImplementedInterfaces();
        }
    }
}