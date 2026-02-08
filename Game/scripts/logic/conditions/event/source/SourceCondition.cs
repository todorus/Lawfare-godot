using System.Linq;
using Godot;
using Lawfare.scripts.logic.conditions.subject;
using Lawfare.scripts.logic.@event;

namespace Lawfare.scripts.logic.conditions.@event.source;

[GlobalClass]
public partial class SourceCondition : EventCondition
{
    [Export] private SubjectCondition[] _sourceConditions = [];

    public override bool Evaluate(GameEvent context)
    {
        return _sourceConditions.All(condition => condition.Evaluate(context, context.Source));
    }
}