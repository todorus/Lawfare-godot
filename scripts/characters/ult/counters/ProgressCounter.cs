using System.Linq;
using Godot;
using Lawfare.scripts.logic.conditions.subject;
using Lawfare.scripts.logic.effects.property.amounts;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.characters.ult.counters;

[GlobalClass]
public partial class ProgressCounter : Resource
{
    [Export] 
    private AmountProvider _goal;

    [Export] private Property _property;
    
    [Export] 
    public SubjectCondition[] SubjectConditions { get; private set; } = [];
    
    
    public Progress GetProgress(GameEvent gameEvent)
    {
        if(!gameEvent.Subjects.Any()) return new Progress(0, 1);
        var subjects = gameEvent.Context.AllSubjects
            .Where(subject => SubjectConditions.All(condition => condition.Evaluate(gameEvent, subject)))
            .ToList();
        if(!subjects.Any()) return new Progress(0, 1);
        
        return subjects.Select(subject => GetProgress(gameEvent, subject))
            .MaxBy(progress => progress.Current);
    }
    
    private Progress GetProgress(GameEvent gameEvent, ISubject subject)
    {
        var current = subject.Read(_property, gameEvent);
        var goal = _goal.GetAmount(gameEvent, subject);
        return new Progress(current, goal);
    }
}