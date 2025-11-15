using UnityEngine;

namespace Structs
{
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
}