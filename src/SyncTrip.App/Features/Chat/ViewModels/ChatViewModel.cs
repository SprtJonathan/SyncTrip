using System.Collections.ObjectModel;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SyncTrip.App.Core.Platform;
using SyncTrip.App.Core.Services;
using SyncTrip.Shared.DTOs.Chat;

namespace SyncTrip.App.Features.Chat.ViewModels;

public partial class ChatViewModel : ObservableObject
{
    private readonly IChatService _chatService;
    private readonly IConvoySignalRService _convoySignalRService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private string convoyId = string.Empty;

    [ObservableProperty]
    private ObservableCollection<MessageDto> messages = new();

    [ObservableProperty]
    private string messageText = string.Empty;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isSending;

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private bool hasMessages;

    [ObservableProperty]
    private bool canLoadMore;

    private DateTime? _oldestMessageDate;

    public ChatViewModel(IChatService chatService, IConvoySignalRService convoySignalRService, INavigationService navigationService)
    {
        _chatService = chatService;
        _convoySignalRService = convoySignalRService;
        _navigationService = navigationService;
    }

    public void Initialize(string convoyId)
    {
        ConvoyId = convoyId;
    }

    [RelayCommand]
    public async Task LoadMessages()
    {
        if (!Guid.TryParse(ConvoyId, out var cId))
            return;

        try
        {
            IsLoading = true;
            ErrorMessage = null;

            var messageList = await _chatService.GetMessagesAsync(cId);

            Messages.Clear();
            foreach (var msg in messageList)
                Messages.Add(msg);

            HasMessages = Messages.Count > 0;
            CanLoadMore = messageList.Count >= 50;

            if (Messages.Count > 0)
                _oldestMessageDate = Messages[Messages.Count - 1].SentAt;

            // Connect SignalR
            _convoySignalRService.MessageReceived += OnMessageReceived;
            await _convoySignalRService.ConnectAsync(cId);
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
    private async Task LoadMore()
    {
        if (!Guid.TryParse(ConvoyId, out var cId) || !_oldestMessageDate.HasValue)
            return;

        try
        {
            IsLoading = true;

            var olderMessages = await _chatService.GetMessagesAsync(cId, 50, _oldestMessageDate);

            foreach (var msg in olderMessages)
                Messages.Add(msg);

            CanLoadMore = olderMessages.Count >= 50;

            if (olderMessages.Count > 0)
                _oldestMessageDate = olderMessages[olderMessages.Count - 1].SentAt;
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
    private async Task SendMessage()
    {
        if (string.IsNullOrWhiteSpace(MessageText))
            return;

        if (!Guid.TryParse(ConvoyId, out var cId))
            return;

        try
        {
            IsSending = true;
            ErrorMessage = null;

            var request = new SendMessageRequest { Content = MessageText.Trim() };
            var messageId = await _chatService.SendMessageAsync(cId, request);

            if (messageId.HasValue)
            {
                MessageText = string.Empty;
            }
            else
            {
                ErrorMessage = "Impossible d'envoyer le message.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur: {ex.Message}";
        }
        finally
        {
            IsSending = false;
        }
    }

    private void OnMessageReceived(MessageDto message)
    {
        Dispatcher.UIThread.Post(() =>
        {
            Messages.Insert(0, message);
            HasMessages = true;
        });
    }

    public async Task Cleanup()
    {
        _convoySignalRService.MessageReceived -= OnMessageReceived;
        await _convoySignalRService.DisconnectAsync();
    }

    [RelayCommand]
    private async Task GoBack()
    {
        await Cleanup();
        await _navigationService.GoBackAsync();
    }
}
