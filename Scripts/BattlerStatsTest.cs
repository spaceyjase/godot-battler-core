using Godot;
using System;

public class BattlerStatsTest : Node2D
{
  public override void _Ready()
  {
    base._Ready();

    var battler = GetChild<Battler>(0);
    battler.Stats.AddModifier(UpgradeableStats.Attack, 20f);
    battler.Stats.AddMultiplier(UpgradeableStats.Attack, 0.5f);
    battler.Stats.AddMultiplier(UpgradeableStats.Attack, 0.2f);

    GD.Print($"Attack (expected = 190): {battler.Stats.Attack}");
  }
}
