using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects;

public interface IEffect
{
    public IChange[] Stage(GameEvent gameEvent, ISubject subject);
}