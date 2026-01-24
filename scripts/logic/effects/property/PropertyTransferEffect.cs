using System;
using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;
using Lawfare.scripts.subject.quantities;
using AmountProvider = Lawfare.scripts.logic.effects.property.amounts.AmountProvider;

namespace Lawfare.scripts.logic.effects.property;

[GlobalClass]
public partial class PropertyTransferEffect : Effect
{
    [Export] public AmountProvider AmountProvider;

    [Export] private Property Property;

    [Export] private bool ProviderCanUseMarket = true;

    [Export] private Receiver Receiver = Receiver.Source;

    protected override IChange[] StageInternal(GameEvent gameEvent, ISubject subject)
    {
        var amount = AmountProvider.GetAmount(gameEvent, subject);
        var recipient = Receiver switch
        {
            Receiver.Source => gameEvent.Source,
            Receiver.Host => gameEvent.Host,
            Receiver.Space => gameEvent.Space,
            _ => throw new ArgumentOutOfRangeException()
        };
        var targetChange = Stage(subject, -amount, ProviderCanUseMarket);
        var sourceChange = Stage(recipient, amount, false);
        return [targetChange, sourceChange];
    }

    private IChange Stage(ISubject source, int amount, bool canUseMarket)
    {
        return new PropertyAddEffect.PropertyAddChange
        {
            Property = Property,
            Amount = amount,
            Subject = source,
            CanUseMarket = canUseMarket
        };
    }
}

public enum Receiver
{
    Source,
    Host,
    Space
}