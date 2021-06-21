using System;
using System.Collections.Generic;
using Godot;

namespace battler.Scripts.StatusEffects
{
  public abstract class StatusEffectBuilder : Reference
  {
    // TODO: should be a list of resources from the folder or something so it is created dynamically...
    private static Dictionary<string, Func<Battler, StatusEffectData, StatusEffect>> statusEffects = 
      new Dictionary<string, Func<Battler, StatusEffectData, StatusEffect>>
    {
      { "haste", (t, d) => new StatusEffectHaste(t, d) },
      { "slow", (t, d) => new StatusEffectSlow(t, d) },
      { "bug", (t, d) => new StatusEffectBug(t, d) }
    };
      
    public static StatusEffect CreateStatusEffect(Battler target, StatusEffectData data)
    {
      return data == null ? null : statusEffects[data.Effect](target, data);
    }
  }
}

