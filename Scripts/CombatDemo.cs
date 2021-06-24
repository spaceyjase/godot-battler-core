using System.Collections.Generic;
using System.Linq;
using battler.Scripts.UI;
using Godot;

namespace battler.Scripts
{
  public class CombatDemo : Node2D
  {
    private ActiveTurnQueue activeTurnQueue;
    private UITurnBar uiTurnBar;
    private UIBattlerHUDList uiBattlerHudList;

    public override void _Ready()
    {
      base._Ready();

      activeTurnQueue = GetNode<ActiveTurnQueue>("ActiveTurnQueue");
      uiTurnBar = GetNode<UITurnBar>("UI/UITurnBar");
      uiBattlerHudList = GetNode<UIBattlerHUDList>("UI/UIBattlerHUDList");

      var party = activeTurnQueue.Battlers.Where(battler => battler.IsPartyMember).ToList();

      uiTurnBar.Setup(activeTurnQueue.Battlers);
      uiBattlerHudList.Setup(party);
    }
  }
}