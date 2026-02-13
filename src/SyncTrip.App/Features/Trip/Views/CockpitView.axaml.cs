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
    private WritableLayer? _waypointsLayer;

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
            vm.WaypointsLoaded += OnWaypointsLoaded;
            await vm.LoadTripCommand.ExecuteAsync(null);
        }
    }

    protected override void OnDetachedFromVisualTree(Avalonia.VisualTreeAttachmentEventArgs e)
    {
        if (DataContext is CockpitViewModel vm)
        {
            vm.PositionsUpdated -= OnPositionsUpdated;
            vm.WaypointsLoaded -= OnWaypointsLoaded;
            _ = vm.StopTracking();
        }

        base.OnDetachedFromVisualTree(e);
    }

    private void InitializeMap()
    {
        var map = MapControl.Map;
        map.Layers.Add(OpenStreetMap.CreateTileLayer());

        _waypointsLayer = new WritableLayer { Style = null };
        map.Layers.Add(_waypointsLayer);

        _positionsLayer = new WritableLayer { Style = null };
        map.Layers.Add(_positionsLayer);

        // Center on France by default
        var (x, y) = SphericalMercator.FromLonLat(2.3522, 48.8566);
        map.Navigator.CenterOnAndZoomTo(new MPoint(x, y), map.Navigator.Resolutions[10]);
    }

    private void OnWaypointsLoaded()
    {
        if (DataContext is not CockpitViewModel vm || _waypointsLayer is null || vm.Trip is null) return;

        _waypointsLayer.Clear();

        foreach (var waypoint in vm.Trip.Waypoints)
        {
            var (wx, wy) = SphericalMercator.FromLonLat(waypoint.Longitude, waypoint.Latitude);

            var (color, scale) = waypoint.Type switch
            {
                1 => ("#28A745", 0.5),  // Start — green
                2 => ("#FFA500", 0.5),  // Stopover — orange
                3 => ("#DC3545", 0.7),  // Destination — red, larger
                _ => ("#6C757D", 0.4)
            };

            var feature = new PointFeature(new MPoint(wx, wy))
            {
                Styles =
                {
                    new SymbolStyle
                    {
                        Fill = new Brush(Color.FromString(color)),
                        SymbolScale = scale,
                        SymbolType = SymbolType.Ellipse
                    },
                    new LabelStyle
                    {
                        Text = waypoint.Name,
                        ForeColor = Color.FromString("#333333"),
                        BackColor = new Brush(Color.White),
                        HorizontalAlignment = LabelStyle.HorizontalAlignmentEnum.Center,
                        Offset = new Offset(0, -25),
                        Font = new Font { Size = 14, Bold = true }
                    }
                }
            };

            _waypointsLayer.Add(feature);
        }

        _waypointsLayer.DataHasChanged();

        CenterMapOnContent(vm);
    }

    private void CenterMapOnContent(CockpitViewModel vm)
    {
        var points = new List<MPoint>();

        // Add user position if available
        if (vm.UserLatitude != 0 || vm.UserLongitude != 0)
        {
            var (ux, uy) = SphericalMercator.FromLonLat(vm.UserLongitude, vm.UserLatitude);
            points.Add(new MPoint(ux, uy));
        }

        // Add waypoints
        if (vm.Trip is not null)
        {
            foreach (var wp in vm.Trip.Waypoints)
            {
                var (wx, wy) = SphericalMercator.FromLonLat(wp.Longitude, wp.Latitude);
                points.Add(new MPoint(wx, wy));
            }
        }

        if (points.Count == 0) return;

        if (points.Count == 1)
        {
            MapControl.Map.Navigator.CenterOnAndZoomTo(points[0], MapControl.Map.Navigator.Resolutions[13]);
            return;
        }

        var minX = points.Min(p => p.X);
        var maxX = points.Max(p => p.X);
        var minY = points.Min(p => p.Y);
        var maxY = points.Max(p => p.Y);

        var padding = Math.Max(maxX - minX, maxY - minY) * 0.2;
        var extent = new MRect(minX - padding, minY - padding, maxX + padding, maxY + padding);

        MapControl.Map.Navigator.ZoomToBox(extent);
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
                Styles =
                {
                    new SymbolStyle
                    {
                        Fill = new Brush(Color.FromString("#512BD4")),
                        SymbolScale = 0.5,
                        SymbolType = SymbolType.Ellipse
                    },
                    new LabelStyle
                    {
                        Text = "Moi",
                        ForeColor = Color.FromString("#512BD4"),
                        BackColor = new Brush(Color.White),
                        HorizontalAlignment = LabelStyle.HorizontalAlignmentEnum.Center,
                        Offset = new Offset(0, -20),
                        Font = new Font { Size = 12, Bold = true }
                    }
                }
            });
        }

        // Member positions
        foreach (var member in vm.MemberPositions)
        {
            var (mx, my) = SphericalMercator.FromLonLat(member.Longitude, member.Latitude);
            _positionsLayer.Add(new PointFeature(new MPoint(mx, my))
            {
                Styles =
                {
                    new SymbolStyle
                    {
                        Fill = new Brush(Color.FromString("#28A745")),
                        SymbolScale = 0.4,
                        SymbolType = SymbolType.Ellipse
                    }
                }
            });
        }

        _positionsLayer.DataHasChanged();
    }
}
