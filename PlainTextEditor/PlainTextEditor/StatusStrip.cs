using System.Windows.Forms;

namespace PlainTextEditor
{
    public partial class PlainTextEditor : Form
    {
        private void InitializeStatusStrip()
        {
            statusStrip = new StatusStrip();
            toolStripStatusLabelWordCount = new ToolStripStatusLabel { Text = "Words: 0" };
            toolStripStatusLabelCharCount = new ToolStripStatusLabel { Text = "Characters: 0" };

            statusStrip.Items.Add(toolStripStatusLabelWordCount);
            statusStrip.Items.Add(toolStripStatusLabelCharCount);

            this.Controls.Add(statusStrip);
        }

        private void UpdateStatusCounts()
        {
            string text = textBoxMain.Text;
            characters = text.Length;
            words = string.IsNullOrEmpty(text) ? 0 : text.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;

            toolStripStatusLabelWordCount.Text = $"Words: {words}";
            toolStripStatusLabelCharCount.Text = $"Characters: {characters}";
        }
    }
}