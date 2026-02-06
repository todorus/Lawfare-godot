using System;
using System.Linq;
using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.logic.targets;
using Lawfare.scripts.subject;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.logic.effects.property.amounts.property;

[GlobalClass]
public partial class DifferenceAmount : AmountProvider
{
    [Export] public int Amount;

    [Export] public int Max = 100;

    [ExportCategory("Range")] [Export] public int Min;

    [Export] public Property Property;

    [Export] public TargetSelector Selector;

    protected override int Count(GameEvent gameEvent, ISubject subject)
    {
        if (Selector == null) return 0;
        var total = Selector
            .Select(gameEvent)
            .Sum(subject => subject?.Quantities?.GetValue(Property) ?? 0);

        var difference = Amount - total;
        return Math.Clamp(difference, Min, Max);
    }
}