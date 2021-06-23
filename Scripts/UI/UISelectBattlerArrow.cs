using System.Collections.Generic;
using System.Linq;
using Godot;

namespace battler.Scripts.UI
{
  public class UISelectBattlerArrow : Node2D
  {
    // Emitted when the player presses 'ui_accept' or 'ui_cancel'
    [Signal] public delegate void TargetSelected(Battler battler);

    [Export] private float moveDuration = 0.1f;

    private Tween tween;
    private readonly List<Battler> targets = new List<Battler>();
    private Battler currentTarget;

    private Battler CurrentTarget
    { 
      get => currentTarget;
      set
      { 
        currentTarget = value;
        MoveTo(currentTarget.BattlerAnimation.FrontAnchorGlobalPosition);
      }
    }

    public void Setup(IEnumerable<Battler> battlers)
    {
      Show();
      targets.AddRange(battlers);
      currentTarget = targets.First();
      
      // Flip target arrow accordingly, enemy or player...
      var scale = Scale;
      scale.x = currentTarget.IsPartyMember ? 1f : -1f;
      Scale = scale;
      
      GlobalPosition = currentTarget.BattlerAnimation.FrontAnchorGlobalPosition;
    }

    private void MoveTo(Vector2 destination)
    {
      if (tween.IsActive())
      {
        tween.StopAll();
      }

      tween.InterpolateProperty(
        this,
        "position",
        Position,
        destination,
        moveDuration,
        Tween.TransitionType.Cubic,
        Tween.EaseType.Out);
      
      tween.Start();
    }

    public UISelectBattlerArrow()
    {
      SetAsToplevel(true);
    }

    public override void _Ready()
    {
      base._Ready();

      tween = GetNode<Tween>("Tween");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
      base._UnhandledInput(@event);

      if (@event.IsActionPressed("ui_accept"))
      {
        EmitSignal(nameof(TargetSelected), CurrentTarget);
      }
      else if (@event.IsActionPressed("ui_cancel"))
      {
        EmitSignal(nameof(TargetSelected), null);
      }

      Battler newTarget;
      var direction = Vector2.Zero;
      if (@event.IsActionPressed("ui_left"))
      {
        direction = Vector2.Left;
      }
      else if (@event.IsActionPressed("ui_up"))
      {
        direction = Vector2.Up;
      }
      else if (@event.IsActionPressed("ui_right"))
      {
        direction = Vector2.Right;
      }
      else if (@event.IsActionPressed("ui_down"))
      {
        direction = Vector2.Down;
      }

      if (direction != Vector2.Zero)
      {
        newTarget = FindClosestTarget(direction);
        if (newTarget != null)
        {
          CurrentTarget = newTarget;
        }
      }
    }

    private Battler FindClosestTarget(Vector2 direction)
    {
      Battler selectedTarget = null;
      var distanceToSelected = float.PositiveInfinity;

      // Select targets at an angle of less than PI / 3 radians compared to 'direction'.
      var candidates = (from battler in targets where battler != CurrentTarget
        let toBattler = battler.GlobalPosition - Position 
        where Mathf.Abs(direction.AngleTo(toBattler)) < Mathf.Pi / 3f 
        select battler).ToList();

      foreach (var battler in candidates)
      {
        var distance = Position.DistanceTo(battler.GlobalPosition);
        if (distance >= distanceToSelected) continue;
        
        selectedTarget = battler;
        distanceToSelected = distance;
      }

      return selectedTarget;
    }
  }
}