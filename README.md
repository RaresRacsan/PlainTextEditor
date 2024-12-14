# PlainTextEditor
PlainTextEditor is a simple text editor built using C# and Windows Forms. It provides basic text editing functionality, including features like creating new files, opening existing files, saving files, and changing the app's theme (light/dark).

## Features

- **New File**: Start a fresh document.
- **Open File**: Open and edit existing files.
- **Print File**: Print current file.
- **Save/Save As**: Save the current document to the same file or choose a new location.
- **Light/Dark Theme**: Toggle between light and dark themes for the editor.
- **Text Editing**: Basic text editing functionality with the ability to type, edit, and delete text.
- **Mode Switching (Plain Text / C++ Editor)**: Switch between Plain Text mode and C++ Editor mode. In C++ mode, syntax highlighting is applied to variable types, control flow keywords, and #include statements.
- **Keyboard Shortcuts**: Use keyboard shortcuts for efficient navigation and file operations.
- **Bracket Matching**: Automatically closes the bracket.

## Keyboard Shortcuts
- **Ctrl + N**: New File.
- **Ctrl + O**: Open File.
- **Ctrl + S**: Save File.
- **Ctrl + T**: Change between themes.
- **Ctrl + P**: Print File.
- **Ctrl + W**: Close File.
- **Ctrl + '+/-'**: Increase/Decrease size of font.
- **Ctrl + '.'**: Change to C++ mode.
- **Ctrl + ','**: Change to Plain Text mode.
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
   git clone https://github.com/RaresRacsan/PlainTextEditor.git
   cd PlainTextEditor
   ```

2. Open the solution file PlainTextEditor.sln in Visual Studio.

3. Ensure you have the required dependencies and libraries installed (Visual Studio should manage these automatically).

4. Press F5 to run the application.

## Usage

1. Create New File: Click File -> New to start a new document. You can then begin typing your text.
2. Open Existing File: Click File -> Open to open an existing text file (.txt).
3. Save: Click File -> Save to save the current file. If the file has not been saved before, the "Save As" dialog will appear.
4. Save As: Click File -> Save As to save the current document to a different location or under a new name.
5. Change Theme: Under the Edit menu, click Theme -> Light Theme or Dark Theme to toggle between the two themes.
6. Text Editing: Use the text box to edit the text content. The text will be saved when you save the file.
7. Print: Click File -> Print -> Proceed to print from the preview page
8. Switch Modes:
   - Plain Text Mode: Switch to Plain Text mode where no syntax highlighting is applied.
   - C++ Editor Mode: Switch to C++ mode, where syntax highlighting for C++ keywords, variable types, control flow statements, and #include directives is applied.

## Screenshots

![image](https://github.com/user-attachments/assets/cac381af-463e-4579-9912-6d8ef4faa632)

![image](https://github.com/user-attachments/assets/31fc89a9-3702-46b0-830a-a5c260975034)


## File Operations

- New File: When starting a new file, it automatically clears the text box for new content.
- Open File: Opens an existing .txt file and loads its content into the text box.
- Save: If the file is already saved, it overwrites the file. Otherwise, it prompts the user with the "Save As" dialog.
- Save As: Prompts the user to choose a location to save the file with a new name.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Contributing

Feel free to fork this repository and submit pull requests. If you encounter any bugs or want to request new features, feel free to open an issue!

Thank you for using PlainTextEditor!
