using System.Linq;
using Godot;
using Lawfare.scripts.board.factions;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects.relation.faction;

[GlobalClass]
public partial class SourceTeamFactionProvider : FactionProvider
{
    public override Faction GetFaction(GameEvent gameEvent, ISubject subject)
    {
        var sourceFaction = gameEvent.Context.GetTeam(gameEvent.Source)?.Faction;
        return gameEvent.Context
            .Factions.FirstOrDefault(faction => faction == sourceFaction);
    }
}