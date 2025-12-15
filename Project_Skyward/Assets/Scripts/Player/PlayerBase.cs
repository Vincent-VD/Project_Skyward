using System;
using Animators;
using NUnit.Framework;
using Unity.Collections;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
	public class PlayerBase : MonoBehaviour
	{
		[SerializeField, Header("Movement")] private float _moveSpeed = 5.0f;
		[SerializeField, Header("Jump")] private float _baseJumpSpeed = 10.0f;
		[SerializeField] private float _gravity = 9.81f;
		[SerializeField] private float _jumpTime = 1.5f;
		[SerializeField] private AnimationCurve _jumpCurve;
		[SerializeField, Header("Other Components")] private BoxCollider _bodyCollider;
		[SerializeField] private GameObject _attackRoot;
		[SerializeField] private BaseSpriteAnimator _animator;
		
		private PlayerInput _playerInput;

		private float _currCooldown = .0f;

		private Transform _playerTransform;
		private bool _shouldMove = false;
		private bool _isOnWall = true;
		private Vector2 _moveVec = Vector2.zero;

		private bool _isGrounded = true;
		
		private float _currJumpSpeed = 0.0f;
		private float _currJumpTime = 0.0f;
		
		private bool bOnce = false;

		

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
			if (!_isGrounded)
			{
				return;
			}
			
			_currJumpSpeed = _baseJumpSpeed;
			_currJumpTime = 0.0f;
			_gravity = 9.81f;
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
				_currJumpTime += Time.deltaTime;
				float currJumpTime = Mathf.Clamp01(_currJumpTime / _jumpTime);

				float curve = _jumpCurve.Evaluate(currJumpTime);
				_currJumpSpeed += -_gravity * curve * Time.fixedDeltaTime;
				Vector3 vertMove = Vector3.up * (_currJumpSpeed * Time.fixedDeltaTime);
				_playerTransform.position += vertMove;
				
				_animator.RequestAnimChange(Structs.BaseMoveStates.Jump);
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
				_animator.RequestEndState(Structs.BaseMoveStates.Jump);
			}
		}

		private void OnCollisionExit(Collision other)
		{
			if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
			{
				_isOnWall = false;
			}
		}

		private void RotateAttackColliderOnMove(Vector2 direction)
		{
			float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			_attackRoot.transform.rotation = Quaternion.Euler(0.0f, -angle, 0.0f);
		}
	}
}