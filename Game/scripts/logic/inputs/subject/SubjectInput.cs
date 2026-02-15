using Lawfare.scripts.context;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.inputs.subject;

public abstract partial class SubjectInput : Input
{
    public override object GetValue(Context context, GameEvent gameEvent) => GetSubjectValue(context, gameEvent);

    protected abstract ISubject GetSubjectValue(Context context, GameEvent gameEvent);
}