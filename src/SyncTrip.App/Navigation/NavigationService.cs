using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using SyncTrip.App.Core.Platform;

namespace SyncTrip.App.Navigation;

public partial class NavigationService : ObservableObject, INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Stack<object> _navigationStack = new();
    private readonly Dictionary<string, Type> _routes = new();

    [ObservableProperty]
    private object? currentViewModel;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void RegisterRoute(string route, Type viewModelType)
    {
        _routes[route] = viewModelType;
    }

    public Task NavigateToAsync(string route, Dictionary<string, string>? parameters = null)
    {
        if (!_routes.TryGetValue(route, out var vmType))
            throw new InvalidOperationException($"Route '{route}' not registered");

        var vm = _serviceProvider.GetRequiredService(vmType);

        if (parameters != null)
        {
            // Call Initialize method if it exists
            var initMethod = vmType.GetMethod("Initialize");
            if (initMethod != null)
            {
                var methodParams = initMethod.GetParameters();
                var args = new object?[methodParams.Length];
                for (int i = 0; i < methodParams.Length; i++)
                {
                    var paramName = methodParams[i].Name!;
                    if (parameters.TryGetValue(paramName, out var value))
                        args[i] = value;
                    else
                        args[i] = methodParams[i].HasDefaultValue ? methodParams[i].DefaultValue : null;
                }
                initMethod.Invoke(vm, args);
            }
        }

        if (CurrentViewModel != null)
            _navigationStack.Push(CurrentViewModel);

        CurrentViewModel = vm;
        return Task.CompletedTask;
    }

    public Task GoBackAsync()
    {
        if (_navigationStack.Count > 0)
            CurrentViewModel = _navigationStack.Pop();

        return Task.CompletedTask;
    }
}
