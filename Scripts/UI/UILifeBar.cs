using System.Collections.Generic;
using Godot;

namespace battler.Scripts.UI
{
  public class UILifeBar : TextureProgress
  {
    // Rate of the animation relative to 'max value'. A value of 1f means the
    // animation fills the entire bar in one second.
    [Export] private float fillRate = 1f;

    private float targetValue;
    private AnimationPlayer animationPlayer;
    private Tween tween;

    public override void _Ready()
    {
      base._Ready();

      animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
      tween = GetNode<Tween>("Tween");
    }

    public void Setup(float health, float maxHealth)
    {
      MaxValue = maxHealth;
      Value = health;
      targetValue = health;

      // Animate the bar using the tween node. When complete, play the 'damage' animation.
      tween.Connect("tween_completed", this, nameof(OnTween_Completed));
    }

    public float TargetValue
    {
      get => targetValue;
      set
      {
        if (targetValue > value)
        {
          animationPlayer.Play("damage");
        }

        targetValue = value;
        if (tween.IsActive())
        {
          tween.StopAll();
        }

        // Calculate the animation duration every frame so as to have a constant rate or
        // speed at which the bar fills or empties.
        var duration = Mathf.Abs(targetValue - (float)Value) / (float)MaxValue * fillRate;
        tween.InterpolateProperty(
          this,
          "value",
          Value,
          targetValue,
          duration,
          Tween.TransitionType.Quad);
        tween.Start();
      }
    }

    private void OnTween_Completed(Object o, NodePath key)
    {
      if (Value < 0.2f * MaxValue)
      {
        animationPlayer.Play("danger");
      }
    }
  }
}
