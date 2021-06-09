using System.Linq;
using Godot;

namespace battler.Scripts.Formulas
{
  public abstract class Formulas
  {
    // The product of the attacker's attack and the action's multiplier
    public static float CalculatePotentialDamage(AttackActionData data, Battler attacker)
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
  }
}