using System.Collections.Generic;
using Godot;
using Lawfare.scripts.board.dice;
using Lawfare.scripts.board.factions;
using Lawfare.scripts.logic.keywords;
using Lawfare.scripts.logic.triggers;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.subject;

public interface ISubject
{
    public Quantities Quantities { get; }

    public HostedTrigger[] Triggers { get; }

    public KeywordBase[] Keywords { get; }

    public Allegiances Allegiances { get; }
    public bool CanHaveFaction { get; }
    IEnumerable<SkillPool> Pools { get; }
    bool IsExpired { get; set; }

    int Minimum(Property property);
    
    public Vector3 DamagePosition { get; }
}