using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using mysqlapi.Interfaces;
using mysqlapi.Models;
using mysqlapi.Services;
using System;
using System.IO;
using System.Net.Mime;
using System.Reflection;
using System.Text;

namespace mysqlapi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Recuperando la cadena de conexion desde secret.json
            var connectionString = Configuration["ConnectionStrings:DBConn"];

            //configuracion de la conexion a la base de datos
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 27));
            services.AddDbContext<StudentDetailContext>(
                dbContextOptions => dbContextOptions
                    .UseMySql(connectionString, serverVersion)
                    .LogTo(Console.WriteLine, LogLevel.Information)
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
        );

            //versionado de la api
            services.AddApiVersioning();

            //Configuraci�n para automapper
            services.AddAutoMapper(typeof(StudentDetailMappings));

            //***** Configuracion de servicios para JWT *****
            //autorizacion
            services.AddAuthorization(options =>
                options.DefaultPolicy =
                new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser().Build()
            );

            var issuer = Configuration["AuthenticationSettings:Issuer"];
            var audience = Configuration["AuthenticationSettings:Audience"];
            var signinKey = Configuration["AuthenticationSettings:SigningKey"];

            //autorizacion
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.Audience = audience;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(signinKey))
                };
            });

            //***** Aqui termina la Configuracion de servicios para JWT *****


            services.AddControllers()
            //***** Aqui comienza la Configuracion para el manejo y captura de error y excepciones *****
            .ConfigureApiBehaviorOptions(options =>
             {
                 options.InvalidModelStateResponseFactory = context =>
                 {
                     var result = new BadRequestObjectResult(context.ModelState);
                     result.ContentTypes.Add(MediaTypeNames.Application.Json);
                     result.ContentTypes.Add(MediaTypeNames.Application.Xml);
                     return result;
                 };
             });
            //***** Aqui termina la Configuracion para el manejo y captura de error y excepciones *****

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Student Details Api",
                    Description = "A sample Asp.Net Core web Api that allows you work with Student Details Data",
                    Contact = new OpenApiContact
                    {
                        Name = "Adri�n Fonseca Vega",
                        Email = "afonsecav88@gmail.com",
                        Url = new Uri("https://www.linkedin.com/in/adrianfvega/")
                    },
                    Version = "v1"
                });

                //A�adido para Generar la documentaci�n xml en las m�todos
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

            });

            //Contenedor de Dependencias
            services.AddScoped<IStudentDetail, StudentDetailService>();
            services.AddScoped<IAuthService, AuthService>();
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "mysqlapi v1"));
            }
            else
            {
                //Agregado para el manejo y captura de error y excepciones 
                app.UseExceptionHandler("/error");
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            //Configurando la aplicaci�n para JWT
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
