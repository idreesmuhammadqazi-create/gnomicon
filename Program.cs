namespace Gnomicon;

/// <summary>
/// Entry point for the Gnomicon application.
/// </summary>
internal static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // Ensure only one instance runs
        using var mutex = new Mutex(true, "Gnomicon_SingleInstance", out bool createdNew);
        
        if (!createdNew)
        {
            MessageBox.Show(
                "Gnomicon is already running.\n\n" +
                "Check the system tray for the Gnomicon icon.",
                "Gnomicon",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        // Configure Windows Forms application
        ApplicationConfiguration.Initialize();
        
        // Run the main form
        using var form = new MainForm();
        Application.Run(form);
    }
}
