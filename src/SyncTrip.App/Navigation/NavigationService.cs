using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SyncTrip.App.Core.Platform;

namespace SyncTrip.App.Navigation;

public partial class NavigationService : ObservableObject, INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Stack<object> _navigationStack = new();
    private readonly Dictionary<string, Type> _routes = new();
    private readonly Stack<string> _routeStack = new();

    private static readonly Dictionary<string, string> RouteTitles = new()
    {
        ["convoydetail"] = "Detail du convoi",
        ["cockpit"] = "Cockpit",
        ["voting"] = "Vote",
        ["chat"] = "Chat",
        ["addvehicle"] = "Ajouter un vehicule",
        ["createconvoy"] = "Creer un convoi",
        ["joinconvoy"] = "Rejoindre un convoi",
        ["registration"] = "Inscription"
    };

    [ObservableProperty]
    private object? currentViewModel;

    [ObservableProperty]
    private bool canGoBack;

    [ObservableProperty]
    private string pageTitle = string.Empty;

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

        _routeStack.Push(route);
        CurrentViewModel = vm;
        CanGoBack = _navigationStack.Count > 0 && route != "login" && route != "main";
        PageTitle = RouteTitles.GetValueOrDefault(route, string.Empty);
        return Task.CompletedTask;
    }

    public Task GoBackAsync()
    {
        if (_navigationStack.Count > 0)
        {
            CurrentViewModel = _navigationStack.Pop();
            if (_routeStack.Count > 0)
                _routeStack.Pop();
        }

        var currentRoute = _routeStack.Count > 0 ? _routeStack.Peek() : string.Empty;
        CanGoBack = _navigationStack.Count > 0 && currentRoute != "login" && currentRoute != "main";
        PageTitle = RouteTitles.GetValueOrDefault(currentRoute, string.Empty);
        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task GoBack()
    {
        await GoBackAsync();
    }
}
