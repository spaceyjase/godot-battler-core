using System;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;

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
  
  private bool isActive = true;
  private float readiness;
  private bool isSelected;
  private bool isSelectable;

  public bool IsPartyMember => isPartyMember;
  public Array<ActionData> Actions => actions;
  public BattlerStats Stats => stats;
  
  // The turn queue will change this property when another battler is acting
  public float TimeScale { get; set; } = 1.0f;
  
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
        GD.Print("Battler '{Name}' isn't selectable!");
      }

      isSelected = value;
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
    if (!IsPartyMember)
    {
      IsSelectable = false;
    }
  }

  public override void _Ready()
  {
    base._Ready();

    stats = stats.Duplicate() as BattlerStats;
    if (stats == null)
    {
      throw new ApplicationException("Stats are null");
    }

    stats.Reinitialise();
    stats.Connect(nameof(BattlerStats.HealthDepleted), this, nameof(OnBattlerStatsHealthDepleted));
  }

  private void TakeDamage(int amount)
  {
    stats.Health -= amount;
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
    // Reset readiness. The value can be greater than zero, depending on the action.
    Readiness = action.ReadinessSaved;
    // Don't set process back to 'true' if the battler isn't active, so its readiness doesn't update.
    // That can be the case if a "stop" or "petrify" status effect is active, or during animation
    // that interrupts the normal flow of battle.
    SetProcess(IsActive);

    // Emit signal to indicate the end of a turn for a player-controlled character.
    EmitSignal(nameof(ActionFinished));
  }
}
