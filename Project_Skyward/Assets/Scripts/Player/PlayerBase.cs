using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
	public class PlayerBase : MonoBehaviour
	{
		[SerializeField] private float _moveSpeed = 5.0f;
		[SerializeField] private float _cooldown = 1.0f;
		[SerializeField] private BoxCollider _bodyCollider;
		[SerializeField] private GameObject _attackRoot;

		private PlayerInput _playerInput;

		private float _currCooldown = .0f;

		private Transform _playerTransform;
		private bool _shouldMove = false;
		private bool _isOnWall = true;
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

			// Update attack collider rotation
			RotateAttackColliderOnMove(input);
		}

		private void FixedUpdate()
		{
			if (_shouldMove)
			{
				Vector2 moveVec = _moveVec * (Time.fixedDeltaTime * _moveSpeed);
				_playerTransform.position += new Vector3(moveVec.x, 0.0f, moveVec.y);
			}
		}

		private void OnCollisionEnter(Collision other)
		{
			if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
			{
				_isOnWall = true;
				_shouldMove = false;
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