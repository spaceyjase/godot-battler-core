using battler.Scripts.StatusEffects;
using Godot;

namespace battler.Scripts
{
  public class AttackActionData : ActionData
  {
    [Export] private float damageMultiplier = 1f;
    [Export] private float hitChance = 100f;
    [Export] private StatusEffectData statusEffect;

    public float DamageMultiplier => damageMultiplier;
    public float HitChance => hitChance;
    public StatusEffectData StatusEffect => statusEffect;

    public float CalculatePotentialDamageFor(Battler actor)
    {
      var totalDamage = Formulas.Formulas.CalculatePotentialDamage(this, actor);
      if (statusEffect != null)
      {
        totalDamage += statusEffect.CalculateTotalDamage();
      }

      return totalDamage;
    }
  }
}