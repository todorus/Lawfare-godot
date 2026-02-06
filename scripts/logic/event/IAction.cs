using Lawfare.scripts.context;
using Lawfare.scripts.logic.effects;
using Lawfare.scripts.subject;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.logic.@event;

public interface IAction
{
    public Skill[] DicePools { get; }

    public bool Applies(GameEvent gameEvent);
    
    public bool CanPerform(ISubject source);
    
    public bool CanTarget(Context context, ISubject target);

    public ChangeGroup[] Stage(GameEvent gameEvent);
}