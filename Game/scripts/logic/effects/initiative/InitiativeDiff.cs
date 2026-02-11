using System.Linq;
using Lawfare.scripts.context;
using Lawfare.scripts.logic.initiative.state;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects.initiative;

public readonly struct InitiativeDiff(IContext context, InitiativeTrackState original, InitiativeTrackState updated)
    : IDiff<InitiativeTrackState>
{
    // Your IDiff requires Subject, but tick targets context.
    // Pragmatic: pick a stable subject as "host" (or add a ContextSubject later).
    public ISubject Subject => context.Judges?.FirstOrDefault() ?? context.AllSubjects.First();

    public InitiativeTrackState Original { get; } = original;
    public InitiativeTrackState Updated { get; } = updated;

    public bool CanMerge(IDiff other) => false;
    public bool CanMerge(IDiff<InitiativeTrackState> other) => false;
    public IDiff Merge(IDiff other) => this;
    public IDiff<InitiativeTrackState> Merge(IDiff<InitiativeTrackState> other) => this;

    public IDiff Apply()
    {
        context.InitiativeTrack = Updated.Clone();
        return this;
    }
}