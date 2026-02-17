using System.Collections.Generic;
using System.Linq;
using Godot;
using Lawfare.scripts.board.dice;
using Lawfare.scripts.board.factions;
using Lawfare.scripts.logic.keywords;
using Lawfare.scripts.logic.triggers;
using Lawfare.scripts.subject;
using Lawfare.scripts.subject.quantities;
using Lawfare.scripts.subject.relations;

namespace Lawfare.scripts.characters.lawyers;

[GlobalClass]
public partial class Team : Resource, ISubject
{
    [Export]
    public Faction Faction;
    
    [Export] private lawyers.LawyerDef[] _lawyerDefs = [];

    private lawyers.Lawyer[] _members;
    public lawyers.Lawyer[] Members { 
        get {
            if (_members == null) 
            {
                _members = _lawyerDefs
                    .Select(def => new lawyers.Lawyer(def))
                    .ToArray();
            }
            return _members;
        } 
    }

    private Quantities _quantities = new();
    public Quantities Quantities => _quantities;
    
    private Relations _relations = new Relations();
    public Relations Relations => _relations;
    public HostedTrigger[] Triggers => [];
    public KeywordBase[] Keywords => [];
    public Allegiances Allegiances => new Allegiances(Faction != null ? new[] { Faction } : []);
    public bool CanHaveFaction => true;
    public IEnumerable<SkillPool> Pools => Members.SelectMany(m => m.Pools);
    public bool IsExpired { get; set; } = false;
    
    public int Minimum(Property property) => property.Minimum;

    public Vector3 DamagePosition { get; }
}