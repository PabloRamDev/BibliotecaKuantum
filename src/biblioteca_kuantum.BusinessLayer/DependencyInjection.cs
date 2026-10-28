using biblioteca_kuantum.BusinessLayer.Mappers;
using biblioteca_kuantum.BusinessLayer.Validators;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using biblioteca_kuantum.BusinessLayer.ServiceContracts;

namespace biblioteca_kuantum.BusinessLayer
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
        {

            services.AddAutoMapper(cfg => { }, typeof(DocumentMappingProfiles).Assembly);

            services.AddValidatorsFromAssemblyContaining<CreateDocumentValidator>();

            services.AddScoped<IDocumentService, DocumentService>();
            return services;
        }
    }
}
