using System.Net.NetworkInformation;
using Godot;

namespace battler.Scripts.Events
{
  // Event bus for distant nodes to communicate using signals, avoiding
  // complex coupling or substantially increasing code complexity.
  public class EventBus : Node
  {
    [Signal]
    public delegate void CombatActionHovered(string displayName, int energyCost);

    [Signal]
    public delegate void PlayerTargetSelectionDone();

    public static EventBus Instance { get; } = new EventBus();
  }
}