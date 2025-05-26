using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Saras.eMarking.Domain.Entities;
using System; 
using System.Threading.Tasks;

namespace Saras.eMarking.Api.Common.Filters
{
    public class AntiforgeryFilterProvider : IFilterProvider
    {
        private readonly AppOptions appOptions;
        public AntiforgeryFilterProvider(AppOptions _appOptions)
        {
            appOptions = _appOptions;
        }
        public int Order => 999;
        public void OnProvidersExecuted(FilterProviderContext context)
        {
            // Method intentionally left empty.
        }
        public void OnProvidersExecuting(FilterProviderContext context)
        {
            if (context == null) { throw new ArgumentNullException(nameof(context)); }
            if (context.ActionContext.ActionDescriptor.FilterDescriptors != null && !appOptions.AppSettings.IsCsrfValidationEnabled)
            {
                var FilterDescriptor = new FilterDescriptor(SkipAntiforgeryPolicy.Instance, FilterScope.Last);
                var filterItem = new FilterItem(FilterDescriptor, SkipAntiforgeryPolicy.Instance);
                context.Results.Add(filterItem);
            }
        }

        // a dummy IAntiforgeryPolicy
        class SkipAntiforgeryPolicy : IAntiforgeryPolicy, IAsyncAuthorizationFilter
        {
            public readonly static SkipAntiforgeryPolicy Instance = new();
            public Task OnAuthorizationAsync(AuthorizationFilterContext context) => Task.CompletedTask;
        }
    }
}
