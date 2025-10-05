# IDA Picker

![Screenshot of IDA Picker](./Assets/screenshot.png)

A WPF application that allows users to quickly select and launch different IDA installations when opening binary files or IDA project files, useful when you have multiple IDBs that use different versions of IDA. The application automatically detects all installed IDA versions on the system, letting the user pick whatever IDA instance they want to use for the file.

## Requirements

- Windows 10/11
- .NET 9.0 or later
- One or more IDA installations

## Usage

### Basic Usage

Launch the application with a file path as an argument:

```
# Load a sample
ida-picker.exe "C:\malware\sample.exe"

# Load an IDB
ida-picker.exe "C:\malware\sample.exe.i64"
```

The application will:
1. Display all detected IDA installations
2. Show each installation with its icon, name, and version
3. Allow selection via mouse click or keyboard (1-9)
4. Launch the selected IDA version with the specified file

### Theme Support

The application will also automatically checks for the current theme applied on the Windows installation and choose between dark and light mode.

### Keyboard Shortcuts

- **1-9**: Select and launch the corresponding IDA installation
- **Numpad 1-9**: Same as above
- **Esc** or **Cancel button**: Close the application

### Context Menu Integration (Optional)

To integrate with Windows Explorer right-click menu, create a registry entry or use a tool like ShellMenuNew or FileTypesMan to add a "Open with IDA..." option that calls `ida-picker.exe "%1"`.

## Why?

> Shouldn't you only ever really need one version of IDA installed at a time? Who is this for?

For most people, yes, you should only really need one version of IDA installed and or daily one at a time, usually the latest. However, if you're like me who tinkers with IDAPython and or IDA a lot in general, you might need multiple installed at once for the following reasons,

- Maintaining IDAPython compatibility
    - As a plugin developer, you may want to keep multiple IDA versions installed to test compatibility across versions, as many of the IDA userbase unfortunately do not always update their IDA to the latest version for numerous reasons.
    - As a plugin end-user, you may notice that certain IDAPython plugins may not work on the latest version of IDA, either because the developer has abandoned the project or because the plugin has not been updated for the latest IDA environment yet. For example, numerous IDAPython-based plugins have been broken for 9.2 due to the lack of PySide6 compatibility and therefore cannot function properly on 9.2.

- Maintaining IDB compatibility 
    - When an IDB saved with an older version of IDA is opened by a newer version of IDA, and when the new IDA saves over the old database, the same IDB can no longer be opened by the previous version of IDA that had originally created the IDB due to the version checks present in IDA. Ideally, you should be the only one using the IDBs you have personally made, and therefore the versioning problem shouldn't be a big deal. However, when you are collaborating with others (without considering IDA Team), your collaborator may not have access to the same IDA version as you. In this case, you may want to use the same IDA build as theirs to prevent version incompatibility.

- Using solutions that rely on undocumented IDA features not present on the latest version
    - The community at large has developed a lot of scripts and or functionalities that revolve around features and or utilities that Hex-rays has either deprecated or was never officially documented. Lumen, a community-based private Lumina server for example only work well on IDA prior to 9.x due to the new certificate checks introduced in 9.x.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Disclaimer

I vibecoded this.
