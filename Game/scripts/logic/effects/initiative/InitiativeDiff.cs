using Godot;
using Lawfare.scripts.context;
using Lawfare.scripts.logic.initiative;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects.initiative;

public readonly struct InitiativeDiff(IContext context, ISubject subject, int original, int updated) : IDiff<int>
{
    public ISubject Subject { get; } = subject;
    public int Original { get; } = original;
    public int Updated { get; } = updated;

    public bool CanMerge(IDiff other) => other is InitiativeDiff o && ReferenceEquals(Subject, o.Subject);

    public bool CanMerge(IDiff<int> other) =>
        other is InitiativeDiff o && ReferenceEquals(Subject, o.Subject);

    public IDiff Merge(IDiff other) => Merge((IDiff<int>)other);

    public IDiff<int> Merge(IDiff<int> other)
    {
        if (other is not InitiativeDiff o)
        {
            GD.PushWarning("InitiativeDiff.Merge: other is not InitiativeDiff");
            return this;
        }

        if (!ReferenceEquals(Subject, o.Subject))
        {
            GD.PushWarning("InitiativeDiff.Merge: Subjects do not match");
            return this;
        }

        // Merge by applying delta of the later diff on top of this diff.
        // This supports both:
        // - o.Original == this.Original (both based on same snapshot)
        // - o.Original == this.Updated  (staged sequentially against a staged state)
        var delta = o.Updated - o.Original;
        var mergedUpdated = checked(Updated + delta);

        return new InitiativeDiff(context, Subject, Original, mergedUpdated);
    }

    public IDiff Apply()
    {
        if (Subject is not IHasInitiative entity)
        {
            GD.PushWarning("InitiativeDiff.Apply: Subject does not implement IHasInitiative");
            return this;
        }

        // Apply is absolute: set the entity to Updated.
        // This avoids ordering issues when multiple diffs apply in sequence.
        context.InitiativeTrack = Initiative.SetDelay(context.InitiativeTrack, entity, Updated);
        return this;
    }
}