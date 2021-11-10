using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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

            //configuracion de la conexion
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 27));
            services.AddDbContext<StudentDetailContext>(
                dbContextOptions => dbContextOptions
                    .UseMySql(Configuration.GetConnectionString("DBConn"), serverVersion)
                    .LogTo(Console.WriteLine, LogLevel.Information)
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
        );

            //versionado de la api
            services.AddApiVersioning();

            //Configuración para automapper
            services.AddAutoMapper(typeof(StudentDetailMappings));

            //***** Configuracion de servicios para JWT *****
            //autorizacion
            services.AddAuthorization(options =>
                options.DefaultPolicy =
                new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser().Build()
            );

            //estos valores los obtenemos de nuestro appsettings
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

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Student Details Api",
                    Description = "A sample Asp.Net Core web Api that allows you work with Student Details Data",
                    Contact = new OpenApiContact
                    {
                        Name = "Adrián Fonseca Vega",
                        Email = "afonsecav88@gmail.com",
                        Url = new Uri("https://www.linkedin.com/in/adrianfvega/")
                    },
                    Version = "v1"
                });

                //Añadido para Generar la documentación xml en las métodos
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

            });

            //Contenedor de Dependencias
            services.AddScoped<IStudentDetail, StudentDetailService>();
            services.AddScoped<IAuthService, AuthService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "mysqlapi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            //Configurando la aplicación para JWT
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
