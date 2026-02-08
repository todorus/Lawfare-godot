using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects.property.amounts;

public interface IAmountProvider
{
    public int GetAmount(GameEvent gameEvent, ISubject subject);
}