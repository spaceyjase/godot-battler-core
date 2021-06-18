using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using battler.Scenes;
using battler.Scripts.AI;
using Godot;
using Godot.Collections;

namespace battler.Scripts
{
  public class Battler : Node2D
  {
    [Export] private BattlerStats stats;
    [Export] private PackedScene aiScene;
    [Export] private Array<ActionData> actions;
    [Export] private bool isPartyMember;
  
    // Emitted when changing 'isSelected'. The UI will react to this for
    // player controlled battlers.
    [Signal] public delegate void SelectionToggled(bool value);
    [Signal] public delegate void ReadyToAct(Battler battler);
    [Signal] public delegate void ReadinessChanged(float newReadiness);
    // Emit when taking damage (and hit).
    [Signal] public delegate void DamageTaken(int amount);
    // Emitted when a received hit missed.
    [Signal] public delegate void HitMissed();
    // Emit when the battler finished their action and arrived back at their resting position.
    [Signal] public delegate void ActionFinished();
    // Emitted when an animation has finished playing.
    [Signal] public delegate void AnimationFinished(string animationName);
  
    private bool isActive = true;
    private float readiness;
    private bool isSelected;
    private bool isSelectable;
    private BattlerAnim battlerAnim;
    private BattlerAI aiInstance;

    public bool IsPartyMember => isPartyMember;
    public Array<ActionData> Actions => actions;
    public BattlerStats Stats => stats;
    public bool IsFallen => stats.Health <= 0f;
  
    // The turn queue will change this property when another battler is acting
    public float TimeScale { get; set; } = 1.0f;

    public void Setup(IEnumerable<Battler> battlers)
    {
      if (aiScene == null) return;
      
      aiInstance = aiScene.Instance() as BattlerAI;
      if (aiInstance == null) GD.PrintErr("Can't create AI scene!");
      
      aiInstance?.Setup(this, battlers);
      AddChild(aiInstance);
    }

    // The BattlerAI instance attached to this battler.
    public BattlerAI AI => aiInstance;

    // When this value reaches 100, the battler is ready to take their turn
    private float Readiness
    {
      get => readiness;
      set
      {
        readiness = value;
        EmitSignal(nameof(ReadinessChanged), readiness);

        if (readiness < 100f) return;
      
        EmitSignal(nameof(ReadyToAct), this);
      
        // pause the process loop now this battler is ready
        SetProcess(false);
      }
    }
  
    public bool IsActive
    {
      get => isActive;
      set
      {
        isActive = value;
        SetProcess(isActive);
      }
    }

    public bool IsSelected
    {
      get => isSelected;
      set
      {
        if (value && !isSelectable)
        {
          GD.Print($"Battler '{Name}' isn't selectable!");
        }

        isSelected = value;
        if (isSelected)
        {
          battlerAnim.MoveForward();
        }
        
        EmitSignal(nameof(SelectionToggled), isSelected);
      }
    }

    public bool IsSelectable
    {
      get => isSelectable;
      private set
      {
        isSelectable = value;
        if (!isSelectable)
        {
          IsSelected = false;
        }
      }
    }

    public bool IsPlayerControlled => aiScene == null;
    public BattlerAnim BattlerAnimation => battlerAnim;

    public override void _Process(float delta)
    {
      base._Process(delta);

      Readiness += stats.Speed * delta * TimeScale;
    }

    public void SetTimeScale(float newScale)
    {
      TimeScale = newScale;
    }

    private void OnBattlerStatsHealthDepleted()
    {
      IsActive = false;
      if (IsPartyMember) return;
      
      IsSelectable = false;
      battlerAnim.QueueAnimation("die");
    }

    public override void _Ready()
    {
      base._Ready();

      if (stats == null)
      {
        throw new ApplicationException("Stats are null");
      }
      stats = stats.Duplicate() as BattlerStats;

      if (stats != null)
      {
        stats.Reinitialise();
        stats.Connect(nameof(BattlerStats.HealthDepleted), this, nameof(OnBattlerStatsHealthDepleted));
      }
      else
      {
        throw new ApplicationException("Stats (duplicate) are null");
      }

      battlerAnim = GetNode<BattlerAnim>("BattlerAnim");
    }

    private void TakeDamage(int amount)
    {
      stats.Health -= amount;
      if (stats.Health > 0f)
      {
        battlerAnim.Play("take_damage");
      }

      GD.Print($"{Name} took {amount} damage. Health is now {stats.Health}");
    }

    public void TakeHit(Hit hit)
    {
      if (hit.DoesHit)
      {
        TakeDamage(hit.Damage);
        EmitSignal(nameof(DamageTaken), hit.Damage);
      }
      else
      {
        EmitSignal(nameof(HitMissed));
      }
    }

    public async Task Act(Action action)
    {
      // If the action costs energy, subtract it.
      stats.Energy -= action.EnergyCost;
      
      // Wait for the action to apply; it's a coroutine so we need to yield.
      await action.Apply();
      battlerAnim.MoveBack();
      
      // Reset readiness. The value can be greater than zero, depending on the action.
      Readiness = action.ReadinessSaved;
      
      // Don't set process back to 'true' if the battler isn't active, so its readiness doesn't update.
      // That can be the case if a "stop" or "petrify" status effect is active, or during animation
      // that interrupts the normal flow of battle.
      SetProcess(IsActive);

      // Emit signal to indicate the end of a turn for a player-controlled character.
      EmitSignal(nameof(ActionFinished));
    }

    public void OnBattlerAnimAnimationFinished(string animationName)
    {
      EmitSignal(nameof(AnimationFinished), animationName);
    }
  }
}
