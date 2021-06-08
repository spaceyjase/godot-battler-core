using Godot;
using System;

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

  public void Reinitialise()
  {
    Health = maxHealth;
  }

  // TODO: revisit
  private void SetBaseAttack(float value)
  {
    baseAttack = value;
    Recalculate("attack");
  }
  
  private void SetBaseDefense(float value)
  {
    baseDefense = value;
    Recalculate("defense");
  }
  
  private void SetBaseSpeed(float value)
  {
    baseSpeed = value;
    Recalculate("speed");
  }
  
  private void SetBaseHitChance(float value)
  {
    baseHitChance = value;
    Recalculate("hitChance");
  }
  
  private void SetBaseEvasion(float value)
  {
    baseEvasion = value;
    Recalculate("evasion");
  }

  private void Recalculate(string attack)
  {
    throw new NotImplementedException();
  }
}