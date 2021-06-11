using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
  [Export] private Elements[] weaknesses;
  [Export] private Elements affinity = Elements.None;

  private float health;
  private int energy;
  // Store a list of modifiers for each property->value which can be any floating-point value, +ve or -ve
  private readonly Dictionary<UpgradeableStats, Dictionary<Guid, float>> modifiers =
    new Dictionary<UpgradeableStats, Dictionary<Guid, float>>();
  private readonly Dictionary<UpgradeableStats, Dictionary<Guid, float>> multipliers =
    new Dictionary<UpgradeableStats, Dictionary<Guid, float>>();

  public float Attack { get; private set; }
  public float Defense { get; private set; }
  public float Speed { get; private set; }
  public float HitChance { get; private set; }
  public float Evasion { get; private set; }
  public Elements Affinity => affinity;
  public IEnumerable<Elements> Weaknesses => weaknesses;

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

  public float Health
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
      modifiers.Add(stat, new Dictionary<Guid, float>());
      multipliers.Add(stat, new Dictionary<Guid, float>());
    }
  }

  public void Reinitialise()
  {
    Health = maxHealth;
    Attack = (float)Get(nameof(baseAttack));
    Defense = (float)Get(nameof(baseDefense));
    Speed = (float)Get(nameof(baseSpeed));
    HitChance = (float)Get(nameof(baseHitChance));
    Evasion = (float)Get(nameof(baseEvasion));
  }

  private void Recalculate(UpgradeableStats stat)
  {
    var fieldName = $"base{stat.ToString()}";
    var value = (float)Get(fieldName);

    var multiplier = 1f;
    multiplier += multipliers[stat].Values.Sum();
    if (!Mathf.IsEqualApprox(multiplier, 1f))
    {
      value *= multiplier;
    }

    var mods = modifiers[stat].Values;
    value += mods.Sum();

    value = Mathf.Round(Mathf.Max(value, 0f));
    
    GetType().GetProperty(stat.ToString()).SetValue(this, value);
    
    GD.Print($"{stat} = {value}");
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

  public Guid AddMultiplier(UpgradeableStats stat, float value)
  {
    var id = Guid.NewGuid();
    if (!multipliers.TryGetValue(stat, out var mods)) return id;
    
    mods.Add(id, value);
    Recalculate(stat);

    return id;
  }
  
  public void RemoveMultiplier(UpgradeableStats stat, Guid id)
  {
    if (!multipliers.TryGetValue(stat, out var mods)) return;
    
    if (mods.Remove(id))
    {
      Recalculate(stat);
    }
  }
}