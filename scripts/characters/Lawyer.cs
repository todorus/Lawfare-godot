using System.Collections.Generic;
using System.Linq;
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
public partial class Lawyer(LawyerDef definition) : GodotObject, ISubject, ICharacter
{
    public string Label => definition.Label;
    public Texture2D Image => definition.Image;
    
    public Action[] Actions => definition.Actions;
    
    private Quantities _quantities => new(definition.StartingQuantities as Quantity[]);
    public Quantities Quantities => _quantities;

    public HostedTrigger[] Triggers =>
        definition.Keywords
            .SelectMany(keyword => keyword.Triggers)
            .Select(trigger => new HostedTrigger { Host = this, Trigger = trigger }).ToArray();
    public KeywordBase[] Keywords => definition.Keywords.Cast<KeywordBase>().ToArray();
    
    private Allegiances _allegiances => new([definition.Faction]);
    public Allegiances Allegiances => _allegiances;
    public bool CanHaveFaction => true;
    public IEnumerable<SkillPool> Pools => definition.Skills.Select(skill => new SkillPool { Skill = skill.Skill, Dice = skill.Amount});
    public bool IsExpired { get; set; }
    public int Minimum(Property property) => property.Minimum;

    public Vector3 DamagePosition { get; }
}