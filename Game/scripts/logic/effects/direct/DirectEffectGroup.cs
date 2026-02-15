using System.Linq;
using Godot;
using Lawfare.scripts.logic.effects.group;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.logic.inputs.amount.refs;
using Lawfare.scripts.logic.inputs.subject.refs;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects.direct;

[GlobalClass]
public partial class DirectEffectGroup : EffectGroup
{
    [Export]
    private SubjectRef _target;
    
    [Export]
    private AmountRef _amount;
    
    [Export]
    private DirectEffect[] _effects;
    
    
    public override ChangeGroup[] Stage(GameEvent gameEvent)
    {
        var target = _target.GetValue(gameEvent) as ISubject;
        var amount = _amount.GetValue(gameEvent) as int? ?? 0;
        var changeGroup = _effects
            .SelectMany(effect => effect.Stage(target, amount))
            .ToArray()
            .ToChangeGroup();
        return [changeGroup];
    }
}