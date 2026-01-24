using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects.property.amounts;

[GlobalClass]
public partial class StaticAmount : AmountProvider
{
    [Export] public int Amount;

    protected override int Count(GameEvent gameEvent, ISubject subject)
    {
        return Amount;
    }
}