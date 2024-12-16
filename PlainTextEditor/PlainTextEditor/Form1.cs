using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace PlainTextEditor
{
    public partial class PlainTextEditor : Form
    {
        /// <summary>
        /// Initialization of variables and items
        /// </summary>
        private StatusStrip statusStrip;
        private ToolStripStatusLabel toolStripStatusLabelWordCount;
        private ToolStripStatusLabel toolStripStatusLabelCharCount;
        private string currentFilePath = null;
        private bool isCppEditorMode = false;
        private string originalFileContent = string.Empty;
        private int words = 0;
        private int characters = 0;
        ToolStripMenuItem sizeToolStripMenuItem = new ToolStripMenuItem("Size");
        private RichTextBox hiddenBuffer = new RichTextBox();
        private Color defaultTextColor = Color.White;
        private PrintDocument printDocument = new PrintDocument();
        private string printText = string.Empty;
        private PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();
        private Panel panelLineNumbers;

        private ToolStripMenuItem findReplaceMenuItem; // Menu item
        private ToolStrip toolStripFindReplace;       // Find and Replace ToolStrip
        private ToolStripTextBox findTextBox;
        private ToolStripTextBox replaceTextBox;
        private ToolStripButton findNextButton;
        private ToolStripButton findPrevButton;
        private ToolStripButton replaceButton;
        private int currentSearchIndex = 0;

        private void InitializeFindReplaceMenu()
        {
            findReplaceMenuItem = new ToolStripMenuItem("Find and Replace", null, FindReplaceMenuItem_Click);
            menuStrip.Items.Add(findReplaceMenuItem);
        }

        private void FindReplaceInitEvent()
        {
            if (toolStripFindReplace == null)
            {
                // Initialize the ToolStrip for Find and Replace
                toolStripFindReplace = new ToolStrip();

                findTextBox = new ToolStripTextBox()
                {
                    ToolTipText = "Enter text to find"
                };

                replaceTextBox = new ToolStripTextBox()
                {
                    ToolTipText = "Enter text to replace"
                };

                findPrevButton = new ToolStripButton("<-", null, FindPrev_Click)
                {
                    ToolTipText = "Previous"
                };

                findNextButton = new ToolStripButton("->", null, FindNext_Click)
                {
                    ToolTipText = "Next"
                };

                replaceButton = new ToolStripButton("Replace", null, Replace_Click)
                {
                    ToolTipText = "Replace content"
                };

                // Adding components to the ToolStrip
                toolStripFindReplace.Items.Add(new ToolStripLabel("Find: "));
                toolStripFindReplace.Items.Add(findTextBox);
                toolStripFindReplace.Items.Add(new ToolStripLabel("Replace:"));
                toolStripFindReplace.Items.Add(replaceTextBox);
                toolStripFindReplace.Items.Add(findPrevButton);
                toolStripFindReplace.Items.Add(findNextButton);
                toolStripFindReplace.Items.Add(replaceButton);

                this.Controls.Add(toolStripFindReplace);
                toolStripFindReplace.Dock = DockStyle.Top;
                UpdateToolStripColor();
            }
            else
            {
                toolStripFindReplace.Visible = !toolStripFindReplace.Visible;
                UpdateToolStripColor();
            }
        }

        private void FindReplaceMenuItem_Click(object sender, EventArgs e)
        {
            FindReplaceInitEvent();
        }

        public class CustomRenderer : ToolStripRenderer
        {
            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
            {
                // Don't render the border
            }
        }

        private void UpdateToolStripColor()
        {
            if(toolStripFindReplace != null)
            {
                // Setting the ToolStrip's renderer to a custom renderer or no borders
                toolStripFindReplace.Renderer = new CustomRenderer();
                findTextBox.BorderStyle = BorderStyle.FixedSingle;
                replaceTextBox.BorderStyle = BorderStyle.FixedSingle;

                if (IsDarkTheme())
                {
                    toolStripFindReplace.BackColor = Color.FromArgb(40, 40, 40); // Dark background
                    toolStripFindReplace.ForeColor = Color.White;               // Light text
                    findTextBox.BackColor = Color.FromArgb(40, 40, 40);
                    findTextBox.ForeColor = Color.White;
                }
                else
                {
                    toolStripFindReplace.BackColor = Color.LightGray;           // Light background
                    toolStripFindReplace.ForeColor = Color.Black;               // Dark text
                    findTextBox.BackColor = Color.LightGray;
                    findTextBox.ForeColor = Color.Black;
                }

                // Update the colors for all child items of the menu
                foreach (ToolStripItem item in toolStripFindReplace.Items)
                {
                    item.BackColor = toolStripFindReplace.BackColor;
                    item.ForeColor = toolStripFindReplace.ForeColor;
                }

                // Force a redraw to ensure the changes take effect
                toolStripFindReplace.Invalidate();
                toolStripFindReplace.Refresh();
            }
        }

        private void FindNext_Click(object sender, EventArgs e)
        {
            string searchText = findTextBox.Text;
            if (string.IsNullOrEmpty(searchText)) return;

            int startIndex = textBoxMain.SelectionStart + textBoxMain.SelectionLength;

            // Search for the next occurence
            currentSearchIndex = textBoxMain.Find(searchText, startIndex, RichTextBoxFinds.None);

            if(currentSearchIndex >= 0)
            {
                textBoxMain.Select(currentSearchIndex, searchText.Length);
                textBoxMain.ScrollToCaret();
            }
            else
            {
                MessageBox.Show("No further matches found.", "Find", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void FindPrev_Click(object sender, EventArgs e)
        {
            string searchText = findTextBox.Text;
            if(string.IsNullOrEmpty(searchText)) return;

            int startIndex = textBoxMain.SelectionStart - 1;

            currentSearchIndex = textBoxMain.Find(searchText, 0, startIndex, RichTextBoxFinds.Reverse);

            if(currentSearchIndex >= 0)
            {
                textBoxMain.Select(currentSearchIndex, searchText.Length);
                textBoxMain.ScrollToCaret();
            }
            else
            {
                MessageBox.Show("No previous matches found.", "Find", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Replace_Click(object sender, EventArgs e)
        {
            string searchText = findTextBox.Text;
            string replacementText = replaceTextBox.Text;

            if(string.IsNullOrEmpty (searchText)) return;

            if (textBoxMain.SelectedText == searchText) textBoxMain.SelectedText = replacementText;

            // Move to the next match
            FindNext_Click(sender, e);
        }

        // Import user32.dll to get scroll position if needed
        [DllImport("user32.dll")]
        private static extern int GetScrollPos(IntPtr hWnd, int nBar);

        private const int SB_VERT = 1;

        /// <summary>
        /// Gets the first visible line index in the RichTextBox.
        /// </summary>
        private int GetFirstVisibleLine(RichTextBox rtb)
        {
            int firstCharIndex = rtb.GetCharIndexFromPosition(new Point(0, 0));
            int firstLine = rtb.GetLineFromCharIndex(firstCharIndex);
            return firstLine;
        }

        /// <summary>
        /// Paint event handler for the line numbers panel.
        /// </summary>
        private void panelLineNumbers_Paint(object sender, PaintEventArgs e)
        {
            panelLineNumbers.Width = 50;

            // Determine the first visible line
            int firstVisibleLine = GetFirstVisibleLine(textBoxMain);

            // Determine the total number of lines
            int totalLines = textBoxMain.GetLineFromCharIndex(textBoxMain.TextLength) + 1;


            using (Font lineNumberFont = new Font(textBoxMain.Font.FontFamily, 12))
            {
                float textLineHeight = textBoxMain.Font.GetHeight(e.Graphics);

                float lineNumberLineHeight = lineNumberFont.GetHeight(e.Graphics);

                float verticalOffset = (textLineHeight - lineNumberLineHeight) / 2;

                int visibleLines = (int)(panelLineNumbers.Height / textLineHeight);

                Brush brush = new SolidBrush(panelLineNumbers.ForeColor);

                for (int i = 0; i < visibleLines; i++)
                {
                    int lineNumber = firstVisibleLine + i + 1;
                    if (lineNumber > totalLines)
                        break;

                    float yPosition = i * textLineHeight - (textBoxMain.GetPositionFromCharIndex(textBoxMain.GetFirstCharIndexFromLine(firstVisibleLine)).Y % textLineHeight) + verticalOffset;

                    string lineNumberText = lineNumber.ToString();
                    SizeF textSize = e.Graphics.MeasureString(lineNumberText, lineNumberFont);

                    e.Graphics.DrawString(lineNumberText, lineNumberFont, brush, panelLineNumbers.Width - textSize.Width - 5, yPosition);
                }
            }
        }


        /// <summary>
        /// Event handler for vertical scrolling of the RichTextBox.
        /// </summary>
        private void TextBoxMain_VScroll(object sender, EventArgs e)
        {
            panelLineNumbers.Invalidate();
        }

        /// <summary>
        /// Event handler for text changes in the RichTextBox to update line numbers.
        /// </summary>
        private void TextBoxMain_TextChanged_ForLineNumbers(object sender, EventArgs e)
        {
            panelLineNumbers.Invalidate();
            UpdateStatusCounts(); // Ensure status counts are updated
        }

        /// <summary>
        /// Event handler for resizing of the RichTextBox.
        /// </summary>
        private void TextBoxMain_Resize(object sender, EventArgs e)
        {
            panelLineNumbers.Invalidate();
        }


        /// <summary>
        /// Starting the windows form application by initializing everything
        /// </summary>
        public PlainTextEditor()
        {
            InitializeComponent();
            InitializeFindReplaceMenu();
            InitializeStatusStrip();
            editTextSize();
            UpdateTitle();
            SetDarkTheme();
            AssignCustomRenderer();
            UpdateStatusCounts();

            printDocument.PrintPage += PrintDocument_PrintPage;
            textBoxMain.VScroll += TextBoxMain_VScroll;
            textBoxMain.TextChanged += TextBoxMain_TextChanged_ForLineNumbers;
            textBoxMain.Resize += TextBoxMain_Resize;
        }

        private void InitializeStatusStrip()
        {
            statusStrip = new StatusStrip();
            toolStripStatusLabelWordCount = new ToolStripStatusLabel { Text = "Words: 0" };
            toolStripStatusLabelCharCount = new ToolStripStatusLabel { Text = "Characters: 0" };

            statusStrip.Items.Add(toolStripStatusLabelWordCount);
            statusStrip.Items.Add(toolStripStatusLabelCharCount);

            this.Controls.Add(statusStrip);
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

        /// <summary>
        /// Function that updates the title of the form, in order to contain the name of the file that
        /// is currently open
        /// </summary>
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

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

                printPreviewDialog.Document = printDocument;

                // Assign the current text to printText
                printText = textBoxMain.Text;


                printPreviewDialog.Width = 800;
                printPreviewDialog.Height = 600;
                printPreviewDialog.Text = "Print Preview - PlainTextEditor";

                // Show the Print Preview Dialog
                printPreviewDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Print Preview failed: {ex.Message}", "Print Preview Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            Font printFont = textBoxMain.Font;

            float leftMargin = e.MarginBounds.Left;
            float topMargin = e.MarginBounds.Top;
            int linesPerPage = (int)(e.MarginBounds.Height / printFont.GetHeight(e.Graphics));
            string[] lines = printText.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            int count = Math.Min(linesPerPage, lines.Length);
            for (int i = 0; i < count; i++)
            {
                e.Graphics.DrawString(lines[i], printFont, Brushes.Black, leftMargin, topMargin + (i * printFont.GetHeight(e.Graphics)));
            }
            printText = string.Join("\n", lines.Skip(count));
            if (lines.Length > count)
            {
                e.HasMorePages = true;
            }
            else
            {
                e.HasMorePages = false;
            }
        }

        private void SetLightTheme()
        {
            SetTitleBarColor();

            defaultTextColor = Color.Black;

            this.BackColor = Color.White;
            this.ForeColor = Color.Black;
            textBoxMain.BackColor = Color.White;
            textBoxMain.ForeColor = Color.Black;
            textBoxMain.BorderStyle = BorderStyle.None;

            menuStrip.BackColor = Color.LightGray;
            menuStrip.ForeColor = Color.Black;

            sizeToolStripMenuItem.BackColor = Color.White;
            sizeToolStripMenuItem.ForeColor = Color.Black;

            foreach (ToolStripItem item in sizeToolStripMenuItem.DropDownItems)
            {
                item.BackColor = Color.White;
                item.ForeColor = Color.Black;
            }

            // Set background color for white theme (light theme)
            aToolStripMenuItem.BackColor = Color.White;
            themeToolStripMenuItem.BackColor = Color.White;
            lightThemeToolStripMenuItem.BackColor = Color.White;
            darkThemeToolStripMenuItem.BackColor = Color.White;
            saveAsToolStripMenuItem.BackColor = Color.White;
            newToolStripMenuItem.BackColor = Color.White;
            saveToolStripMenuItem.BackColor = Color.White;
            exitToolStripMenuItem.BackColor = Color.White;
            openToolStripMenuItem.BackColor = Color.White;
            shortcutsToolStripMenuItem.BackColor = Color.White;
            plainTextToolStripMenuItem.BackColor = Color.White;
            cCToolStripMenuItem.BackColor = Color.White;
            printToolStripMenuItem.BackColor = Color.White;

            // Set foreground color (text color) for menu items
            editToolStripMenuItem.ForeColor = Color.Black;
            aToolStripMenuItem.ForeColor = Color.Black;
            themeToolStripMenuItem.ForeColor = Color.Black;
            lightThemeToolStripMenuItem.ForeColor = Color.Black;
            darkThemeToolStripMenuItem.ForeColor = Color.Black;
            saveAsToolStripMenuItem.ForeColor = Color.Black;
            newToolStripMenuItem.ForeColor = Color.Black;
            saveToolStripMenuItem.ForeColor = Color.Black;
            exitToolStripMenuItem.ForeColor = Color.Black;
            openToolStripMenuItem.ForeColor = Color.Black;
            shortcutsToolStripMenuItem.ForeColor = Color.Black;
            plainTextToolStripMenuItem.ForeColor = Color.Black;
            cCToolStripMenuItem.ForeColor = Color.Black;
            printToolStripMenuItem.ForeColor = Color.Black;

            // Set the background and foreground color of line numbers panel
            panelLineNumbers.BackColor = Color.White;
            panelLineNumbers.ForeColor = Color.Black;

            UpdateToolStripColor();

            AssignCustomRenderer();

            // Set status strip colors
            statusStrip.BackColor = Color.LightGray;
            statusStrip.ForeColor = Color.Black;
            toolStripStatusLabelWordCount.ForeColor = Color.Black;
            toolStripStatusLabelCharCount.ForeColor = Color.Black;

            // Invalidate the panel to refresh line numbers
            panelLineNumbers.Invalidate();
        }
        private void SetDarkTheme()
        {
            SetTitleBarColor();

            defaultTextColor = Color.White;

            this.BackColor = Color.FromArgb(30, 30, 30);
            this.ForeColor = Color.White; // Corrected to white
            textBoxMain.BackColor = Color.FromArgb(30, 30, 30);
            textBoxMain.ForeColor = Color.White;
            textBoxMain.BorderStyle = BorderStyle.None;

            menuStrip.BackColor = Color.FromArgb(40, 40, 40);
            menuStrip.ForeColor = Color.White;

            sizeToolStripMenuItem.BackColor = Color.FromArgb(40, 40, 40);
            sizeToolStripMenuItem.ForeColor = Color.White;

            foreach (ToolStripItem item in sizeToolStripMenuItem.DropDownItems)
            {
                item.BackColor = Color.FromArgb(40, 40, 40);
                item.ForeColor = Color.White;
            }

            // Set background color for dark theme
            aToolStripMenuItem.BackColor = Color.FromArgb(40, 40, 40);
            themeToolStripMenuItem.BackColor = Color.FromArgb(40, 40, 40);
            lightThemeToolStripMenuItem.BackColor = Color.FromArgb(40, 40, 40);
            darkThemeToolStripMenuItem.BackColor = Color.FromArgb(40, 40, 40);
            saveAsToolStripMenuItem.BackColor = Color.FromArgb(40, 40, 40);
            newToolStripMenuItem.BackColor = Color.FromArgb(40, 40, 40);
            saveToolStripMenuItem.BackColor = Color.FromArgb(40, 40, 40);
            exitToolStripMenuItem.BackColor = Color.FromArgb(40, 40, 40);
            openToolStripMenuItem.BackColor = Color.FromArgb(40, 40, 40);
            shortcutsToolStripMenuItem.BackColor = Color.FromArgb(40, 40, 40);
            plainTextToolStripMenuItem.BackColor = Color.FromArgb(40, 40, 40);
            cCToolStripMenuItem.BackColor = Color.FromArgb(40, 40, 40);
            printToolStripMenuItem.BackColor = Color.FromArgb(40, 40, 40);

            // Set foreground color (text color) for menu items
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
            shortcutsToolStripMenuItem.ForeColor = Color.White;
            plainTextToolStripMenuItem.ForeColor = Color.White;
            cCToolStripMenuItem.ForeColor = Color.White;
            printToolStripMenuItem.ForeColor = Color.White;


            statusStrip.BackColor = Color.FromArgb(40, 40, 40);
            statusStrip.ForeColor = Color.White;



            // Set the background and foreground color of line numbers panel
            panelLineNumbers.BackColor = Color.FromArgb(40, 40, 40);
            panelLineNumbers.ForeColor = Color.White;

            UpdateToolStripColor();

            AssignCustomRenderer(); // Ensure the renderer matches the theme

            // Set status strip colors
            statusStrip.BackColor = Color.FromArgb(40, 40, 40);
            statusStrip.ForeColor = Color.White;
            toolStripStatusLabelWordCount.ForeColor = Color.White;
            toolStripStatusLabelCharCount.ForeColor = Color.White;

            // Invalidate the panel to refresh line numbers
            panelLineNumbers.Invalidate();
        }


        /// <summary>
        /// Function for the strip menu item called "New",
        /// it is creating a new empty file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Function for the "Open" strip menu item, to open an already created file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Function for the "Save" strip menu item, to save the currently opened file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            MessageBox.Show("A simple notepad created by Rares Racsan using C# and Win.Forms\nFor more details check @RaresRacsan on github.", "About");
        }

        /// <summary>
        /// Function for the "Save As" strip menu item, to save the currently opened file as a .txt of .* (all files)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

        /// <summary>
        /// Function for changing the theme to light in case the "Light" strip menu item is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lightThemeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetLightTheme();
        }

        /// <summary>
        /// Function for changing the theme to dark in case the "Dark" strip menu item is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void darkThemeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetDarkTheme();
        }

        /// <summary>
        /// Color table for strip menu items
        /// </summary>
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

        /// <summary>
        /// Helper function that checks wether the current theme is dark theme
        /// </summary>
        /// <returns></returns>
        private bool IsDarkTheme()
        {

            return menuStrip.BackColor == Color.FromArgb(40, 40, 40);
        }

        /// <summary>
        /// Function that updates the words count and the characters count (the number of words and characters
        /// that are currently in the textMainBox text)
        /// </summary>
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

            if (isCppEditorMode)
            {
                int selectionStart = textBoxMain.SelectionStart;

                // Set the default color for new text
                textBoxMain.SelectionStart = selectionStart;
                textBoxMain.SelectionLength = 0; // Ensure we're formatting new input
                textBoxMain.SelectionColor = defaultTextColor;

                // Apply highlighting after new input
                ApplyCppHighlighting();
            }

            // Invalidate the line numbers panel to trigger repaint
            panelLineNumbers.Invalidate();
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
            textBoxMain.Font = new Font("Consolas", 12, FontStyle.Bold);

            // Size submenu
            editToolStripMenuItem.DropDownItems.Add(sizeToolStripMenuItem);

            // Add size options
            string[] fontSizes = { "8", "12", "16", "20", "24" };
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

        /// <summary>
        /// Function for application shortcuts:
        /// Ctrl + S - save file,
        /// Ctrl + N - new file,
        /// Ctrl + O - open file,
        /// Ctrl + "+" - increase font size,
        /// Ctrl + "-" - decrease font size,
        /// Ctrl + T - change theme dark/light,
        /// Ctrl + W - close application
        /// Ctrl + F - find and replace
        /// Ctrl + ',' - change to plain text mode
        /// Ctrl + '.' - change to c++ mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlainTextEditor_KeyDown(object sender, KeyEventArgs e)
        {
            // Shortcut implementations
            // Find and Replace
            if(e.Control && e.KeyCode == Keys.F)
            {
                FindReplaceInitEvent();
            }

            // Save Current File
            if (e.Control && e.KeyCode == Keys.S)
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
            if (e.Control && e.KeyCode == Keys.N)
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
            if (e.Control && e.KeyCode == Keys.O)
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

            // Increase Font size
            if (e.Control && e.KeyCode == Keys.Oemplus)
            {
                float currentSize = textBoxMain.Font.SizeInPoints;
                float newSize = currentSize + 4;
                ChangeFontSize((int)(newSize));
            }

            // Decrease Font Size
            if (e.Control && e.KeyCode == Keys.OemMinus)
            {
                float currentSize = textBoxMain.Font.SizeInPoints;
                if (currentSize > 8)
                {
                    float newSize = currentSize - 4;
                    ChangeFontSize((int)(newSize));
                }
            }
            // Print (Ctrl + P)
            if (e.Control && e.KeyCode == Keys.P)
            {
                printToolStripMenuItem_Click(sender, e);
            }

            // Change Themes
            if (e.Control && e.KeyCode == Keys.T)
            {
                if (IsDarkTheme())
                {
                    SetLightTheme();
                }
                else
                {
                    SetDarkTheme();
                }
            }

            // Exit Application
            if (e.Control && e.KeyCode == Keys.W)
            {
                if (!string.IsNullOrEmpty(textBoxMain.Text) && textBoxMain.Text != originalFileContent)
                {
                    var result = MessageBox.Show("Do you want to save changes?", "Unsaved Changes", MessageBoxButtons.YesNo);

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
                }
                System.Environment.Exit(0);
            }

            // Change to cpp theme
            if (e.Control && e.KeyCode == Keys.OemPeriod)
            {
                SetCppEditorMode();
            }

            // Change to plain mode theme
            if (e.Control && e.KeyCode == Keys.Oemcomma)
            {
                SetPlainTextMode();
            }

            // Pressing Tab -> spaces
            if (e.KeyCode == Keys.Tab)
            {
                // Define the number of spaces to insert (e.g., 4 spaces)
                const int tabSize = 4;
                string spaces = new string(' ', tabSize);

                // Get the current cursor position
                int cursorPosition = textBoxMain.SelectionStart;

                // Insert spaces at the cursor position
                textBoxMain.Text = textBoxMain.Text.Insert(cursorPosition, spaces);

                // Move the cursor to the end of the inserted spaces
                textBoxMain.SelectionStart = cursorPosition + tabSize;

                // Prevent the default behavior of the Tab key (focus shift)
                e.SuppressKeyPress = true;
                e.Handled = true;
            }

            // Creating space block between {}
            if (e.KeyCode == Keys.Enter)
            {
                int cursorPos = textBoxMain.SelectionStart;
                if (textBoxMain.Text.Length > 0)
                {
                    char lastChar = textBoxMain.Text[cursorPos - 1];
                    if (lastChar == '{')
                    {
                        textBoxMain.Text = textBoxMain.Text.Insert(cursorPos, "\n\t\n");
                        textBoxMain.SelectionStart = cursorPos + 2;
                        e.Handled = true;
                    }
                }
            }

            // Deleting both brackets if one next to the other
            if (e.KeyCode == Keys.Back)
            {
                int cursorPos = textBoxMain.SelectionStart;
                if (cursorPos > 0)
                {
                    // Get the characters before the cursor and after the cursor
                    char prevChar = textBoxMain.Text[cursorPos - 1];
                    char nextChar = (cursorPos < textBoxMain.Text.Length) ? textBoxMain.Text[cursorPos] : '\0';

                    if ((prevChar == '(' && nextChar == ')') || (prevChar == '[' && prevChar == ']') || (prevChar == '{' && nextChar == '}'))
                    {
                        // Remove both brackets
                        textBoxMain.Text = textBoxMain.Text.Remove(cursorPos - 1, 2);

                        // Set the cursor position after the brackets are removed
                        textBoxMain.SelectionStart = cursorPos - 1;
                        e.Handled = true;
                    }
                }
            }
        }

        /// <summary>
        /// Keypress event created for bracket matching
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlainTextEditor_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Matching brackets
            if (e.KeyChar == '(' || e.KeyChar == '[' || e.KeyChar == '{')
            {
                char openingBracket = e.KeyChar;
                string closingBracket = GetClosingBracket(openingBracket);
                int cursorPoint = textBoxMain.SelectionStart;

                textBoxMain.Text = textBoxMain.Text.Insert(cursorPoint, openingBracket.ToString());
                textBoxMain.SelectionStart = cursorPoint + 1;

                textBoxMain.Text = textBoxMain.Text.Insert(textBoxMain.SelectionStart, closingBracket);
                textBoxMain.SelectionStart = cursorPoint + 1;

                e.Handled = true;
            }
        }

        /// <summary>
        /// Helper function for the match of the brackets, it is returning the corrent closing bracket for
        /// the currently typed bracket
        /// </summary>
        /// <param name="openBracket"></param>
        /// <returns></returns>
        private string GetClosingBracket(char openBracket)
        {
            switch (openBracket)
            {
                case '(': return ")";
                case '{': return "}";
                case '[': return "]";
                default: return "";
            }
        }

        /// <summary>
        /// Function that makes sure that the user is saving the progress before closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlainTextEditor_FormClosing(object sender, FormClosingEventArgs e)
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
                    e.Cancel = true;
                }
            }
        }

        private void shortcutsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Shortcuts:\n- CTRL + N - new file\n- CTRL + S - save file\n- CTRL + O - open file\n- CTRL + P - print file\n- CTRL + W - close file\n- CTRL + T - change theme\n- CTRL + '+' - increase font size\n- CTRL + '-' - decrease font size\n- CTRL + '.' - change to c++ mode\n- CTRL + ',' - change to plain text mode", "Shortcuts");
        }

        private void plainTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetPlainTextMode();
            UpdateTitle();
        }

        private void cCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetCppEditorMode();
            UpdateTitle();
        }

        private void ApplyCppHighlighting()
        {
            if (!isCppEditorMode) return; // Skip if not in C++ mode

            string text = textBoxMain.Text;
            int selectionStart = textBoxMain.SelectionStart;
            int selectionLength = textBoxMain.SelectionLength;

            // Temporarily disable TextChanged event to avoid recursion
            textBoxMain.TextChanged -= textBoxMain_TextChanged;

            // Preserve font size and style
            hiddenBuffer.Font = textBoxMain.Font;

            // Perform highlighting on the hidden buffer
            hiddenBuffer.Text = text;
            hiddenBuffer.SelectAll();
            hiddenBuffer.SelectionColor = defaultTextColor; // Reset to default color

            // Highlight patterns and keywords
            HighlightCppKeyWords(hiddenBuffer);
            HighlightPattern(hiddenBuffer, "\".*?\"", Color.LightGreen); // Strings
            HighlightPattern(hiddenBuffer, "<.*?>", Color.LightGreen);  // Angle brackets
            HighlightPattern(hiddenBuffer, "//.*?$", Color.LightGray, RegexOptions.Multiline); // Single-line comments
            HighlightPattern(hiddenBuffer, @"/\*.*?\*/", Color.LightGray, RegexOptions.Singleline); // Multi-line comments

            // Replace the visible TextBox content with the highlighted content
            textBoxMain.Rtf = hiddenBuffer.Rtf;

            // Restore the cursor position and font
            textBoxMain.SelectionStart = selectionStart;
            textBoxMain.SelectionLength = selectionLength;

            // Reattach the TextChanged event
            textBoxMain.TextChanged += textBoxMain_TextChanged;
        }

        private void HighlightCppKeyWords(RichTextBox buffer)
        {
            string[] variableTypeKeyWords = { "int", "float", "double", "char", "bool", "wchar_t", "void", "short", "signed", "unsigned", "long", "auto", "decltype", "const", "constexpr", "mutable", "nullptr", "true", "false" };
            string[] controlFlowKeywords = { "if", "else", "switch", "case", "default", "while", "do", "for", "break", "continue", "return", "goto" };
            string[] cppStandardKeywords = { "std", "cout", "cin", "endl", "namespace", "using", "::" };
            string[] includeDirectives = { "#include" };
            string[] classRelatedKeywords = { "class", "struct", "public", "private", "protected", "virtual", "friend", "static", "explicit", "inline", "final", "override", "abstract", "typeid", "typename", "this", "new", "delete", "new[]", "delete[]" };
            string[] exceptionHandling = { "try", "catch", "throw", "noexcept" };
            string[] typeAndTypeModifiers = { "typedef", "typeid", "typename", "volatile", "alignas", "alignof", "thread_local" };
            string[] templateKeyWords = { "template" };
            string[] miscellaneous = { "const_cast", "dynamic_cast", "reinterpret_cast", "static_cast" };

            Dictionary<string[], Color> keywordCategories = new Dictionary<string[], Color>
            {
                { variableTypeKeyWords, Color.FromArgb(0, 123, 255) }, // - Blue -
                { controlFlowKeywords, Color.FromArgb(255, 105, 180) }, // - Pink -
                { cppStandardKeywords, Color.FromArgb(255, 159, 28) }, // - Orange -
                { includeDirectives, Color.FromArgb(34, 139, 34) }, // - Forest Green -
                { classRelatedKeywords, Color.FromArgb(0, 128, 128) }, // - Teal -
                { exceptionHandling, Color.FromArgb(255, 193, 7) }, // - Yellow -
                { typeAndTypeModifiers, Color.FromArgb(220, 53, 69) }, // - Red -
                { templateKeyWords, Color.FromArgb(173, 216, 230) }, //  - Light Blue -
                { miscellaneous, Color.FromArgb(169, 169, 169) } // - Gray -
            };

            foreach (var category in keywordCategories)
            {
                string[] keywords = category.Key;
                Color color = category.Value;

                foreach (string keyword in keywords)
                {
                    int startIndex = 0;
                    while ((startIndex = buffer.Text.IndexOf(keyword, startIndex)) != -1)
                    {
                        bool isWordBoundary = (startIndex == 0 || !char.IsLetterOrDigit(buffer.Text[startIndex - 1])) &&
                                              (startIndex + keyword.Length == buffer.Text.Length || !char.IsLetterOrDigit(buffer.Text[startIndex + keyword.Length]));
                        if (isWordBoundary)
                        {
                            buffer.Select(startIndex, keyword.Length);
                            buffer.SelectionColor = color;
                        }
                        startIndex += keyword.Length;
                    }
                }
            }
        }

        // Helper method to highlight patterns using regular expressions
        private void HighlightPattern(RichTextBox buffer, string pattern, Color color, RegexOptions options = RegexOptions.None)
        {
            MatchCollection matches = Regex.Matches(buffer.Text, pattern, options);

            foreach (Match match in matches)
            {
                buffer.Select(match.Index, match.Length);
                buffer.SelectionColor = color;
            }
        }

        // Enable C++ Editor Mode
        private void SetCppEditorMode()
        {
            isCppEditorMode = true;
            textBoxMain.TextChanged += textBoxMain_TextChanged;
            ApplyCppHighlighting(); // Trigger initial highlighting
        }

        // Disable C++ Editor Mode
        private void SetPlainTextMode()
        {
            isCppEditorMode = false;
            textBoxMain.TextChanged -= textBoxMain_TextChanged;

            // Reset all text to default color
            textBoxMain.SelectAll();
            textBoxMain.SelectionColor = defaultTextColor;
            textBoxMain.DeselectAll();
        }
    }
}
