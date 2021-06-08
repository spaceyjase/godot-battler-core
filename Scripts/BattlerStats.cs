using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class BattlerStats : Resource
{
  [Signal] public delegate void HealthDepleted();
  [Signal] public delegate void HealthChanged(int oldValue, int newValue);
  [Signal] public delegate void EnergyChanged(int oldValue, int newValue);

  [Export] private float maxHealth = 100f;
  [Export] private int maxEnergy = 6;

  [Export] private float baseAttack = 10f;
  [Export] private float baseDefense = 10f;
  [Export] private float baseSpeed = 70f;
  [Export] private float baseHitChance = 100f;
  [Export] private float baseEvasion;

  private float health;
  private int energy;
  // Store a list of modifiers for each property->value which can be any floating-point value, +ve or -ve
  private Dictionary<UpgradeableStats, Dictionary<Guid, float>> modifiers =
    new Dictionary<UpgradeableStats, Dictionary<Guid, float>>();

  public float Attack => baseAttack;
  public float Defense => baseDefense;
  public float Speed => baseSpeed;
  public float HitChance => baseHitChance;
  public float Evasion => baseEvasion;

  public int Energy
  {
    get => energy;
    set
    {
      var previousEnergy = energy;
      energy = Mathf.Clamp(value, 0, maxEnergy);
      EmitSignal(nameof(EnergyChanged), previousEnergy, energy);
    }
  }

  private float Health
  {
    get => health;
    set
    {
      var previousHealth = health;
      health = Mathf.Clamp(value, 0f, maxHealth);
      EmitSignal(nameof(HealthChanged), previousHealth, health);
      if (Mathf.IsEqualApprox(health, 0f))
      {
        EmitSignal(nameof(HealthDepleted));
      }
    }
  }

  public BattlerStats()
  {
    foreach (UpgradeableStats stat in Enum.GetValues(typeof(UpgradeableStats)))
    {
      // upgrades for each stat are unique, key-value pairs.
      modifiers.Add(stat, new Dictionary<string, float>());
    }
  }

  public void Reinitialise()
  {
    Health = maxHealth;
  }

  // TODO: revisit
  private void SetBaseAttack(float value)
  {
    baseAttack = value;
    Recalculate(UpgradeableStats.Attack);
  }
  
  private void SetBaseDefense(float value)
  {
    baseDefense = value;
    Recalculate(UpgradeableStats.Defense);
  }
  
  private void SetBaseSpeed(float value)
  {
    baseSpeed = value;
    Recalculate(UpgradeableStats.Speed);
  }
  
  private void SetBaseHitChance(float value)
  {
    baseHitChance = value;
    Recalculate(UpgradeableStats.HitChance);
  }
  
  private void SetBaseEvasion(float value)
  {
    baseEvasion = value;
    Recalculate(UpgradeableStats.Evasion);
  }

  private void Recalculate(UpgradeableStats stat)
  {
    // ReSharper disable once PossibleNullReferenceException
    var value = (float)GetType().GetProperty(stat.ToString()).GetValue(this);
    var mods = this.modifiers[stat].Values;
    value += mods.Sum();
    GetType().GetProperty(nameof(stat))?.SetValue(this, value);
  }

  public Guid AddModifier(UpgradeableStats stat, float value)
  {
    var id = Guid.NewGuid();
    if (!modifiers.TryGetValue(stat, out var mods)) return id;
    
    mods.Add(id, value);
    Recalculate(stat);

    return id;
  }

  public void RemoveModifier(UpgradeableStats stat, Guid id)
  {
    if (!modifiers.TryGetValue(stat, out var mods)) return;
    
    if (mods.Remove(id))
    {
      Recalculate(stat);
    }
  }
}