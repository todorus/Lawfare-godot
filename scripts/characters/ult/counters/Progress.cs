using System.Collections.Generic;
using System.Linq;

namespace Lawfare.scripts.characters.ult.counters;

public readonly struct Progress(int current, int goal)
{
    public readonly int Current = current;
    public readonly int Goal = goal;
    
    public float Percentage => Goal == 0 ? 0 : (float)Current / Goal;
    public bool IsComplete => Current >= Goal;
}

public static class ProgressExtensions
{
    public static Progress Combine(this IEnumerable<Progress> progresses)
    {
        return progresses.Aggregate(
            new Progress(0,0), (progressA, progressB) => 
                new Progress(
                    progressA.Current + progressB.Current, 
                    progressA.Goal + progressB.Goal
                )
            );
    }
}