using battler.Scripts.StatusEffects;
using Godot;

// A damage dealing hit to be applied to a target battler.
// Encapsulating calculations for how hits are applied based on some properties.
namespace battler.Scripts
{
  public class Hit : Reference
  {
    public int Damage { get; }
    public float HitChance { get; }
    public StatusEffect Effect { get; set; }

    public Hit()
    {
    
    }

    public Hit(int damage, float hitChance = 100f, StatusEffect effect = null)
    {
      this.Damage = damage;
      this.HitChance = hitChance;
      this.Effect = effect;
    }

    public bool DoesHit => GD.Randf() * 100f < HitChance;
  }
}