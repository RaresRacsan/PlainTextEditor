using System.Runtime.InteropServices;
using System.IO;
using System.Drawing; // <----- Addition: Required for CustomColorTable
using System.Windows.Forms; // <----- Addition: Required for CustomColorTable

namespace PlainTextEditor
{
    public partial class PlainTextEditor : Form
    {
        private StatusStrip statusStrip;
        private ToolStripStatusLabel toolStripStatusLabelWordCount;
        private ToolStripStatusLabel toolStripStatusLabelCharCount;
        private string currentFilePath = null;
        private string originalFileContent = string.Empty;
        private int words = 0;
        private int characters = 0;

        ToolStripMenuItem sizeToolStripMenuItem = new ToolStripMenuItem("Size");


        public PlainTextEditor()
        {
            InitializeComponent();
            InitializeStatusStrip();
            editTextSize();
            UpdateTitle();
            SetDarkTheme();
            AssignCustomRenderer(); // <----- Addition: Assign custom renderer after initializing components
            UpdateStatusCounts();

        }

        private void InitializeStatusStrip()
        {
            statusStrip = new StatusStrip();
            toolStripStatusLabelWordCount = new ToolStripStatusLabel { Text = "Words: 0" };
            toolStripStatusLabelCharCount = new ToolStripStatusLabel { Text = "Characters: 0" };

            statusStrip.Items.Add(toolStripStatusLabelWordCount);
            statusStrip.Items.Add(toolStripStatusLabelCharCount);

            this.Controls.Add(statusStrip);

            // Set initial theme
            if (IsDarkTheme())
            {
                statusStrip.BackColor = Color.FromArgb(40, 40, 40);
                statusStrip.ForeColor = Color.White;
            }
            else
            {
                statusStrip.BackColor = Color.LightGray;
                statusStrip.ForeColor = Color.Black;
            }
        }

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE dwAttribute, ref uint pvAttribute, uint cbAttribute);

        public enum DWMWINDOWATTRIBUTE
        {
            DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
        }

        private void SetTitleBarColor()
        {
            uint value = 1; // Enable dark mode for the title bar
            DwmSetWindowAttribute(this.Handle, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, ref value, (uint)Marshal.SizeOf(value));
        }

        private void UpdateTitle()
        {
            string fileName = string.IsNullOrEmpty(currentFilePath) ? "New File" : Path.GetFileName(currentFilePath);
            this.Text = $"PlainTextEditor - {fileName}";
        }

        private void SaveFile()
        {
            File.WriteAllText(currentFilePath, textBoxMain.Text);
            originalFileContent = textBoxMain.Text;
            UpdateTitle();
        }

