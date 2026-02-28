using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace LoanManagement.Plugins
{
    public class LoanEmiCalculation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            IOrganizationService service = factory.CreateOrganizationService(context.UserId);

            if (context.Depth > 3) return; // prevent infinite loop

            if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
            {
                Entity entity = (Entity)context.InputParameters["Target"];

                if (entity.LogicalName != "lms_loanapplication")
                    return;

                if (!context.PostEntityImages.Contains("PostImage"))
                    return;

                Entity loan = context.PostEntityImages["PostImage"];

                decimal principal = loan.GetAttributeValue<Money>("lms_loanamount")?.Value ?? 0;
                decimal rate = loan.GetAttributeValue<decimal?>("lms_interestrate") ?? 0;
                int tenure = loan.GetAttributeValue<int?>("lms_tenuremonths") ?? 0;

                if (principal == 0 || rate == 0 || tenure == 0)
                    return;

                double monthlyRate = (double)(rate / 12 / 100);
                double emi = (double)principal *
                             monthlyRate *
                             Math.Pow(1 + monthlyRate, tenure) /
                             (Math.Pow(1 + monthlyRate, tenure) - 1);

                Entity updateLoan = new Entity(entity.LogicalName, entity.Id);
                updateLoan["lms_emi"] = new Money(Convert.ToDecimal(emi));

                service.Update(updateLoan);
            }
        }
    }
}