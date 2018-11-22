using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace DeckOfCards.WebApi.Filters
{
    /// <summary>
    /// This attribute marks that the controller is guaranteed to be given a validated model.  If MVC ModelState is invalid, a 400 is returned
    /// with appropriate validation messages prior to the controller executing any logic.
    /// </summary>
    public class PreValidatedModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //a poor man DI can be achieved by tapping into request services below:
            //because of this, you can completely remove the MVC dependence in your Query Model layer, and instead resolve complex FluentValidation objects in this block!
            if (!context.ModelState.IsValid)
            {
                var logger = context.HttpContext.RequestServices.GetService(typeof(ILogger<PreValidatedModelAttribute>)) as ILogger<PreValidatedModelAttribute>;
                logger?.LogWarning("Validation failed MVC binding.  Short circuiting Request Id {requestId}",context.HttpContext.TraceIdentifier);
                context.Result = new BadRequestObjectResult(context.ModelState);//todo - return as part of generic hypermedia
            }
        }
    }
}
