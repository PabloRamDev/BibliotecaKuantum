using biblioteca_kuantum.DataAccessLayer.Data;
using biblioteca_kuantum.DataAccessLayer.Repositories;
using biblioteca_kuantum.DataAccessLayer.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace biblioteca_kuantum.DataAccessLayer
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
        {
            string? connectionString = configuration.GetConnectionString("PostgresConnection");

            services.AddDbContext<AppDbContext>
                (options =>
                options.UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention()
                );

            services.AddScoped<IDocumentRepository, DocumentRepository>();

            return services;
        }
    }
}
