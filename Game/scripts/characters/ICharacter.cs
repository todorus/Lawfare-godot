using Godot;
using Lawfare.scripts.characters.lawyers;
using Lawfare.scripts.info;
using Lawfare.scripts.logic.cards;
using Lawfare.scripts.subject.quantities;
using Lawfare.scripts.subject.relations;

namespace Lawfare.scripts.characters;

public interface ICharacter : IInfo
{
    public Texture2D Image { get; }

    public Action[] Actions { get; }
    
    public Quantities Quantities { get; }
    
    public Relations Relations { get;  }
}