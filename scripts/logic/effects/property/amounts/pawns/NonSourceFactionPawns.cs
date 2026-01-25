using System.Linq;
using Godot;
using Lawfare.scripts.characters;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects.property.amounts.pawns;

[GlobalClass]
public partial class NonSourceFactionPawns : AmountProvider
{
    protected override int Count(GameEvent gameEvent, ISubject subject)
    {
        return gameEvent.Subjects
            .Where(subject => subject is ICharacter)
            .Count(pawn => !pawn.Allegiances.Contains(gameEvent.Faction));
    }
}