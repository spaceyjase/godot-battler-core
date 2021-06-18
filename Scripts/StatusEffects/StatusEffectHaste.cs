using System;
using battler.Scripts.Enum;
using Godot;

namespace battler.Scripts.StatusEffects
{
  public class StatusEffectHaste : StatusEffect
  {
    public float SpeedBonus { get; set; }
    
    private Guid statModifierId;

    public StatusEffectHaste(Battler target, StatusEffectData data)
      : base(target, data)
    {
      Id = "haste";
      SpeedBonus = data.EffectPower;
    }

    protected override void Start()
    {
      statModifierId = target.Stats.AddModifier(UpgradeableStats.Speed, SpeedBonus);
    }

    protected override void ExpireImpl()
    {
      target.Stats.RemoveModifier(UpgradeableStats.Speed, statModifierId);
      
      base.ExpireImpl();
    }
  }
}