using SyncTrip.Mobile.Features.Profile.ViewModels;

namespace SyncTrip.Mobile.Features.Profile.Views;

/// <summary>
/// Page de profil utilisateur permettant d'afficher et de modifier les informations personnelles.
/// </summary>
public partial class ProfilePage : ContentPage
{
    private readonly ProfileViewModel _viewModel;
    private bool _isUpdatingCheckboxes;

    /// <summary>
    /// Initialise une nouvelle instance de la page de profil.
    /// </summary>
    /// <param name="viewModel">ViewModel de la page de profil.</param>
    public ProfilePage(ProfileViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    /// <summary>
    /// Gère l'apparition de la page et charge le profil.
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadProfileCommand.ExecuteAsync(null);
        UpdateCheckboxesFromViewModel();
    }

    /// <summary>
    /// Met à jour l'état des CheckBox en fonction des permis dans le ViewModel.
    /// </summary>
    private void UpdateCheckboxesFromViewModel()
    {
        _isUpdatingCheckboxes = true;

        CheckboxLicenseB.IsChecked = _viewModel.LicenseTypes.Contains(1);
        CheckboxLicenseA.IsChecked = _viewModel.LicenseTypes.Contains(2);
        CheckboxLicenseC.IsChecked = _viewModel.LicenseTypes.Contains(3);
        CheckboxLicenseD.IsChecked = _viewModel.LicenseTypes.Contains(4);

        _isUpdatingCheckboxes = false;
    }

    /// <summary>
    /// Gère le changement d'état du CheckBox Permis B (LicenseType = 1).
    /// </summary>
    private void OnLicenseB_CheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        if (_isUpdatingCheckboxes) return;

        if (e.Value)
            _viewModel.AddLicenseCommand.Execute(1);
        else
            _viewModel.RemoveLicenseCommand.Execute(1);
    }

    /// <summary>
    /// Gère le clic sur le label Permis B pour basculer la CheckBox.
    /// </summary>
    private void OnLicenseBLabel_Tapped(object? sender, EventArgs e)
    {
        if (_viewModel.IsEditing)
        {
            CheckboxLicenseB.IsChecked = !CheckboxLicenseB.IsChecked;
        }
    }

    /// <summary>
    /// Gère le changement d'état du CheckBox Permis A (LicenseType = 2).
    /// </summary>
    private void OnLicenseA_CheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        if (_isUpdatingCheckboxes) return;

        if (e.Value)
            _viewModel.AddLicenseCommand.Execute(2);
        else
            _viewModel.RemoveLicenseCommand.Execute(2);
    }

    /// <summary>
    /// Gère le clic sur le label Permis A pour basculer la CheckBox.
    /// </summary>
    private void OnLicenseALabel_Tapped(object? sender, EventArgs e)
    {
        if (_viewModel.IsEditing)
        {
            CheckboxLicenseA.IsChecked = !CheckboxLicenseA.IsChecked;
        }
    }

    /// <summary>
    /// Gère le changement d'état du CheckBox Permis C (LicenseType = 3).
    /// </summary>
    private void OnLicenseC_CheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        if (_isUpdatingCheckboxes) return;

        if (e.Value)
            _viewModel.AddLicenseCommand.Execute(3);
        else
            _viewModel.RemoveLicenseCommand.Execute(3);
    }

    /// <summary>
    /// Gère le clic sur le label Permis C pour basculer la CheckBox.
    /// </summary>
    private void OnLicenseCLabel_Tapped(object? sender, EventArgs e)
    {
        if (_viewModel.IsEditing)
        {
            CheckboxLicenseC.IsChecked = !CheckboxLicenseC.IsChecked;
        }
    }

    /// <summary>
    /// Gère le changement d'état du CheckBox Permis D (LicenseType = 4).
    /// </summary>
    private void OnLicenseD_CheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        if (_isUpdatingCheckboxes) return;

        if (e.Value)
            _viewModel.AddLicenseCommand.Execute(4);
        else
            _viewModel.RemoveLicenseCommand.Execute(4);
    }

    /// <summary>
    /// Gère le clic sur le label Permis D pour basculer la CheckBox.
    /// </summary>
    private void OnLicenseDLabel_Tapped(object? sender, EventArgs e)
    {
        if (_viewModel.IsEditing)
        {
            CheckboxLicenseD.IsChecked = !CheckboxLicenseD.IsChecked;
        }
    }
}
