using System.IO;
using System.Linq;
using DDD.Example.Api.Extensions;
using DDD.Example.Application;
using DDD.Example.Domain;
using DDD.Example.Infrastructure.Impl;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DDD.Example.Api {
    public class Startup {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services) {
            services.AddOptions();
            services.AddHttpContextAccessor();

            services.AddMvc();
            services.AddControllers().AddDataAnnotationsLocalization();
            services.AddCors();
            
            ConfigurePresentation(services);
            services.AddDomain();
            services.AddApplication();
            services.AddInfrastructure();
        }

        private void ConfigurePresentation(IServiceCollection services) {
            services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
            services.ConfigureOptions(Configuration, configurator => {
                configurator.Add<KestrelServerOptions>("Kestrel");
            });
            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "keys")));
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime) {
            var forwardedHeadersOptions = new ForwardedHeadersOptions {ForwardedHeaders = ForwardedHeaders.All};
            forwardedHeadersOptions.KnownNetworks.Clear();
            forwardedHeadersOptions.KnownProxies.Clear();
            app.UseForwardedHeaders(forwardedHeadersOptions);
            
            app.UseStaticFiles();
            if (env.IsDevelopment() || env.IsStaging()) {
                app.UseDeveloperExceptionPage();
            }

            var corsOrigins = Configuration.GetSection("CorsOrigins")
                .AsEnumerable().Where(x => x.Value != null).Select(x => x.Value).ToArray();
            app.UseCors(x => {
                x.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithOrigins(corsOrigins);
            });

            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}