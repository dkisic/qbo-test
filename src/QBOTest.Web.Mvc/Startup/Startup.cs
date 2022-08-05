using System;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Castle.Facilities.Logging;
using Abp.AspNetCore;
using Abp.AspNetCore.Mvc.Antiforgery;
using Abp.Castle.Logging.Log4Net;
using QBOTest.Authentication.JwtBearer;
using QBOTest.Configuration;
using QBOTest.Identity;
using QBOTest.Web.Resources;
using Abp.AspNetCore.SignalR.Hubs;
using Abp.Dependency;
using Abp.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.WebEncoders;
using Newtonsoft.Json.Serialization;
using QBOTest.QuickBooks;
using SoapCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using QBOTest.Web.QuickbookDesktop;
using QBOTest.Users;
using Abp.Domain.Repositories;
using QBOTest.Partners;
using System.ServiceModel.Channels;
using System.Collections.Generic;
using System.ServiceModel;

namespace QBOTest.Web.Startup
{
    public class Startup
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConfigurationRoot _appConfiguration;

        public Startup(IWebHostEnvironment env)
        {
            _hostingEnvironment = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // MVC
            services.AddControllersWithViews(
                    options =>
                    {
                        options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                        options.Filters.Add(new AbpAutoValidateAntiforgeryTokenAttribute());
                    }
                )
                .AddRazorRuntimeCompilation()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new AbpMvcContractResolver(IocManager.Instance)
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    };
                });

            IdentityRegistrar.Register(services);
            AuthConfigurer.Configure(services, _appConfiguration);

            services.Configure<WebEncoderOptions>(options =>
            {
                options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
            });
            
            services.AddScoped<IWebResourceManager, WebResourceManager>();

            services.AddSoapCore();
            //services.TryAddSingleton<IQBDWebService>(x=> new QBDWebService(x.GetRequiredService<IUserAuthentication>(), x.GetRequiredService<IRepository<Partner, Guid>>()));
            services.AddMvc();

            services.AddSoapExceptionTransformer((ex) => ex.Message);

            services.Configure<QuickBooksConfig>(_appConfiguration.GetSection("QuickBooksConfig"));

            services.AddSignalR();

            // Configure Abp and Dependency Injection
            services.AddAbpWithoutCreatingServiceProvider<QBOTestWebMvcModule>(
                // Configure Log4Net logging
                options => options.IocManager.IocContainer.AddFacility<LoggingFacility>(
                    f => f.UseAbpLog4Net().WithConfig(
                        _hostingEnvironment.IsDevelopment()
                            ? "log4net.config"
                            : "log4net.Production.config"
                        )
                )
            );

           

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseAbp(); // Initializes ABP framework.

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseJwtTokenMiddleware();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<AbpCommonHub>("/signalr");
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute("defaultWithArea", "{area}/{controller=Home}/{action=Index}/{id?}");
            });



			app.UseEndpoints(endpoints =>
			{
				endpoints.UseSoapEndpoint<IQBDWebService>("/QBDService.asmx", new SoapEncoderOptions(), SoapSerializer.XmlSerializer);
			});

			//var transportBinding = new HttpTransportBindingElement();
			//var textEncodingBinding = new TextMessageEncodingBindingElement(MessageVersion.Soap11, System.Text.Encoding.UTF8);
			//var customBinding = new CustomBinding(transportBinding, textEncodingBinding);
			//app.UseEndpoints(endpoints => {
			//    endpoints.UseSoapEndpoint<IQBDWebService>(opt=> {
			//        opt.Path = "/QBDService.asmx";
			//        opt.AdditionalEnvelopeXmlnsAttributes = new Dictionary<string, string>() {

			//        { "xsi","http://www.w3.org/2001/XMLSchema-instance"},
			//        {"xsd","http://www.w3.org/2001/XMLSchema" }
			//        };
			//        opt.SoapSerializer = SoapSerializer.DataContractSerializer;
			//        opt.EncoderOptions = [MessageVersion.Soap11];

			//    });
			//});

		}
    }
}
