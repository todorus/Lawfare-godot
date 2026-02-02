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

    protected override IDiff[] StageInternal(GameEvent gameEvent, ISubject subject)
    {
        var amount = AmountProvider.GetAmount(gameEvent, subject);
        if (subject?.Quantities == null)
        {
            GD.PushWarning("PropertyAddChange.Apply: Subject or Subject.Quantities is null");
            var zeroQuantity = new Quantity
            {
                Property = Property,
                Amount = 0
            };
            return [new PropertyDiff(subject, zeroQuantity, zeroQuantity)];
        }

        var original = new Quantity
        {
            Property = Property,
            Amount = subject.Quantities.Get(Property)
        };
            
        int stagedValue = subject.Quantities.StageAdd(Property, amount);
            
        var updated = new Quantity
        {
            Property = Property,
            Amount = stagedValue
        };
            
        return [new PropertyDiff(subject, original, updated)];
    }
    
    public readonly struct PropertyDiff(ISubject subject, Quantity original, Quantity updated) : IDiff<Quantity>
    {
        public ISubject Subject { get; } = subject;
        public Quantity Original { get; } = original;
        public Quantity Updated { get; } = updated;
        
        public bool CanMerge(IDiff other) => CanMerge((IDiff<Quantity>) other);
        
        public bool CanMerge(IDiff<Quantity> other)
        {
            if (other is not PropertyDiff otherDiff)
            {
                return false;
            }

            return Subject == otherDiff.Subject && Original.Property == otherDiff.Original.Property;
        }

        public IDiff<Quantity> Merge(IDiff<Quantity> other) => (IDiff<Quantity>) Merge((IDiff) other);
        
        public IDiff Merge(IDiff other)
        {
            if (other is not PropertyDiff otherDiff)
            {
                GD.PushWarning("PropertyAddDiff.Merge: other is not PropertyAddDiff");
                return this;
            }

            if (Subject != otherDiff.Subject || Original.Property != otherDiff.Original.Property)
            {
                GD.PushWarning("PropertyAddDiff.Merge: Subjects or Properties do not match");
                return this;
            }

            var mergedUpdated = new Quantity
            {
                Property = Original.Property,
                Amount = Updated.Amount + otherDiff.Updated.Amount - Original.Amount
            };

            return new PropertyDiff(Subject, Original, mergedUpdated);
        }
        
        public IDiff Apply()
        {
            if (Subject?.Quantities == null)
            {
                GD.PushWarning("PropertyAddDiff.Apply: Subject or Subject.Quantities is null");
                return null;
            }

            var difference = Updated.Amount - Original.Amount;
            var actualAmount = Subject.Quantities.Add(Original.Property, difference);
            var newUpdated = new Quantity
            {
                Property = Original.Property,
                Amount = actualAmount
            };
            return new PropertyDiff(Subject, Original, newUpdated);
        }
    }
}