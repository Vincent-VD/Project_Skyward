using System;
using UnityEngine;

namespace Combat
{
	public struct BaseModifier
	{
		BaseModifier(ModifierClass modifierClass,
					ModifierType modifierType,
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

		public enum ModifierType
		{
			Attack,
			Defense,
			CritRate,
			CritDmg,
			Movement,
			ApplRate,
			ProcRate,
			
			// Abs buffs/deb modify base stats for a time
			AbsAttack,
			AbsDefense,
			AbsCritRate,
			AbsCritDmg,
		}
		
		private ModifierClass _modifierClass;
		private ModifierType _modifierType;
		
		private float _modifierValue;

		public ModifierClass GetModifierClass()
		{
			return _modifierClass;
		}

		public ModifierType GetModifierType()
		{
			return _modifierType;
		}

		public float AttackPhase()
		{
			if (_modifierClass == ModifierClass.Buff)
			{
				return _modifierValue;
			}
			// Attack value is not influenced by this modifier
			return 1.0f;
		}

		public float DefensePhase()
		{
			if (_modifierClass == ModifierClass.Debuff)
			{
				return _modifierValue;
			}
			return 1.0f;
		}

		public float ApplicationPhase()
		{
			if (_modifierClass == ModifierClass.ApplicationRate)
			{
				return _modifierValue;
			}
			return 0.0f;
		}
		
	}
}