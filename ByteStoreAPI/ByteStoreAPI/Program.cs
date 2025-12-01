using ByteStoreAPI.Data;
using ByteStoreAPI.Interfaces;
using ByteStoreAPI.Repositories;
using ByteStoreAPI.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using ByteStoreAPI.Services;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Http.Features;
using System.Collections.Generic;

namespace ByteStoreAPI
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            // Configurar limites de tamanho para multipart/form-data
            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 104857600; // 100 MB
                options.ValueLengthLimit = int.MaxValue;
                options.ValueCountLimit = int.MaxValue;
                options.MultipartHeadersLengthLimit = int.MaxValue;
            });

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "ByteStore API", Version = "v1" });
                
                // Adicionar suporte para multipart/form-data
                options.MapType<IFormFile>(() => new OpenApiSchema
                {
                    Type = "string",
                    Format = "binary"
                });
                
                // Configurar schema customizado para o endpoint de criação de produto
                options.CustomSchemaIds(type => type.FullName);
                
                // Configurar schema para CreateProductFormDTO para garantir que o campo Data apareça
                options.MapType<ByteStoreAPI.DTOs.CreateProductFormDTO>(() => new OpenApiSchema
                {
                    Type = "object",
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["Data"] = new OpenApiSchema
                        {
                            Type = "string",
                            Description = "JSON string com os dados do produto. Exemplo: {\"Nome\":\"Produto\",\"Descricao\":\"Descrição\",\"Categoria\":\"Categoria\",\"Variations\":[{\"Preco\":100.00,\"Estoque\":10,\"Cor\":\"Vermelho\",\"Tamanho\":\"M\",\"FileIndexes\":[0,1]}]}"
                        },
                        ["Files"] = new OpenApiSchema
                        {
                            Type = "array",
                            Items = new OpenApiSchema
                            {
                                Type = "string",
                                Format = "binary"
                            },
                            Description = "Lista de arquivos de imagem para as variações do produto"
                        }
                    },
                    Required = new HashSet<string> { "Data" }
                });
                
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new List<string>()
                    }
                });
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });

            var mysqlString = "server=localhost; port=3306; database=bytestore; user=root; Persist Security Info =False; Convert Zero Datetime=True";
            builder.Services.AddDbContext<ByteStoreDbContext>(options =>
                options.UseMySql(mysqlString, ServerVersion.AutoDetect(mysqlString))
            );

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            builder.Services.AddTransient<IClientService, ClientService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ISaleService, SaleService>();
            builder.Services.AddScoped<IVendorService, VendorService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IQuestionService, QuestionService>();
            builder.Services.AddScoped<ICouponService, CouponService>();

            builder.Services.AddTransient<IClientRepository, ClientRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<ISaleRepository, SaleRepository>();
            builder.Services.AddScoped<IVendorRepository, VendorRepository>();
            builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
            builder.Services.AddScoped<ICouponRepository, CouponRepository>();


            builder.Services.AddScoped<IFileUploadService, LocalFileUploadService>();
            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            app.UseStaticFiles();

            app.UseCors();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}