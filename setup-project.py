import os
import subprocess

namespace = input("Namespace: ")
unity_name = input("Unity project name: ")
monoscript_name = input("Addressable build name: ")

LOCAL_PATH = os.path.abspath(os.path.dirname(__file__))
SOLUTION_PATH = os.path.join(LOCAL_PATH, "source", namespace)
SOLUTION_FILE = os.path.join(SOLUTION_PATH, namespace + ".sln")
PROJECT_PATH = os.path.join(LOCAL_PATH, "source", namespace, namespace)
PROJECT_FILE = os.path.join(PROJECT_PATH, namespace + ".csproj")
UNITY_PROJ_PATH = os.path.join(LOCAL_PATH, "unity", unity_name)

os.rename(os.path.join(LOCAL_PATH, "source", "TemplateMod"), SOLUTION_PATH)
os.rename(os.path.join(SOLUTION_PATH, "TemplateMod"), PROJECT_PATH)
os.rename(os.path.join(SOLUTION_PATH, "TemplateMod.sln"), SOLUTION_FILE)
os.rename(os.path.join(PROJECT_PATH, "TemplateMod.csproj"), PROJECT_FILE)
os.rename(os.path.join(LOCAL_PATH, "unity", "ModAssets"), UNITY_PROJ_PATH)

def replace_instances_of_string_in_file(path : str, to_replace : str, replacement : str) -> None:
    with open(path) as f:
        oldContent = f.read()
    with open(path, "w") as f:
        f.write(oldContent.replace(to_replace, replacement))
        f.truncate();

replace_instances_of_string_in_file(SOLUTION_FILE, "TemplateMod", namespace)
replace_instances_of_string_in_file(os.path.join(PROJECT_PATH, "Plugin.cs"), "TemplateMod", namespace)
replace_instances_of_string_in_file(os.path.join(PROJECT_PATH, "Assets", "AssetManager.cs"), "TemplateMod", namespace)
build_addressables_script = os.path.join(UNITY_PROJ_PATH, "Assets", "BuildPipeline", "Editor", "Building", "AddressableBuilder.cs")
replace_instances_of_string_in_file(build_addressables_script, "TemplateMod", namespace)
replace_instances_of_string_in_file(build_addressables_script, "templatemod", monoscript_name)

subprocess.run("py setup-unity-project.py", cwd=UNITY_PROJ_PATH)
