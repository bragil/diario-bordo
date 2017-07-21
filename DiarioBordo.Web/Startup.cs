using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.SqlClient;
using DiarioBordo.Data;
using DiarioBordo.Business;

namespace DiarioBordo.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddSession();
            services.AddMvc();
            services.AddSingleton<IConfiguration>(Configuration);

            services.AddScoped<IDbConnection, SqlConnection>(s => {
                var conn = new SqlConnection(Configuration.GetConnectionString("diariobordo"));
                conn.Open();
                return conn;
            });

            services.AddScoped<RegistroRepository>();
            services.AddScoped<TagRepository>();
            services.AddScoped<RegistroService>();
            services.AddScoped<TagService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //    app.UseBrowserLink();
            //}
            //else
            //{
            app.UseExceptionHandler("/error");
            //}

            app.UseStaticFiles();
            app.UseSession();
            app.UseMvcWithDefaultRoute();
        }
    }
}
