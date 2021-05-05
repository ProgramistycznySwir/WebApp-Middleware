using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using System.Web;

namespace WebApp_Middleware
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            /// Put middleware here:
            //app.Use(async (context, next) =>
            //{
            //    await context.Response.WriteAsync(context.Request.Headers["User-Agent"].ToString());
            //    await next();
            //});
            //app.UseMiddleware<MyMiddleware>();
            app.UseMyMiddleware();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }

    public static class MyMiddlewareExtensions
    {
        /// <summary>
        /// Wrapper for MyMiddleware class.
        /// </summary>
        /// <param name="app"></param>
        public static void UseMyMiddleware(this IApplicationBuilder app)
            => app.UseMiddleware<MyMiddleware>();
    }

    public class MyMiddleware
    {
        RequestDelegate next;
        HashSet<string> unsuportedBrowsers = new HashSet<string> 
            { "Edge", "EdgeChromium", "IE" };

        public MyMiddleware(RequestDelegate next) => this.next = next;

        public Task Invoke(HttpContext context)
        {
            string userAgent = context.Request.Headers["User-Agent"].ToString();

            if (unsuportedBrowsers.Any(a => userAgent.Contains(a)))
                context.Response.WriteAsync("Unsupported browser");

            return next(context);
        }
    }
}
