@page "/messages"

@using Syncfusion.Blazor.Notifications
@using Syncfusion.Blazor.Buttons
@using Syncfusion.Blazor.Lists
@using Syncfusion.Blazor.Navigations

<h3>Toggleable Notification Panel</h3>

<SfButton CssClass="e-primary" @onclick="TogglePanel">Show Notifications</SfButton>

<SfSidebar Target=".content" MediaQuery="(min-width: 600px)" Width="400px" IsOpen="@ShowPanel" DockSize="0" EnableDock="false" Position="SidebarPosition.Right">
    <div style="padding: 10px;">
        <h4>Notifications</h4>
        <SfListView DataSource="@Messages" ShowCheckBox="false">
            <ListViewTemplates>
                <Template>
                    <SfMessage Severity="MessageSeverity.Info" CssClass="e-info" ShowIcon="true">
                        <a href="@((context as MessageItem).Url)" target="_blank">@((context as MessageItem).Text)</a>
                    </SfMessage>
                </Template>
            </ListViewTemplates>
        </SfListView>
        @if (!Messages.Any())
        {
            <p>No messages to display.</p>
        }
    </div>
</SfSidebar>

<div class="content" style="margin-top: 20px;">
    <input type="text" @bind="NewMessageText" placeholder="Enter message text" />
    <input type="url" @bind="NewMessageUrl" placeholder="Enter message URL" />
    <SfButton CssClass="e-success" @onclick="AddMessage">Add Message</SfButton>
</div>

@code {
    private List<MessageItem> Messages = new()
    {
        new MessageItem { Text = "Notification 1", Url = "https://example.com/1" },
        new MessageItem { Text = "Notification 2", Url = "https://example.com/2" }
    };

    private string NewMessageText;
    private string NewMessageUrl;
    private bool ShowPanel = false;

    private void AddMessage()
    {
        if (!string.IsNullOrWhiteSpace(NewMessageText) && !string.IsNullOrWhiteSpace(NewMessageUrl))
        {
            Messages.Insert(0, new MessageItem { Text = NewMessageText, Url = NewMessageUrl });
            NewMessageText = string.Empty;
            NewMessageUrl = string.Empty;
        }
    }

    private void TogglePanel()
    {
        ShowPanel = !ShowPanel;
    }

    private class MessageItem
    {
        public string Text { get; set; }
        public string Url { get; set; }
    }
}
