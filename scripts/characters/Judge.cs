using System.Collections.Generic;
using Godot;
using Lawfare.scripts.board.dice;
using Lawfare.scripts.board.factions;
using Lawfare.scripts.logic.cards;
using Lawfare.scripts.logic.keywords;
using Lawfare.scripts.logic.triggers;
using Lawfare.scripts.subject;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.characters;

[GlobalClass]
public partial class Judge(JudgeDef definition) : GodotObject, ISubject, ICharacter
{
    public Quantities Quantities => new Quantities();
    public HostedTrigger[] Triggers => [];
    public KeywordBase[] Keywords => [];
    public Allegiances Allegiances => new Allegiances();
    public bool CanHaveFaction => false;
    public IEnumerable<SkillPool> Pools => [];
    public bool IsExpired { get; set; }
    public int Minimum(Property property) => property.Minimum;

    public Vector3 DamagePosition => Vector3.Zero;
    public string Label => definition.Label;
    public Texture2D Image => definition.Image;
    public Action[] Actions => [];
}