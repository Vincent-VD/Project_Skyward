using System;
using Animators;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
	public class PlayerAttack : MonoBehaviour
	{
		[Serializable]
		struct AttackData
		{
			public Structs.BaseMoveStates moveState;
			public float totalTime;     // total attack time
			public float hitStart;      // start time for collision
			public float hitEnd;        // end time for collision
			public float linkOpen;      // earliest time a next input can be buffered
			public float linkClose;     // latest time a next input can be buffered
			public float attackSpeed;	// Attack animation speed multiplier
		}
		
		[SerializeField] private BaseSpriteAnimator _animator;
		[SerializeField] private StateMachine.BaseAnimationStateMachine _animationStateMachine;
		[SerializeField, Header("Attack 1")] private AttackData _attack1Data;
		[SerializeField, Header("Attack 2")] private AttackData _attack2Data;
		[SerializeField, Header("Attack 3")] private AttackData _attack3Data;
		[SerializeField, Header("Cooldown")] private float _attackCooldown;


		private BoxCollider _attackCollider;

		private AttackData _currAnimFrame;
		private float _currAnimTime;
		private bool _isAttacking = false;

		private float _currAttackCooldown;
		private bool _isAttackCooldownActive = false;

		private float _attackSpeedFactor = 0.0f;

		void Start()
		{
			_attackCollider = GetComponent<BoxCollider>();
		}

		// Update is called once per frame
		void Update()
		{
			// Update current animation frame when attacking
			if (_isAttacking)
			{
				_currAnimTime += Time.deltaTime;

				// Enable attack hitbox at hitStart
				if (_currAnimTime >= _currAnimFrame.hitStart * _attackSpeedFactor)
				{
					_attackCollider.enabled = true;
				}
				
				// Disable attack hitbox at hitEnd
				if (_currAnimTime >= _currAnimFrame.hitEnd * _attackSpeedFactor)
				{
					_attackCollider.enabled = false;
				}
				
				if (_currAnimTime >= _currAnimFrame.totalTime * _attackSpeedFactor)
				{
					if (_animationStateMachine.RequestEndState(_currAnimFrame.moveState))
					{
						_animator.SetAnimationState(_animationStateMachine.GetMoveState());
					}
				}

				if (_currAnimTime >= _currAnimFrame.linkClose * _attackSpeedFactor)
				{
					_isAttacking = false;
					_isAttackCooldownActive = true;
					_currAnimTime = 0.0f;
				}
			}
			if (_isAttackCooldownActive)
			{
				_currAttackCooldown += Time.deltaTime;

				if (_currAttackCooldown > _attackCooldown)
				{
					_currAttackCooldown = 0.0f;
					_isAttackCooldownActive = false;
				}
			}
		}

		public void OnAttack(InputAction.CallbackContext context)
		{
			// Avoid processing multiple mouse click events
			if (!context.performed)
			{
				return;
			}

			if (_isAttackCooldownActive)
			{
				Debug.Log("Attack in cooldown");
				return;
			}

			Structs.BaseMoveStates currState = _animationStateMachine.GetMoveState();
			Structs.BaseMoveStates newState = Structs.BaseMoveStates.Attack1;

			switch (currState)
			{
				case Structs.BaseMoveStates.Idle:
				case Structs.BaseMoveStates.Move:
				case Structs.BaseMoveStates.Jump:
				{
					SetStateAndAnimFrame(newState, _attack1Data);
					break;
				}
				case Structs.BaseMoveStates.Attack1:
				{
					// If input is registered between linkOpen and linkClose
					if (_currAnimTime >= _currAnimFrame.linkOpen &&
					    _currAnimTime <= _currAnimFrame.linkClose)
					{
						SetStateAndAnimFrame(newState, _attack2Data);
					}
					break;
				}
				case Structs.BaseMoveStates.Attack2:
				{
					if (_currAnimTime >= _currAnimFrame.linkOpen &&
					    _currAnimTime <= _currAnimFrame.linkClose)
					{
						SetStateAndAnimFrame(newState, _attack3Data);
					}
					break;
				}
				default:
				{
					Debug.Log("Default OnAttack state");
					break;
				}
			}
		}

		private void SetStateAndAnimFrame(Structs.BaseMoveStates newState, AttackData attackData)
		{
			if (_animationStateMachine.RequestNewState(newState))
			{
				_animator.SetAnimationState(newState);
				_animator.SetAttackAnimationSpeed(attackData.attackSpeed);
				_attackSpeedFactor = 1 / attackData.attackSpeed;
			}
			_currAnimFrame = attackData;
			_isAttacking = true;
		}
	}
}