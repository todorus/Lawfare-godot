using System;
using System.Linq;

namespace Lawfare.scripts.logic.initiative.state;

public class InitiativeTrackState
{
    public InitiativeSlotState[] Slots = Array.Empty<InitiativeSlotState>();
    public int CurrentIndex { get; set; }
    public int RoundEndIndex { get; set; }
    public int TrackLength => Slots.Length;

    public InitiativeTrackState Clone()
    {
        return new InitiativeTrackState
        {
            Slots = Slots.Select(s => s.Clone()).ToArray(),
            CurrentIndex = CurrentIndex,
            RoundEndIndex = RoundEndIndex
        };
    }
}