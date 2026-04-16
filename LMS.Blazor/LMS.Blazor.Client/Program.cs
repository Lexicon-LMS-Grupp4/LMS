using BlazorBlueprint.Components;
using LMS.Blazor.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace LMS.Blazor.Client;

internal class Program
{
    static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        builder.Services.AddAuthorizationCore();
        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddAuthenticationStateDeserialization();
        builder.Services.AddBlazorBlueprintComponents();

        builder.Services.AddScoped(sp => new HttpClient
        {
            BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
        });

        builder.Services.AddScoped<IApiService, ClientApiService>();
        builder.Services.AddScoped<ICourseService, CourseService>();
        builder.Services.AddScoped<IModuleService, ModuleService>();
        builder.Services.AddScoped<IActivityTypeService, ActivityTypeService>();
        builder.Services.AddScoped<IActivityService, ActivityService>();
        builder.Services.AddScoped<ISubmissionService, SubmissionService>();
        builder.Services.AddScoped<IDocumentService, DocumentService>();
        builder.Services.AddScoped<IUserService, UserService>();

        await builder.Build().RunAsync();
    }
}
