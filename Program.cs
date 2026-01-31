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

        ApplicationConfiguration.Initialize();
        
        using var form = new MainForm();
        Application.Run(form);
    }
}
