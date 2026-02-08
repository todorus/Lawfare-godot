using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects.targets.team;

[GlobalClass]
public partial class SourceTeamProvider : SubjectProvider
{
    public override ISubject[] GetSubjects(GameEvent gameEvent)
    {
        var source = gameEvent.Source;
        var context = gameEvent.Context;
        return [context.GetTeam(source)];
    }
}