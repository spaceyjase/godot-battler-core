using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace battler.Scripts.UI
{
  public class UITurnBar : Control
  {
    private readonly PackedScene battlerIcon = ResourceLoader.Load<PackedScene>(
      $"res://Scenes/UI/{nameof(UIBattlerIcon)}.tscn");

    private TextureRect background;
    private AnimationPlayer animationPlayer;

    public override void _Ready()
    {
      base._Ready();

      background = GetNode<TextureRect>("Background");
      animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public void Setup(IEnumerable<Battler> battlers) // TODO: UI controls should implement an interface
    {
      foreach (var battler in battlers)
      {
        var type = battler.IsPartyMember ? UIBattlerIcon.Types.Player : UIBattlerIcon.Types.Enemy;
        var icon = CreateIcon(type, battler.UIData.Texture);

        battler.Connect(nameof(Battler.ReadinessChanged), this,
          nameof(OnBattler_ReadinessChanged), new Array { icon });
        background.AddChild(icon);
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

    private void OnBattler_ReadinessChanged(float readiness, UIBattlerIcon icon)
    {
      // TODO: dedicated property here because game juice; e.g. animating progress.
      icon.Snap(readiness / 100f);
    }

    private UIBattlerIcon CreateIcon(UIBattlerIcon.Types type, Texture texture)
    {
      var icon = battlerIcon.Instance<UIBattlerIcon>();
      icon.Icon = texture;
      icon.Type = type;

      icon.PositionRange = new Vector2(
        -icon.RectSize.x / 2f,
        -icon.RectSize.x / 2f + background.RectSize.x);

      return icon;
    }
  }
}