using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBase : MonoBehaviour
{
	[SerializeField] private float _moveSpeed = 5.0f;
	[SerializeField] private float _cooldown = 1.0f;
	[SerializeField] private SpriteRenderer _spriteRenderer;
	[SerializeField] private float _animFrameTime = 0.2f;

	private SortedDictionary<MoveTypes.MoveState, string> _moveStateMap = new SortedDictionary<MoveTypes.MoveState, string>();
	private MoveTypes.MoveDir _moveDir = MoveTypes.MoveDir.Right;
	private MoveTypes.MoveState _moveState = MoveTypes.MoveState.Idle;

	private PlayerInput _playerInput;
	private float _currCooldown = .0f;
	private int _animFrame = 0;
	private float _currAnimeFrameTime = 0.0f;
	
	private Transform _playerTransform;
	private bool _shouldMove = false;
	private Vector2 _moveVec = Vector2.zero;
    
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		// Get player Transform component
		_playerTransform = GetComponent<Transform>();
		
		// Get player input component and enable current action map
		_playerInput = GetComponent<PlayerInput>();
		InputSystem.actions.Disable();
		_playerInput.currentActionMap?.Enable();
		
		// Add state to string conversions
		_moveStateMap.Add(MoveTypes.MoveState.Idle, "Idle");
		_moveStateMap.Add(MoveTypes.MoveState.Move, "Move");
		_moveStateMap.Add(MoveTypes.MoveState.Attack, "Attack");
		
		// Asserts
		Assert.IsNotNull(_playerTransform);
		Assert.IsNotNull(_playerInput.currentActionMap);
		Assert.AreEqual(_playerInput.currentActionMap, InputSystem.actions.FindActionMap("Player"));
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

		// Update sprite frames
		if (_currAnimeFrameTime >= _animFrameTime)
		{
			_animFrame = (_animFrame + 1) % 8;
			Sprite newSprite = SpriteManager.Instance.GetPlayerSprite(_moveState, _moveDir, _animFrame);
			_spriteRenderer.sprite = newSprite;
			_currAnimeFrameTime = 0.0f;
		}
	}

	public void OnMove(InputAction.CallbackContext context)
	{
		var input = context.ReadValue<Vector2>();
		
		// Movement
		_shouldMove = context.performed;
		_moveVec = input;

		// Check if movement should be processed
		float absX = Mathf.Abs(input.x);
		float absY = Mathf.Abs(input.y);
		if (absX < 0.05f & absY < 0.05f)
		{
			_moveState = MoveTypes.MoveState.Idle;
			_animFrame = 0;
			_shouldMove = false;
			return;
		}
		
		// Sprite logic
		MoveTypes.MoveDir moveDir = MoveTypes.MoveDir.None;
		
		// k = tan(22.5°) ≈ 0.4142
		// s = tan(67.5°) ≈ 2.4142
		const float k = 0.41421356f;
		const float s = 2.41421356f;
		if (absY <= k * absX)
		{
			moveDir = input.x >= 0 ? MoveTypes.MoveDir.Right : MoveTypes.MoveDir.Left;
		}
		else if (absY >= s * absX)
		{
			moveDir = input.y >= 0 ? MoveTypes.MoveDir.Up : MoveTypes.MoveDir.Down;
		}
		else
		{
			moveDir = input.x >= 0 ? (input.y >= 0 ? MoveTypes.MoveDir.RightUp : MoveTypes.MoveDir.DownRight)
									: (input.y >= 0 ? MoveTypes.MoveDir.LeftUp : MoveTypes.MoveDir.LeftDown);
				
		}
		
		_moveDir = moveDir;
		_moveState = MoveTypes.MoveState.Move;
		
		Sprite newSprite = SpriteManager.Instance.GetPlayerSprite(_moveState, _moveDir, _animFrame);
		_spriteRenderer.sprite = newSprite;
	}

	private void FixedUpdate()
	{
		if (_shouldMove)
		{
			Vector2 moveVec = _moveVec * (Time.fixedDeltaTime * _moveSpeed);
			_playerTransform.position += new Vector3(-moveVec.y, 0.0f, moveVec.x);
		}
	}
}