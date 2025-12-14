using Animators;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
	public class PlayerBase : MonoBehaviour
	{
		[SerializeField, Header("Movement")] private float _moveSpeed = 5.0f;
		[SerializeField, Header("Jump")] private float _baseJumpSpeed = 10.0f;
		[SerializeField] private float _gravity = 9.81f;
		[SerializeField] private float _cooldown = 1.0f;
		[SerializeField, Header("Other Components")] private BoxCollider _bodyCollider;
		[SerializeField] private GameObject _attackRoot;
		[SerializeField] private BaseSpriteAnimator _animator;
		
		private PlayerInput _playerInput;

		private float _currCooldown = .0f;

		private Transform _playerTransform;
		private bool _shouldMove = false;
		private bool _isOnWall = true;
		private Vector2 _moveVec = Vector2.zero;

		private bool _isGrounded = false;
		
		private float _currJumpSpeed = 0.0f;
		

		// Start is called once before the first execution of Update after the MonoBehaviour is created
		void Start()
		{
			// Get player Transform component
			_playerTransform = GetComponent<Transform>();

			// Get player input component and enable current action map
			_playerInput = GetComponent<PlayerInput>();
			InputSystem.actions.Disable();
			_playerInput.currentActionMap?.Enable();

			// Asserts
			Assert.IsNotNull(_playerTransform);
			Assert.IsNotNull(_playerInput.currentActionMap);
			Assert.AreEqual(_playerInput.currentActionMap, InputSystem.actions.FindActionMap("Player"));
		}

		void Update()
		{
			_currCooldown += Time.deltaTime;

			if (_currCooldown >= _cooldown)
			{
				/*GameObject dmgTree = Instantiate(_toSpawn, _launchPoint);*/
				/*dmgTree.GetComponent<S_DamageTree>().Init(_elementType);*/
				_currCooldown = 0.0f;
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
				_shouldMove = false;
				return;
			}
			
			Structs.MoveDir _moveDir;
			
			// Sprite logic
			// k = tan(22.5°) ≈ 0.4142
			// s = tan(67.5°) ≈ 2.4142
			const float k = 0.41421356f;
			const float s = 2.41421356f;
			if (absY <= k * absX)
			{
				_moveDir = input.x >= 0 ? Structs.MoveDir.Right : Structs.MoveDir.Left;
			}
			else if (absY >= s * absX)
			{
				_moveDir = input.y >= 0 ? Structs.MoveDir.Up : Structs.MoveDir.Down;
			}
			else
			{
				_moveDir = input.x >= 0 ? (input.y >= 0 ? Structs.MoveDir.RightUp : Structs.MoveDir.DownRight)
					: (input.y >= 0 ? Structs.MoveDir.LeftUp : Structs.MoveDir.LeftDown);
				
			}

			GetComponentInChildren<BaseSpriteAnimator>().SetMoveDir(_moveDir);

			// Update attack collider rotation
			RotateAttackColliderOnMove(input);
		}

		public void OnJump(InputAction.CallbackContext context)
		{
			_currJumpSpeed = _baseJumpSpeed;
			_isGrounded = false;
		}

		private void FixedUpdate()
		{
			if (_shouldMove)
			{
				Vector2 moveVec = _moveVec * (Time.fixedDeltaTime * _moveSpeed);
				_playerTransform.position += new Vector3(moveVec.x, 0.0f, moveVec.y);
				_animator.RequestAnimChange(Structs.BaseMoveStates.Move);
				_animator.SetMoveSpeed(moveVec);
			}
			else
			{
				_animator.RequestEndState(Structs.BaseMoveStates.Move);
				_animator.SetMoveSpeed(Vector2.zero);
			}

			if (!_isGrounded)
			{
				_currJumpSpeed += -_gravity * Time.fixedDeltaTime;
				Vector3 vertMove = new Vector3(0.0f, 1.0f, 0.0f) * (_currJumpSpeed * Time.fixedDeltaTime);
				_playerTransform.position += vertMove;
			}
		}

		private void OnCollisionEnter(Collision other)
		{
			if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
			{
				_isOnWall = true;
				_shouldMove = false;
			}

			if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
			{
				_isGrounded = true;
			}
		}

		private void OnCollisionExit(Collision other)
		{
			if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
			{
				_isOnWall = false;
			}
			
			if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
			{
				_isGrounded = false;
			}
		}

		private void RotateAttackColliderOnMove(Vector2 direction)
		{
			float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			_attackRoot.transform.rotation = Quaternion.Euler(0.0f, -angle, 0.0f);
		}
	}
}