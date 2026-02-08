using System.Linq;
using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects.property.amounts.composite;

[GlobalClass]
public partial class Sum : AmountProvider
{
    [Export] private AmountProvider[] _amountProviders = [];

    protected override int Count(GameEvent gameEvent, ISubject subject)
    {
        if (_amountProviders == null || _amountProviders.Length == 0) return 0;

        return _amountProviders.Sum(provider => provider.GetAmount(gameEvent, subject));
    }
}