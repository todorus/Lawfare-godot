using Godot;
using Lawfare.scripts.info;
using Lawfare.scripts.logic.cards;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.characters;

public interface ICharacter : IInfo
{
    public Texture2D Image { get; }

    public Action[] Actions { get; }
    
    public Quantities Quantities { get; }
}