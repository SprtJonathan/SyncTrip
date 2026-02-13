using System.Collections.ObjectModel;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SyncTrip.App.Core.Platform;
using SyncTrip.App.Core.Services;
using SyncTrip.Shared.DTOs.Voting;

namespace SyncTrip.App.Features.Voting.ViewModels;

public partial class VotingViewModel : ObservableObject
{
    private readonly IVotingService _votingService;
    private readonly ISignalRService _signalRService;
    private readonly INavigationService _navigationService;
    private DispatcherTimer? _countdownTimer;

    [ObservableProperty]
    private string convoyId = string.Empty;

    [ObservableProperty]
    private string tripId = string.Empty;

    [ObservableProperty]
    private StopProposalDto? activeProposal;

    [ObservableProperty]
    private bool hasActiveProposal;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private string? successMessage;

    [ObservableProperty]
    private string countdown = string.Empty;

    [ObservableProperty]
    private int selectedStopType = 1;

    [ObservableProperty]
    private string locationName = string.Empty;

    [ObservableProperty]
    private double latitude;

    [ObservableProperty]
    private double longitude;

    public ObservableCollection<StopTypeItem> StopTypes { get; } = new()
    {
        new StopTypeItem { Id = 1, Name = "Carburant" },
        new StopTypeItem { Id = 2, Name = "Pause" },
        new StopTypeItem { Id = 3, Name = "Repas" },
        new StopTypeItem { Id = 4, Name = "Photo" }
    };

    [ObservableProperty]
    private StopTypeItem? selectedStopTypeItem;

    public VotingViewModel(IVotingService votingService, ISignalRService signalRService, INavigationService navigationService)
    {
        _votingService = votingService;
        _signalRService = signalRService;
        _navigationService = navigationService;
        SelectedStopTypeItem = StopTypes[0];
    }

    public void Initialize(string convoyId, string tripId)
    {
        ConvoyId = convoyId;
        TripId = tripId;
    }

    [RelayCommand]
    public async Task LoadActiveProposal()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;

            if (!Guid.TryParse(ConvoyId, out var cId) || !Guid.TryParse(TripId, out var tId))
                return;

            var proposal = await _votingService.GetActiveProposalAsync(cId, tId);
            SetActiveProposal(proposal);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ProposeStop()
    {
        if (string.IsNullOrWhiteSpace(LocationName))
        {
            ErrorMessage = "Veuillez entrer un nom de lieu.";
            return;
        }

        if (!Guid.TryParse(ConvoyId, out var cId) || !Guid.TryParse(TripId, out var tId))
            return;

        try
        {
            IsLoading = true;
            ErrorMessage = null;
            SuccessMessage = null;

            var request = new ProposeStopRequest
            {
                StopType = SelectedStopTypeItem?.Id ?? 1,
                Latitude = Latitude,
                Longitude = Longitude,
                LocationName = LocationName
            };

            var proposalId = await _votingService.ProposeStopAsync(cId, tId, request);
            if (proposalId.HasValue)
            {
                SuccessMessage = "Proposition soumise au vote !";
                LocationName = string.Empty;
                await LoadActiveProposal();
            }
            else
            {
                ErrorMessage = "Impossible de creer la proposition.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task VoteYes()
    {
        await CastVote(true);
    }

    [RelayCommand]
    private async Task VoteNo()
    {
        await CastVote(false);
    }

    private async Task CastVote(bool isYes)
    {
        if (ActiveProposal is null) return;
        if (!Guid.TryParse(ConvoyId, out var cId) || !Guid.TryParse(TripId, out var tId))
            return;

        try
        {
            IsLoading = true;
            ErrorMessage = null;

            var request = new CastVoteRequest { IsYes = isYes };
            await _votingService.CastVoteAsync(cId, tId, ActiveProposal.Id, request);
            SuccessMessage = isYes ? "Vote OUI enregistre !" : "Vote NON enregistre !";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    public void SubscribeToSignalR()
    {
        _signalRService.StopProposed += OnStopProposed;
        _signalRService.VoteUpdated += OnVoteUpdated;
        _signalRService.ProposalResolved += OnProposalResolved;
    }

    public void UnsubscribeFromSignalR()
    {
        _signalRService.StopProposed -= OnStopProposed;
        _signalRService.VoteUpdated -= OnVoteUpdated;
        _signalRService.ProposalResolved -= OnProposalResolved;
        _countdownTimer?.Stop();
    }

    private void OnStopProposed(StopProposalDto proposal)
    {
        Dispatcher.UIThread.Post(() => SetActiveProposal(proposal));
    }

    private void OnVoteUpdated(Guid proposalId, int yesCount, int noCount)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (ActiveProposal is not null && ActiveProposal.Id == proposalId)
            {
                // Refresh to get updated counts
                _ = LoadActiveProposal();
            }
        });
    }

    private void OnProposalResolved(StopProposalDto proposal)
    {
        Dispatcher.UIThread.Post(() =>
        {
            _countdownTimer?.Stop();
            SetActiveProposal(null);
            SuccessMessage = proposal.Status == 2
                ? "Arret accepte !"
                : "Proposition rejetee.";
        });
    }

    private void SetActiveProposal(StopProposalDto? proposal)
    {
        ActiveProposal = proposal;
        HasActiveProposal = proposal is not null;

        _countdownTimer?.Stop();
        if (proposal is not null && proposal.Status == 1)
        {
            _countdownTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _countdownTimer.Tick += (s, e) =>
            {
                var remaining = proposal.ExpiresAt - DateTime.UtcNow;
                if (remaining <= TimeSpan.Zero)
                {
                    Countdown = "00:00";
                    _countdownTimer?.Stop();
                }
                else
                {
                    Countdown = remaining.ToString(@"mm\:ss");
                }
            };
            _countdownTimer.Start();
        }
    }

    [RelayCommand]
    private async Task GoBack()
    {
        await _navigationService.GoBackAsync();
    }

    public class StopTypeItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
