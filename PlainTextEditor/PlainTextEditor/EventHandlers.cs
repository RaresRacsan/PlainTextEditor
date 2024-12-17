using System;
using System.IO;
using System.Windows.Forms;

namespace PlainTextEditor
{
    public partial class PlainTextEditor : Form
    {
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
            MessageBox.Show("A simple notepad created by Rares Racsan using C# and Win.Forms\nFor more details check @RaresRacsan on github.", "About");
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

        private void shortcutsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Shortcuts:\n- CTRL + N - new file\n- CTRL + S - save file\n- CTRL + O - open file\n- CTRL + P - print file\n- CTRL + W - close file\n- CTRL + T - change theme\n-CTRL + F - find and replace\n- CTRL + '+' - increase font size\n- CTRL + '-' - decrease font size\n- CTRL + '.' - change to c++ mode\n- CTRL + ',' - change to plain text mode", "Shortcuts");
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

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

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
    } 
}