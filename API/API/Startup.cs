using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            //ensure that data base exists
            Configuration = configuration;
            using (var db = new SQLiteContext())
            {
                db.Database.EnsureCreatedAsync();
                db.SaveChangesAsync();
            }

        }

        public IConfiguration Configuration { get; }

        // here we add services for later use in our API.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IPoczekalnia, SerwisPoczekalni>();
            services.AddDbContext<SQLiteContext>();
            services.AddDbContext<ConfigContext>();
            services.AddControllers().AddNewtonsoftJson
                (s => s.SerializerSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver());

            services.AddAutoMapper
                (AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<ILiteRepo, SQLiteRepo>();
            services.AddScoped<IConfig, ConfigRepo>();
            services.AddRazorPages();
        }

        // Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //here we add midleware (order maters) 
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}
