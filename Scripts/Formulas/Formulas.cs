using System.Linq;
using Godot;

namespace battler.Scripts.Formulas
{
  public abstract class Formulas
  {
    // The product of the attacker's attack and the action's multiplier
    private static float CalculatePotentialDamage(AttackActionData data, Battler attacker)
    {
      return attacker.Stats.Attack * data.DamageMultiplier;
    }

    // The base damage is attacker.attack * attacker.multiplier - defender.defense
    // This multiplies it by a weakness multiplier, ensuring it is between [1, 999].
    public static int CalculateBaseDamage(AttackActionData data, Battler attacker, Battler defender)
    {
      var damage = CalculatePotentialDamage(data, attacker);
      damage -= defender.Stats.Defense;
      damage *= CalculateWeaknessMultiplier(data, defender);
      return (int)Mathf.Clamp(damage, 1f, 999f);
    }

    private static float CalculateWeaknessMultiplier(AttackActionData data, Battler defender)
    {
      var multiplier = 1f;
      var element = data.Element;

      if (element == Elements.None) return multiplier;
      
      // If the defender has an affinity with the action's element, multiply by 0.75
      if (Types.WeaknessMapping[defender.Stats.Affinity] == element)
      {
        multiplier = 0.75f; // TODO: magic number
      }
      // ...otherwise, if the element is part of the defender's weaknesses, we multiply to 1.5
      else if (defender.Stats.Weaknesses.Any(e => e == Types.WeaknessMapping[element]))
      {
        multiplier = 1.5f;  // TODO: also magic
      }

      return multiplier;
    }

    // Hit chance is (attacker.HitChance - defender.Evasion) * actionHitChance + affinity bonus + 
    // element tried bonus - defender affinity bonus.
    public static float CalculateHitChance(AttackActionData data, Battler attacker, Battler defender)
    {
      var chance = attacker.Stats.HitChance - defender.Stats.Evasion;
      // Hit chance is a value between 0 and 100 for consistency with the battler's stats
      // as it is used as a multiplier here, we need to divide by 100 first.
      chance *= data.HitChance / 100f;
      
      // Use the affinity and weaknesses to apply a hit chance bonus or penalty.
      var element = data.Element;
      if (element == attacker.Stats.Affinity)
      {
        chance += 5f; // TODO: magic number
      }

      if (element == Elements.None) return Mathf.Clamp(chance, 0f, 100f);
      
      // if the action's element is part of the defender's weaknesses, increase the hit rating by 10.
      if (defender.Stats.Weaknesses.Any(e => e == Types.WeaknessMapping[element]))
      {
        chance += 10f; // TODO: magic
      }
      // However, if the defender has an affinity with the action's element, decrease the hit rating by 10.
      if (Types.WeaknessMapping[defender.Stats.Affinity] == element)
      {
        chance -= 10f;
      }

      return Mathf.Clamp(chance, 0f, 100f);
    }
  }
}