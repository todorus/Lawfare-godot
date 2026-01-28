using Lawfare.scripts.board.factions;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects.relation.faction;

public interface IFactionProvider
{
    public Faction GetFaction(GameEvent gameEvent, ISubject subject);
}