using System;
using battler.Scripts.Enum;
using Godot;

namespace battler.Scripts.StatusEffects
{
  public class StatusEffectSlow : StatusEffect
  {
    private float SpeedReduction
    {
      get => speedReduction;
      set => speedReduction = Mathf.Clamp(value, 0.01f, 0.99f);
    }
    
    private Guid statModifierId;
    private float speedReduction;

    public StatusEffectSlow(Battler target, StatusEffectData data)
      : base(target, data)
    {
      Id = "slow";
      SpeedReduction = data.EffectRate;
    }

    protected override void Start()
    {
      statModifierId = target.Stats.AddModifier(UpgradeableStats.Speed,
        // calculated from the speed reduction ratio.
        -1f * speedReduction * target.Stats.Speed);
    }
    
    protected override void ExpireImpl()
    {
      target.Stats.RemoveModifier(UpgradeableStats.Speed, statModifierId);
      
      base.ExpireImpl();
    }
  }
}
