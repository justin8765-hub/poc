@page "/edit-save-toggle"

<div class="d-flex align-items-center">
    <button class="btn btn-primary d-flex align-items-center" @onclick="ToggleEditSave">
        <i class="bi @(isEditMode ? "bi-save" : "bi-pencil") me-2"></i>
        @(isEditMode ? "Save" : "Edit")
    </button>
</div>

@code {
    private bool isEditMode = false;

    private void ToggleEditSave()
    {
        if (isEditMode)
        {
            // Call your save logic
            SaveChanges();
        }

        isEditMode = !isEditMode; // Toggle between edit and save modes
    }

    private void SaveChanges()
    {
        // Add your save logic here
        Console.WriteLine("Changes saved!");
    }
}
