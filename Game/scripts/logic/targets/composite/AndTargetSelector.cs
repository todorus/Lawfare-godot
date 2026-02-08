using System.Collections.Generic;
using System.Linq;
using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.targets.composite;

[GlobalClass]
public partial class AndTargetSelector : TargetSelector
{
    [Export] private TargetSelector[] _selectors;

    public override ISubject[] Select(GameEvent gameEvent)
    {
        HashSet<ISubject> resultSet = null;

        foreach (var selector in _selectors)
        {
            var selectedTargets = selector.Select(gameEvent);
            if (resultSet == null)
                resultSet = new HashSet<ISubject>(selectedTargets);
            else
                resultSet.IntersectWith(selectedTargets);
        }

        return resultSet?.ToArray() ?? [];
    }
}