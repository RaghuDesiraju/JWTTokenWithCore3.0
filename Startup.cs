using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JWTToken.API.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;//dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer -v 3.1.0
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using JWTToken.API.Helpers;
using Microsoft.AspNetCore.Http;

//dotnet new webapi -o JWTToken.API -n JWTToken.API
//dotnet add package Microsoft.EntityFrameworkCore
//also run dotnet tool install --global dotnet-ef --version 3.0.0
//dotnet add package Microsoft.EntityFrameworkCore.Design
//for the first time run following 2
//dotnet ef migrations add InitialCreate
//dotnet ef database update 
//dotnet add package Newtonsoft.Json
//dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer -v 3.1.0
namespace JWTToken.API
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
            
            services.AddDbContext<DataContext>(x => x.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));            
            /*services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            .AddJsonOptions(opt => {
                opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });*/
            services.AddCors();           
            services.AddScoped<IAuthRepository,AuthRepository>();
            services.AddScoped<IUserRepository,UserRepository>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options=> {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(
                        Configuration.GetSection("AppSettings:Token").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                });
                services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            /*if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }*/
             if (env.IsDevelopment())
            {
                  //if an appln encounters exception captures the exception in developer friendly page
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(builder => {
                                        builder.Run (async context => {
                                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                            var error = context.Features.Get<IExceptionHandlerFeature>();
                                            if(error != null)
                                            {
                                                context.Response.AddApplicationError(error.Error.Message);
                                                await context.Response.WriteAsync(error.Error.Message);            
                                            }
                                        }) ;
                                    });       
            }

             app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
             app.UseAuthentication();
             app.UseRouting();
             app.UseHttpsRedirection();

            

             app.UseAuthorization();

             app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
