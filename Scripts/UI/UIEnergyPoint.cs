using Godot;

namespace battler.Scripts.UI
{
  public class UIEnergyPoint : TextureRect
  {
    private readonly Vector2 POSITION_SELECTED = new Vector2(0f, -6f);

    private TextureRect fill;
    private Tween tween;
    private Color colourTransparent;

    public override void _Ready()
    {
      base._Ready();

      fill = GetNode<TextureRect>("Fill");
      tween = GetNode<Tween>("Tween");

      colourTransparent = fill.Modulate;
    }

    public void Appear()
    {
      // animate the fill node's colour from transparent to opaque
      tween.InterpolateProperty(
        this,
        "modulate",
        colourTransparent,
        Colors.Green,
        0.3f);
      tween.Start();
    }

    public void Disappear()
    {
      // animate the fill node's colour from opaque to transparent
      tween.InterpolateProperty(
        this,
        "modulate",
        fill.Modulate,
        colourTransparent,
        0.3f);  // magic
      tween.Start();
    }

    public void Select()
    {
      // This animation offsets the node to the selected position. The 'out' easing
      // means the animation starts at full speed and slows down as it reaches its target.
      tween.InterpolateProperty(
        fill,
        "rect_position",
        Vector2.Zero,
        POSITION_SELECTED,
        0.2f,
        Tween.TransitionType.Cubic,
        Tween.EaseType.Out);
      tween.Start();
    }
    
    public void Deselect()
    {
      tween.InterpolateProperty(
        fill,
        "rect_position",
        POSITION_SELECTED,
        Vector2.Zero,
        0.2f,
        Tween.TransitionType.Cubic,
        Tween.EaseType.Out);
      tween.Start();
    }
  }
}
