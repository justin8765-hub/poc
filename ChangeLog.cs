@page "/messages"

@using Syncfusion.Blazor.Notifications
@using Syncfusion.Blazor.Buttons
@using Syncfusion.Blazor.Lists
@using Syncfusion.Blazor.Navigations
@inject NavigationManager Navigation

<h3 class="mb-3">Toggleable Notification Panel</h3>

<SfButton CssClass="e-primary mb-3" @onclick="ToggleSidebar">Show Notifications</SfButton>

<SfSidebar Target="body" IsOpen="@ShowSidebar" EnableDock="false" DockSize="0" Width="25%" Position="SidebarPosition.Right" Type="SidebarType.Over">
    <div class="p-3">
        <h4 class="mb-3">Notifications</h4>
        <SfListView DataSource="@Messages" ShowCheckBox="false">
            <ListViewTemplates>
                <Template>
                    <SfMessage Severity="MessageSeverity.Info" CssClass="e-info mb-2" ShowIcon="true">
                        <a href="#" @onclick="() => NavigateToLink((context as MessageItem).Url)" class="text-decoration-none">@((context as MessageItem).Text)</a>
                    </SfMessage>
                </Template>
            </ListViewTemplates>
        </SfListView>
        @if (!Messages.Any())
        {
            <p class="text-muted">No messages to display.</p>
        }
    </div>
</SfSidebar>

<div class="content mt-4">
    <div class="mb-3">
        <input type="text" @bind="NewMessageText" placeholder="Enter message text" class="form-control mb-2" />
        <input type="url" @bind="NewMessageUrl" placeholder="Enter message URL" class="form-control mb-2" />
        <SfButton CssClass="e-success" @onclick="AddMessage">Add Message</SfButton>
    </div>
</div>

@code {
    private List<MessageItem> Messages = new()
    {
        new MessageItem { Text = "Notification 1", Url = "https://example.com/1" },
        new MessageItem { Text = "Notification 2", Url = "https://example.com/2" }
    };

    private string NewMessageText;
    private string NewMessageUrl;
    private bool ShowSidebar = false;

    private void AddMessage()
    {
        if (!string.IsNullOrWhiteSpace(NewMessageText) && !string.IsNullOrWhiteSpace(NewMessageUrl))
        {
            Messages.Insert(0, new MessageItem { Text = NewMessageText, Url = NewMessageUrl });
            NewMessageText = string.Empty;
            NewMessageUrl = string.Empty;
        }
    }

    private void ToggleSidebar()
    {
        ShowSidebar = !ShowSidebar;
    }

    private void NavigateToLink(string url)
    {
        if (!string.IsNullOrWhiteSpace(url))
        {
            Navigation.NavigateTo(url);
        }
    }

    private class MessageItem
    {
        public string Text { get; set; }
        public string Url { get; set; }
    }
}
