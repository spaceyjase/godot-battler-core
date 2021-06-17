using Godot;

namespace battler.Scripts
{
  public class AttackActionData : ActionData
  {
    [Export] private float damageMultiplier = 1f;
    [Export] private float hitChance = 100f;

    public float DamageMultiplier => damageMultiplier;
    public float HitChance => hitChance;

    public float CalculatePotentialDamageFor(Battler actor)
    {
      return Formulas.Formulas.CalculatePotentialDamage(this, actor);
    }
  }
}