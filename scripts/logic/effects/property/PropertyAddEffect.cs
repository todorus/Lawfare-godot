using System.Collections.Generic;
using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.logic.modifiers;
using Lawfare.scripts.subject;
using Lawfare.scripts.subject.quantities;
using AmountProvider = Lawfare.scripts.logic.effects.property.amounts.AmountProvider;

namespace Lawfare.scripts.logic.effects.property;

[GlobalClass]
public partial class PropertyAddEffect : PropertyEffect
{
    [Export] public AmountProvider AmountProvider;

    protected override IChange[] StageInternal(GameEvent gameEvent, ISubject subject)
    {
        var amount = AmountProvider.GetAmount(gameEvent, subject);
        return
        [
            new PropertyAddChange
            (
                gameEvent.Space,
                Property,
                amount,
                subject
            )
        ];
    }

    public struct PropertyAddChange : IChange
    {
        public ISubject Space { get; }

        public Property Property;
        public int Amount;
        public ISubject Subject;
        public bool CanUseMarket = true;

        public PropertyAddChange(
            ISubject space,
            Property property,
            int amount,
            ISubject subject,
            bool canUseMarket = true
        )
        {
            Space = space;
            Property = property;
            Amount = amount;
            Subject = subject;
            CanUseMarket = canUseMarket;
        }

        public IReadOnlyList<IModification> Modifications => new List<IModification>();

        public IChange Apply()
        {
            if (Subject?.Quantities == null)
            {
                GD.PushWarning("PropertyAddChange.Apply: Subject or Subject.Quantities is null");
                return this;
            }

            var actualChange = Subject.Quantities.Add(Property, Amount);
            return new PropertyAddChange
            {
                Property = Property,
                Amount = actualChange,
                Subject = Subject
            };
        }
    }
}