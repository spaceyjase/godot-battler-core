using System.Threading.Tasks;
using Godot;

namespace battler.Scripts.UI
{
  public class UIActionButton : TextureButton
  {
    private TextureRect iconNode;
    private Label labelNode;

    public override void _Ready()
    {
      base._Ready();

      iconNode = GetNode<TextureRect>("HBoxContainer/Icon");
      labelNode = GetNode<Label>("HBoxContainer/Label");
    }

    // Called from a parent UI widget. Initialises the button.
    public async Task Setup(ActionData action, bool canBeUsed)
    {
      // Wait until the button is ready...
      if (!IsInsideTree())
      {
        await ToSignal(this, "ready");
      }

      if (action.Icon != null)
      {
        iconNode.Texture = action.Icon;
      }

      labelNode.Text = action.Label;
    
      // Can the player use the action or not? If not, the button should be disabled.
      Disabled = !canBeUsed;
    }

    private void OnPressed()
    {
      ReleaseFocus();
    }
  }
}
