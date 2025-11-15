using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Managers
{
	public class SpriteManager : MonoBehaviour
	{
		public static SpriteManager Instance { get; private set; }

		[SerializeField] private DefaultAsset _playerAttackRoot = null;
		[SerializeField] private DefaultAsset _playerIdleRoot = null;
		[SerializeField] private DefaultAsset _playerMoveRoot = null;

		private Sprite[][] _playerAssets = new Sprite[(int)Structs.MoveState.Count][];

		private void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(this);
			}
			else
			{
				Instance = this;
				Instance.Init();
			}

			DontDestroyOnLoad(this);
		}

		private void Init()
		{
			//Player Sprite init
			Sprite[] attackSprites = GetSpritesFromFolder(_playerAttackRoot);
			Sprite[] idleSprites = GetSpritesFromFolder(_playerIdleRoot);
			Sprite[] moveSprites = GetSpritesFromFolder(_playerMoveRoot);

			_playerAssets[(int)Structs.MoveState.Attack] = attackSprites;
			_playerAssets[(int)Structs.MoveState.Idle] = idleSprites;
			_playerAssets[(int)Structs.MoveState.Move] = moveSprites;
		}

		public Sprite GetPlayerSprite(Structs.MoveState moveState, Structs.MoveDir moveDir, int frame)
		{
			if (moveDir == Structs.MoveDir.None)
			{
				Debug.LogError("GetPlayerSprite called MoveDir.None");
				return null;
			}

			return _playerAssets[(int)moveState][(int)moveDir * 8 + frame];
		}

		private Sprite[] GetSpritesFromFolder(DefaultAsset assetFolder)
		{
			string assetPath = AssetDatabase.GetAssetPath(assetFolder);
			string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { assetPath });
			string[] assetPaths = guids.Select(AssetDatabase.GUIDToAssetPath).ToArray();
			Sprite[] currState = new Sprite[assetPaths.Length];
			for (int i = 0; i < assetPaths.Length; i++)
			{
				string currAsset = assetPaths[i];
				currState[i] = AssetDatabase.LoadAssetAtPath<Sprite>(currAsset);
			}

			return currState;
		}
	}
}