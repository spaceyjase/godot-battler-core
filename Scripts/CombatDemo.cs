using System.Collections.Generic;
using System.Linq;
using battler.Scenes;
using battler.Scripts.UI;
using Godot;
using Godot.Collections;

namespace battler.Scripts
{
  public class CombatDemo : Node2D
  {
    [Signal] public delegate void CombatEnded(string message);
    
    private ActiveTurnQueue activeTurnQueue;
    private UITurnBar uiTurnBar;
    private UIBattlerHUDList uiBattlerHudList;
    private UIDamageLabelBuilder uiDamageLabelBuilder;

    private enum CombatResult
    {
      Defeat,
      Victory
    }

    public override void _Ready()
    {
      base._Ready();

      activeTurnQueue = GetNode<ActiveTurnQueue>("ActiveTurnQueue");
      uiTurnBar = GetNode<UITurnBar>("UI/UITurnBar");
      uiBattlerHudList = GetNode<UIBattlerHUDList>("UI/UIBattlerHUDList");
      uiDamageLabelBuilder = GetNode<UIDamageLabelBuilder>("UI/UIDamageLabelBuilder");

      var party = activeTurnQueue.Battlers.Where(battler => battler.IsPartyMember).ToList();

      uiTurnBar.Setup(activeTurnQueue.Battlers);
      uiBattlerHudList.Setup(party);
      uiDamageLabelBuilder.Setup(activeTurnQueue.Battlers);

      foreach (var battler in activeTurnQueue.Battlers)
      {
        battler.Stats.Connect(nameof(BattlerStats.HealthDepleted), this,
          nameof(OnBattlerStats_HealthDepleted), new Array { battler });
      }
    }

    private IEnumerable<Battler> GetAllBattlersOf(Battler actor)
    {
      return activeTurnQueue.Battlers
        .Where(battler => battler.IsPartyMember == actor.IsPartyMember).ToList();
    }

    private static bool AreAllFallen(IEnumerable<Battler> battlers)
    {
      return battlers.All(battler => battler.IsFallen);
    }

    private void EndCombat(CombatResult result)
    {
      activeTurnQueue.IsActive = false;
      uiTurnBar.FadeOut();
      uiBattlerHudList.FadeOut();

      var message = result == CombatResult.Victory ? "Victory!" : "Defeat!";
      EmitSignal(nameof(CombatEnded), message);
    }

    private void OnBattlerStats_HealthDepleted(Battler battler)
    {
      var team = GetAllBattlersOf(battler);
      if (AreAllFallen(team))
      {
        EndCombat(battler.IsPartyMember ? CombatResult.Defeat : CombatResult.Victory);
      }
    }
  }
}