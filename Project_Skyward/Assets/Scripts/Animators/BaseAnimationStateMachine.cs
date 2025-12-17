using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
	public class BaseAnimationStateMachine : MonoBehaviour
	{
		//  Parameters
		[Serializable]
		private struct AnimPriorityTuple
		{
			public Structs.BaseMoveStates moveState;
			public int priority;
		}

		[SerializeField] private AnimPriorityTuple[] _animPriorities;

		private Structs.BaseMoveStates _activeMoveState = Structs.BaseMoveStates.Idle;
		private Stack<Structs.BaseMoveStates> _moveStateStack = new Stack<Structs.BaseMoveStates>();

		public bool RequestNewState(Structs.BaseMoveStates newState)
		{
			int newStatePriority = FindAnimPriority(newState);
			int currStatePriority = FindAnimPriority(_activeMoveState);

			if (newStatePriority < currStatePriority ||
			    newState == _activeMoveState) return false;

			if (newState != _activeMoveState)
			{
				_moveStateStack.Push(_activeMoveState);
				_activeMoveState = newState;
			}

			return true;
		}

		public Structs.BaseMoveStates RequestEndState(Structs.BaseMoveStates stateToEnd)
		{
			// Only end current state if the state to end if the current state
			//  And if we actually have a state to undo
			if (_activeMoveState == stateToEnd ||
			    _moveStateStack.Count != 0)
			{
				_activeMoveState = _moveStateStack.Peek();
				_moveStateStack.Pop();
			}

			return _activeMoveState;
		}

		public Structs.BaseMoveStates GetMoveState()
		{
			return _activeMoveState;
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
