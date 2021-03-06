using System.Threading.Tasks;
using Godot;

namespace battler.Scripts
{
  public abstract class Action : Reference
  {
    [Signal] private delegate void Finished();

    protected readonly ActionData data;
    protected readonly Battler actor;
    protected Battler[] targets;

    public Action()
    {
    
    }

    // ctor to create from code
    protected Action(ActionData data, Battler actor, Battler[] targets)
    {
      this.data = data;
      this.actor = actor;
      this.targets = targets;
    }
  
    public async Task<bool> Apply()
    {
      return await ApplyImpl();
    }

    protected virtual Task<bool> ApplyImpl()
    {
      EmitSignal(nameof(Finished));
      return Task.FromResult(true);
    }

    // Returns true if the action should target opponents by default
    public bool TargetsOpponents() => true;

    // Need to know how much readiness should be retained after performing this action
    public float ReadinessSaved => data.ReadinessSaved;

    // Allows us to highlight energy points an action will use in the energy bar.
    public int EnergyCost => data.EnergyCost;
  }
}
