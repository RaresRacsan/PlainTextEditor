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
        private ToolStripMenuItem findReplaceMenuItem;
        private ToolStrip toolStripFindReplace;
        private ToolStripTextBox findTextBox;
        private ToolStripTextBox replaceTextBox;
        private ToolStripButton findNextButton;
        private ToolStripButton findPrevButton;
        private ToolStripButton replaceButton;
        private int currentSearchIndex = 0;

        // Import user32.dll to get scroll position if needed
        [DllImport("user32.dll")]
        private static extern int GetScrollPos(IntPtr hWnd, int nBar);

        private const int SB_VERT = 1;

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
    }
}
