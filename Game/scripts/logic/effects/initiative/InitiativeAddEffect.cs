using System.Linq;
using Godot;
using Lawfare.scripts.logic.effects.property.amounts;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.logic.initiative;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects.initiative;

[GlobalClass]
public partial class InitiativeAddEffect : EffectOld
{
    [Export] public AmountProvider AmountProvider;
    protected override IDiff[] StageInternal(GameEvent gameEvent, ISubject subject)
    {
        if (subject is not IHasInitiative iniativeSubject) return [];
        
        var amount = AmountProvider.GetAmount(gameEvent, subject);
        return Initiative.MoveEntity(gameEvent.Context, iniativeSubject, amount)
            .Cast<IDiff>().ToArray();
    }
}