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
using Lawfare.scripts.subject.relations;

namespace Lawfare.scripts.characters.lawyers;

[GlobalClass]
public partial class Lawyer(LawyerDef definition) : GodotObject, ISubject, ICharacter
{
    [Signal]
    public delegate void CanElicitChangedEventHandler();
    
    public string Label => definition.Label;
    public Texture2D Image => definition.Image;
    
    public Action[] Actions => definition.Actions;
    
    public ElicitStatement[] ElicitStatementRequirements =>
        definition?.Goal?.ElicitStatementRequirements ?? [];
    
    public Quantities Quantities { get; } = new(definition.Quantities);
    public Relations Relations { get; } = new Relations();

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
    
    private Witness[] _canElicit = [];

    public Witness[] CanElicit
    {
        get => _canElicit;
        set
        {
            _canElicit = value;
            EmitSignalCanElicitChanged();
        }
    }
}