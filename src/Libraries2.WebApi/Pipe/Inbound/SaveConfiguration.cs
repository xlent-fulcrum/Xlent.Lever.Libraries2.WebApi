using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Context;
using Xlent.Lever.Libraries2.Core.MultiTenant.Context;
using Xlent.Lever.Libraries2.Core.MultiTenant.Model;
using Xlent.Lever.Libraries2.Core.Platform.Configurations;

namespace Xlent.Lever.Libraries2.WebApi.Pipe.Inbound
{
    /// <summary>
    /// Extracts organization and environment values from request uri and adds these values to an executioncontext. 
    /// These values are later used to get organization and environment specific configurations for logging and request handling. 
    /// </summary>
    public class SaveConfiguration : DelegatingHandler
    {
        private readonly ITenantConfigurationValueProvider _tenantConfigurationProvider;

        private static ILeverServiceConfiguration _serviceConfiguration;

        /// <summary></summary>
        [Obsolete("Use constructor with ValueProvider", true)]
        public SaveConfiguration(ITenantConfigurationValueProvider configurationValueProvider, ILeverServiceConfiguration serviceConfiguration) : this(configurationValueProvider.ValueProvider, serviceConfiguration)
        {
        }

        /// <summary></summary>
        public SaveConfiguration(IValueProvider valueProvider, ILeverServiceConfiguration serviceConfiguration)
        {
            _tenantConfigurationProvider = new TenantConfigurationValueProvider(valueProvider);
            _serviceConfiguration = serviceConfiguration;
        }

        /// <inheritdoc />
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var rgx = new Regex("/v[^/]+/([^/]+)/([^/]+)/");

            var match = rgx.Match(request.RequestUri.OriginalString);
            if (match.Success && match.Groups.Count == 3)
            {
                var organization = match.Groups[1].Value;
                var environment = match.Groups[2].Value;
                var tenant = new Tenant(organization, environment);
                _tenantConfigurationProvider.Tenant = tenant;
                try
                {
                    var configuration = await _serviceConfiguration.GetConfigurationForAsync(tenant);
                    _tenantConfigurationProvider.LeverConfiguration = configuration;
                }
                catch
                {
                    // Deliberately ignore errors for configuration. This will have to be taken care of when the configuration is needed.
                }
            }

            var result = await base.SendAsync(request, cancellationToken);
            return result;
        }

    }
}
