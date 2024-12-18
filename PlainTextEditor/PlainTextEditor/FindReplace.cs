using System;
using System.Drawing;
using System.Windows.Forms;

namespace PlainTextEditor
{
    public partial class PlainTextEditor : Form
    {
        /// <summary>
        /// Initializing the find and replace menu item
        /// </summary>
        private void InitializeFindReplaceMenu()
        {
            findReplaceMenuItem = new ToolStripMenuItem("Find and Replace", null, FindReplaceMenuItem_Click);
            menuStrip.Items.Add(findReplaceMenuItem);
        }

        /// <summary>
        /// Event for the find and replace toolstripmenuitem, creating its fields and assigning the functions to the buttons
        /// </summary>
        private void FindReplaceInitEvent()
        {
            if (toolStripFindReplace == null)
            {
                toolStripFindReplace = new ToolStrip();

                findTextBox = new ToolStripTextBox() { ToolTipText = "Enter text to find" };
                replaceTextBox = new ToolStripTextBox() { ToolTipText = "Enter text to replace" };
                findPrevButton = new ToolStripButton("<-", null, FindPrev_Click) { ToolTipText = "Previous" };
                findNextButton = new ToolStripButton("->", null, FindNext_Click) { ToolTipText = "Next" };
                replaceButton = new ToolStripButton("Replace", null, Replace_Click) { ToolTipText = "Replace content" };

                toolStripFindReplace.Items.Add(new ToolStripLabel("Find: "));
                toolStripFindReplace.Items.Add(findTextBox);
                toolStripFindReplace.Items.Add(new ToolStripLabel("Replace:"));
                toolStripFindReplace.Items.Add(replaceTextBox);
                toolStripFindReplace.Items.Add(findPrevButton);
                toolStripFindReplace.Items.Add(findNextButton);
                toolStripFindReplace.Items.Add(replaceButton);

                this.Controls.Add(toolStripFindReplace);
                toolStripFindReplace.Dock = DockStyle.Top;
                UpdateToolStripColor(); // Theme.cs
            }
            else
            {
                toolStripFindReplace.Visible = !toolStripFindReplace.Visible;
                UpdateToolStripColor(); // Theme.cs
            }
        }

        /// <summary>
        /// Click event for the find and replace menu item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindReplaceMenuItem_Click(object sender, EventArgs e)
        {
            FindReplaceInitEvent();
        }

        /// <summary>
        /// Function to find the next wanted string in the textMainBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindNext_Click(object sender, EventArgs e)
        {
            string searchText = findTextBox.Text;
            if (string.IsNullOrEmpty(searchText)) return;

            int startIndex = textBoxMain.SelectionStart + textBoxMain.SelectionLength;
            currentSearchIndex = textBoxMain.Find(searchText, startIndex, RichTextBoxFinds.None);

            if (currentSearchIndex >= 0)
            {
                textBoxMain.Select(currentSearchIndex, searchText.Length);
                textBoxMain.ScrollToCaret();
            }
            else
            {
                MessageBox.Show("No further matches found.", "Find", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Function to search the previous wanted string in the textMainBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindPrev_Click(object sender, EventArgs e)
        {
            string searchText = findTextBox.Text;
            if (string.IsNullOrEmpty(searchText)) return;

            int startIndex = textBoxMain.SelectionStart - 1;
            currentSearchIndex = textBoxMain.Find(searchText, 0, startIndex, RichTextBoxFinds.Reverse);

            if (currentSearchIndex >= 0)
            {
                textBoxMain.Select(currentSearchIndex, searchText.Length);
                textBoxMain.ScrollToCaret();
            }
            else
            {
                MessageBox.Show("No previous matches found.", "Find", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Function to replace the current wanted string with another string that the user enters into the replaceTextBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Replace_Click(object sender, EventArgs e)
        {
            string searchText = findTextBox.Text;
            string replacementText = replaceTextBox.Text;

            if (string.IsNullOrEmpty(searchText)) return;

            if (string.IsNullOrEmpty(replacementText))
            {
                MessageBox.Show("The Replace With box is empty.", "Replace", MessageBoxButtons.OK);
            }
            else
            {
                if (textBoxMain.SelectedText == searchText) textBoxMain.SelectedText = replacementText;
                FindNext_Click(sender, e);
            }
        }
    }
}