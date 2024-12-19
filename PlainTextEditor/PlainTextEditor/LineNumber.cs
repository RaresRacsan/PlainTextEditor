using System.Drawing;
using System.Windows.Forms;

namespace PlainTextEditor
{
    public partial class PlainTextEditor : Form
    {
        /// <summary>
        /// Painting line numbers and bookmarks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panelLineNumbers_Paint(object sender, PaintEventArgs e)
        {
            panelLineNumbers.Width = 50;

            int firstVisibleLine = GetFirstVisibleLine(textBoxMain);
            int totalLines = textBoxMain.GetLineFromCharIndex(textBoxMain.TextLength) + 1;

            using (Font lineNumberFont = new Font(textBoxMain.Font.FontFamily, textBoxMain.Font.Size))
            {
                float lineHeight = textBoxMain.Font.GetHeight(e.Graphics);
                float lineSpacing = lineHeight * 1.01f;
                int verticalOffset = 3;
                int visibleLines = (int)(panelLineNumbers.Height / lineSpacing);
                Brush brush = new SolidBrush(panelLineNumbers.ForeColor);

                for (int i = 0; i < visibleLines; i++)
                {
                    int lineNumber = firstVisibleLine + i + 1;
                    if (lineNumber > totalLines) break;

                    float yPosition = (i * lineSpacing) + verticalOffset;
                    string lineNumberText = lineNumber.ToString();
                    Size textSize = TextRenderer.MeasureText(lineNumberText, lineNumberFont);
                    Point drawPoint = new Point(
                        panelLineNumbers.Width - textSize.Width - 5,
                        (int)yPosition
                    );

                    TextRenderer.DrawText(
                        e.Graphics,
                        lineNumberText,
                        lineNumberFont,
                        drawPoint,
                        panelLineNumbers.ForeColor,
                        Color.Transparent,
                        TextFormatFlags.NoPadding
                    );

                    // Draw bookmark if exists for this line
                    if (bookmarks.Contains(lineNumber))
                    {
                        float bookmarkY = yPosition + (lineHeight / 4);
                        e.Graphics.FillEllipse(Brushes.Red, new RectangleF(5, bookmarkY, 8, 8));
                    }
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