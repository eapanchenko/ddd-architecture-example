using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DDD.Example.Api.Extensions;

public static class ServiceCollectionExtensions {
    public static IServiceCollection AddConfiguration<TOptions>(this IServiceCollection services, IConfiguration configuration,
        string sectionName) where TOptions : class {
        services.Configure<TOptions>(options => configuration.GetSection(sectionName).Bind(options));
        services.AddSingleton(x => x.GetRequiredService<IOptions<TOptions>>().Value);
        return services;
    }

    public static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfiguration configuration,
        Action<IOptionsConfigurator> configure) {
        var optionsConfigurator = new OptionsConfiguratorImpl(services, configuration);
        configure(optionsConfigurator);
        return services;
    }

    class OptionsConfiguratorImpl : IOptionsConfigurator {
        private readonly IServiceCollection _services;
        private readonly IConfiguration _configuration;

        public OptionsConfiguratorImpl(IServiceCollection services, IConfiguration configuration) {
            _services = services;
            _configuration = configuration;
        }

        public IOptionsConfigurator Add<TOptions>(string sectionName) where TOptions : class {
            _services.Configure<TOptions>(options => _configuration.GetSection(sectionName).Bind(options));
            _services.AddSingleton(x => x.GetRequiredService<IOptions<TOptions>>().Value);
            return this;
        }
    }
}