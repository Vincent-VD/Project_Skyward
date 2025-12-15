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

	public enum BaseMoveStates
	{
		Idle,
		Move,
		Jump,
		Attack1,
		Attack2,
		Attack3,
		Special,
		Stun,
		Count
	}
}