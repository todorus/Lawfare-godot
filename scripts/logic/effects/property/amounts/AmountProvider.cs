using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects.property.amounts;

[GlobalClass]
public abstract partial class AmountProvider : Resource, IAmountProvider
{
    [Export] private int Multiplier = 1;

    [Export] private int Offset;

    public int GetAmount(GameEvent gameEvent, ISubject subject)
    {
        var count = Count(gameEvent, subject);
        return Offset + Multiplier * count;
    }

    protected abstract int Count(GameEvent gameEvent, ISubject subject);
}