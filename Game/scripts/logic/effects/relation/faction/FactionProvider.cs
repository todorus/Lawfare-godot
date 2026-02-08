using Godot;
using Lawfare.scripts.board.factions;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects.relation.faction;

[GlobalClass]
public abstract partial class FactionProvider : Resource, IFactionProvider
{
    public abstract Faction GetFaction(GameEvent gameEvent, ISubject subject);
}