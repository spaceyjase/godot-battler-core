using System;
using System.Collections.Generic;
using System.Linq;
using battler.Scripts.Enum;
using Godot;

namespace battler.Scripts.AI
{
  public class BattlerAI : Node
  {
    // The actor that has this AI brain.
    private Battler actor;
    // An array of battlers in the AI's party, including the 'actor'.
    private readonly List<Battler> party = new List<Battler>();
    // An array of battlers that are in the opposing party.
    private readonly List<Battler> opponents = new List<Battler>();
    // Store a dictionary of opponents that are weak to this action.
    private readonly Dictionary<ActionData, List<Battler>> weaknesses = new Dictionary<ActionData, List<Battler>>();
    // Queue of actions; if not empty, the action is popped from here on the next turn. Used
    // to plan actions over multiple turns (e.g. Boss charging energy).
    private readonly Queue<ActionData> nextActions = new Queue<ActionData>();
    private readonly RandomNumberGenerator rng = new RandomNumberGenerator();

    // Filters and saves the list of party members and opponents.
    public void Setup(Battler _actor, IEnumerable<Battler> battlers)
    {
      rng.Randomize();
      actor = _actor;
      foreach (var battler in battlers)
      {
        var isOpponent = battler.IsPartyMember != _actor.IsPartyMember;
        if (isOpponent)
        {
          opponents.Add(battler);
        }
        else
        {
          party.Add(battler);
        }
      }

      CalculateWeaknesses();
    }
    
    // Returns a structure representing an action and its targets.
    public Choice Choose()
    {
      if (opponents.Count == 0)
      {
        GD.PrintErr("You must call Setup() on the AI and give it opponents!");
      }

      var battleInfo = GatherInformation();

      var targets = new List<Battler>();
      var action = nextActions.Count != 0 ? nextActions.Dequeue() : ChooseAction(battleInfo);
      if (action.IsTargetingSelf)
      {
        targets.Add(actor);
      }
      else
      {
        targets.AddRange(ChooseTargets(action, battleInfo));
      }

      return new Choice
      {
        Action = action,
        Targets = targets
      };
    }

    protected virtual IEnumerable<Battler> ChooseTargets(ActionData action, BattleInfo battleInfo)
    {
      return new[]{ battleInfo.WeakestTarget };
    }

    protected virtual ActionData ChooseAction(BattleInfo battleInfo)
    {
      return actor.Actions[0];
    }

    private void CalculateWeaknesses()
    {
      foreach (var action in actor.Actions)
      {
        weaknesses[action] = new List<Battler>();
        foreach (var opponent in opponents.Where(opponent => IsWeakTo(opponent, action)))
        {
          weaknesses[action].Add(opponent);
        }
      }
    }

    private BattleInfo GatherInformation()
    {
      var actions = GetAvailableActions().ToArray();
      var attackActions = GetAttackActionsFrom(actions).ToArray();
      var defensiveActions = GetDefensiveActionsFrom(actions);

      var info = new BattleInfo
      {
        WeakestTarget = GetBattlerWithLowestHealth(opponents),
        WeakestAlly = GetBattlerWithLowestHealth(party),
        HealthStatus = GetHealthStatus(actor),
        FallenPartyCount = party.Count(ally => ally.IsFallen),
        FallenOpponentsCount = opponents.Count(opponent => opponent.IsFallen),
        AvailableActions = actions,
        AttackActions = attackActions,
        DefensiveActions = defensiveActions,
        StongestAction = FindMostDamagingActionFrom(attackActions)
      };

      return info;
    }

    private AttackActionData FindMostDamagingActionFrom(IEnumerable<AttackActionData> attackActions)
    {
      // The strongest is the one that will inflict the most damage, in theory.
      AttackActionData strongestAction = null;
      var highestDamage = 0f;
      
      foreach (var action in attackActions)
      {
        var totalDamage = action.CalculatePotentialDamageFor(actor);
        if (totalDamage <= highestDamage) continue;
        
        strongestAction = action;
        highestDamage = totalDamage;
      }

      return strongestAction;
    }

    private static bool IsHealthBelow(Battler battler, float ratio)
    {
      ratio = Mathf.Clamp(ratio, 0f, 1f);
      return battler.Stats.Health < battler.Stats.MaxHealth * ratio;
    }

    // Useful if above a certain threshold, strategy may change.
    private static bool IsHealthAbove(Battler battler, float ratio)
    {
      ratio = Mathf.Clamp(ratio, 0f, 1f);
      return battler.Stats.Health > battler.Stats.MaxHealth * ratio;
    }

    private static bool IsWeakTo(Battler battler, ActionData data)
    {
      return battler.Stats.Weaknesses.Contains(data.Element);
    }

    private static HealthStatus GetHealthStatus(Battler battler)
    {
      if (IsHealthBelow(battler, 0.1f))
      {
        return HealthStatus.Critical;
      }

      if (IsHealthBelow(battler, 0.3f))
      {
        return HealthStatus.Low;
      }

      if (IsHealthBelow(battler, 0.6f))
      {
        return HealthStatus.Medium;
      }

      return IsHealthBelow(battler, 1.0f) ? HealthStatus.High : HealthStatus.Full;
    }

    private static Battler GetBattlerWithLowestHealth(IReadOnlyList<Battler> battlers)
    {
      var weakest = battlers[0];
      foreach (var battler in battlers.Where(battler => battler.Stats.Health < weakest.Stats.Health))
      {
        weakest = battler;
      }

      return weakest;
    }

    // Returns actions that are NOT attack actions in available actions.
    private static IEnumerable<ActionData> GetDefensiveActionsFrom(IEnumerable<ActionData> actions)
    {
      return actions.Where(action => !(action is AttackActionData)).ToList();
    }

    // Returns attack actions in available actions.
    private static IEnumerable<AttackActionData> GetAttackActionsFrom(IEnumerable<ActionData> actions)
    {
      var attackActions = new List<AttackActionData>();
      foreach (var action in actions)
      {
        if (action is AttackActionData attackActionData)
        {
          attackActions.Add(attackActionData);
        }
      }

      return attackActions;
    }

    // Actions the agent can use this turn.
    private IEnumerable<ActionData> GetAvailableActions()
    {
      var actions = new List<ActionData>();
      actions.AddRange(actor.Actions.Where(action => action.CanBeUsedBy(actor)));
      
      return actions;
    }
    
  }
}
