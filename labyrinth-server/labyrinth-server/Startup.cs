using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace labyrinth_server
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


            services.AddCors(cors => cors.AddPolicy("Labyrinth", x =>
            {
                x.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));

            //https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/projects?tabs=dotnet-core-cli
            //https://stackoverflow.com/questions/55827106/when-configuring-retryonfailure-what-is-the-maxretrydelay-parameter
            services.AddEntityFrameworkNpgsql().AddDbContext<LabyrinthContext>(opt =>
            {
                opt.UseNpgsql(Configuration["POSTGRES"],
                    x =>
                    {
                        x.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                        x.EnableRetryOnFailure(10, TimeSpan.FromSeconds(5), new List<string>());
                    });
            });


            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo {Title = "Labyrinth", Version = "v1"});
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            InitializeDatabase(app);


            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseCors("Labyrinth");

            //app.UseSwagger();
            //app.UseSwaggerUI(x =>
            //{

            //    x.SwaggerEndpoint("/swagger/v1/swagger.json", "Labyrinth");
            //    x.OAuthClientId("Labyrinth");
            //});


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //app.UseMvc();
        }


        private void InitializeDatabase(IApplicationBuilder app)
        {
            //https://riptutorial.com/asp-net-core/example/17400/using-scoped-services-during-application-startup---database-seeding
            //https://github.com/aspnet/MusicStore/blob/1.0.0/src/MusicStore/Models/SampleData.cs#L22-L34
            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
            serviceScope.ServiceProvider.GetRequiredService<LabyrinthContext>().Database.Migrate();
        }
    }
}
