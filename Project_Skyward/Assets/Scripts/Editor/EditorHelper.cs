using System.IO;
using System.Linq;
using Structs;
using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager;

namespace Editor
{
	public class EditorHelper : MonoBehaviour
	{

		[MenuItem("Tools/Sprites/Create Sprite Animation")]
		public static void CreateAnimClipsFromSpriteSheet()
		{
			string folderPath = Application.dataPath;

			string absFolder = EditorUtility.OpenFilePanel("Select File", folderPath, "png");
			if (string.IsNullOrEmpty(absFolder))
			{
				EditorUtility.DisplayDialog("Error", "No File Selected", "OK");
				return;
			}
			
			var dataPath = Application.dataPath.Replace("\\", "/");
			absFolder = absFolder.Replace("\\", "/");
			if (!absFolder.StartsWith(dataPath))
			{
				EditorUtility.DisplayDialog("Not inside Assets",
					"Please pick a folder inside your project's Assets/ directory.", "OK");
				return;
			}
			
			// Convert absolute path -> asset path (e.g. "Assets/Textures/UI")
			string assetFolder = "Assets" + absFolder.Substring(dataPath.Length);

			// Find all Texture2D assets in this folder (recurses into subfolders)
			string[] guids = AssetDatabase.FindAssets("", new[] { assetFolder });

			// Convert GUIDs to asset paths
			string[] assetPaths = guids.Select(AssetDatabase.GUIDToAssetPath).ToArray();
			
			Sprite[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPaths[0])
				.OfType<Sprite>()
				.ToArray();

			Debug.Log(sprites.Length);
			int animFrames = sprites.Length / (int)Structs.MoveDir.Count; // 8 directions
			
			string path = EditorUtility.SaveFolderPanel("Save Animation", folderPath, assetFolder);
			
			string filenameBase = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPaths[0]).name.Replace("_Spritesheet", "");

			for (int currDirection = 0; currDirection < (int)Structs.MoveDir.Count; currDirection++)
			{
				AnimationClip clip = new AnimationClip();
				
				ObjectReferenceKeyframe[] spriteKeyFrames = new ObjectReferenceKeyframe[animFrames];
				for (int currFrame = 0; currFrame < animFrames; currFrame++)
				{
					var sprite = sprites[currFrame + currDirection * animFrames];
					spriteKeyFrames[currFrame] = new ObjectReferenceKeyframe
					{
						time = currFrame * (1/6.0f), //TODO: this needs to be redone if the animations ever change
						value = sprite
					};
				}
				clip.name = "Adol_Attack" + currDirection;
				
				
				var spriteBinding = EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite");
				AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, spriteKeyFrames);
				
				path = path.Replace(dataPath, "");

				AssetDatabase.CreateAsset(clip, "Assets" + path + "/" + filenameBase + MoveDirToString((MoveDir)currDirection) + ".anim");
			}
		}

		private static string MoveDirToString(Structs.MoveDir direction)
		{
			switch (direction)
			{
				case Structs.MoveDir.Right:
					return "E";
				case Structs.MoveDir.RightUp:
					return "NE";
				case Structs.MoveDir.Up:
					return "N";
				case Structs.MoveDir.LeftUp:
					return "NW";
				case Structs.MoveDir.Left:
					return "W";
				case Structs.MoveDir.LeftDown:
					return "SW";
				case Structs.MoveDir.Down:
					return "S";
				case Structs.MoveDir.DownRight:
					return "SE";
				default:
					Debug.LogError("Invalid direction");
					return "-1";
			}
		}
	}
}