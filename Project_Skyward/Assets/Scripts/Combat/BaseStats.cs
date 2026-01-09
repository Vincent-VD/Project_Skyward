using System;
using UnityEngine;

namespace Combat
{
	public class BaseStats : MonoBehaviour
	{
		// Trivial stats
		[Header("Base stats")]
		[SerializeField] private int _baseHealth = 100;
		[SerializeField] private int _baseAttack = 100;
		[SerializeField] private int _baseDefense = 100;
		[SerializeField] private float _baseCritRate = 5.0f;
		[SerializeField] private float _baseCritDmg = 50.0f;
		
		// Attribute stats
		[Header("Attributes")]
		[SerializeField] private float _baseMovementSpeed = 1.0f;
		[SerializeField] private int _debuffApplRate = 20;
		[SerializeField] private int _debuffProcRate = 20;
		
		[Header("Modifier Limit")]
		[SerializeField] private int _modifierLimit = 64;
		private BaseModifier[] _modifiers;

		// Values applied through buffs are stored here
		private int _currHealth;		// current health value
		private int _healthBonus;		// bonus max health above base health
		private int _attackBonus;		// attack buff based on base attack
		private int _defenseBonus;		// defense buff based on base defense
		private float _critRateBonus;	// crit rate buff
		private float _critDmgBonus;	// crit damage buff

		// Total effective stats
		private int _totalAttack;
		private int _totalDefense;
		private float _totalCritRate;
		private float _totalCritDmg;

		private void Start()
		{
			_currHealth = _baseHealth;
			_healthBonus = 0;
			_attackBonus = 0;
			_defenseBonus = 0;
			_critRateBonus = 0.0f;
			_critDmgBonus = 0.0f;

			_totalAttack = _baseAttack;
			_totalDefense = _baseDefense;
			_totalCritRate = _baseCritRate;
			_totalCritDmg = _baseCritDmg;
			
			_modifiers = new BaseModifier[_modifierLimit];
		}

		// Returns amount of damage an attack does before it hits the enemy
		//  BaseDmg is damage *before* the target's stats are taken into account
		public int CalcBaseDmg()
		{
			return _totalAttack;
		}

		// Returns total damage after taking BaseDmg
		//  TotalDmg is damage *after* the target's stats are taken into account
		public int CalcTotalDamage(int baseDamage)
		{
			return _totalDefense;
		}

		// Pos value increases curr health, neg value increases it
		public void UpdateHealth(int damage)
		{
			_currHealth += damage;
		}

		private void ApplyModifiers(BaseModifier.ModifierType  modifierType)
		{
			int baseDmg = CalcBaseDmg();
			foreach (BaseModifier modifier in _modifiers)
			{
				if (modifier.GetModifierType() == modifierType)
				{
					
				}
			}
		}
	}
}