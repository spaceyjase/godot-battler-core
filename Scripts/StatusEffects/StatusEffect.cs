using System;
using System.Runtime.InteropServices;
using Godot;

namespace battler.Scripts.StatusEffects
{
  // Represents and applies the effect of a given status to a battler.
  // The status takes effect as soon as the node ia dded to the scene tree.
  public abstract class StatusEffect : Node // For process.
  {
    private float durationSeconds;
    private float timeLeftSeconds = -float.NegativeInfinity;
    // time left in the current tick, if the effect is ticking.
    private float tickingClock;
    protected Battler target;
    private bool isActive = true;

    public string Id { get; set; } = "base_effect";
    public float TimeScale { get; set; } = 1f;
    public bool CanStack { get; set; }
    public float TimeLeft => timeLeftSeconds;
    
    public float DurationSeconds
    {
      get => durationSeconds;
      set
      {
        durationSeconds = value;
        timeLeftSeconds = durationSeconds;
      }
    }

    // If true, the effect applies every tick.
    public bool IsTicking { get; set; } = false;
    public float TickingInterval { get; set; } = 1f;

    public bool IsActive
    {
      get => isActive;
      set
      {
        isActive = value;
        SetProcess(isActive);
      }
    }

    protected StatusEffect()
    {
      
    }

    protected StatusEffect(Battler target, StatusEffectData data)
    {
      this.target = target;
      DurationSeconds = data.DurationSeconds;
      
      // if ticking, initialise the corresponding variables.
      IsTicking = data.IsTicking;
      TickingInterval = data.TickingInterval;
      tickingClock = TickingInterval;
    }

    // Status effects are nodes that come into effect when added to the battler's
    // 'StatusEffectContainer' node.
    public override void _Ready()
    {
      base._Ready();

      Start();
    }

    protected virtual void Start() { }

    protected virtual void Apply() { }

    public void Expire()
    {
      ExpireImpl();
    }

    protected virtual void ExpireImpl()
    {
      QueueFree();
    }

    public override void _Process(float delta)
    {
      base._Process(delta);

      timeLeftSeconds -= delta * TimeScale;
      
      // if ticking, need to know when to apply it. For example, for poison,
      // you want to inflict damage every 'interval' seconds.
      if (IsTicking)
      {
        var oldClock = tickingClock;

        tickingClock = Mathf.Wrap(tickingClock - delta * TimeScale, 0f, TickingInterval);
        
        // when the effect wraps, the ticking clock is greater than the 'old clock';
        // the effect can be applied.
        if (tickingClock > oldClock)
        {
          Apply();
        }
      }

      if (timeLeftSeconds > 0f) return;
      
      SetProcess(false);
      Expire();
    }
  }
}