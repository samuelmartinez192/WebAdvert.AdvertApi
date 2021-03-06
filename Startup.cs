using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AutoMapper;
using AdvertApi.Services;
using AdvertApi.HealthChecks;
using Microsoft.OpenApi.Models;
using Amazon.Util;
using Amazon.ServiceDiscovery;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc.Formatters;
using Amazon.ServiceDiscovery.Model;

namespace AdvertApi
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
            services.AddAutoMapper(typeof(Startup));
            services.AddTransient<IAdvertStorageService, DynamoDBAdvertStorage>();
            services.AddTransient<StorageHealthCheck>();
            services.AddCors(options =>
            {
                options.AddPolicy("AllOrigin", policy => policy.WithOrigins("*").AllowAnyHeader());
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Web Advertisement Apis",
                    Version = "version 1"

                });
            });

            services.AddControllers();
            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(url: "/swagger/v1/swagger.json", name: "Web Advert Api");
            });

            //await RegisterToCloudMap();

            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }

        //private async Task RegisterToCloudMap()
        //{
        //    const string serviceId = "srv-n36g7df6g3fxlapl";

        //    var instanceId = EC2InstanceMetadata.InstanceId;

        //    if (!String.IsNullOrEmpty(instanceId))
        //    {
        //        var ipv4 = EC2InstanceMetadata.PrivateIpAddress;
        //        var client = new AmazonServiceDiscoveryClient();
        //        await client.RegisterInstanceAsync(new RegisterInstanceRequest
        //        {
        //            InstanceId = instanceId,
        //            ServiceId = serviceId,
        //            Attributes = new Dictionary<string, string>() { { "AWS_INSTANCE_IPV4" , ipv4 },
        //            { "AWS_INSTANCE_PORT","80" } }
        //        });
        //    }
        //}
    }
}
