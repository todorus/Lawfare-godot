using System.Linq;
using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.logic.keywords;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.conditions.subject.keyword;

[GlobalClass]
public partial class DoesNotHaveKeyword : SubjectCondition
{
    [Export] private Keyword _keyword;

    public override bool Evaluate(GameEvent gameEventData, ISubject subject)
    {
        return !subject.Keywords.ToList().Contains(_keyword);
    }
}