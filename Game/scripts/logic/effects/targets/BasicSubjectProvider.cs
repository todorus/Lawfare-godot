using System.Linq;
using Godot;
using Lawfare.scripts.logic.conditions.subject;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects.targets;

[GlobalClass]
public partial class BasicSubjectProvider : SubjectProvider
{
    [Export]
    public SubjectCondition[] SubjectsConditions = [];
    
    public override ISubject[] GetSubjects(GameEvent gameEvent)
    {
        return gameEvent.Context.AllSubjects
            .Where(subject => SubjectsConditions.All(condition => condition.Evaluate(gameEvent, subject)))
            .ToArray();
    }
    
}