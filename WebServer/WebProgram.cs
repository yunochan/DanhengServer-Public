using System.Net;
using EggLink.DanhengServer.Util;
using Microsoft.AspNetCore;

namespace EggLink.DanhengServer.WebServer;

public class WebProgram
{
    public static void Main(string[] args, int port, string address)
    {
        BuildWebHost(args, port, address).Start();
    }

    public static IWebHost BuildWebHost(string[] args, int port, string address)
    {
        var b = WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .ConfigureLogging((hostingContext, logging) => { logging.ClearProviders(); })
            .UseUrls(address);

        if (ConfigManager.Config.HttpServer.UseSSL)
            b.UseKestrel(options =>
            {
                options.Listen(IPAddress.Any, port, listenOptions =>
                {
                    listenOptions.UseHttps(
                        ConfigManager.Config.KeyStore.KeyStorePath,
                        ConfigManager.Config.KeyStore.KeyStorePassword
                    );
                });
            });

        return b.Build();
    }
}
    public static class HttpContextProvider
    {
        private static AsyncLocal<HttpContext> _httpContext = new AsyncLocal<HttpContext>();

        public static HttpContext HttpContext
        {
            get => _httpContext.Value ?? throw new InvalidOperationException("HttpContext is not set.");
            set => _httpContext.Value = value;
        }
    }

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
        });

        services.AddControllers();
        services.AddHttpContextAccessor();

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

        app.Use(async (context, next) =>
        {
            HttpContextProvider.HttpContext = context;
            using var buffer = new MemoryStream();
            var request = context.Request;
            var response = context.Response;

            var bodyStream = response.Body;
            response.Body = buffer;

            await next.Invoke();
            buffer.Position = 0;
            context.Response.Headers["Content-Length"] = (response.ContentLength ?? buffer.Length).ToString();
            context.Response.Headers.Remove("Transfer-Encoding");
            await buffer.CopyToAsync(bodyStream);
        });

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseCors("AllowAll");

        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}