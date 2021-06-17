using System.Collections.Generic;

namespace battler.Scripts.AI
{
  public struct Choice
  {
    public ActionData Action { get; set; }
    public List<Battler> Targets { get; set; }
  }
}