using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace LoanManagement.Plugins
{
    public class LoanApprovalCreation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context =
                (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory factory =
                (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            IOrganizationService service =
                factory.CreateOrganizationService(context.UserId);

            ITracingService tracingService =
                (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // 1. Declare the variable ONCE at the start
            string riskLevel = "Low";

            // 2. Assign value from SharedVariables if it exists
            if (context.SharedVariables.Contains("RiskLevel"))
            {
                // Note: We do NOT use 'string' here again
                riskLevel = context.SharedVariables["RiskLevel"].ToString();
                tracingService.Trace("Successfully retrieved SharedVariable 'RiskLevel': {0}", riskLevel);
            }
            else
            {
                tracingService.Trace("SharedVariable 'RiskLevel' was NOT found in this context.");
            }

            // Prevent infinite loop
            if (context.Depth > 3)
                return;

            // Ensure Post Image exists
            if (!context.PostEntityImages.Contains("PostImage"))
                return;

            Entity loan = context.PostEntityImages["PostImage"];

            // Get Loan Amount
            decimal loanAmount =
                loan.GetAttributeValue<Money>("lms_loanamount")?.Value ?? 0;

            if (loanAmount <= 500000)
                return;

            // 🔹 Read RiskLevel from SharedVariables
            riskLevel = "Low"; // default

            if (context.SharedVariables.Contains("RiskLevel"))
            {
                riskLevel = context.SharedVariables["RiskLevel"].ToString();
            }

            // 🔎 Check if approval already exists
            QueryExpression query = new QueryExpression("lms_loanapproval");
            query.ColumnSet = new ColumnSet("lms_approvalnumber");
            query.Criteria.AddCondition("lms_loan", ConditionOperator.Equal, loan.Id);

            EntityCollection existingApprovals = service.RetrieveMultiple(query);

            if (existingApprovals.Entities.Count > 0)
                return; // Avoid duplicate approval

            // 🏗 Create Approval Record
            Entity approval = new Entity("lms_loanapproval");

            approval["lms_loan"] =
                new EntityReference("lms_loanapplication", loan.Id);

            // Approval Status → Pending
            approval["lms_approvalstatus"] =
                new OptionSetValue(100000000);

            // Approval Level → Manager
            approval["lms_approvallevel"] =
                new OptionSetValue(100000000);

            // Approver (Hardcoded User)
            approval["lms_approver"] =
                new EntityReference("systemuser",
                    new Guid("6a6be4eb-a910-f111-8341-7ced8daef29a"));

            // 🔹 Map RiskLevel to OptionSet
            OptionSetValue riskOption = null;

            if (riskLevel == "High")
                riskOption = new OptionSetValue(100000000);
            else if (riskLevel == "Medium")
                riskOption = new OptionSetValue(100000001);
            else
                riskOption = new OptionSetValue(100000002);

            approval["lms_risklevel"] = riskOption;

            service.Create(approval);
        }
    }
}