using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace battler.Scripts.UI
{
  public class UIDamageLabelBuilder : Node2D
  {
    [Export] private PackedScene damageLabelScene;
    [Export] private PackedScene missLabelScene;

    public void Setup(IEnumerable<Battler> battlers)
    {
      foreach (var battler in battlers)
      {
        battler.Connect(nameof(Battler.DamageTaken), this, nameof(OnBattler_DamageTaken), new Array{ battler });
        battler.Connect(nameof(Battler.HitMissed), this, nameof(OnBattler_HitMissed), new Array{ battler });
      }
    }

    private void OnBattler_DamageTaken(int amount, Battler battler)
    {
      var label = damageLabelScene.Instance<UIDamageLabel>();
      label.Setup(UIDamageLabel.Types.Damage, battler.BattlerAnimation.TopAnchorGlobalPosition, amount);
      AddChild(label);
    }

    private void OnBattler_HitMissed(Battler battler)
    {
      var label = missLabelScene.Instance<Node2D>();
      AddChild(label);
      label.GlobalPosition = battler.BattlerAnimation.TopAnchorGlobalPosition;
    }
  }
}
