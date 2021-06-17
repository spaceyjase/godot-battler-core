using System.Collections.Generic;
using battler.Scripts.Enum;

namespace battler.Scripts.AI
{
  internal struct BattleInfo
  {
    public Battler WeakestTarget { get; set; }
    public Battler WeakestAlly { get; set; }
    public HealthStatus HealthStatus { get; set; }
    public int FallenPartyCount { get; set; }
    public int FallenOpponentsCount { get; set; }
    public ActionData[] AvailableActions { get; set; }
    public AttackActionData[] AttackActions { get; set; }
    public IEnumerable<ActionData> DefensiveActions { get; set; }
    public AttackActionData StongestActions { get; set; }
  }
}