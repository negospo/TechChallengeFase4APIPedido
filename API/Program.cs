using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.IO.Compression;
using System;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using Dapper;

namespace API
{
    public class Program
    {
        static string PrefixPath => UsePrefixPath ? "/pedido" : string.Empty;
        static bool UsePrefixPath => true;

        public static void Main(string[] args)
        {
            if (!ValidateEnviromentVariables())
            {
                Log.Critical("Application failed to start due to missing environment variables.");
                Environment.Exit(-1);
            }

            StartConfigs();

            //Cria o builder e configura
            var builder = WebApplication.CreateBuilder(args);
            ConfigBuilder(builder);

            //Cria o WebApplication e roda
            var app = builder.Build();
            ConfigApp(app);
            RunSQLScript();
            app.Run();
        }

        static void RunSQLScript()
        {
            var _connPg = Infrastructure.Persistence.Database.Connection();
            var filePath = Path.GetFullPath(@"./Fiap.sql");
            FileInfo file = new FileInfo(filePath);
            string script = file.OpenText().ReadToEnd();
            var db_cmd = Infrastructure.Persistence.Database.Command(script, _connPg);
            _connPg.Open();
            db_cmd.ExecuteNonQuery();
            _connPg.Close();
        }

        static void ConfigBuilder(WebApplicationBuilder builder)
        {
            Infrastructure.Persistence.Settings.PostgreSQLConnectionString = Settings.PostgreSQLConnectionString;
            Infrastructure.Persistence.Settings.APIPagamentoPath = Settings.APIPagamentoPath;
            Infrastructure.Persistence.Settings.APIStatusPath = Settings.APIStatusPath;


            builder.Services.AddTransient<Application.Interfaces.UseCases.IClienteUseCase, Application.Implementations.ClienteUseCase>();
            builder.Services.AddTransient<Application.Interfaces.Repositories.IClienteRepository, Infrastructure.Persistence.Repositories.ClienteRepository>();

            builder.Services.AddTransient<Application.Interfaces.UseCases.IProdutoUseCase, Application.Implementations.ProdutoUseCase>();
            builder.Services.AddTransient<Application.Interfaces.Repositories.IProdutoRepository, Infrastructure.Persistence.Repositories.ProdutoRepository>();

            builder.Services.AddTransient<Application.Interfaces.UseCases.IPedidoUseCase, Application.Implementations.PedidoUseCase>();
            builder.Services.AddTransient<Application.Interfaces.Repositories.IPedidoRepository, Infrastructure.Persistence.Repositories.PedidoRepository>();

            builder.Services.AddTransient<Application.Interfaces.UseCases.IPedidoStatusUseCase, Infrastructure.Status.Service>();
            builder.Services.AddTransient<Application.Interfaces.UseCases.IPedidoPagamentoUseCase, Infrastructure.Pagamento.Service>();


            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddHealthChecks();
            builder.Services.AddCors();

            //Configura��es de localiza��o
            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("pt-BR");
                options.SupportedCultures = new List<CultureInfo> { new CultureInfo("pt-BR") };
            });

            //Configura��es de compress�o do json
            builder.Services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
            builder.Services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                options.EnableForHttps = true;
            });

            //Previne valores nulos no json e reescreve os converters
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = true;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });


            //Suprime a valida��o auto dos models permitindo que seja feita uma valida��o manual
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            ConfigSwagger(builder);
            ConfigureJWT(builder);
        }

        /// <summary>
        /// Configura o WebApplication
        /// </summary>
        static void ConfigApp(WebApplication app)
        {
            if (UsePrefixPath)
                app.UsePathBase(PrefixPath);

            app.UseHealthChecks("/health");
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint($"{PrefixPath}/swagger/v1/swagger.json", ""));
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.UseRequestLocalization();
            //app.UseMiddleware<Middlewares.CustonException>();
            app.UseCors(x => x
             .AllowAnyOrigin()
             .AllowAnyMethod()
             .AllowAnyHeader());
        }

        /// <summary>
        /// Configura��es gerais
        /// </summary>
        static void StartConfigs()
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        /// <summary>
        /// Configura o JWT
        /// </summary>
        static void ConfigureJWT(WebApplicationBuilder builder)
        {
            var key = Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("JWToken:Secret"));

            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    LifetimeValidator = CustomLifetimeValidator
                };
            });

            bool CustomLifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken tokenToValidate, TokenValidationParameters @param)
            {
                if (expires != null)
                {
                    return expires > DateTime.UtcNow;
                }
                return false;
            }
        }

        /// <summary>
        /// Configura o Swagger
        /// </summary>
        static void ConfigSwagger(WebApplicationBuilder builder)
        {
            builder.Services.AddSwaggerGen(doc =>
            {
                doc.SwaggerDoc("v1", new OpenApiInfo { Title = "API Pedido", Version = "v1" });
                doc.CustomSchemaIds(x => x.FullName.Replace($"{AppDomain.CurrentDomain.FriendlyName}.", ""));
                doc.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description =
                   "JWT Authorization Header - utilizado com Bearer Authentication.\r\n\r\n" +
                   "Digite 'Bearer' [espa�o] e ent�o seu token no campo abaixo.\r\n\r\n" +
                   "Exemplo (informar sem as aspas): 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                doc.IncludeXmlComments(xmlPath);


                doc.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                         Array.Empty<string>()
                    }
                });
            });
        }

        /// <summary>
        /// Valida as variaveis de ambiente
        /// </summary>
        /// <returns></returns>
        static bool ValidateEnviromentVariables()
        {
            bool result = true;

            Log.Information("*********** Validate Enviroment Variables ***********");
            Log.Information($"Running in {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").ToUpper()}");

            //var listVars = new List<string> { "POSTGRE_HOST", "POSTGRE_USER", "POSTGRE_PWD", "REDIS_CONNECTION_STRING", "GCP_AUTH_FILE_PATH" };
            //listVars.ForEach(variable =>
            //{
            //    if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(variable)))
            //    {
            //        Log.Critical($"Variable {variable} Not Found");
            //        result = false;
            //    }
            //});

            return result;
        }

    }
}