<SfMaskedTextBox @bind-Value="DurationString"
                 Mask="##:##"
                 FloatLabelType="Auto"
                 Placeholder="Work Duration (hh:mm)"
                 PromptChar="_">
</SfMaskedTextBox>


@code {
    private MyModel model = new();

    private string DurationString
    {
        get => model.Duration.HasValue
            ? model.Duration.Value.ToString(@"hh\:mm")
            : string.Empty;

        set
        {
            if (string.IsNullOrWhiteSpace(value) || value.Contains("_"))
            {
                model.Duration = null;
            }
            else if (TimeSpan.TryParse(value, out var parsed))
            {
                model.Duration = parsed;
            }
        }
    }

    public class MyModel
    {
        public TimeSpan? Duration { get; set; }
    }
}
