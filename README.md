# ULTRAKILL Template Project
A template project for making ULTRAKILL mods using the game's assets as references with addressables.

## Prerequisites
- [Git](https://git-scm.com/)
- [.NET SDK](https://dotnet.microsoft.com/download)
- [VanityReprised](https://github.com/eternalUnion/VanityReprised)
  
## Setup
1. Create a copy of this repository using the **Use this template** button in the top right
2. Clone the newly generated repository using Git, or a Git client.
3. You now have a copy of this template on your computer!

### C# Project Setup
- If you have a custom ULTRAKILL directory, edit [UltrakilDir.user.example](src/UltrakillDir.user.example) and remove .example from the file name
- You're probably going to need more Unity libraries. These can be added by editing the csproj file, use the format from other imports.

### Unity Project Setup
1. Use Vanity Reprised to generate a Unity project with ULTRAKILL files
2. Copy all the contents of the Unity Project, and move them to the ModAssets directory
3. Delete unnecessary Rude stuff if you're not making a custom level (optional)

These steps will generate a .NET Standard 2.1 project and Unity project in the `src` and `unity` directories respectively.
