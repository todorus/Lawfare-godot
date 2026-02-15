using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects.group;

public interface IEffectGroup
{
    public bool Applies(GameEvent gameEvent);

    public ChangeGroup[] Stage(GameEvent gameEvent);
}