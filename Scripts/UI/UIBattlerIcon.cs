using System;
using System.Collections.Generic;
using Godot;

namespace battler.Scripts.UI
{
  [Tool]
  public class UIBattlerIcon : TextureRect
  {
    [Export] private Texture icon;
    [Export] private Types type = Types.Enemy;

    public Texture Icon
    {
      get => icon;
      set
      {
        icon = value;
        if (iconNode == null)
        {
          iconNode = GetNode<TextureRect>("Icon");
        }
        iconNode.Texture = icon;
      }
    }
    
    public Types Type
    {
      get => type;
      set
      {
        type = value;
        Texture = types[type];
      }
    }

    public Vector2 PositionRange { get; set; } = Vector2.Zero;

    public enum Types
    {
      Ally,
      Player,
      Enemy
    }

    private readonly Dictionary<Types, Texture> types = new Dictionary<Types,Texture>();
    private TextureRect iconNode;

    public UIBattlerIcon()
    {
      types[Types.Ally] = ResourceLoader.Load<Texture>("res://Assets/UI/portrait_bg_ally.png");
      types[Types.Player] = ResourceLoader.Load<Texture>("res://Assets/UI/portrait_bg_player.png");
      types[Types.Enemy] = ResourceLoader.Load<Texture>("res://Assets/UI/portrait_bg_enemy.png");
    }

    public void Snap(float ratio)
    {
      var rectPosition = RectPosition;
      rectPosition.x = Mathf.Lerp(PositionRange.x, PositionRange.y, ratio);
      RectPosition = rectPosition;
    }
  }
}
