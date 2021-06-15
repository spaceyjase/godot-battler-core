using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace battler.Scripts
{
  public class ActiveTurnQueue : Node
  {
    public bool IsActive
    {
      get => isActive;
      set
      {
        isActive = value;
        foreach (var battler in battlers)
        {
          battler.IsActive = isActive;
        }
      }
    }

    public float TimeScale
    {
      get => timeScale;
      set
      {
        timeScale = value;
        foreach (var battler in battlers)
        {
          battler.TimeScale = timeScale;
        }
      }
    }

    private readonly List<Battler> partyMembers = new List<Battler>();
    private readonly List<Battler> opponents = new List<Battler>();
  
    // All battlers in the encounter are children of this node.
    private readonly List<Battler> battlers = new List<Battler>();
  
    private bool isActive = true;
    private float timeScale = 1.0f;

    public override void _Ready()
    {
      base._Ready();

      foreach (var child in GetChildren())
      {
        if (!(child is Battler battler)) continue;

        battlers.Add(battler);
        battler.Connect(nameof(Battler.ReadyToAct), this, nameof(OnBattlerReadyToAct));
        if (battler.IsPartyMember)
        {
          partyMembers.Add(battler);
        }
        else
        {
          opponents.Add(battler);
        }
      }
    }

    private void OnBattlerReadyToAct(Battler battler)
    {
      Task.FromResult(PlayTurn(battler));
    }

    private async Task PlayTurn(Battler battler)
    {
      ActionData actionData = null;
      var targets = new List<Battler>();

      battler.Stats.Energy += 1;
      var opponentTargets = battler.IsPartyMember ? opponents : partyMembers;
      var potentialTargets = opponentTargets.Where(opponentTarget => opponentTarget.IsSelectable).ToList();

      if (battler.IsPlayerControlled)
      {
        battler.IsSelected = true;
      
        // slow down a bit while the player is selected
        TimeScale = 0.05f;  // TODO: magic number
      
        // wait for the player to select a valid action and target
        var selectionComplete = false;
        while (!selectionComplete)
        {
          // the player has to select an action first and then a target
          actionData = await PlayerSelectAction(battler);
          // if an action applies an effect to the battler only, automatically set it as the target
          if (actionData.IsTargetingSelf)
          {
            targets.Clear();
            targets.Add(battler);
          }
          else
          {
            targets = await PlayerSelectTargets(actionData, potentialTargets);
            await ToSignal(GetTree(), "idle_frame");
          }
          // if the player selected a correct action and target, break.
          selectionComplete = targets.Count != 0;
        }

        // Ready to act - reset time scale and deselect the battler
        TimeScale = 1.0f; // TODO: magic number
        battler.IsSelected = false;
      }
      else
      {
        actionData = battler.Actions[0];
        targets = potentialTargets;
      }
    
      // Create a new attack action based on the action data and targets
      var action = new AttackAction(actionData, battler, targets.ToArray());
      await battler.Act(action);
      await ToSignal(battler, nameof(Battler.ActionFinished));
    }

    private async Task<ActionData> PlayerSelectAction(Battler battler)
    {
      await ToSignal(GetTree(), "idle_frame");
      return battler.Actions[0];
    }

    private async Task<List<Battler>> PlayerSelectTargets(ActionData actionData, List<Battler> potentialTargets)
    {
      await ToSignal(GetTree(), "idle_frame");
      return opponents;
    }
  }
}