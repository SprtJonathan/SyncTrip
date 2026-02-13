using Mapsui;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling;
using SyncTrip.Mobile.Features.Trip.ViewModels;

namespace SyncTrip.Mobile.Features.Trip.Views;

/// <summary>
/// Page cockpit avec carte Mapsui et positions GPS temps réel.
/// </summary>
public partial class CockpitPage : ContentPage
{
    private readonly CockpitViewModel _viewModel;
    private WritableLayer? _membersLayer;
    private MyLocationLayer? _myLocationLayer;

    public CockpitPage(CockpitViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        InitializeMap();
    }

    private void InitializeMap()
    {
        var map = MapControl.Map;
        if (map is null) return;

        // Tuiles OpenStreetMap
        map.Layers.Add(OpenStreetMap.CreateTileLayer());

        // Couche pour la position de l'utilisateur
        _myLocationLayer = new MyLocationLayer(map);
        _myLocationLayer.IsCentered = true;
        map.Layers.Add(_myLocationLayer);

        // Couche pour les positions des membres
        _membersLayer = new WritableLayer { Name = "Members" };
        map.Layers.Add(_membersLayer);

        _viewModel.PositionsUpdated += UpdateMap;
    }

    private void UpdateMap()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            // Mettre à jour la position de l'utilisateur
            if (_myLocationLayer is not null && (_viewModel.UserLatitude != 0 || _viewModel.UserLongitude != 0))
            {
                _myLocationLayer.UpdateMyLocation(new MPoint(_viewModel.UserLongitude, _viewModel.UserLatitude));
            }

            // Mettre à jour les positions des membres
            if (_membersLayer is not null)
            {
                _membersLayer.Clear();

                foreach (var member in _viewModel.MemberPositions)
                {
                    var (x, y) = SphericalMercator.FromLonLat(member.Longitude, member.Latitude);
                    var feature = new PointFeature(new MPoint(x, y));
                    feature.Styles.Add(new SymbolStyle
                    {
                        SymbolScale = 0.4,
                        Fill = new Mapsui.Styles.Brush(Mapsui.Styles.Color.FromString("#512BD4"))
                    });
                    _membersLayer.Add(feature);
                }

                MapControl.Map?.Refresh();
            }
        });
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadTripCommand.ExecuteAsync(null);
    }

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.PositionsUpdated -= UpdateMap;
        await _viewModel.StopTracking();
    }
}
