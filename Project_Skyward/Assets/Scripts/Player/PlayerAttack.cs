using Structs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
	public class PlayerAttack : MonoBehaviour
	{
		[SerializeField] private float _attackCooldown = 1.0f;
		[SerializeField] private PlayerSpriteBody _playerSpriteBody;

		private BoxCollider _attackCollider;

		private Structs.AttackStates _currAttackState;
		private float _currCooldown;

		void Start()
		{
			_attackCollider = GetComponent<BoxCollider>();
		}

		// Update is called once per frame
		void Update()
		{
			// Only update cooldown timer when collider is enabled
			if (_attackCollider.enabled)
			{
				_currCooldown += Time.deltaTime;

				if (_currCooldown >= _attackCooldown)
				{
					_attackCollider.enabled = false;
					_currAttackState = AttackStates.None;
					_playerSpriteBody.SetAttackState(_currAttackState);
				}
			}
		}

		private void OnCollisionEnter(Collision other)
		{
			Debug.Log("OnCollisionEnter");
		}

		public void OnAttack(InputAction.CallbackContext context)
		{
			// Avoid processing multiple mouse click events
			if (!context.performed)
			{
				return;
			}
			
			if (_attackCollider.enabled == false)
			{
				if (_currAttackState != Structs.AttackStates.ForceCooldown)
				{
					_currCooldown = 0.0f;
					_currAttackState = Structs.AttackStates.StandardCombo1;
					_attackCollider.enabled = true;
				}
				else
				{
					Debug.Log("Attack in force cooldown");
				}
			}
			else
			{
				switch (_currAttackState)
				{
					case Structs.AttackStates.StandardCombo1:
					{
						Debug.Log("StandardCombo1 transition to StandardCombo2");
						_currAttackState = Structs.AttackStates.StandardCombo2;
						_currCooldown = 0.0f;
						break;
					}
					case Structs.AttackStates.StandardCombo2:
					{
						Debug.Log("StandardCombo2 transition to StandardCombo3");
						_currAttackState = Structs.AttackStates.StandardCombo3;
						_currCooldown = 0.0f;
						break;
					}
					case Structs.AttackStates.StandardCombo3:
					{
						Debug.Log("StandardCombo3 transition to ForceCooldown");
						_currCooldown = 0.0f;
						break;
					}
				}
			}
			
			_playerSpriteBody.SetAttackState(_currAttackState);
		}
	}
}