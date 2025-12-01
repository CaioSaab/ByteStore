using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ByteStoreAPI.Data;
using System;
using System.Threading;

namespace ByteStoreAPI.Tests.Integration
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove o DbContext padrão (tanto o DbContext quanto o DbContextOptions)
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ByteStoreDbContext>));

                if (dbContextDescriptor != null)
                {
                    services.Remove(dbContextDescriptor);
                }

                // Remove também o ByteStoreDbContext se estiver registrado
                var dbContextServiceDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(ByteStoreDbContext));

                if (dbContextServiceDescriptor != null)
                {
                    services.Remove(dbContextServiceDescriptor);
                }

                // Adiciona o DbContext em memória para testes
                // Usa um nome fixo para que todas as requisições HTTP compartilhem o mesmo banco
                // IMPORTANTE: O banco em memória precisa ser compartilhado entre requisições HTTP
                services.AddDbContext<ByteStoreDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb_IntegrationTests", b => b.EnableNullChecks(false));
                }, ServiceLifetime.Scoped);

                // Build do service provider
                var sp = services.BuildServiceProvider();

                // Garante que o banco seja criado
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ByteStoreDbContext>();
                    db.Database.EnsureCreated();
                }
            });
        }
    }
}

