using System.Diagnostics;

namespace GoPass.API.Middlewares
{
    public class TimingMiddleware
    {
        private readonly RequestDelegate _next;

        public TimingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            await _next(context); // Llama al siguiente middleware o al endpoint

            stopwatch.Stop();
            Console.WriteLine($"Tiempo de ejecución de la solicitud: {stopwatch.ElapsedMilliseconds} ms");
        }
    }

}
