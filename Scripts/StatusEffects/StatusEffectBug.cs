namespace battler.Scripts.StatusEffects
{
  public class StatusEffectBug : StatusEffect
  {
    private readonly int damage;

    public StatusEffectBug(Battler target, StatusEffectData data)
      : base(target, data)
    {
      Id = "bug";
      damage = data.TickingDamage;
      CanStack = true;
    }
    
    protected override void Apply()
    {
      base.Apply();

      target.TakeHit(new Hit(damage));
    }
  }
}