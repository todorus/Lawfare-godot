using Godot;
using Lawfare.scripts.characters;
using Lawfare.scripts.context;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;
using Lawyer = Lawfare.scripts.characters.lawyers.Lawyer;

namespace Lawfare.scripts.interaction;

public partial class Selection : Node
{
    [Signal]
    public delegate void ActionChangedEventHandler(GodotObject action);
    [Signal]
    public delegate void SourceChangedEventHandler(GodotObject source);
    
    [Signal]
    public delegate void ResolutionEventHandler(Resolution resolution);
    
    [Signal]
    public delegate void ActionResolvedEventHandler();
    
    [Signal]
    public delegate void CanElicitChangedEventHandler(Witness[] witnesses);
    
    [Export]
    private Context _context;

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
            if (_source is Lawyer previousLawyer)
            {
                previousLawyer.CanElicitChanged -= OnCanElicitChanged;   
            }
            _source = value;
            EmitSignalSourceChanged(value as GodotObject);
            if (value is Lawyer lawyer)
            {
                lawyer.CanElicitChanged += OnCanElicitChanged;
            }
            OnCanElicitChanged();
        }
    }

    private void OnCanElicitChanged()
    {
        if(_source is not Lawyer lawyer)
        {
            EmitSignalCanElicitChanged([]);
            return;
        }
        
        EmitSignalCanElicitChanged(lawyer.CanElicit);
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
            var gameEvent = ToActionEvent(_source, subject, Action);
            if (!CanExecute(gameEvent))
            {
                // TODO feedback
                return;
            }
            
            Execute(_source, subject, Action);
            Action = null;
        }
        else if(subject is Lawyer)
        {
            Source = subject;
            Action = null;
        }
    }

    private bool CanExecute(GameEvent gameEvent)
    {
        return gameEvent.Action.Applies(gameEvent);
    }

    private GameEvent ToActionEvent(ISubject source, ISubject target, IAction action)
    {
        return new GameEvent
        {
            Type = EventType.Action,
            Source = source,
            Target = target,
            Action = action,
            Context = _context,
            Faction = source?.Allegiances?.Primary
        };
    }

    public void Execute(ISubject source, ISubject target, IAction action)
    {
        if (source == null || target == null) return;

        var actionEvent = new GameEvent
        {
            Type = EventType.Action,
            Source = source,
            Target = target,
            Action = action,
            Context = _context
        };
        var resolution = actionEvent.Resolve();
        EmitSignalResolution(resolution);
        
        var afterActionEvent = actionEvent with
        {
            Type = EventType.AfterAction,
            Resolution = resolution
        };
        var afterResolution = afterActionEvent.Resolve();
        EmitSignalResolution(afterResolution);
        EmitSignalActionResolved();
    } 
}