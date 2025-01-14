@* NotificationBell.razor *@
@using Syncfusion.Blazor.Notifications
@using Syncfusion.Blazor.Popups

<div class="position-relative">
    <SfBadge Content="@NotificationCount" Style="BadgeStyle.Primary">
        <button class="btn btn-light position-relative" @onclick="ToggleNotifications">
            <i class="bi bi-bell"></i>
        </button>
    </SfBadge>

    <SfPopup Target=".position-relative" IsModal="false" ShowCloseIcon="true" Width="300px" Visible="@ShowNotifications" @bind-Visible="ShowNotifications">
        <h6 class="px-3 py-2 border-bottom">Notifications</h6>
        @if (Notifications?.Any() == true)
        {
            <div class="list-group">
                @foreach (var notification in Notifications)
                {
                    <a href="#" class="list-group-item list-group-item-action">@notification</a>
                }
            </div>
        }
        else
        {
            <p class="px-3 py-2 text-muted">No new notifications</p>
        }
    </SfPopup>
</div>

@code {
    [Parameter] public List<string> Notifications { get; set; } = new();
    [Parameter] public int NotificationCount { get; set; } = 0;
    [Parameter] public EventCallback OnNotificationClick { get; set; }

    private bool ShowNotifications { get; set; } = false;

    private void ToggleNotifications()
    {
        ShowNotifications = !ShowNotifications;
        if (ShowNotifications)
        {
            OnNotificationClick.InvokeAsync(null); // Trigger callback if needed
        }
    }
}
