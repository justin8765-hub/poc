<SfDialog IsModal="true" Visible="@ShowDialog" Width="400px" Header="Notifications" ShowCloseIcon="true" @ref="NotificationDialog" @DialogClosed="OnDialogClosed">
    <DialogTemplates>
        <Content>
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
        </Content>
    </DialogTemplates>
</SfDialog>
