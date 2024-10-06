import os
import shutil
import stat
import subprocess

LOCAL_PATH = os.path.abspath(os.path.dirname(__file__))
complete_submodules = []

class Style():
    BLACK = '\033[30m'
    RED = '\033[31m'
    GREEN = '\033[32m'
    YELLOW = '\033[33m'
    BLUE = '\033[34m'
    MAGENTA = '\033[35m'
    CYAN = '\033[36m'
    WHITE = '\033[37m'
    UNDERLINE = '\033[4m'
    RESET = '\033[0m'

os.system("") # fixes win10 colours,, lol !?

def clone_repo(repo : str, path : str) -> None:
    if subprocess.call(f"git clone \"git@github.com:{repo}.git\" \"{path}\"") != 0:
        print(f"{Style.RED}failure cloning {repo}! the installer will delete everything and close.{Style.RESET}")
        error_cleanup()
        exit(1)
    print(f"{Style.GREEN}successfully cloned {repo}!{Style.RESET}")

def add_submodule(repo : str, path : str) -> None:
    return_code = subprocess.call(f"git submodule add \"git@github.com:{repo}.git\" \"{path}\"")
    complete_submodules.append((repo, path))
    if return_code != 0:
        print(f"{Style.RED}failure cloning {repo}! the installer will delete everything and close.{Style.RESET}")
        error_cleanup()
        exit(1)
    print(f"{Style.GREEN}successfully added {repo} as a submodule!{Style.RESET}")

def copy_folders_from_repo(repo_name : str, destination : str, folders : list[str]):
    TEMP_NAME = "InstallerTemp"
    temp_folder = os.path.join(LOCAL_PATH, TEMP_NAME)
    if os.path.exists(temp_folder):
        shutil.rmtree(temp_folder, onerror = remove_readonly)
    clone_repo(repo_name, TEMP_NAME)
    for folder in folders:
        copy_path = os.path.join(destination, folder)
        if os.path.exists(copy_path):
            shutil.rmtree(copy_path, onerror = remove_readonly)
        shutil.copytree(os.path.join(temp_folder, folder), copy_path)
    shutil.rmtree(temp_folder, onerror = remove_readonly)

def remove_readonly(func, path, _):
    # https://stackoverflow.com/questions/21261132/shutil-rmtree-to-remove-readonly-files
    # clear the readonly bit and reattempt the removal
    try:
        os.chmod(path, stat.S_IWRITE)
        func(path)
    except:
        print(f"{Style.RED}failed to delete {path}!{Style.RESET}")

def error_cleanup() -> None:
    for tuple in complete_submodules:
        print(f"{Style.RED}removing repo {tuple[0]} at {tuple[1]} {Style.RESET}")
        subprocess.call(f"git rm \"{tuple[1]}\"")

# copy configs
copy_folders_from_repo("Tundra-Editor/Core", os.path.join(LOCAL_PATH, "Assets"), ["Lib", "Runtime"])
shutil.copytree(os.path.join(LOCAL_PATH, "Assets", "BuiltInResources"), os.path.join(LOCAL_PATH, "Assets", "BuildPipeline", "BuiltInResources"))

#copy runtime dir
copy_folders_from_repo("Tundra-Editor/Config", os.path.join(LOCAL_PATH), ["ProjectSettings", "Packages"])

# add addressableassetsdata
addressable_folder = os.path.join(LOCAL_PATH, "Assets", "AddressableAssetsData")
if os.path.exists(addressable_folder):
    shutil.rmtree(addressable_folder, onerror = remove_readonly)
clone_repo("Tundra-Editor/AddressableAssetsData", "Assets/AddressableAssetsData")
addressable_git_folder = os.path.join(addressable_folder, ".git")
if os.path.exists(addressable_git_folder):
    shutil.rmtree(addressable_git_folder, onerror = remove_readonly)
shutil.copy(os.path.join(addressable_folder, "AddressableAssetSettings.asset.template"), os.path.join(addressable_folder, "AddressableAssetSettings.asset"))
shutil.copy(os.path.join(addressable_folder, "AddressableAssetSettings.asset.meta.template"), os.path.join(addressable_folder, "AddressableAssetSettings.asset.meta"))

# add submodules
add_submodule("Tundra-Editor/Assets", "Assets/Common")
add_submodule("Tundra-Editor/Components", "Assets/Components")
add_submodule("Tundra-Editor/Prefabs", "Assets/ULTRAKILL Prefabs")