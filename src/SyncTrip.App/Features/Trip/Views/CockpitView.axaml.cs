using Avalonia.Controls;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling;
using SyncTrip.App.Features.Trip.ViewModels;

namespace SyncTrip.App.Features.Trip.Views;

public partial class CockpitView : UserControl
{
    private WritableLayer? _positionsLayer;

    public CockpitView()
    {
        InitializeComponent();
    }

    protected override async void OnAttachedToVisualTree(Avalonia.VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        InitializeMap();

        if (DataContext is CockpitViewModel vm)
        {
            vm.PositionsUpdated += OnPositionsUpdated;
            await vm.LoadTripCommand.ExecuteAsync(null);
        }
    }

    protected override void OnDetachedFromVisualTree(Avalonia.VisualTreeAttachmentEventArgs e)
    {
        if (DataContext is CockpitViewModel vm)
        {
            vm.PositionsUpdated -= OnPositionsUpdated;
            _ = vm.StopTracking();
        }

        base.OnDetachedFromVisualTree(e);
    }

    private void InitializeMap()
    {
        var map = MapControl.Map;
        map.Layers.Add(OpenStreetMap.CreateTileLayer());

        _positionsLayer = new WritableLayer { Style = null };
        map.Layers.Add(_positionsLayer);

        // Center on France by default
        var (x, y) = SphericalMercator.FromLonLat(2.3522, 48.8566);
        map.Navigator.CenterOnAndZoomTo(new MPoint(x, y), map.Navigator.Resolutions[10]);
    }

    private void OnPositionsUpdated()
    {
        if (DataContext is not CockpitViewModel vm || _positionsLayer is null) return;

        _positionsLayer.Clear();

        // User position
        if (vm.UserLatitude != 0 || vm.UserLongitude != 0)
        {
            var (ux, uy) = SphericalMercator.FromLonLat(vm.UserLongitude, vm.UserLatitude);
            _positionsLayer.Add(new PointFeature(new MPoint(ux, uy))
            {
                Styles = { new SymbolStyle { Fill = new Brush(Color.FromString("#512BD4")), SymbolScale = 0.5 } }
            });
        }

        // Member positions
        foreach (var member in vm.MemberPositions)
        {
            var (mx, my) = SphericalMercator.FromLonLat(member.Longitude, member.Latitude);
            _positionsLayer.Add(new PointFeature(new MPoint(mx, my))
            {
                Styles = { new SymbolStyle { Fill = new Brush(Color.FromString("#28A745")), SymbolScale = 0.4 } }
            });
        }

        _positionsLayer.DataHasChanged();
    }
}
