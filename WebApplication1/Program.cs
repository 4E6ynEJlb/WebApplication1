global using LogicsLib;
global using Repos;
using LogicsLib.Services;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace MyMakler
{
    public class Program
    {
        public static void Main(string[] args)
        {            
            var builder = WebApplication.CreateBuilder();            
            builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));//   <*> ¹9
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
            app.UseFileServer(new FileServerOptions      //   <*> correct file routing
            {
                EnableDirectoryBrowsing = true,
                FileProvider = new PhysicalFileProvider(builder.Configuration.GetValue<string>("PicsDirectory")),
                RequestPath = new PathString("/pics"),
                EnableDefaultFiles = false
            });
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
