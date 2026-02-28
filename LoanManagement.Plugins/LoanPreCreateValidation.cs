using System;
using Microsoft.Xrm.Sdk;

namespace LoanManagement.Plugins
{
    public class LoanPreCreateValidation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context =
                (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Only proceed if Target exists
            if (!context.InputParameters.Contains("Target") ||
                !(context.InputParameters["Target"] is Entity))
                return;

            Entity entity = (Entity)context.InputParameters["Target"];

            // Ensure correct entity
            if (entity.LogicalName != "lms_loanapplication")
                return;

            int creditScore = 0;

            // 1️- Try to get CreditScore from Target (Create / Update)
            if (entity.Contains("lms_creditscore"))
            {
                creditScore = entity.GetAttributeValue<int>("lms_creditscore");
            }
            // 2️- If not in Target, get from PreImage (for Update scenarios)
            else if (context.PreEntityImages.Contains("PreImage"))
            {
                Entity preImage = context.PreEntityImages["PreImage"];

                if (preImage.Contains("lms_creditscore"))
                {
                    creditScore = preImage.GetAttributeValue<int>("lms_creditscore");
                }
            }

            // 🔒 Validation Rule
            if (creditScore < 600)
            {
                throw new InvalidPluginExecutionException(
                    "Loan cannot be created. Credit Score must be 600 or above.");
            }

            // 📊 Risk Level Calculation
            string riskLevel = "Low";

            if (creditScore < 650)
                riskLevel = "High";
            else if (creditScore <= 750)
                riskLevel = "Medium";

            // 🔁 Store in SharedVariables
            context.SharedVariables["RiskLevel"] = riskLevel;
            tracingService.Trace("SharedVariable 'RiskLevel' set to: {0}", riskLevel);
        }
    }
}