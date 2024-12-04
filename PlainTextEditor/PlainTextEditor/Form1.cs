using System.Runtime.InteropServices;

namespace PlainTextEditor
{
    public partial class PlainTextEditor : Form
    {
        private string currentFilePath = null;
        private string originalFileContent = string.Empty;

        public PlainTextEditor()
        {
            InitializeComponent();
            UpdateTitle();
            SetDarkTheme();
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
            string fileName = string.IsNullOrEmpty(currentFilePath) ? "New File " : Path.GetFileName(currentFilePath);
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

            // Set background color for white theme (light theme)
            editToolStripMenuItem.BackColor = Color.FromArgb(255, 255, 255);  // White background
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
    }
}
