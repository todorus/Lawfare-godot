using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.inputs.subject.refs;

[GlobalClass]
public abstract partial class SubjectRef : ValueRef
{
    public override object GetValue(GameEvent gameEvent) => GetSubjectValue(gameEvent);

    protected abstract ISubject GetSubjectValue(GameEvent gameEvent);
}