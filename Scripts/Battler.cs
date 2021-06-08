using Godot;
using Godot.Collections;

public class Battler : Node2D
{
  [Export] private BattlerStats stats;
  [Export] private PackedScene aiScene;
  [Export] private Array actions;
  [Export] private bool isPartyMember;
  
  // Emitted when changing 'isSelected'. The UI will react to this for
  // player controlled battlers.
  [Signal] public delegate void SelectionToggled(bool value);
  [Signal] public delegate void ReadyToAct(Battler battler);
  [Signal] public delegate void ReadinessChanged(float newReadiness);
  
  private bool isActive = true;
  private float readiness;
  private bool isSelected;
  private bool isSelectable;

  public bool IsPartyMember => isPartyMember;
  public Array Actions => actions;
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
        GD.Print("Battler isn't selectable!");
      }

      isSelected = value;
      EmitSignal(nameof(SelectionToggled), isSelected);
    }
  }

  public bool IsSelectable
  {
    get => isSelectable;
    set
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

    Readiness = Readiness + stats.Speed * delta * TimeScale;
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

    stats.Connect(nameof(BattlerStats.HealthDepleted), this, nameof(OnBattlerStatsHealthDepleted));
  }
}
