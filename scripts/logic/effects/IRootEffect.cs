using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects;

public interface IRootEffect
{
    public bool Applies(GameEvent gameEvent, ISubject root);

    public ChangeGroup[] Stage(GameEvent gameEvent, ISubject root);
}