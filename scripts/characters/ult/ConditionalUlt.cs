using System.Linq;
using Godot;
using Lawfare.scripts.characters.ult.counters;
using Lawfare.scripts.context;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.characters.ult;

[GlobalClass]
public partial class ConditionalUlt : Ult
{
    [Export]
    private ProgressCounter[] _counters = [];
    
    public override void Charge(int amount) {}

    private Progress _progress = new(0, 1);

    public override float Progress => _progress.Percentage;
    public override bool Active => _progress.IsComplete;
    
    public override void Update(ISubject host, IContext context)
    {
        var gameEvent = new GameEvent
        {
            Context = context,
            Host = host,
        };
        _progress = _counters
            .Select(counter => counter.GetProgress(gameEvent))
            .Combine();
        EmitOnChange();
    }
}