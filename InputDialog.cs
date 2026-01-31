namespace Gnomicon;

/// <summary>
/// Simple input dialog for getting user input.
/// </summary>
public class InputDialog : Form
{
    private readonly Label _promptLabel;
    private readonly TextBox _inputTextBox;
    private readonly Button _okButton;
    private readonly Button _cancelButton;
    private string? _result;

    public string? Result => _result;

    public InputDialog(string prompt, string defaultValue = "")
    {
        _promptLabel = new Label
        {
            Text = prompt,
            AutoSize = true,
            Location = new Point(20, 20)
        };

        _inputTextBox = new TextBox
        {
            Text = defaultValue,
            Location = new Point(20, 50),
            Width = 300
        };

        _okButton = new Button
        {
            Text = "OK",
            DialogResult = DialogResult.OK,
            Location = new Point(20, 90),
            Width = 80
        };
        _okButton.Click += (s, e) => { _result = _inputTextBox.Text; Close(); };

        _cancelButton = new Button
        {
            Text = "Cancel",
            DialogResult = DialogResult.Cancel,
            Location = new Point(120, 90),
            Width = 80
        };
        _cancelButton.Click += (s, e) => { Close(); };

        Controls.Add(_promptLabel);
        Controls.Add(_inputTextBox);
        Controls.Add(_okButton);
        Controls.Add(_cancelButton);

        AcceptButton = _okButton;
        CancelButton = _cancelButton;

        Text = "Gnomicon";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(340, 140);
        MinimizeBox = false;
        MaximizeBox = false;
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        _inputTextBox.Focus();
        _inputTextBox.SelectAll();
    }
}
