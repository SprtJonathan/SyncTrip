using Avalonia.Controls;
using Avalonia.Controls.Templates;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SyncTrip.App.Navigation;

public class ViewLocator : IDataTemplate
{
    public Control Build(object? data)
    {
        if (data is null)
            return new TextBlock { Text = "No VM provided" };

        var vmType = data.GetType();
        var vmName = vmType.FullName!;

        // Convention: XxxViewModel -> XxxView
        var viewName = vmName
            .Replace(".ViewModels.", ".Views.")
            .Replace("ViewModel", "View");

        var viewType = vmType.Assembly.GetType(viewName);

        if (viewType != null)
        {
            var view = (Control)Activator.CreateInstance(viewType)!;
            view.DataContext = data;
            return view;
        }

        return new TextBlock { Text = $"View not found: {viewName}" };
    }

    public bool Match(object? data)
    {
        return data is ObservableObject;
    }
}
