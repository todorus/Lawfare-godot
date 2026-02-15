using Lawfare.scripts.context;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.inputs.subject;

public partial class TargetRef : SubjectRef
{
    protected override ISubject GetSubjectValue(Context context, GameEvent gameEvent)
    {
        return gameEvent.Source;
    }
}