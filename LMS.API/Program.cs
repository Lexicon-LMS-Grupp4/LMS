using LMS.API.Extensions;
using LMS.API.Services;
using LMS.Infractructure.Data;
using LMS.Infractructure.Repositories;

namespace LMS.API;

public class Program
{
    public static void Main(string[] args)
    {
        // testing PR
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);

        builder.Services.ConfigureSql(builder.Configuration);
        builder.Services.ConfigureControllers();

        builder.Services.AddRepositories();
        builder.Services.AddServiceLayer();

        builder.Services.AddScoped<ICourseRepository, CourseRepository>();
        builder.Services.AddScoped<IModuleRepository, ModuleRepository>();
        builder.Services.AddScoped<IActivityRepository, ActivityRepository>();
        builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();

        builder.Services.ConfigureAuthentication(builder.Configuration);
        builder.Services.ConfigureIdentity();

        builder.Services.AddHostedService<DataSeedHostingService>();
        builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MapperProfile>());
        builder.Services.ConfigureCors();
        builder.Services.ConfigureSwagger();


        var app = builder.Build();


        // Configure the HTTP request pipeline.
        app.ConfigureExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(opt =>
            {
                opt.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            });
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowAll");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
