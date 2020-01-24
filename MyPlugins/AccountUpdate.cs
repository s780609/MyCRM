using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MyPlugins
{
  public  class AccountUpdate :IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the tracing service
            ITracingService tracingService =
            (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.  
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            // The InputParameters collection contains all the data passed in the message request.  
            if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
            {
                // Obtain the target entity from the input parameters.  
                Entity account = (Entity)context.InputParameters["Target"];



                // Obtain the organization service reference which you will need for  
                // web service calls.  
                IOrganizationServiceFactory serviceFactory =
                    (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                try
                {
                    // Plug-in business logic goes here.  
                    decimal revenue = 0;
                    int numberofemployees = 0;
                    // IF the value is entered by the user while updating
                    if (account.Attributes.Contains("revenue") && account.Attributes["revenue"] != null)
                    {
                        revenue = ((Money)account.Attributes["revenue"]).Value;
                    }
                    else {
                        Entity image = context.PreEntityImages["PreImage"];
                        if (image.Attributes.Contains("revenue"))
                        {
                            revenue = ((Money)image.Attributes["revenue"]).Value;
                        }
                        else {
                            //skipping Plugin
                            return;
                        }
                    }
                    if (account.Attributes.Contains("numberofemployees") && account.Attributes["numberofemployees"] != null) {
                        numberofemployees = Convert.ToInt32(account.Attributes["numberofemployees"].ToString());
                    }
                    else {
                        Entity image = context.PreEntityImages["PreImage"];
                        if (image.Attributes.Contains("numberofemployees"))
                        {
                            revenue = ((Money)image.Attributes["numberofemployees"]).Value;
                        }
                        else
                        {
                            //skipping Plugin
                            return;
                        }
                    }

                    if ( revenue ==0 || numberofemployees ==0 ) {
                        account.Attributes.Add("contoso_annualrevenueperemployee",null);
                        return;
                    }
                    decimal revenueperemployee = revenue / numberofemployees;

                    account.Attributes.Add("contoso_annualrevenueperemployee",new Money(revenueperemployee));
                }

                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in HelloWorldPlugin.", ex);
                }

                catch (Exception ex)
                {
                    tracingService.Trace("HelloWorldPlugin: {0}", ex.ToString());
                    throw;
                }
            }
        }
    }
}
