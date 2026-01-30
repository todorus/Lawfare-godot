using System.Linq;
using Godot;
using Lawfare.scripts.logic.conditions.subject;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.conditions.diff.subject;

[GlobalClass]
public partial class BasicSubjectDiffCondition : SubjectDiffCondition
{
    [Export]
    private SubjectCondition[] _conditions = [];
    
    protected override bool Evaluate(GameEvent gameEvent, ISubject subject)
    {
        return _conditions.All(condition => condition.Evaluate(gameEvent, subject));
    }
}