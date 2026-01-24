using System.Linq;
using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;
using SubjectCondition = Lawfare.scripts.logic.conditions.subject.SubjectCondition;

namespace Lawfare.scripts.logic.targets;

[GlobalClass]
public partial class GenericTargetSelector : TargetSelector
{
    [Export] private SubjectCondition[] _conditions;

    public override ISubject[] Select(GameEvent gameEvent)
    {
        return gameEvent.Subjects
            .Where(subject => _conditions.All(condition => condition.Evaluate(gameEvent, subject)))
            .ToArray();
    }
}