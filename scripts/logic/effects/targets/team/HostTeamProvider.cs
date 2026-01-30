using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects.targets.team;

[GlobalClass]
public partial class HostTeamProvider : SubjectProvider
{
    public override ISubject[] GetSubjects(GameEvent gameEvent)
    {
        var host = gameEvent.Host;
        var context = gameEvent.Context;
        return [context.GetTeam(host)];
    }
}