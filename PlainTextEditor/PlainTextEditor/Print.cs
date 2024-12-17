using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace PlainTextEditor
{
    public partial class PlainTextEditor : Form
    {
        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                printPreviewDialog.Document = printDocument;
                printText = textBoxMain.Text;
                printPreviewDialog.Width = 800;
                printPreviewDialog.Height = 600;
                printPreviewDialog.Text = "Print Preview - PlainTextEditor";
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
            e.HasMorePages = lines.Length > count;
        }
    }
}