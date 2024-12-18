using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace PlainTextEditor
{
    public partial class PlainTextEditor : Form
    {
        // Keyword categories (class-level)
        private static string[] variableTypeKeyWords = { "int", "float", "double", "char", "bool", "wchar_t", "void", "short", "signed", "unsigned", "long", "auto", "decltype", "const", "constexpr", "mutable", "nullptr", "true", "false" };
        private static string[] controlFlowKeywords = { "if", "else", "switch", "case", "default", "while", "do", "for", "break", "continue", "return", "goto" };
        private static string[] cppStandardKeywords = { "std", "cout", "cin", "endl", "namespace", "using", "::" };
        private static string[] includeDirectives = { "#include" };
        private static string[] classRelatedKeywords = { "class", "struct", "public", "private", "protected", "virtual", "friend", "static", "explicit", "inline", "final", "override", "abstract", "typeid", "typename", "this", "new", "delete", "new[]", "delete[]" };
        private static string[] exceptionHandling = { "try", "catch", "throw", "noexcept" };
        private static string[] typeAndTypeModifiers = { "typedef", "typeid", "typename", "volatile", "alignas", "alignof", "thread_local" };
        private static string[] templateKeyWords = { "template" };
        private static string[] miscellaneous = { "const_cast", "dynamic_cast", "reinterpret_cast", "static_cast" };

        // Dictionary for keyword categories and their colors
        private Dictionary<string[], Color> keywordCategories = new Dictionary<string[], Color>
        {
            { variableTypeKeyWords, Color.FromArgb(0, 123, 255) }, // - Blue -
            { controlFlowKeywords, Color.FromArgb(255, 105, 180) }, // - Pink -
            { cppStandardKeywords, Color.FromArgb(255, 159, 28) }, // - Orange -
            { includeDirectives, Color.FromArgb(34, 139, 34) }, // - Forest Green -
            { classRelatedKeywords, Color.FromArgb(0, 128, 128) }, // - Teal -
            { exceptionHandling, Color.FromArgb(255, 193, 7) }, // - Yellow -
            { typeAndTypeModifiers, Color.FromArgb(220, 53, 69) }, // - Red -
            { templateKeyWords, Color.FromArgb(173, 216, 230) }, // - Light Blue -
            { miscellaneous, Color.FromArgb(169, 169, 169) } // - Gray -
        };

        private void InitTextColorMenu()
        {
            // Create sub-items for each keyword category
            changeTextColorMenu.DropDownItems.Add(CreateTextColorMenuItem("Variable Types", variableTypeKeyWords));
            changeTextColorMenu.DropDownItems.Add(CreateTextColorMenuItem("Control Flow", controlFlowKeywords));
            changeTextColorMenu.DropDownItems.Add(CreateTextColorMenuItem("C++ Standard", cppStandardKeywords));
            changeTextColorMenu.DropDownItems.Add(CreateTextColorMenuItem("Include Directives", includeDirectives));
            changeTextColorMenu.DropDownItems.Add(CreateTextColorMenuItem("Class Keywords", classRelatedKeywords));
            changeTextColorMenu.DropDownItems.Add(CreateTextColorMenuItem("Exception Handling", exceptionHandling));
            changeTextColorMenu.DropDownItems.Add(CreateTextColorMenuItem("Miscellaneous", miscellaneous));

            // Add Reset to Default
            ToolStripMenuItem resetToDefaultMenuItem = new ToolStripMenuItem("Reset to Default");
            resetToDefaultMenuItem.Click += ResetToDefault_Click;
            changeTextColorMenu.DropDownItems.Add(resetToDefaultMenuItem);

            // Add Change Text Color to Edit Menu
            editToolStripMenuItem.DropDownItems.Add(changeTextColorMenu);
        }

        private ToolStripMenuItem CreateTextColorMenuItem(string name, string[] keywordCategory)
        {
            ToolStripMenuItem menuItem = new ToolStripMenuItem(name);
            menuItem.Tag = keywordCategory; // Use the Tag to store the keyword category
            menuItem.Click += ChangeTextColor_Click;
            return menuItem;
        }

        private void ChangeTextColor_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem clickedItem = sender as ToolStripMenuItem;
            if (clickedItem == null) return;

            // Get the keyword category linked to the clicked item
            string[] keywordCategory = clickedItem.Tag as string[];
            if (keywordCategory == null) return;

            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    // Update the color for the selected keyword category
                    keywordCategories[keywordCategory] = colorDialog.Color;

                    // Reapply highlighting
                    ApplyCppHighlighting();

                    // Save settings
                    SaveColorSettings();
                }
            }
        }

        private void ResetToDefault_Click(object sender, EventArgs e)
        {
            // Restore the default colors
            keywordCategories = new Dictionary<string[], Color>
            {
                { variableTypeKeyWords, Color.FromArgb(0, 123, 255) }, // Blue
                { controlFlowKeywords, Color.FromArgb(255, 105, 180) }, // Pink
                { cppStandardKeywords, Color.FromArgb(255, 159, 28) }, // Orange
                { includeDirectives, Color.FromArgb(34, 139, 34) }, // Forest Green
                { classRelatedKeywords, Color.FromArgb(0, 128, 128) }, // Teal
                { exceptionHandling, Color.FromArgb(255, 193, 7) }, // Yellow
                { typeAndTypeModifiers, Color.FromArgb(220, 53, 69) }, // Red
                { templateKeyWords, Color.FromArgb(173, 216, 230) }, // Light Blue
                { miscellaneous, Color.FromArgb(169, 169, 169) } // Gray
            };

            // Reapply highlighting
            ApplyCppHighlighting();

            // Save settings
            SaveColorSettings();
        }

        private void SaveColorSettings()
        {
            var settings = new Dictionary<string, string>();

            foreach (var category in keywordCategories)
            {
                string key = string.Join(",", category.Key); // Unique key for each category
                settings[key] = ColorTranslator.ToHtml(category.Value);
            }

            File.WriteAllText("colorSettings.json", JsonConvert.SerializeObject(settings));
        }

        private void LoadColorSettings()
        {
            if (!File.Exists("colorSettings.json")) return;

            var settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText("colorSettings.json"));

            foreach (var category in keywordCategories.Keys)
            {
                string key = string.Join(",", category); // Match the saved key
                if (settings.ContainsKey(key))
                {
                    keywordCategories[category] = ColorTranslator.FromHtml(settings[key]);
                }
            }
        }

        private void HighlightCppKeyWords(RichTextBox buffer)
        {
            if (!isCppEditorMode) return;

            foreach (var category in keywordCategories)
            {
                string[] keywords = category.Key;
                Color color = category.Value;

                foreach (string keyword in keywords)
                {
                    int startIndex = 0;
                    while ((startIndex = buffer.Text.IndexOf(keyword, startIndex)) != -1)
                    {
                        bool isWordBoundary = (startIndex == 0 || !char.IsLetterOrDigit(buffer.Text[startIndex - 1])) &&
                                              (startIndex + keyword.Length == buffer.Text.Length || !char.IsLetterOrDigit(buffer.Text[startIndex + keyword.Length]));
                        if (isWordBoundary)
                        {
                            buffer.Select(startIndex, keyword.Length);
                            buffer.SelectionColor = color;
                        }
                        startIndex += keyword.Length;
                    }
                }
            }
        }


        private void HighlightPattern(RichTextBox buffer, string pattern, Color color, RegexOptions options = RegexOptions.None)
        {
            MatchCollection matches = Regex.Matches(buffer.Text, pattern, options);

            foreach (Match match in matches)
            {
                buffer.Select(match.Index, match.Length);
                buffer.SelectionColor = color;
            }
        }

        private void SetCppEditorMode()
        {
            isCppEditorMode = true;
            textBoxMain.TextChanged += textBoxMain_TextChanged;
            ApplyCppHighlighting(); // Trigger initial highlighting
        }

        private void SetPlainTextMode()
        {
            isCppEditorMode = false;
            textBoxMain.TextChanged -= textBoxMain_TextChanged;

            // Reset all text to default color
            textBoxMain.SelectAll();
            textBoxMain.SelectionColor = defaultTextColor;
            textBoxMain.DeselectAll();
        }

        private void ApplyCppHighlighting()
        {
            if (!isCppEditorMode) return; // Skip if not in C++ mode

            string text = textBoxMain.Text;
            int selectionStart = textBoxMain.SelectionStart;
            int selectionLength = textBoxMain.SelectionLength;

            // Temporarily disable TextChanged event to avoid recursion
            textBoxMain.TextChanged -= textBoxMain_TextChanged;

            // Preserve font size and style
            hiddenBuffer.Font = textBoxMain.Font;

            // Perform highlighting on the hidden buffer
            hiddenBuffer.Text = text;
            hiddenBuffer.SelectAll();
            hiddenBuffer.SelectionColor = defaultTextColor; // Reset to default color

            // Highlight patterns and keywords
            HighlightCppKeyWords(hiddenBuffer);
            HighlightPattern(hiddenBuffer, "\".*?\"", Color.LightGreen); // Strings
            HighlightPattern(hiddenBuffer, "<.*?>", Color.LightGreen);  // Angle brackets
            HighlightPattern(hiddenBuffer, "//.*?$", Color.LightGray, RegexOptions.Multiline); // Single-line comments
            HighlightPattern(hiddenBuffer, @"/\*.*?\*/", Color.LightGray, RegexOptions.Singleline); // Multi-line comments

            // Highlight function names
            HighlightPattern(hiddenBuffer, @"\b([a-zA-Z_][a-zA-Z_0-9]*)\s*(?=\()", Color.MediumPurple);

            // Replace the visible TextBox content with the highlighted content
            textBoxMain.Rtf = hiddenBuffer.Rtf;

            // Restore the cursor position and font
            textBoxMain.SelectionStart = selectionStart;
            textBoxMain.SelectionLength = selectionLength;

            // Reattach the TextChanged event
            textBoxMain.TextChanged += textBoxMain_TextChanged;
        }
    }
}