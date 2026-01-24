using System.Collections.Generic;
using Lawfare.scripts.logic.modifiers;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects;

public interface IChange
{
    public ISubject Space { get; }
    public IReadOnlyList<IModification> Modifications { get; }
    public IChange Apply();
}