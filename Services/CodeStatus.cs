
using BlazorComponentBus;
using BlazorThreeJS.Solutions;
using FoundryRulesAndUnits.Units;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorThreeJS;

public class CodeStatus
{
    public string Version()
    {
        var version = GetType().Assembly.GetName().Version ?? new Version(0, 0, 0, 0);
        var ver = $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        return ver;
    }   
}

public static class BlazorThreeJSExtensions
{
    public static IServiceCollection AddBlazorThreeJSServices(this IServiceCollection services)
    {
        //services.AddSingleton<IEnvConfig>(provider => envConfig);
        //Mentor Services
        services.AddScoped<ComponentBus>();

        services.AddScoped<IThreeDService, ThreeDService>();

        services.AddScoped<IUnitSystem, UnitSystem>();
        return services;
    }


}
