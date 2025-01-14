@page "/badge"
@using Syncfusion.Blazor.Notifications

<SfBadge CssClass="e-badge-info" Value="@badgeCount" Content="@badgeCount" Position="TopRight">
    <button @onclick="IncrementBadge" class="e-btn">
        <i class="e-icons e-bell"></i>
    </button>
</SfBadge>

@code {
    private int badgeCount = 0;

    private void IncrementBadge()
    {
        badgeCount++;
    }
}
