using Godot;

namespace battler.Scripts.UI
{
  public class UIMenuSelectArrow : Position2D
  {
    private Tween tween;

    public override void _Ready()
    {
      base._Ready();

      tween = GetNode<Tween>("Tween");
    }

    public UIMenuSelectArrow()
    {
      // Need to move independently from our parent.
      SetAsToplevel(true);
    }

    public void MoveTo(Vector2 target)
    {
      if (tween.IsActive())
      {
        tween.Stop(this, "position");
      }
      
      // Tween the arrow position, which is global, for 0.1 seconds to
      // make the menu feel responsive.
      tween.InterpolateProperty(this, "position", Position, target,
        0.1f /* magic */, Tween.TransitionType.Cubic, Tween.EaseType.Out);

      tween.Start();
    }
  }
}
