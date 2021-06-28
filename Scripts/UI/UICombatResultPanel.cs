using System.Security.Cryptography.X509Certificates;
using Godot;

namespace battler.Scripts.UI
{
  [Tool]
  public class UICombatResultPanel : Panel
  {
    [Export] private string text;

    public string Text
    {
      get => text;
      set
      {
        text = value;
        if (label != null)
        {
          label.Text = text;
        }
      }
    }
    
    private Label label;
    private AnimationPlayer animationPlayer;

    public override void _Ready()
    {
      base._Ready();

      label = GetNode<Label>("Label");
      animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public void FadeIn()
    {
      animationPlayer.Play("fade_in");
    }
    
    public void FadeOut()
    {
      animationPlayer.Play("fade_out");
    }
  }
}