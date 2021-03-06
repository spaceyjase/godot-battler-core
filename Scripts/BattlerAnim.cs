using Godot;

namespace battler.Scenes
{
  [Tool]
  public class BattlerAnim : Position2D
  {
    // Forwards the animation player's finished signal.
    [Signal] public delegate void AnimationFinished(string name);

    // emitted by animations when a combat action should apply its next effect, like damage
    [Signal] public delegate void Triggered();

    // A battler looks in one of two directions.
    private enum Direction
    {
      LEFT,
      RIGHT
    }
    
    [Export] private Direction LookDirection { get; set; } = Direction.RIGHT;

    public Vector2 FrontAnchorGlobalPosition => frontAnchor.GlobalPosition;
    public Vector2 TopAnchorGlobalPosition => topAnchor.GlobalPosition;

    private Vector2 startPosition = Vector2.Zero;
    private AnimationPlayer animPlayer;
    private AnimationPlayer animPlayerDamage;
    private Tween tween;
    private Position2D frontAnchor;
    private Position2D topAnchor;

    public override void _Ready()
    {
      base._Ready();

      animPlayer = GetNode<AnimationPlayer>("Pivot/AnimationPlayer");
      animPlayerDamage = GetNode<AnimationPlayer>("Pivot/AnimationPlayerDamage");
      tween = GetNode<Tween>("Tween");
      frontAnchor = GetNode<Position2D>("FrontAnchor");
      topAnchor = GetNode<Position2D>("TopAnchor");

      startPosition = Position;
      //var scale = Scale;
      //scale.x = LookDirection == Direction.RIGHT ? -1 : 1f;
      //Scale = scale;
    }

    public void Play(string animationName)
    {
      if (animationName == "take_damage")
      {
        animPlayerDamage.Play(animationName);
        // Restart the animation if it is already playing.
        animPlayerDamage.Seek(0f);
      }
      else
      {
        animPlayer.Play(animationName);
      }
    }

    public bool IsPlaying => animPlayer.IsPlaying();

    public void QueueAnimation(string animationName)
    {
      animPlayer.Queue(animationName);
      if (!IsPlaying)
      {
        animPlayer.Play();
      }
    }

    // Move forward and back to emphasis the start and end of a battler's turn.
    public void MoveForward()
    {
      tween.InterpolateProperty(
        this,
        "position",
        Position,
        Position + Vector2.Right * Scale.x * 40f,
        0.3f,
        Tween.TransitionType.Quart);
      tween.Start();
    }

    // Moves the node back to the start position.
    public void MoveBack()
    {
      tween.InterpolateProperty(
        this,
        "position",
        Position,
        startPosition,
        0.3f,
        Tween.TransitionType.Quart);
      tween.Start();
    }

    public void OnAnimationPlayerAnimationFinished(string animationName)
    {
      EmitSignal(nameof(AnimationFinished), animationName);
    }
  }
}