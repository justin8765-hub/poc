<FormButtons>
    <div class="d-flex justify-content-end align-items-center gap-3">
        @if (ShowSavedInfo)
        {
            <div class="d-flex align-items-center" style="height: 40px;">
                <SfMessage Severity="MessageSeverity.Success">Task Saved!</SfMessage>
            </div>
        }

        <div style="height: 40px;">
            <SfProgressButton Content="Save"
                              IsPrimary="true"
                              Duration="4000"
                              OnClick="SaveProgressButton" />
        </div>
    </div>
</FormButtons>
