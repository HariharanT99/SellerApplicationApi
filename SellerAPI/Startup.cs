using BLL.ILogic;
using BLL.Logic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity.Context;
using Identity.Model;
using Microsoft.EntityFrameworkCore;
using Identity.Action;
using Identity.IAction;
using DAL.Data;
using BLL;
using Mapster;
using Entity;
using CustomModel;

namespace SellerAPI
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
                
            services.AddControllers();

            services.AddDbContext<SellerAppContext>(options => options.UseSqlServer(Configuration.GetConnectionString("SellerDbConnection")));

            services.AddDbContext<IdentityDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("SellerDbConnection")));

            services.AddIdentity<Identity.Model.User, Identity.Model.Role>(options =>
            {
                options.Password.RequiredLength = 10;
                options.Password.RequireUppercase = false;
                options.Password.RequiredUniqueChars = 3;
                options.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<IdentityDbContext>();


            //services.AddScoped<IValueService, ValueService>();

            services.AddScoped<IUserAction, UserAction>();
            services.AddScoped<IRoleAction, RoleAction>();
            services.AddScoped<IAccountAction, AccountAction>();

            services.AddScoped<IService, Service>();


            services.AddCors(options =>
                {
                    options
                        .AddPolicy("Policy",
                        builder =>
                            builder
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowAnyOrigin());
                });


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SellerAPI", Version = "v1" });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SellerAPI v1"));
            }

            TypeAdapterConfig<Product,GetProductCustomModel>.NewConfig()
                .Map(dest => dest.Pictures, src => src.Pictures);

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseCors("Policy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
