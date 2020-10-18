namespace HireMe.Middlewares.Security
{
    using Microsoft.AspNetCore.Rewrite;

    public class RewriteRouteRules
    {
        public static void ReWriteRequests(RewriteContext context)
        {
            /* var request = context.HttpContext.Request;


             if (request.QueryString.Value.Contains("SearchString=", StringComparison.OrdinalIgnoreCase))
             {
                 context.HttpContext.Request.Path = context.HttpContext.Request.Path.Value.Replace("SearchString=", "sexxxxx");
             }
             */
        }
    }

}