﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace PlainTextEditor
{
    public partial class PlainTextEditor : Form
    {
        private void ToggleBookmark(int lineNumber)
        {
            lineNumber += 1;

            if (bookmarks == null)
            {
                bookmarks = new List<int>();
            }

            if (bookmarks.Contains(lineNumber))
            {
                bookmarks.Remove(lineNumber);
            }
            else
            {
                bookmarks.Add(lineNumber);
            }

            if (currentFilePath != null)
            {
                allBookmarks[currentFilePath] = bookmarks;
            }

            panelLineNumbers.Invalidate();
        }

        private void SaveAllBookmarks()
        {
            try
            {
                if (currentFilePath != null)
                {
                    allBookmarks[currentFilePath] = bookmarks;
                    string json = System.Text.Json.JsonSerializer.Serialize(allBookmarks);
                    File.WriteAllText(bookmarksStoragePath, json);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR saving bookmarks: {ex.Message}");
            }
        }

        private void LoadAllBookmarks()
        {
            try
            {
                if (File.Exists(bookmarksStoragePath))
                {
                    string json = File.ReadAllText(bookmarksStoragePath);
                    allBookmarks = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, List<int>>>(json)
                                  ?? new Dictionary<string, List<int>>();

                    if (currentFilePath != null && allBookmarks.ContainsKey(currentFilePath))
                    {
                        bookmarks = allBookmarks[currentFilePath];
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR loading bookmarks: {ex.Message}");
            }
        }
    }
}