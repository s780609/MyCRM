using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Query;
using System.Activities;
using System.Threading;

namespace MyCustomWorkflows
{
   public class GetTaxWorkflow : CodeActivity
    {
        [Input("key")]
        public InArgument<string> Key { get; set; }
        [Output("Tax")]
        public OutArgument<string> Tax { get; set; }
        protected override void Execute(CodeActivityContext executionContext)
        {
            //Create the tracing service
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();

            //Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.InitiatingUserId);

           string key =  Key.Get(executionContext);

            //Get data from Configuration Entity
            //Call organization web service

            QueryByAttribute query = new QueryByAttribute("contoso_configuration");
            query.ColumnSet = new ColumnSet(new String[] {"contotso_value" });
            query.AddAttributeValue("contoso_name",key);
            EntityCollection collection = service.RetrieveMultiple(query);

            if ( collection.Entities.Count != 1 ) {
                tracingService.Trace("Something is wrong with configuration");
            }

            Entity config = collection.Entities.FirstOrDefault();
            //config.Attributes["contoso_value"].ToString();
            
            //EntityCollection collection = service.RetrieveMultiple(query);

            Tax.Set(executionContext, config.Attributes["contoso_value"].ToString());
        }
    }
}
