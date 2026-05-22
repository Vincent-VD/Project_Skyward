using Structs;

namespace Combat
{
	public struct BaseModifier
	{
		BaseModifier(ModifierClass modifierClass,
					 StatType modifierType,
					 float value)
		{
			_modifierClass = modifierClass;
			_modifierType = modifierType;
			_modifierValue = value;
		}

		public enum ModifierClass
		{
			Buff,
			Debuff,
			ApplicationRate
		}



		private ModifierClass _modifierClass;
		private StatType _modifierType;

		private float _modifierValue;

		public ModifierClass GetModifierClass()
		{
			return _modifierClass;
		}

		public StatType GetModifierType()
		{
			return _modifierType;
		}

		public float GetModifierValueOfType(StatType type)
		{
			return _modifierType == type ? _modifierValue : 0.0f;
		}

		public float GetModifierOfClass(ModifierClass type)
		{
			return _modifierClass == type ? _modifierValue : 0.0f;
		}

	}
}