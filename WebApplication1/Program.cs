global using LogicsLib;
global using Repos;
using LogicsLib.Services;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Filters;

namespace MyMakler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder().AddJsonFile("DbConfiguration.json").SetBasePath(Directory.GetCurrentDirectory()).Build();
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddJsonFile("ConstsConfiguration.json");
            builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
            builder.Services.AddScoped<ILogics, Logics>();
            // Add services to the container.
            builder.Services.AddHostedService<DetachedPicsService>();
            builder.Services.AddHostedService<OldAdsService>();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.ExampleFilters();
            });
            builder.Services.AddSwaggerExamplesFromAssemblyOf<GetAllAdsArgsExample>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseExceptionHandlerMiddleware();
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();
            app.Run();
        }
    }
}
