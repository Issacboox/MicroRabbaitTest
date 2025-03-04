using MicroRabbit.Infra.Ioc;
using Microsoft.OpenApi.Models;
using MediatR;

namespace MicroRabbit.Banking.Api
{
    public static class ServiceCollectionExtentions
    {
        public static IServiceCollection AddRegisterServices(this IServiceCollection services)
        {
            DependencyContainer.RegisterServices(services);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Banking Microservice",
                    Version = "v1",
                    Description = "API for the Banking Microservice"
                });
            });

             services.AddMediatR(typeof(Program)); // ใช้ Program แทน Startup

            return services;
        }
    }
}
