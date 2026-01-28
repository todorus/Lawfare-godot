using Godot;
using Lawfare.scripts.board.factions;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects.relation.faction;

[GlobalClass]
public partial class OpposingFactionProvider : FactionProvider
{
    public override Faction GetFaction(GameEvent gameEvent, ISubject subject)
    {
        throw new System.NotImplementedException();
    }
}