using System.Threading.Tasks;
using Godot;

namespace battler.Scripts.UI
{
  // A menu displaying lists of actions the player can select.
  public class UIActionMenu : Control
  {
    [Signal] public delegate void ActionSelected(ActionData action);

    private readonly PackedScene uiActionList = ResourceLoader.Load<PackedScene>(
      $"res://Scenes/UI/{nameof(UIActionList)}.tscn");

    public override void _Ready()
    {
      base._Ready();
      
      Hide();
    }

    public void Open(Battler battler)
    {
      var list = uiActionList.Instance<UIActionList>();
      AddChild(list);

      list.Connect(nameof(UIActionList.ActionSelected), this, nameof(OnActionListActionSelected));
      list.Setup(battler);
      
      Show();
      
      list.Focus();
    }

    public void Close()
    {
      Hide();
      QueueFree();
    }

    private void OnActionListActionSelected(ActionData action)
    {
      EmitSignal(nameof(ActionSelected), action);
      Close();
    }
  }
}