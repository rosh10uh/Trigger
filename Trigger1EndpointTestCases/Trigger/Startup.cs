using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using OneRPP.Restful.DAO;
using OneRPP.Restful.Services;
using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Trigger.DAL.Shared;
using Trigger.DTO;
using Trigger.DTO.SmsService;
using Trigger.Middleware;

namespace Trigger
{
    /// <summary>
    /// Startup Class
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// declare configuration object
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Setup Startup
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="hostingEnvironment"></param>
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json", optional: true);
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        /// <summary>
        /// ConfigureServices
        /// </summary>
        /// <param name="services"></param>
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Dictionary.AddConfigValues(Configuration);
            services.AddCors();

            services.AddApiVersioning(o =>
           {
               o.ReportApiVersions = true;
               o.AssumeDefaultVersionWhenUnspecified = true;
               o.DefaultApiVersion = new ApiVersion(1, 0);
               o.ApiVersionReader = new HeaderApiVersionReader("api-version");
           });

            services.AddMvc(options =>
            {
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.Configure<ApiBehaviorOptions>(options => {
                options.SuppressModelStateInvalidFilter = true;
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Trigger API", Description = "Trigger API" });
                c.SwaggerDoc("v2", new Info { Title = "Trigger API Version 2", Description = "Trigger API 2" });
                c.SwaggerDoc("v2.1", new Info { Title = "Trigger API  Version 2.1", Description = "Trigger API" });
                c.SwaggerDoc("v2.2", new Info { Title = "Trigger API  Version 2.2", Description = "Trigger API" });
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme { In = "header", Description = "Please enter valid Bearer tocken", Name = "Authorization", Type = "apiKey" });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
                { "Bearer", Enumerable.Empty<string>() }
                });

                var xmlPath = System.AppDomain.CurrentDomain.BaseDirectory + @"Trigger.xml";
                c.IncludeXmlComments(xmlPath);

            });

            AuthorizationOptions authorization = new AuthorizationOptions();
            Configuration.Bind("Authorization:Options", authorization);

            services.Configure<AppSettings>(Configuration.GetSection("DBConnectionPools"));
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.Configure<SmsSettings>(Configuration.GetSection("SMSSettings"));
            services.Configure<TwilioSettings>(Configuration.GetSection("TwilioSettings"));

           
            services.AddOneAuthorityService(option =>
            {
                option.Authority = authorization.Authority;
                option.RequireHttpsMetadata = authorization.RequireHttpsMetadata;
                option.ApiName = authorization.ApiName;
                option.ApiSecret = authorization.ApiSecret;
            });

            services.AddLog4Net();
            services.AddOneEndpointService();
            services.AddOneEndpointDataManagerService();
            services.AddTriggerService();
            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetSection("DBConnectionPools").GetSection("DefaultConnection").GetSection("ConnectionString").Value));
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline. 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            const string slash = "/";

            app.UseCors(builder =>
                 builder.WithOrigins("*").AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()
             );

            app.UseLog4Net("log4net.config");
            app.UseAuthentication();

            app.UseHangfireServer();
            app.UseHangfireDashboard();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(string.Format("{0}swagger{1}v2.2{2}swagger.json", slash, slash, slash), "Trigger API Version 2.2");
                c.SwaggerEndpoint(string.Format("{0}swagger{1}v2.1{2}swagger.json", slash, slash, slash), "Trigger API Version 2.1");
                c.SwaggerEndpoint(string.Format("{0}swagger{1}v2{2}swagger.json", slash, slash, slash), "Trigger API Version 2");
                c.SwaggerEndpoint(string.Format("{0}swagger{1}v1{2}swagger.json",slash,slash,slash), "Trigger API");

            });


            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                   Path.Combine(Directory.GetCurrentDirectory(), "ExcelTemplate")),
                RequestPath = "/ExcelTemplate"
            });
            app.UseStaticFiles();
            app.UseMiddleware<LandingMiddleware>();
            app.UseOneEndPoint();
            app.UseMvc();

        }

    }
}
