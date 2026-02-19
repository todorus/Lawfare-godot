using System.Collections.Generic;
using System.Linq;
using Godot;
using Lawfare.scripts.board.factions;
using Lawfare.scripts.@case;
using Lawfare.scripts.characters;
using Lawfare.scripts.logic.initiative;
using Lawfare.scripts.subject.quantities;
using Lawyer = Lawfare.scripts.characters.lawyers.Lawyer;
using Team = Lawfare.scripts.characters.lawyers.Team;

namespace Lawfare.scripts.context.court;

public partial class Hearing : Context
{
    [Signal]
    public delegate void WitnessChangedEventHandler(Witness witness);
    [Signal]
    public delegate void ProsecutionChangedEventHandler(Team prosecution);
    [Signal]
    public delegate void DefenseChangedEventHandler(Team defense);
    [Signal]
    public delegate void JudgesChangedEventHandler(Judge[] judges);
    [Signal]
    public delegate void TestimonyChangedEventHandler(Testimony[] testimony);
    
    [Signal]
    public delegate void InitializedEventHandler(Context context);
    
    [Signal]
    public delegate void AnyChangedEventHandler();
    
    [Export]
    private PropertyConfig _propertyConfig;
    
    [Export]
    private DocketEntry _docketEntry;
    public DocketEntry DocketEntry
    {
        get => _docketEntry;
        set
        {
            CurrentWitness = value.Witnesses[0];
            foreach (var lawyer in Lawyers)
            {
                lawyer.Quantities.Set(_propertyConfig.Charge, 0);
            }
            
            EmitSignalProsecutionChanged(value.Prosecution);
            EmitSignalDefenseChanged(value.Defense);
            EmitSignalJudgesChanged(value.Judges);
            CurrentFaction = _docketEntry.Case.ProsecutorCaseFile.Faction;
            
            var lawyersWithInitiative = Lawyers.Select(lawyer => (lawyer as IHasInitiative, lawyer.Initiative)).ToArray();
            InitiativeTrack = Initiative.Seed(lawyersWithInitiative);
            
            EmitSignalInitialized(this);
            EmitSignalAnyChanged();
        }
    }

    private Faction _currentFaction;
    public Faction CurrentFaction
    {
        get => _currentFaction;
        set
        {
            _currentFaction = value;
            UpdateTestimonies();
        }
    }
    
    public override Judge[] Judges => DocketEntry.Judges;

    public override Faction[] Factions =>
    [
        DocketEntry.Prosecution.Faction,
        DocketEntry.Defense.Faction
    ];

    public override Lawyer[] Lawyers =>
        new List<Lawyer>()
            .Concat(DocketEntry.Defense.Members)
            .Concat(DocketEntry.Prosecution.Members)
            .ToArray();
    
    public override Team[] Teams => 
    [
        DocketEntry.Prosecution,
        DocketEntry.Defense
    ];
    
    public override Witness[] Witnesses => DocketEntry.Witnesses;

    private Witness _currentWitness;
    private Witness CurrentWitness
    {
        get => _currentWitness;
        set
        {
            _currentWitness = value;
            EmitSignalWitnessChanged(CurrentWitness);
            UpdateTestimonies();
            EmitSignalAnyChanged();
        }
    }

    public override void _Ready()
    {
        base._Ready();
        DocketEntry = _docketEntry;
    }
    
    private void UpdateTestimonies()
    {
        var testimonies = DocketEntry.Case
            .GetTestimoniesByWitnessAndFaction(CurrentWitness.Definition, CurrentFaction);
        EmitSignalTestimonyChanged(testimonies);
    }
}