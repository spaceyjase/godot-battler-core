using System.Collections.Generic;
using Godot;

namespace battler.Scripts.AI
{
  // An aggressive AI that always chooses the strongest action against a single, weak target.
  public class AggressiveBattlerAI : BattlerAI
  {
    protected override ActionData ChooseAction(BattleInfo info)
    {
      return info.StongestAction;
    }

    protected override IEnumerable<Battler> ChooseTargets(ActionData action, BattleInfo battleInfo)
    {
      return new[] { battleInfo.WeakestTarget } ;
    }
  }
}