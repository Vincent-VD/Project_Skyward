using UnityEditor;
using UnityEngine;

namespace Editor
{
	public class SpriteTrim : MonoBehaviour
	{
		[MenuItem("Tools/Sprites/Convert To Sprite")]
		private static void ConvertToSprite()
		{
			// Convert GUIDs to asset paths
			string[] assetPaths = EditorHelper.GetFolderAssets();

			foreach (string path in assetPaths)
			{
				Debug.Log(path);
				if (!path.Contains(EditorHelper.pngExtension))
				{
					continue;
				}
				Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
				Vector2 pos = new Vector2(0, 0);
				Vector2 size = new Vector2(texture.width, texture.height);
				Sprite sprite = Sprite.Create(texture, new Rect(pos, size), new Vector2(0.5f, 0.5f),
					pixelsPerUnit: 100);
				AssetDatabase.CreateAsset(sprite, EditorHelper.EnsureAssetExtension(path));
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}

		}
	}
}