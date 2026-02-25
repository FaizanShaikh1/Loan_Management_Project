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

            if (context.Depth > 1) return;

            if (!context.PostEntityImages.Contains("PostImage"))
                return;

            Entity loan = context.PostEntityImages["PostImage"];

            decimal loanAmount =
                loan.GetAttributeValue<Money>("lms_loanamount")?.Value ?? 0;

            if (loanAmount <= 500000)
                return;

            // 🔎 Check if approval already exists
            QueryExpression query = new QueryExpression("lms_loanapproval");
            query.ColumnSet = new ColumnSet("lms_approvalnumber");
            query.Criteria.AddCondition("lms_loan", ConditionOperator.Equal, loan.Id);

            EntityCollection existingApprovals = service.RetrieveMultiple(query);

            if (existingApprovals.Entities.Count > 0)
                return; // Already exists

            // 🏗 Create Approval Record
            Entity approval = new Entity("lms_loanapproval");

            approval["lms_loan"] =
                new EntityReference("lms_loanapplication", loan.Id);

            approval["lms_approvalstatus"] =
                new OptionSetValue(100000000); // Pending

            approval["lms_approvallevel"] =
                new OptionSetValue(100000000); // Manager

            // Replace with real user GUID
            approval["lms_approver"] =
                new EntityReference("systemuser", new Guid("6a6be4eb-a910-f111-8341-7ced8daef29a"));

            service.Create(approval);
        }
    }
}