using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Animators
{
	public class BaseSpriteAnimator : MonoBehaviour
	{
		[Serializable]
		public struct AnimPriorityTuple
		{
			public Structs.BaseMoveStates moveState;
			public int priority;
		}
		[SerializeField] private float _animFrameTime = 0.2f;
		[SerializeField] private AnimPriorityTuple[] _animPriorities;
		[SerializeField] private Animator _animator;
    
		private SpriteRenderer _spriteRenderer;

		private Structs.MoveDir _moveDir = Structs.MoveDir.Right;
		private Structs.BaseMoveStates _moveState = Structs.BaseMoveStates.Idle;
		
		private Stack<Structs.BaseMoveStates> _moveStack = new Stack<Structs.BaseMoveStates>();
    
		private int _animFrame = 0;
		private float _currAnimeFrameTime = 0.0f;
    
		void Start()
		{
			// Get sprite renderer component
			_spriteRenderer = GetComponent<SpriteRenderer>();
			_animator.SetInteger("MoveState", (int)_moveState);
			_animator.SetInteger("MoveDir", (int)_moveDir);
		
			Assert.NotNull(_spriteRenderer, "Sprite renderer not found, make sure it's a component of the same object");

			Assert.That(_animFrameTime > 0f);
		}

		// Update animation frames based on anim frame time
		void Update()
		{
			_currAnimeFrameTime += Time.deltaTime;

			if (_currAnimeFrameTime >= _animFrameTime)
			{
				_animFrame = (_animFrame + 1) % 8;
			
				/*Sprite newSprite = Managers.SpriteManager.Instance.GetPlayerSprite(_moveState, _moveDir, _animFrame);
				_spriteRenderer.sprite = newSprite;*/
				_currAnimeFrameTime = 0.0f;
			}
		}

		// Update move direction
		public void SetMoveDir(Structs.MoveDir dir)
		{
			if (dir != _moveDir)
			{
				_animator.SetInteger("MoveDir", (int)dir);
				_animator.SetFloat("MoveDirFl", (float)dir / (float)Structs.MoveDir.Count);
				_animator.SetTrigger("Right");
			}
			
			_moveDir = dir;
		}

		public void SetMoveSpeed(Vector2 vec)
		{
			if (vec != Vector2.zero)
			{
				_animator.SetFloat("MoveX", vec.x);
				_animator.SetFloat("MoveY", vec.y);
			}
		}

		// Request animation change, will not change to lower priority animation
		public bool RequestAnimChange(Structs.BaseMoveStates newState)
		{
			int newStatePriority = FindAnimPriority(newState);
			int currStatePriority = FindAnimPriority(_moveState);

			if (newStatePriority < currStatePriority) return false;

			if (newState != _moveState)
			{
				// Set anim graph states
				_animator.SetBool("Move", newState == Structs.BaseMoveStates.Move);
				_animator.SetBool("Jump", newState == Structs.BaseMoveStates.Jump);
				
				_moveStack.Push(_moveState);
				_moveState = newState;
				_animFrame = 0;
				_currAnimeFrameTime = 0.0f;
			}

			return true;

		}
		
		public bool RequestEndState(Structs.BaseMoveStates stateToEnd)
		{
			// Only end current state if the state to end if the current state
			//  And if we actually have a state to undo
			if (_moveState != stateToEnd ||
			    _moveStack.Count == 0)
				return false;
			
			// Reset anim graph states
			_animator.SetBool("Move", _moveStack.Peek() == Structs.BaseMoveStates.Move);
			_animator.SetBool("Jump", _moveStack.Peek() == Structs.BaseMoveStates.Jump);
		
			_moveState = _moveStack.Peek();
			_moveStack.Pop();
			_animFrame = 0;
			_currAnimeFrameTime = 0.0f;
			return true;
		}

		private int FindAnimPriority(Structs.BaseMoveStates state)
		{
			foreach (AnimPriorityTuple item in _animPriorities)
			{
				if (item.moveState == state)
				{
					return item.priority;
				}
			}
			return -1;
		}
	}
}
