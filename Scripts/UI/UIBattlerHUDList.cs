using System.Collections.Generic;
using Godot;

namespace battler.Scripts.UI
{
  public class UIBattlerHUDList : VBoxContainer
  {
    private readonly PackedScene uiBattlerHud = ResourceLoader.Load<PackedScene>(
      $"res://Scenes/UI/{nameof(UIBattlerHUD)}.tscn");

    private AnimationPlayer animationPlayer;

    public override void _Ready()
    {
      base._Ready();

      animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public void Setup(IEnumerable<Battler> battlers)
    {
      foreach (var battler in battlers)
      {
        var battlerHud = uiBattlerHud.Instance<UIBattlerHUD>();
        AddChild(battlerHud);
        battlerHud.Setup(battler);
      }
    }

    public void FadeIn()
    {
      animationPlayer.Play("fade_in");
    }
    
    public void FadeOut()
    {
      animationPlayer.Play("fade_out");
    }
  }
}
