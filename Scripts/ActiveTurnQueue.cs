using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using battler.Scripts.UI;
using Godot;

namespace battler.Scripts
{
  public class ActiveTurnQueue : Node
  {
    [Export] private PackedScene uiActionMenuScene;
    [Export] private PackedScene selectArrowScene;
    
    // Emit a signal when play turn has finished. Used to play the next battler's turn.
    [Signal] private delegate void PlayerTurnFinished();
    
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

    public IEnumerable<Battler> Battlers => battlers;

    private readonly List<Battler> partyMembers = new List<Battler>();
    private readonly List<Battler> opponents = new List<Battler>();
    
    // If true, the player is currently playing a turn.
    private bool isPlayerPlaying = false;
    // Queue of player-controlled battlers that have to take turns.
    private readonly Queue<Battler> playerQueue = new Queue<Battler>();
  
    // All battlers in the encounter are children of this node.
    private readonly List<Battler> battlers = new List<Battler>();
  
    private bool isActive = true;
    private float timeScale = 1.0f;

    public override void _Ready()
    {
      base._Ready();

      Connect(nameof(PlayerTurnFinished), this, nameof(OnPlayerTurnFinished));

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

      foreach (var battler in battlers)
      {
        battler.Setup(battlers);
      }
    }

    private void OnBattlerReadyToAct(Battler battler)
    {
      // If the battler is controlled by the player but another player-controlled battler is
      // currently in the middle of a turn, add this one to the queue.
      if (battler.IsPlayerControlled && isPlayerPlaying)
      {
        playerQueue.Enqueue(battler);
      }
      else
      {
        Task.FromResult(PlayTurn(battler));
      }
    }

    private async Task PlayTurn(Battler battler)
    {
      ActionData actionData = null;
      var targets = new List<Battler>();

      battler.IsSelected = true;

      battler.Stats.Energy += 1;
      var opponentTargets = battler.IsPartyMember ? opponents : partyMembers;
      var potentialTargets = opponentTargets.Where(opponentTarget => opponentTarget.IsSelectable).ToList();

      if (battler.IsPlayerControlled)
      {
        battler.IsSelected = true;
      
        // slow down a bit while the player is selected
        TimeScale = 0.05f;  // TODO: magic number

        // It's the start of a player-controlled battler's turn.
        isPlayerPlaying = true;
      
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
            var target = await PlayerSelectTargets(actionData, potentialTargets);
            if (target != null)
            {
              targets.Add(target);
            }

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
        var result = battler.AI.Choose();
        actionData = result.Action;
        targets = result.Targets;
        GD.Print($"{battler.Name} attacks {targets[0].Name} with action {actionData.Label}");
      }
    
      // Create a new attack action based on the action data and targets
      var action = new AttackAction(actionData, battler, targets.ToArray());
      await battler.Act(action);
      //await ToSignal(battler, nameof(Battler.ActionFinished));

      if (battler.IsPlayerControlled)
      {
        EmitSignal(nameof(PlayerTurnFinished));
      }
    }

    private async Task<ActionData> PlayerSelectAction(Battler battler)
    {
      var actionMenu = uiActionMenuScene.Instance<UIActionMenu>();
      AddChild(actionMenu);
      
      var awaiter = ToSignal(actionMenu, nameof(UIActionMenu.ActionSelected));
      actionMenu.Open(battler);
      await awaiter;

      return awaiter.GetResult()[0] as ActionData;
    }

    private async Task<Battler> PlayerSelectTargets(ActionData actionData, List<Battler> potentialTargets)
    {
      var arrow = selectArrowScene.Instance<UISelectBattlerArrow>();
      AddChild(arrow);

      var awaiter = ToSignal(arrow, nameof(UISelectBattlerArrow.TargetSelected));
      arrow.Setup(opponents);
      await awaiter;

      var target = awaiter.GetResult()[0] as Battler;
      
      arrow.QueueFree();

      return target;
    }

    private void OnPlayerTurnFinished()
    {
      // When a player-controlled battler finishes their turn and the queue is empty,
      // the player is no longer playing.
      if (playerQueue.Count == 0)
      {
        isPlayerPlaying = false;
      }
      else
      {
        // otherwise, pop from the queue and let the corresponding battler take a turn.
        Task.FromResult(PlayTurn(playerQueue.Dequeue()));
      }
    }
  }
}