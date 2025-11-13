using UnityEditor;
using UnityEngine;

public class SpriteRename : MonoBehaviour
{

	[MenuItem("Tools/Sprites/Fix Rename")]
	static void FixRename()
	{
		var assetPaths = EditorHelper.GetFolderAssets();

		// Nr of assets / 2 (png + .asset) / 8 (directions)
		int animFrames = assetPaths.Length / (2 * (int)MoveTypes.MoveDir.Count);
		int currFrame = 0;
		int currDir = 0;
		string currMoveDir = currDir.ToString("00");
		
		foreach (var path in assetPaths)
		{
			Debug.Log(path);
			if (!path.Contains(EditorHelper.assetExtension))
			{
				continue;
			}

			string filename = path.Substring(path.LastIndexOf("/") + 1,
				path.Length - (path.LastIndexOf("/") + 1) - EditorHelper.assetExtension.Length);
			Debug.Log($"Old filename: {filename}");

			string newFilename = $"C_1_{currMoveDir}_{currFrame:00}";
			Debug.Log($"New filename: {newFilename}");
			
			currFrame = (currFrame + 1) % animFrames;
			if (currFrame == 0)
			{
				currDir = (currDir + 1) % (int)MoveTypes.MoveDir.Count;
			}
			currMoveDir = currDir.ToString("00");

			AssetDatabase.RenameAsset(path, newFilename);
			AssetDatabase.Refresh();
		}
	}
}