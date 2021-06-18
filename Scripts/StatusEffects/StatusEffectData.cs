using Godot;

namespace battler.Scripts.StatusEffects
{
  public class StatusEffectData : Resource
  {
    // The text 'id' for the effect type.
    [Export] private string effect;
    [Export] private float durationSeconds = 20f;
    // Modifier amount that the effect applies or removes from/to a character's stats.
    [Export] private float effectPower = 20f;
    [Export] private float effectRate = 0.5f;

    // If 'true' the effect applies once every ticking interval...
    [Export] private bool isTicking;
    // Duration between ticks in seconds.
    [Export] private float tickingIntervalSeconds = 4f;
    // Damage inflicted by the effect every tick.
    [Export] private float tickingDamage = 3f;

    public float DurationSeconds => durationSeconds;
    public bool IsTicking => isTicking;
    public float TickingInterval => tickingIntervalSeconds;

    // The total theoretical damage the effect will inflict over time (for ticking effects).
    public float CalculateTotalDamage()
    {
      var damage = 0f;
      if (isTicking)
      {
        damage += durationSeconds / tickingIntervalSeconds * tickingDamage;
      }

      return damage;
    }
  }
}