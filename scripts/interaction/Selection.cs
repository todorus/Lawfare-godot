using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.interaction;

public partial class Selection : Node
{
    [Signal]
    public delegate void ActionChangedEventHandler(GodotObject action);
    [Signal]
    public delegate void SourceChangedEventHandler(GodotObject source);

    private IAction _action;

    private IAction Action
    {
        get => _action;
        set
        {
            _action = value;
            EmitSignalActionChanged(value as GodotObject);
        }
    }
    
    private ISubject _source;
    private ISubject Source
    {
        get => _source;
        set
        {
            _source = value;
            EmitSignalSourceChanged(value as GodotObject);
        }
    }

    public void Secondary()
    {
        Action = null;
    }

    public void Primary(GodotObject godotObject)
    {
        if (godotObject is IAction action)
        {
            Action = action;
            return;
        }

        if (godotObject is not ISubject subject)
        {
            return;
        }
        
        if (Action != null)
        {
            // TODO check if the action can be executed by the source on the subject
            Execute(_source, subject, Action);
            Action = null;
        }
        else
        {
            Source = subject;
        }
    }

    public void Execute(ISubject source, ISubject target, IAction action)
    {
        if (source == null || target == null) return;

        var gameEvent = new GameEvent
        {
            Type = EventType.Action,
            Source = source,
            Target = target,
            Action = action
        };
        var resolution = gameEvent.Resolve();
    } 
}