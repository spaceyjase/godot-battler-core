using Godot;

// A damage dealing hit to be applied to a target battler.
// Encapsulating calculations for how hits are applied based on some properties.
public class Hit : Reference
{
  private int damage;
  private float hitChance;

  public Hit(int damage, float hitChance = 100f)
  {
    this.damage = damage;
    this.hitChance = hitChance;
  }

  public bool DoesHit => GD.Randf() * 100f < hitChance;
}