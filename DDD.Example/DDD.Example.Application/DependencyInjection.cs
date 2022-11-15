using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace DDD.Example.Application {
    public static class DependencyInjection {
        public static void AddApplication(this IServiceCollection serviceCollection) {
            serviceCollection.AddMediatR(typeof(DependencyInjection).GetTypeInfo().Assembly);
        }
    }
}