        private void SaveAs()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                currentFilePath = saveFileDialog.FileName;
                File.WriteAllText(currentFilePath, textBoxMain.Text);
                originalFileContent = textBoxMain.Text;
                UpdateTitle();
            }
        }

        private void SetLightTheme()
        {
            this.BackColor = Color.White;
            this.ForeColor = Color.Black;
            textBoxMain.BackColor = Color.White;
            textBoxMain.ForeColor = Color.Black;
            textBoxMain.BorderStyle = BorderStyle.None;

            menuStrip.BackColor = Color.LightGray;
            menuStrip.ForeColor = Color.Black;

            sizeToolStripMenuItem.BackColor = Color.White;
            sizeToolStripMenuItem.ForeColor = menuStrip.ForeColor;

            foreach (ToolStripItem item in sizeToolStripMenuItem.DropDownItems)
            {
                item.BackColor = Color.White;
                item.ForeColor = menuStrip.ForeColor;
            }

            // Set background color for white theme (light theme)
            // Commented out the explicit white background to maintain consistency
            // editToolStripMenuItem.BackColor = Color.FromArgb(255, 255, 255);  // White background
            aToolStripMenuItem.BackColor = Color.FromArgb(255, 255, 255);  // White background
            themeToolStripMenuItem.BackColor = Color.FromArgb(255, 255, 255);  // White background
            lightThemeToolStripMenuItem.BackColor = Color.FromArgb(255, 255, 255);  // White background
            darkThemeToolStripMenuItem.BackColor = Color.FromArgb(255, 255, 255);  // White background
            saveAsToolStripMenuItem.BackColor = Color.FromArgb(255, 255, 255);  // White background
            newToolStripMenuItem.BackColor = Color.FromArgb(255, 255, 255);  // White background
            saveToolStripMenuItem.BackColor = Color.FromArgb(255, 255, 255);  // White background
            exitToolStripMenuItem.BackColor = Color.FromArgb(255, 255, 255);  // White background
            openToolStripMenuItem.BackColor = Color.FromArgb(255, 255, 255);  // White background

            // Set foreground color (text color) for white theme (light theme)
            editToolStripMenuItem.ForeColor = Color.Black;  // Black text
            aToolStripMenuItem.ForeColor = Color.Black;  // Black text
            themeToolStripMenuItem.ForeColor = Color.Black;  // Black text
            lightThemeToolStripMenuItem.ForeColor = Color.Black;  // Black text
            darkThemeToolStripMenuItem.ForeColor = Color.Black;  // Black text
            saveAsToolStripMenuItem.ForeColor = Color.Black;  // Black text
            newToolStripMenuItem.ForeColor = Color.Black;  // Black text
            saveToolStripMenuItem.ForeColor = Color.Black;  // Black text
            exitToolStripMenuItem.ForeColor = Color.Black;  // Black text
            openToolStripMenuItem.ForeColor = Color.Black;  // Black text

            // <----- Addition: Ensure Edit button matches the MenuStrip background in light mode ----->
            editToolStripMenuItem.BackColor = menuStrip.BackColor;  // Set to LightGray to match MenuStrip
                                                                    // <----- Addition ends ----->

            AssignCustomRenderer(); // <----- Addition: Update renderer when theme changes

            statusStrip.BackColor = Color.LightGray;
            statusStrip.ForeColor = Color.Black;
            toolStripStatusLabelWordCount.ForeColor = Color.Black;
            toolStripStatusLabelCharCount.ForeColor = Color.Black;
        }

        private void SetDarkTheme()
        {
            SetTitleBarColor();

            this.BackColor = Color.FromArgb(30, 30, 30);
            this.ForeColor = Color.FromArgb(30, 30, 30);
            textBoxMain.BackColor = Color.FromArgb(30, 30, 30);
            textBoxMain.ForeColor = Color.White;
            textBoxMain.BorderStyle = BorderStyle.None;

            menuStrip.BackColor = Color.FromArgb(40, 40, 40);
            menuStrip.ForeColor = Color.White;

            sizeToolStripMenuItem.BackColor = menuStrip.BackColor;
            sizeToolStripMenuItem.ForeColor = menuStrip.ForeColor;

            foreach (ToolStripItem item in sizeToolStripMenuItem.DropDownItems)
            {
                item.BackColor = menuStrip.BackColor;
                item.ForeColor = menuStrip.ForeColor;
            }

            editToolStripMenuItem.BackColor = Color.FromArgb(40, 40, 40);
            aToolStripMenuItem.BackColor = Color.FromArgb(40, 40, 40);
            themeToolStripMenuItem.BackColor = Color.FromArgb(40, 40, 40);
            lightThemeToolStripMenuItem.BackColor = Color.FromArgb(40, 40, 40);
            darkThemeToolStripMenuItem.BackColor = Color.FromArgb(40, 40, 40);
            saveAsToolStripMenuItem.BackColor = Color.FromArgb(40, 40, 40);
            newToolStripMenuItem.BackColor = Color.FromArgb(40, 40, 40);
            saveToolStripMenuItem.BackColor = Color.FromArgb(40, 40, 40);
            exitToolStripMenuItem.BackColor = Color.FromArgb(40, 40, 40);
            openToolStripMenuItem.BackColor = Color.FromArgb(40, 40, 40);
            editToolStripMenuItem.ForeColor = Color.White;
            aToolStripMenuItem.ForeColor = Color.White;
            themeToolStripMenuItem.ForeColor = Color.White;
            lightThemeToolStripMenuItem.ForeColor = Color.White;
            darkThemeToolStripMenuItem.ForeColor = Color.White;
            saveAsToolStripMenuItem.ForeColor = Color.White;
            newToolStripMenuItem.ForeColor = Color.White;
            saveToolStripMenuItem.ForeColor = Color.White;
            exitToolStripMenuItem.ForeColor = Color.White;
            openToolStripMenuItem.ForeColor = Color.White;

            AssignCustomRenderer(); // <----- Addition: Update renderer when theme changes

            statusStrip.BackColor = Color.FromArgb(40, 40, 40);
            statusStrip.ForeColor = Color.White;
            toolStripStatusLabelWordCount.ForeColor = Color.White;
            toolStripStatusLabelCharCount.ForeColor = Color.White;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxMain.Text) && textBoxMain.Text != originalFileContent)
            {
                var result = MessageBox.Show("Do you want to save changes?", "Unsaved Changes", MessageBoxButtons.YesNoCancel);

                if (result == DialogResult.Yes)
                {
                    if (string.IsNullOrEmpty(currentFilePath))
                    {
                        SaveAs();
                    }
                    else
                    {
                        SaveFile();
                    }
                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }
            }
            textBoxMain.Clear();
            currentFilePath = null;
            originalFileContent = string.Empty;
            UpdateTitle();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                currentFilePath = openFileDialog.FileName;
                originalFileContent = File.ReadAllText(currentFilePath);
                textBoxMain.Text = File.ReadAllText(currentFilePath);
                UpdateTitle();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                SaveAs();
            }
            else if (textBoxMain.Text != originalFileContent)
            {
                SaveFile();
            }
            else
            {
                MessageBox.Show("No changes to save.", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to exit?", "Exit application", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void aToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("A simple notepad created by Rares Racsan using C# and Windows Forms\nFor more details check @RaresRacsan on github.", "About");
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

        private void lightThemeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetLightTheme();
        }

        private void darkThemeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetDarkTheme();
        }

        public class CustomColorTable : ProfessionalColorTable
        {
            private bool isDarkTheme;

            public CustomColorTable(bool darkTheme)
            {
                isDarkTheme = darkTheme;
            }

            public override Color MenuItemSelected
            {
                get
                {
                    return isDarkTheme ? Color.FromArgb(60, 60, 60) : Color.LightBlue;
                }
            }

            public override Color MenuItemSelectedGradientBegin
            {
                get
                {
                    return isDarkTheme ? Color.FromArgb(60, 60, 60) : Color.LightBlue;
                }
            }

            public override Color MenuItemSelectedGradientEnd
            {
                get
                {
                    return isDarkTheme ? Color.FromArgb(60, 60, 60) : Color.LightBlue;
                }
            }

            public override Color MenuItemBorder
            {
                get
                {
                    return Color.Transparent; // Removes the border around menu items
                }
            }

            public override Color ToolStripBorder
            {
                get
                {
                    return Color.Transparent; // Removes the border around the MenuStrip
                }
            }


            public override Color ImageMarginGradientBegin
            {
                get
                {
                    return isDarkTheme ? Color.FromArgb(40, 40, 40) : Color.LightGray;
                }
            }

            public override Color ImageMarginGradientMiddle
            {
                get
                {
                    return isDarkTheme ? Color.FromArgb(40, 40, 40) : Color.LightGray;
                }
            }

            public override Color ImageMarginGradientEnd
            {
                get
                {
                    return isDarkTheme ? Color.FromArgb(40, 40, 40) : Color.LightGray;
                }
            }

            public override Color MenuItemPressedGradientBegin
            {
                get
                {
                    return isDarkTheme ? Color.FromArgb(80, 80, 80) : Color.SkyBlue;
                }
            }

            public override Color MenuItemPressedGradientEnd
            {
                get
                {
                    return isDarkTheme ? Color.FromArgb(80, 80, 80) : Color.SkyBlue;
                }
            }

            public override Color MenuItemPressedGradientMiddle
            {
                get
                {
                    return isDarkTheme ? Color.FromArgb(80, 80, 80) : Color.SkyBlue;
                }
            }

            // Customize the background color of the dropdown menu
            public override Color ToolStripDropDownBackground
            {
                get
                {
                    return isDarkTheme ? Color.FromArgb(40, 40, 40) : Color.White;
                }
            }
        }

        private void AssignCustomRenderer()
        {
            menuStrip.Renderer = new ToolStripProfessionalRenderer(new CustomColorTable(IsDarkTheme()));
        }

        private bool IsDarkTheme()
        {

            return menuStrip.BackColor == Color.FromArgb(40, 40, 40);
        }

        private void UpdateStatusCounts()
        {
            string text = textBoxMain.Text;
            characters = text.Length;
            words = string.IsNullOrEmpty(text) ? 0 : text.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;

            toolStripStatusLabelWordCount.Text = $"Words: {words}";
            toolStripStatusLabelCharCount.Text = $"Characters: {characters}";
        }

        private void textBoxMain_TextChanged(object sender, EventArgs e)
        {
            UpdateStatusCounts();
        }

        private void ChangeFontSize(int newSize)
        {
            textBoxMain.Font = new Font(textBoxMain.Font.FontFamily, newSize);
        }

        private void CustomFontSize_Click(object sender, EventArgs e)
        {
            using (var inputDialog = new Form())
            {
                inputDialog.Text = "Set Custom Font Size";
                inputDialog.Size = new Size(300, 150);

                var label = new Label { Text = "Enter font size:", Left = 10, Top = 10, Width = 250 };
                var textBox = new TextBox { Left = 10, Top = 40, Width = 250 };
                var okButton = new Button { Text = "OK", Left = 10, Top = 70, Width = 80 };
                var cancelButton = new Button { Text = "Cancel", Left = 100, Top = 70, Width = 80 };

                okButton.Click += (s, e) =>
                {
                    if (int.TryParse(textBox.Text, out int newSize) && newSize > 0)
                    {
                        ChangeFontSize(newSize);
                        inputDialog.DialogResult = DialogResult.OK;
                        inputDialog.Close();
                    }
                    else
                    {
                        MessageBox.Show("Please enter a valid positive number.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                };

                cancelButton.Click += (s, e) =>
                {
                    inputDialog.DialogResult = DialogResult.Cancel;
                    inputDialog.Close();
                };

                inputDialog.Controls.Add(label);
                inputDialog.Controls.Add(textBox);
                inputDialog.Controls.Add(okButton);
                inputDialog.Controls.Add(cancelButton);

                inputDialog.ShowDialog();
            }
        }

        private void editTextSize()
        {
            // Changing the font of the textBoxMain
            textBoxMain.Font = new Font("Consolas", 12);

            // Size submenu
            editToolStripMenuItem.DropDownItems.Add(sizeToolStripMenuItem);

            // Add size options
            string[] fontSizes = { "8", "12", "14", "18", "24" };
            foreach (var size in fontSizes)
            {
                ToolStripMenuItem sizeOption = new ToolStripMenuItem(size);
                sizeOption.Click += (s, e) => ChangeFontSize(int.Parse(size));
                sizeToolStripMenuItem.DropDownItems.Add(sizeOption);
            }

            // Add a "Custom Size..." option
            ToolStripMenuItem customSizeOption = new ToolStripMenuItem("Custom Size...");
            customSizeOption.Click += CustomFontSize_Click;
            sizeToolStripMenuItem.DropDownItems.Add(customSizeOption);
        }

        private void PlainTextEditor_KeyDown(object sender, KeyEventArgs e)
        {
            // Shortcut implementations
            // Save Current File
            if(e.Control && e.KeyCode == Keys.S) 
            {
                if (string.IsNullOrEmpty(currentFilePath))
                {
                    SaveAs();
                }
                else if (textBoxMain.Text != originalFileContent)
                {
                    SaveFile();
                }
            }

            // Create New File
            if(e.Control && e.KeyCode == Keys.N)
            {
                if (!string.IsNullOrEmpty(textBoxMain.Text) && textBoxMain.Text != originalFileContent)
                {
                    var result = MessageBox.Show("Do you want to save changes?", "Unsaved Changes", MessageBoxButtons.YesNoCancel);

                    if (result == DialogResult.Yes)
                    {
                        if (string.IsNullOrEmpty(currentFilePath))
                        {
                            SaveAs();
                        }
                        else
                        {
                            SaveFile();
                        }
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        return;
                    }
                }
                textBoxMain.Clear();
                currentFilePath = null;
                originalFileContent = string.Empty;
                UpdateTitle();
            }

            // Open An Existing File
            if(e.Control && e.KeyCode == Keys.O)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    currentFilePath = openFileDialog.FileName;
                    originalFileContent = File.ReadAllText(currentFilePath);
                    textBoxMain.Text = File.ReadAllText(currentFilePath);
                    UpdateTitle();
                }
            }
        }
    }
}
