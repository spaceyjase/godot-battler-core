using Godot;

// A damage dealing hit to be applied to a target battler.
// Encapsulating calculations for how hits are applied based on some properties.
public class Hit : Reference
{
  public int Damage { get; }
  public float HitChance { get; }

  public Hit(int damage, float hitChance = 100f)
  {
    this.Damage = damage;
    this.HitChance = hitChance;
  }

  public bool DoesHit => GD.Randf() * 100f < HitChance;
}