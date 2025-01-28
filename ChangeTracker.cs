@page "/edit-save-toggle"
@using Syncfusion.Blazor.Inputs

<div class="container mt-3">
    <div class="d-flex justify-content-end align-items-center">
        <SfSwitch @bind-Checked="isEditMode" ValueChange="OnValueChange">
            <SwitchOnLabelTemplate>
                <i class="bi bi-save"></i> Save
            </SwitchOnLabelTemplate>
            <SwitchOffLabelTemplate>
                <i class="bi bi-pencil"></i> Edit
            </SwitchOffLabelTemplate>
        </SfSwitch>
    </div>
</div>

@code {
    private bool isEditMode = false;

    private void OnValueChange(Syncfusion.Blazor.Inputs.ChangeEventArgs<bool> args)
    {
        if (!args.Value)
        {
            // Call Save method when toggling to "Save"
            SaveChanges();
        }
    }

    private void SaveChanges()
    {
        // Add your save logic here
        Console.WriteLine("Changes saved!");
    }
}
