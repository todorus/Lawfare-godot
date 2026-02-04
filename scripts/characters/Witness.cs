using System.Collections.Generic;
using Godot;
using Lawfare.scripts.board.dice;
using Lawfare.scripts.board.factions;
using Lawfare.scripts.characters.lawyers;
using Lawfare.scripts.logic.cards;
using Lawfare.scripts.logic.keywords;
using Lawfare.scripts.logic.triggers;
using Lawfare.scripts.subject;
using Lawfare.scripts.subject.quantities;
using Lawfare.scripts.subject.relations;
using Ult = Lawfare.scripts.characters.ult.Ult;

namespace Lawfare.scripts.characters;

[GlobalClass]
public partial class Witness(WitnessDef definition) : GodotObject, ISubject, ICharacter
{
    public readonly WitnessDef Definition = definition;
    
    public Quantities Quantities { get; } = new();
    public Relations Relations { get; } = new();
    
    public HostedTrigger[] Triggers => [];
    public KeywordBase[] Keywords => [];
    public Allegiances Allegiances => new();
    public bool CanHaveFaction => false;
    public IEnumerable<SkillPool> Pools => [];
    public bool IsExpired { get; set; }
    public int Minimum(Property property) => property.Minimum;

    public Vector3 DamagePosition => Vector3.Zero;
    public string Label => Definition.Label;
    public Texture2D Image => Definition.Image;
    public Action[] Actions => [];
    
    public Ult Ult { get; } = null;
}