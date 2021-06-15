using System.Collections.Generic;
using battler.Scripts.Enum;

namespace battler.Scripts
{
  public abstract class Types
  {
    public static Dictionary<Elements, Elements> WeaknessMapping { get; } = new Dictionary<Elements, Elements>
    {
      { Elements.None, Elements.Nothing },
      { Elements.Code, Elements.Art },
      { Elements.Art, Elements.Design },
      { Elements.Design, Elements.Code },
      { Elements.Bug, Elements.Nothing },
    };
  }
}