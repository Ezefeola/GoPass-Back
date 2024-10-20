namespace GoPass.API.Middlewares
{
    public class UserMiddlewares
    {
        private readonly RequestDelegate _requestDelegate;

        public UserMiddlewares(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
        }

        public async Task GetTokenInHeader(HttpContext httpContext)
        {
            string authHeader = httpContext.Request.Headers["Authorization"].ToString();

            if(authHeader is null)
            {
                throw new Exception();
            }

            await _requestDelegate(httpContext);
        }


    }
}
