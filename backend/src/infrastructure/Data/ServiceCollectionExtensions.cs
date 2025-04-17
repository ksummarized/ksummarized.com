using Microsoft.Extensions.DependencyInjection;
using core.Ports;

namespace infrastructure.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTodoServices(this IServiceCollection services)
    {
        services.AddScoped<IListService, ListService>();
        services.AddScoped<IItemService, ItemService>();
        services.AddScoped<ITagService, TagService>();

        return services;
    }
}
