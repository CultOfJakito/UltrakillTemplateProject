using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

namespace Editor.WaffleBuildPipeline
{
	public class BuildAddressables : MonoBehaviour
	{
		//EDIT THESE!!
		
		//AssetPathLocation needs to lead to a getter that returns the path where you store all your bundles in the mod.
		private const string AssetPathLocation = "{Ultracrypt.Assets.AssetPath}";
		//This is just the name of your mod.
		private const string MonoscriptBundleNaming = "ultracrypt";
		
		//Don't touch these unless you know what you're doing.
		private const string ResultPath = "Built Bundles";
		private const string WbpTemplateName = "WBP Assets";
		private const string MonoscriptPostfix = "_monoscripts.bundle";
		private const string DefaultAddressablesCatalogPath = "{UnityEngine.AddressableAssets.Addressables.RuntimePath}";
		
		private static AddressableAssetSettings Settings => AddressableAssetSettingsDefaultObject.Settings;
		private static string AddressablesLibrary => Application.dataPath + "/../Library/com.unity.addressables/aa/Windows";
		private static string OldMonoscriptPath => $@"{DefaultAddressablesCatalogPath}\\{EditorUserBuildSettings.activeBuildTarget}\\{MonoscriptBundleNaming + MonoscriptPostfix}";
		private static string NewMonoscriptPath => $@"{AssetPathLocation}\\{MonoscriptBundleNaming + MonoscriptPostfix}";
		
		[MenuItem("WaffleBuildPipeline/Build")]
		public static void Build()
		{
			ValidateAddressables();
			EnsureCustomTemplateExists();
			SetCorrectValuesForSettings();
			
			if (!Directory.Exists(ResultPath))
			{
				Directory.CreateDirectory(ResultPath);
			}
			
			AddressableAssetSettings.BuildPlayerContent();
			
			if (Directory.Exists(Path.Combine(Application.dataPath + "/../Library/com.unity.addressables/aa/Windows", "StandaloneWindows")))
			{
				EditorUtility.DisplayDialog("Switch version", "You need to be on _x86_64 in build settings or addressables won't work.", "ok", "also ok");
			}

			FixAndCopyCatalog();
		}

		private static void SetCorrectValuesForSettings()
		{
			Settings.profileSettings.SetValue(Settings.activeProfileId, "RemoteBuildPath", ResultPath);
			Settings.profileSettings.SetValue(Settings.activeProfileId, "RemoteLoadPath", AssetPathLocation);
			
			Settings.IgnoreUnsupportedFilesInBuild = true;
			Settings.OverridePlayerVersion = string.Empty;
			Settings.BuildRemoteCatalog = false;
			Settings.MonoScriptBundleCustomNaming = MonoscriptBundleNaming;
		}
		
		private static void EnsureCustomTemplateExists()
		{
			foreach (ScriptableObject so in Settings.GroupTemplateObjects)
			{
				if (so.name == WbpTemplateName)
				{
					return;
				}
			}

			if (!Settings.CreateAndAddGroupTemplate(WbpTemplateName, "Assets for Waffle's Build Pipeline.", typeof(BundledAssetGroupSchema)))
			{
				Debug.LogError($"Failed to create the '{WbpTemplateName}' template, whar?");
				return;
			}

			AddressableAssetGroupTemplate wbpAssetsTemplate = Settings.GroupTemplateObjects[Settings.GroupTemplateObjects.Count - 1] as AddressableAssetGroupTemplate;

			if ((bool)wbpAssetsTemplate && wbpAssetsTemplate.Name != WbpTemplateName)
			{
				Debug.LogError($"Somehow got wrong template, this shouldn't be possible? [got {wbpAssetsTemplate.name}]");
				return;
			}

			BundledAssetGroupSchema groupSchema = wbpAssetsTemplate.GetSchemaByType(typeof(BundledAssetGroupSchema)) as BundledAssetGroupSchema;

			if (!(bool)groupSchema)
			{
				Debug.LogError("Getting the schema from the template is null?");
				return;
			}
			
			groupSchema.IncludeInBuild = true;
			groupSchema.IncludeAddressInCatalog = true;
			groupSchema.BuildPath.SetVariableByName(Settings, "RemoteBuildPath");
			groupSchema.LoadPath.SetVariableByName(Settings, "RemoteLoadPath");
			groupSchema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.NoHash;
			groupSchema.UseAssetBundleCrcForCachedBundles = false;
			groupSchema.UseAssetBundleCrc = false;
		}
		
		// TundraEditor: Core/Editor/TundraInit.cs
		// thanks pitr i stole this completely ;3
		private static void ValidateAddressables(bool forceRewrite = false)
		{
			// TODO check the content
			var templatePostfix = ".template";
			var metaPostfix = ".meta";

			var assetPath = "Assets/AddressableAssetsData/AddressableAssetSettings.asset";
			var assetTemplatePath = assetPath + templatePostfix;

			var metaPath = assetPath + metaPostfix;
			var metaTemplatePath = assetPath + metaPostfix + templatePostfix;

			var valid = File.Exists(assetPath);

			if (!valid || forceRewrite)
			{
				Debug.Log($"Rewriting Addressables: {assetPath}");
				File.Copy(assetTemplatePath, assetPath, true);
				File.Copy(metaTemplatePath, metaPath, true);
				// Mark the asset database as dirty to force a refresh
				AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
				AddressableAssetSettingsDefaultObject.Settings = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>(assetPath);
			}
		}

		private static void FixAndCopyCatalog()
		{
			// afaik you cant change the monoscript bundle load path, so you have to edit the catalog :3
			// fucking kill me, but we ball
			string catalog = File.ReadAllText(Path.Combine(AddressablesLibrary, "catalog.json"));
			catalog = catalog.Replace(OldMonoscriptPath, NewMonoscriptPath);
			File.WriteAllText(Path.Combine(ResultPath, "catalog.json"), catalog);
			File.Copy(Path.Combine(AddressablesLibrary, "StandaloneWindows64", MonoscriptBundleNaming + MonoscriptPostfix), 
				Path.Combine(ResultPath, MonoscriptBundleNaming + MonoscriptPostfix), true);
		}
	}
}