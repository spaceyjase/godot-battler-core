using Godot;

namespace battler.Scripts
{
  public class BattlerUIData : Resource
  {
    [Export] private string displayName;
    [Export] private Texture texture;

    public string DisplayName => displayName;
    public Texture Texture => texture;
  }
}
