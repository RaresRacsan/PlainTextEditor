using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PlainTextEditor
{
    public partial class PlainTextEditor : Form
    {
        /// <summary>
        /// Click event for the light theme menu item - change the current theme to light theme
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lightThemeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetLightTheme();
        }

        /// <summary>
        /// Click event for the dark theme menu item - change the current theme to dark theme
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void darkThemeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetDarkTheme();
        }

        /// <summary>
        /// Setting all the components of the form to light theme
        /// </summary>
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

        /// <summary>
        /// Setting all the components of the form to dark theme
        /// </summary>
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
        /// Helper function that checks whether the current theme is dark theme
        /// </summary>
        /// <returns></returns>
        private bool IsDarkTheme()
        {
            return menuStrip.BackColor == Color.FromArgb(40, 40, 40);
        }

        /// <summary>
        /// Helper function to assign the custom rendered to the menustrip
        /// </summary>
        private void AssignCustomRenderer()
        {
            menuStrip.Renderer = new ToolStripProfessionalRenderer(new CustomColorTable(IsDarkTheme()));
        }

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

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE dwAttribute, ref uint pvAttribute, uint cbAttribute);

        public enum DWMWINDOWATTRIBUTE
        {
            DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
        }

        /// <summary>
        /// Helper function to enable the dark theme for the title bar
        /// </summary>
        private void SetTitleBarColor()
        {
            uint value = 1; // Enable dark mode for the title bar
            DwmSetWindowAttribute(this.Handle, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, ref value, (uint)Marshal.SizeOf(value));
        }

        /// <summary>
        /// Custom rendered for the toolstrip - no borders
        /// </summary>
        public class CustomRenderer : ToolStripRenderer
        {
            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
            {
                // Don't render the border
            }
        }

        /// <summary>
        /// Function that updates the color of the toolstrip depending on the current theme
        /// </summary>
        private void UpdateToolStripColor()
        {
            if (toolStripFindReplace != null)
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
    }
}