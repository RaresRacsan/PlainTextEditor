using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace PlainTextEditor
{
    public partial class PlainTextEditor : Form
    {
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

        private void ChangeFontSize(int newSize)
        {
            textBoxMain.Font = new Font(textBoxMain.Font.FontFamily, newSize);
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
    }
}