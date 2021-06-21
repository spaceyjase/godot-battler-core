using System.Collections.Generic;
using System.Linq;
using Godot;

namespace battler.Scripts.StatusEffects
{
  public class StatusEffectContainer : Node
  {
    // Maximum number of instances of one type of status effect that can be applied
    // to a Battler at a time.
    private const int MAX_STACKS = 5;

    // List of effects that can be stacked.
    private static readonly string[] STACKING_EFFECTS = new[] { "bug" }; // TODO: magic

    // List of effects that cannot be stacked. When a new effect is applied, it replaces or
    // refreshes the previous one.
    private static readonly string[] NON_STACKING_EFFECTS = new[] { "haste", "slow" };

    private float timeScale = 1f;
    private bool isActive = true;

    public float TimeScale
    {
      get => timeScale;
      set
      {
        timeScale = value;
        foreach (var effect in this.GetChildren<StatusEffect>())
        {
          effect.TimeScale = timeScale;
        }
      }
    }

    public bool IsActive
    {
      get => isActive;
      set
      {
        isActive = value;
        foreach (var effect in this.GetChildren<StatusEffect>())
        {
          effect.IsActive = isActive;
        }
      }
    }

    // Adds a new instance of a status effect as a child, ensuring the effects don't
    // don't stack beyond the maximum.
    public void Add(StatusEffect effect)
    {
      if (effect.CanStack)
      {
        if (HasMaximumStacksOf(effect.Id))
        {
          RemoveEffectExpiringTheSoonest(effect.Id);
        }
      }
      else if (HasNode(effect.Name))
      {
        (GetNode(effect.Name) as StatusEffect)?.Expire();
      }
    }

    private void RemoveEffectExpiringTheSoonest(string id)
    {
      StatusEffect toRemove = null;
      var smallestTime = float.PositiveInfinity;

      foreach (var effect in this.GetChildren<StatusEffect>())
      {
        if (effect.Id == id) continue;
        if (effect.TimeLeft >= smallestTime) continue;
        
        toRemove = effect;
        smallestTime = effect.TimeLeft;
      }

      toRemove?.Expire();
    }

    private bool HasMaximumStacksOf(string id)
    {
      var count = this.GetChildren<StatusEffect>().Count(effect => effect.Id == id);

      return count == MAX_STACKS;
    }

    public void RemoveType(string id)
    {
      foreach (var effect in this.GetChildren<StatusEffect>())
      {
        if (effect.Id == id)
        {
          effect.Expire();
        }
      }
    }

    public void RemoveAll()
    {
      foreach (var effect in this.GetChildren<StatusEffect>())
      {
        effect.Expire();
      }
    }
    
    
  }
}