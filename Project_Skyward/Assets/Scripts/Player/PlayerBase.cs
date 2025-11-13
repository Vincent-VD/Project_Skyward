using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBase : MonoBehaviour
{
	[SerializeField] private float _cooldown = 1.0f;
	[SerializeField] private SpriteRenderer _spriteRenderer;
	[SerializeField] private float _animFrameTime = 0.2f;

	public enum MoveDir
	{
		None = -1,
		Right,
		RightUp,
		Up,
		LeftUp,
		Left,
		LeftDown,
		Down,
		DownRight,
		Count = 8,
	}

	public enum MoveState
	{
		Idle,
		Move,
		Attack,
		Count = 3
	}

	private SortedDictionary<MoveState, string> _moveStateMap = new SortedDictionary<MoveState, string>();
	private MoveDir _moveDir = MoveDir.Right;
	private MoveState _moveState = MoveState.Idle;

	private PlayerInput _playerInput;
	private float _currCooldown = .0f;
	private int _animFrame = 0;
	private float _currAnimeFrameTime = 0.0f;
    
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		// Get player input component and enable current action map
		_playerInput = GetComponent<PlayerInput>();
		InputSystem.actions.Disable();
		_playerInput.currentActionMap?.Enable();
		
		// Add state to string conversions
		_moveStateMap.Add(MoveState.Idle, "Idle");
		_moveStateMap.Add(MoveState.Move, "Move");
		_moveStateMap.Add(MoveState.Attack, "Attack");
	}

	void Update()
	{
		_currCooldown += Time.deltaTime;
		_currAnimeFrameTime += Time.deltaTime;

		if (_currCooldown >= _cooldown)
		{
			/*GameObject dmgTree = Instantiate(_toSpawn, _launchPoint);*/
			/*dmgTree.GetComponent<S_DamageTree>().Init(_elementType);*/
			_currCooldown = 0.0f;
		}

		if (_currAnimeFrameTime >= _animFrameTime)
		{
			_animFrame = (_animFrame + 1) % 8;
			Sprite newSprite = SpriteManager.Instance.GetPlayerSprite(_moveState, _moveDir, _animFrame);
			_spriteRenderer.sprite = newSprite;
			_currAnimeFrameTime = 0.0f;
		}
	}

	private void OnMove(InputAction.CallbackContext context)
	{
		var input = context.ReadValue<Vector2>();

		float absX = Mathf.Abs(input.x);
		 float absY = Mathf.Abs(input.y);
		if (absX < 0.05f & absY < 0.05f)
		{
			_moveState = MoveState.Idle;
			_animFrame = 0;
			return;
		}
		
		MoveDir moveDir = MoveDir.None;
		
		// k = tan(22.5°) ≈ 0.4142
		// s = tan(67.5°) ≈ 2.4142
		const float k = 0.41421356f;
		const float s = 2.41421356f;
		if (absY <= k * absX)
		{
			moveDir = input.x >= 0 ? MoveDir.Right : MoveDir.Left;
		}
		else if (absY >= s * absX)
		{
			moveDir = input.y >= 0 ? MoveDir.Up : MoveDir.Down;
		}
		else
		{
			moveDir = input.x >= 0 ? (input.y >= 0 ? MoveDir.RightUp : MoveDir.DownRight)
									: (input.y >= 0 ? MoveDir.LeftUp : MoveDir.LeftDown);
				
		}
		
		_moveDir = moveDir;
		_moveState = MoveState.Move;
		
		Sprite newSprite = SpriteManager.Instance.GetPlayerSprite(_moveState, _moveDir, _animFrame);
		_spriteRenderer.sprite = newSprite;
	}
}