using System.Linq;
using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.inputs.subject.refs;

[GlobalClass]
public partial class WitnessRef : SubjectRef
{
    protected override ISubject GetSubjectValue(GameEvent gameEvent)
    {
        return gameEvent.Context.Witnesses.First();
    }
}