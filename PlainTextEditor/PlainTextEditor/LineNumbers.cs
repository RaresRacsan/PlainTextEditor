using System.Drawing;
using System.Windows.Forms;

namespace PlainTextEditor
{
    public partial class PlainTextEditor : Form
    {
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

        private void TextBoxMain_VScroll(object sender, EventArgs e)
        {
            panelLineNumbers.Invalidate();
        }

        private void TextBoxMain_TextChanged_ForLineNumbers(object sender, EventArgs e)
        {
            panelLineNumbers.Invalidate();
            UpdateStatusCounts(); // Ensure status counts are updated
        }

        private void TextBoxMain_Resize(object sender, EventArgs e)
        {
            panelLineNumbers.Invalidate();
        }

        private int GetFirstVisibleLine(RichTextBox rtb)
        {
            int firstCharIndex = rtb.GetCharIndexFromPosition(new Point(0, 0));
            int firstLine = rtb.GetLineFromCharIndex(firstCharIndex);
            return firstLine;
        }
    }
}