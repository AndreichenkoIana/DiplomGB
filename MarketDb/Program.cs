
using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using MarketDb.Abstraction;
using MarketDb.Data;
using MarketDb.Repo;
using Microsoft.Extensions.FileProviders;
using OfficeOpenXml;

namespace MarketDb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAutoMapper(typeof(MappingProFile));

            builder.Services.AddMemoryCache(options =>
            {
                options.TrackStatistics = true;
            });
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

            builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
            {
                var connectionString = config.GetConnectionString("db");

                containerBuilder.Register(c => new ProductsContext(connectionString ?? ""))
                                .InstancePerDependency();

                containerBuilder.RegisterType<ProductRepository>().As<IProductRepository>().InstancePerLifetimeScope();
                containerBuilder.RegisterType<GroupRepository>().As<IGroupRepository>().InstancePerLifetimeScope();
                containerBuilder.RegisterType<StoreRepository>().As<IStoreRepository>().InstancePerLifetimeScope();

            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            var staticFilePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles");
            Directory.CreateDirectory(staticFilePath);

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(staticFilePath),
                RequestPath = "/static"
            });


            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();

        }
    }
}
