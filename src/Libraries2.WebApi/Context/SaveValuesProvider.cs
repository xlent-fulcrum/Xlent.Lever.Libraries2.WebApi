using Xlent.Lever.Libraries2.Core.Context;
using Xlent.Lever.Libraries2.Core.MultiTenant.Context;
using Xlent.Lever.Libraries2.Core.MultiTenant.Model;
using Xlent.Lever.Libraries2.Core.Platform.Configurations;

namespace Xlent.Lever.Libraries2.WebApi.Context
{
    /// <summary>
    /// Stores values in the execution context which is unaffected by asynchronous code that switches threads or context. 
    /// </summary>
    /// <remarks>Updating values in a thread will not affect the value in parent/sibling threads</remarks>
    public class SaveValuesProvider : ITenantConfigurationValueProvider
    {
        private const string CorrelationIdKey = "FulcrumCorrelationId";
        private const string TenantIdKey = "TenantId";
        private const string LeverConfigurationIdKey = "LeverConfigurationId";
        private const string CallingClientNameKey = "CallingClientName";

        /// <summary></summary>
        /// <param name="valueProvider">The <see cref="IValueProvider"/> to use as a getter and setter.</param>
        // ReSharper disable once SuggestBaseTypeForParameter
        public SaveValuesProvider(HttpContextValueProvider valueProvider)
        {
            ValueProvider = valueProvider;
        }

        /// <summary>
        /// An instance that uses <see cref="HttpContextValueProvider"/> as a getter and setter.
        /// </summary>
        public static SaveValuesProvider Instance { get; } = new SaveValuesProvider(new HttpContextValueProvider());

        /// <inheritdoc />
        public IValueProvider ValueProvider { get; }

        /// <inheritdoc />
        public string CorrelationId
        {
            get => ValueProvider.GetValue<string>(CorrelationIdKey);
            set => ValueProvider.SetValue(CorrelationIdKey, value);
        }

        /// <inheritdoc />
        public ITenant Tenant
        {
            get => ValueProvider.GetValue<Tenant>(TenantIdKey);
            set => ValueProvider.SetValue(TenantIdKey, value);
        }

        /// <inheritdoc />
        public ILeverConfiguration LeverConfiguration
        {
            get => ValueProvider.GetValue<ILeverConfiguration>(LeverConfigurationIdKey);
            set => ValueProvider.SetValue(LeverConfigurationIdKey, value);
        }

        /// <inheritdoc />
        public string CallingClientName
        {
            get => ValueProvider.GetValue<string>(CallingClientNameKey);
            set => ValueProvider.SetValue(CallingClientNameKey, value);
        }
    }
}
