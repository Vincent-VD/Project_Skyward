using Structs;
using UnityEngine;
using Random = UnityEngine.Random;

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

		// Total effective stats
		private int _totalHealth;
		private int _totalAttack;
		private int _totalDefense;
		private float _totalCritRate;
		private float _totalCritDmg;
		private float _totalMovementSpeed;
		private float _totalDebuffApplRate;
		private float _totalDebuffProcRate;

		private void Start()
		{
			_currHealth = _baseHealth;
			_healthBonus = 0;
			_attackBonus = 0;
			_defenseBonus = 0;

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
			int totalDmg = _totalAttack;

			float rand = Random.Range(0.0f, 100.0f);
			if (rand >= _totalCritRate)
			{
				totalDmg = (int)(totalDmg * _totalCritDmg);
			}

			return totalDmg;
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

		public void RecalcStat(StatType statType)
		{
			float statBonus = 0.0f;
			foreach (BaseModifier modifier in _modifiers)
			{
				statBonus += modifier.GetModifierValueOfType(statType);
			}

			switch (statType)
			{
				case StatType.Health:
				{
					UpdateStat(ref _totalHealth, ref _healthBonus, _baseHealth, statBonus);
					break;
				}
				case StatType.Attack:
				{
					UpdateStat(ref _totalAttack, ref _attackBonus, _baseAttack, statBonus);
					break;
				}
				case StatType.Defense:
				{
					UpdateStat(ref _totalDefense, ref _defenseBonus, _baseDefense, statBonus);
					break;
				}
				case StatType.CritRate:
				{
					UpdateStat(ref _totalCritRate, _baseCritRate, statBonus);
					break;
				}
				case StatType.CritDmg:
				{
					UpdateStat(ref _totalCritDmg, _baseCritDmg, statBonus);
					break;
				}
				case StatType.MovementSpeed:
				{
					UpdateStat(ref _totalMovementSpeed, _baseMovementSpeed, statBonus);
					break;
				}
				case StatType.DebuffApplRate:
				{
					UpdateStat(ref _totalDebuffApplRate, _debuffApplRate, statBonus);
					break;
				}
				case StatType.DebuffProcRate:
				{
					UpdateStat(ref _totalDebuffProcRate, _debuffProcRate, statBonus);
					break;
				}
				default:
				{
					Debug.LogErrorFormat("Invalid/Unhandled stat type: {0}", statType);
					break;
				}
			}
		}

		private void UpdateStat(ref int totalStat, ref int bonusStat, int baseStat, float statBonus)
		{
			totalStat = baseStat + (int)(baseStat * statBonus);
			bonusStat = totalStat - baseStat;
		}

		private void UpdateStat(ref float totalStat, float baseStat, float statBonus)
		{
			totalStat = baseStat + statBonus;
		}
	}
}
