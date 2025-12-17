using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Animators
{
	public class BaseSpriteAnimator : MonoBehaviour
	{
		[SerializeField] private Animator _animator;

		public void SetMoveSpeed(Vector2 vec)
		{
			if (vec != Vector2.zero)
			{
				_animator.SetFloat("MoveX", vec.x);
				_animator.SetFloat("MoveY", vec.y);
			}
		}

		// Request animation change, will not change to lower priority animation
		public void SetAnimationState(Structs.BaseMoveStates newState)
		{
			// Set anim graph states
			_animator.SetInteger("MoveState", (int)newState);
		}
	}
}
