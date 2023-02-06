using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Transactions;
using System.Reflection;
namespace workauto.filter
{
    public class TransactionScopeFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            bool hasNotTransactionalAttribute = false;
            var actionDesc = (ControllerActionDescriptor)context.ActionDescriptor;
            if (context.ActionDescriptor is ControllerActionDescriptor)
            {
               
                Console.WriteLine(actionDesc.ControllerName);
                hasNotTransactionalAttribute = actionDesc.MethodInfo
                    .IsDefined(typeof(NotTransactionalAttribute));
            }
            if (hasNotTransactionalAttribute || actionDesc.ControllerName != "WorkApi")
            {
                await next();
                return;
            }
            using var txScope =  new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var result = await next();
            if (result.Exception == null)
            {
                txScope.Complete();
            }
        }
    }
}
