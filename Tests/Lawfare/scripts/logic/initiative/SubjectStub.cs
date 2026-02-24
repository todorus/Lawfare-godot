using System.Collections.Generic;
using Godot;
using Lawfare.scripts.board.dice;
using Lawfare.scripts.board.factions;
using Lawfare.scripts.logic.keywords;
using Lawfare.scripts.logic.triggers;
using Lawfare.scripts.subject;
using Lawfare.scripts.subject.quantities;
using Lawfare.scripts.subject.relations;

namespace Tests.Lawfare.scripts.logic.initiative;

public class SubjectStub(string label) : ISubject
{
    public string Label = label;

    public Quantities Quantities { get; set; } = null;
    public Relations Relations { get; set; } = null;
    public HostedTrigger[] Triggers { get; set; } = [];
    public KeywordBase[] Keywords { get; set; } = [];
    public Allegiances Allegiances { get; set; } = new Allegiances();
    public bool CanHaveFaction { get; set; } = true;
    public IEnumerable<SkillPool> Pools { get; set; } = [];
    public bool IsExpired { get; set; } = false;
    public bool HasActed { get; set; }
    public int Minimum(Property property) => property.Minimum;

    public Vector3 DamagePosition { get; }
}