using System;
using System.Windows.Forms;

namespace PlainTextEditor
{
    public partial class PlainTextEditor : Form
    {
        private void PlainTextEditor_KeyDown(object sender, KeyEventArgs e)
        {
            // Shortcut implementations
            // Find and Replace
            if (e.Control && e.KeyCode == Keys.F)
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
    }
}