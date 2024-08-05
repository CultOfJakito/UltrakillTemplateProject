# ULTRAKILL Template Project
A template project for making ULTRAKILL mods using the game's assets as references with addressables.

***If you don't have Tundra, this is useless to you.***

## Prerequisites
- Git installed, set up with an account that **has access to the Tundra organization on GitHub**.
- Python installed (I used 3.11, I don't know which version you need)
- .NET SDK 8 installed
  
## Setup
1. Run `py setup-project.py` in the root directory of the repo.
2. Enter the namespace, unity project name, and addressable prefix.

These steps will generate a .NET Standard 2.1 project and Unity project in the `source` and `unity` directories.
