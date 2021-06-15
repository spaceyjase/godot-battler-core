using battler.Scripts.Enum;
using Godot;

namespace battler.Scripts
{
  public class ActionData : Resource
  {
    // UI properties
    [Export] private Texture icon;
    [Export] private string label = "Base combat action";
  
    // Amount of energy the action costs to perform
    [Export] private int energyCost = 0;
    // Element type of the action; used later to add bonus damage if weak to such elements
    [Export] private Elements element = Elements.None;
    [Export] private bool isTargetingSelf;
    [Export] private bool isTargetingAll;
    [Export] private float readinessSaved;
    //[Export] private StatusEffectData statusEffect;
  
    // Used to filter potential targets on a battler's turn.
    public bool IsTargetingSelf => isTargetingSelf;
    public bool IsTargetingAll => isTargetingAll;

    public Elements Element => element;

    // The amount of readiness left to the battler after acting.
    // Can be used to design weak attacks that allow you to take a turn fast, etc.
    public float ReadinessSaved => readinessSaved;
    public int EnergyCost => energyCost;

    public bool CanBeUsedBy(Battler battler) => energyCost <= battler.Stats.Energy;
  }
}