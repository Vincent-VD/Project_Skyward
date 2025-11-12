using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class EditorHelper : MonoBehaviour
{
	public static readonly string assetExtension = ".asset";
	public static readonly string pngExtension = ".png";
	public static string[] GetFolderAssets()
	{
		string folderPath = Application.dataPath;

		string absFolder = EditorUtility.OpenFolderPanel("Select Folder", folderPath, folderPath);
		if (string.IsNullOrEmpty(absFolder))
		{
			EditorUtility.DisplayDialog("Error", "No Folder Selected", "OK");
			return null;
		}

		// Ensure it's inside the project’s Assets/
		var dataPath = Application.dataPath.Replace("\\", "/");
		absFolder = absFolder.Replace("\\", "/");
		if (!absFolder.StartsWith(dataPath))
		{
			EditorUtility.DisplayDialog("Not inside Assets",
				"Please pick a folder inside your project's Assets/ directory.", "OK");
			return null;
		}

		// Convert absolute path -> asset path (e.g. "Assets/Textures/UI")
		string assetFolder = "Assets" + absFolder.Substring(dataPath.Length);

		// Find all Texture2D assets in this folder (recurses into subfolders)
		string[] guids = AssetDatabase.FindAssets("", new[] { assetFolder });

		// Convert GUIDs to asset paths
		string[] assetPaths = guids.Select(AssetDatabase.GUIDToAssetPath).ToArray();

		Debug.Log($"Found {assetPaths.Length} textures in {assetFolder}");
		return assetPaths;
	}
	
	public static string EnsureAssetExtension(string path) =>
		Path.ChangeExtension(path, ".asset");
}