using Godot;

public class ActionData : Resource
{
  // UI properties
  [Export] private Texture icon;
  [Export] private string label = "Base combat action";
  
  // Amount of energy the action costs to perform
  [Export] private int energyCost = 0;
  // Element type of the action; used later to add bonus damage if weak to such elements
  [Export] private Elements element = Elements.None;
  
  // Used to filter potential targets on a battler's turn.
  public bool IsTargetingSelf { get; }
  public bool IsTargetingAll { get; }

  // The amount of readiness left to the battler after acting.
  // Can be used to design weak attacks that allow you to take a turn fast, etc.
  public float ReadinessSaved { get; set; } = 0f;
  public int EnergyCost => energyCost;

  public bool CanBeUsedBy(Battler battler) => energyCost <= battler.Stats.Energy;
}