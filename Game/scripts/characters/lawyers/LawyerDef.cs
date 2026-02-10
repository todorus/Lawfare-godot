using Godot;
using Lawfare.scripts.board.factions;
using Lawfare.scripts.logic.cards;
using Lawfare.scripts.logic.keywords;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.characters.lawyers;

[GlobalClass]
public partial class LawyerDef : Resource
{
    [ExportCategory("Presentation")]
    [Export]
    public string Label { get; private set; }
    
    [Export]
    public Texture2D Image { get; private set; }
    
    [Export]
    public Texture2D Portrait { get; private set; }
    
    [ExportCategory("Gameplay")]
    [Export] 
    public Faction Faction { get; private set; }
    
    [Export]
    public int Initiative { get; private set; }
    
    [Export]
    public Action[] Actions { get; private set; } = [];
    
    [Export]
    public Quantity[] Quantities { get; private set; } = [];
    
    [Export]
    public SkillQuantity[] Skills { get; private set; } = [];
    
    [Export]
    public Keyword[] Keywords { get; private set; } = [];
}