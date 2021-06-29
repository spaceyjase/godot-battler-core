using Godot;

namespace battler.Scripts.UI
{
  public class UIDamageLabel : Position2D
  {
    [Export] private Color damageColour = new Color("#b0305c");
    [Export] private Color healColour = new Color("#3ca370");

    public enum Types
    {
      Heal,
      Damage
    }

    private readonly Color COLOR_TRANSPARENT = new Color(1f, 1f, 1f, 0f);

    private int amount;
    private Label label;
    private Tween tween;
    private Color colour;

    private Color Colour
    {
      get => colour;
      set
      {
        colour = value;
        if (label != null)
        {
          label.Modulate = colour;
        }
      }
    }

    public override void _Ready()
    {
      base._Ready();

      label = GetNode<Label>("Label");
      tween = GetNode<Tween>("Tween");

      label.Text = amount.ToString();
      Animate();
    }

    private async void Animate()
    {
      var angle = (float)GD.RandRange(-Mathf.Pi / 3f, Mathf.Pi / 3f);
      var offset = Vector2.Up.Rotated(angle) * 60f;

      tween.InterpolateProperty(
        label,
        "rect_position",
        label.RectPosition,
        label.RectPosition * offset,
        0.4f,
        Tween.TransitionType.Quad,
        Tween.EaseType.Out);
      
      tween.InterpolateProperty(
        this,
        "modulate",
        Modulate,
        COLOR_TRANSPARENT,
        0.1f,
        Tween.TransitionType.Linear,
        Tween.EaseType.In);

      var awaiter = ToSignal(tween, "tween_all_completed");
      tween.Start();
      await awaiter;

      QueueFree();
    }

    public void Setup(Types type, Vector2 startPosition, int amount)
    {
      GlobalPosition = startPosition;
      this.amount = amount;

      Colour = type == Types.Damage ? damageColour : healColour;
    }
  }
}