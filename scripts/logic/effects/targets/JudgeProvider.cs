using System.Linq;
using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects.targets;

[GlobalClass]
public partial class JudgeProvider : SubjectProvider
{
    public override ISubject[] GetSubjects(GameEvent gameEvent)
    {
        return gameEvent.Context.Judges.ToArray<ISubject>();
    }
}