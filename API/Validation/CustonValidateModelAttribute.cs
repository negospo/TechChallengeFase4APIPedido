using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Validation
{
    public class CustonValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new CustonValidationFailedResult(context.ModelState);
            }
        }
    }
}
