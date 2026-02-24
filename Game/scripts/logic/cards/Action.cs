using System.Linq;
using Godot;
using Godot.Collections;
using Lawfare.scripts.logic.conditions.subject;
using Lawfare.scripts.logic.effects;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.logic.initiative;
using Lawfare.scripts.logic.inputs;
using Lawfare.scripts.subject;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.logic.cards;

[GlobalClass]
public partial class Action : Resource, IAction
{
    [Export]
    public string Label { get; private set; }
    
    [Export]
    public string Description { get; private set; }
    
    [Export]
    private int _initiativeCost = 0;
    
    [ExportGroup("Targeting")]
    [Export]
    public bool RequiresTarget { get; private set; } = true;
    
    [Export]
    private SubjectCondition[] _targetConditions = [];
    
    public Skill[] DicePools => [];
    
    [ExportGroup("Effect")]
    [Export]
    public Dictionary<InputLabel, EffectInput> Inputs { get; private set; } = new();
    
    [Export]
    public Effect[] Effects { get; private set; } = [];

    public bool CanPerform(ISubject source)
    {
        var gameEvent = new GameEvent { Source = source };
        return true; // TODO implement for real using future prequisites model
    }
    
    public bool CanTarget(GameEvent gameEvent, ISubject target)
    {
        return _targetConditions.All(condition => condition.Evaluate(gameEvent, target));
    }

    public bool Applies(GameEvent gameEvent)
    {
        return true; // TODO implement for real using future prequisites model
    }

    public ChangeGroup[] Stage(GameEvent gameEvent)
    {
        var initiativeChangeGroup = StageInitiative(gameEvent);
        var effectDiffs = Effects.SelectMany(effect => effect.Stage(gameEvent)).ToArray();
        return effectDiffs
            .Concat(initiativeChangeGroup)
            .ToArray();
    }
    
    private ChangeGroup[] StageInitiative(GameEvent gameEvent)
    {
        IDiff[] diffs = Initiative.MoveEntity(gameEvent.Context, gameEvent.Source as IHasInitiative, _initiativeCost)
            .Cast<IDiff>().ToArray();
        if(diffs.Length == 0) return [];

        return [diffs.ToChangeGroup()];
    }
}