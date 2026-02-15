using Godot;
using Lawfare.scripts.subject;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.logic.effects.direct.property;

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