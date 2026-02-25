using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;    

namespace LoanManagement.Plugins
{
    public class LoanPreCreateValidation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Get execution context
            IPluginExecutionContext context =
                (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Only proceed if target exists
            if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
            {
                Entity entity = (Entity)context.InputParameters["Target"];

                // Make sure this is Loan Application table
                if (entity.LogicalName != "lms_loanapplication")
                    return;

                // Check if credit score field exists
                if (entity.Contains("lms_creditscore"))
                {
                    int creditScore = entity.GetAttributeValue<int>("lms_creditscore");

                    if (creditScore < 600)
                    {
                        throw new InvalidPluginExecutionException(
                            "Loan cannot be created. Credit Score must be 600 or above.");
                    }
                }
            }
        }
    }
}
