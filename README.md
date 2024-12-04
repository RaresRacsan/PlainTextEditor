# PlainTextEditor
PlainTextEditor is a simple text editor built using C# and Windows Forms. It provides basic text editing functionality, including features like creating new files, opening existing files, saving files, and changing the app's theme (light/dark).

## Features

- **New File**: Start a fresh document.
- **Open File**: Open and edit existing files.
- **Save/Save As**: Save the current document to the same file or choose a new location.
- **Light/Dark Theme**: Toggle between light and dark themes for the editor.
- **Text Editing**: Basic text editing functionality with the ability to type, edit, and delete text.

## Technologies Used

- C# (Windows Forms)
- .NET Framework

## Requirements

To run the project, you'll need:

- Visual Studio 2022 or later with the **Windows Forms App** template.
- .NET Framework 4.7.2 or higher.

## Setup and Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/yourusername/MyNotepad.git
   cd MyNotepad

    Open the solution file MyNotepad.sln in Visual Studio.

    Ensure you have the required dependencies and libraries installed (Visual Studio should manage these automatically).

    Press F5 to run the application.

Usage

    Create New File: Click File -> New to start a new document. You can then begin typing your text.
    Open Existing File: Click File -> Open to open an existing text file (.txt).
    Save: Click File -> Save to save the current file. If the file has not been saved before, the "Save As" dialog will appear.
    Save As: Click File -> Save As to save the current document to a different location or under a new name.
    Change Theme: Under the Edit menu, click Theme -> Light Theme or Dark Theme to toggle between the two themes.
    Text Editing: Use the text box to edit the text content. The text will be saved when you save the file.

Screenshot

![image](https://github.com/user-attachments/assets/a6158d32-3125-4334-a7a7-f4a4ea16b41a)
![image](https://github.com/user-attachments/assets/2755ff9b-30b6-4355-92f9-04c8035d60af)
Code Overview

The project is built using Windows Forms, and the code is organized as follows:

    MainForm.cs: Contains the main form with the text box (TextBoxMain) for editing, as well as the menu strip with options like New, Open, Save, Save As, and Theme selection.
    Theme Handling: A custom ToolStripProfessionalRenderer is used to handle dark and light themes, overriding the OnRenderMenuItemBackground method to adjust the look and feel of the menu items.
    Menu Item Actions: The File menu provides actions for creating, opening, saving, and saving as files, while the Edit menu contains the theme toggling functionality.

Theme Customization

The app allows you to toggle between Light and Dark themes. The MenuStrip, TextBox, and background colors adjust according to the selected theme. A custom renderer is used to style the MenuStrip items when hovered or selected.

    Dark Theme: The background is dark with white text.
    Light Theme: The background is light with black text.

File Operations

    New File: When starting a new file, it automatically clears the text box for new content.
    Open File: Opens an existing .txt file and loads its content into the text box.
    Save: If the file is already saved, it overwrites the file. Otherwise, it prompts the user with the "Save As" dialog.
    Save As: Prompts the user to choose a location to save the file with a new name.

License

This project is licensed under the MIT License - see the LICENSE file for details.
Acknowledgements

    The custom theme rendering uses ToolStripProfessionalRenderer and ProfessionalColorTable for customizing the menu and item appearances.
    The project uses standard Windows Forms components for file handling and text editing.

Contributing

Feel free to fork this repository and submit pull requests. If you encounter any bugs or want to request new features, feel free to open an issue!

Thank you for using MyNotepad!
