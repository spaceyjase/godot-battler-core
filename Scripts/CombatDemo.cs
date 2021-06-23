using battler.Scripts.UI;
using Godot;

namespace battler.Scripts
{
  public class CombatDemo : Node2D
  {
    private ActiveTurnQueue activeTurnQueue;
    private UITurnBar uiTurnBar;

    public override void _Ready()
    {
      base._Ready();

      activeTurnQueue = GetNode<ActiveTurnQueue>("ActiveTurnQueue");
      uiTurnBar = GetNode<UITurnBar>("UI/UITurnBar");

      uiTurnBar.Setup(activeTurnQueue.Battlers);
    }
  }
}