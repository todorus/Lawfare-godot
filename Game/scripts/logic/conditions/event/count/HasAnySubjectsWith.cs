using System.Linq;
using Godot;
using Lawfare.scripts.logic.conditions.subject;
using Lawfare.scripts.logic.@event;

namespace Lawfare.scripts.logic.conditions.@event.count;

[GlobalClass]
public partial class HasAnySubjectsWith : EventCondition
{
    [Export] public SubjectCondition[] SubjectConditions = [];

    public override bool Evaluate(GameEvent context)
    {
        var count = context.Subjects
            .Count(subject => SubjectConditions.All(condition => condition.Evaluate(context, subject)));
        return count > 0;
    }
}