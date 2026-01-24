using Lawfare.scripts.logic.effects;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.logic.@event;

public interface IAction
{
    public Skill[] DicePools { get; }

    public bool Applies(GameEvent gameEvent);

    public ChangeGroup[] Stage(GameEvent gameEvent);
}