using System.Linq;
using Godot;
using Lawfare.scripts.@case;
using Lawfare.scripts.context;
using Lawfare.scripts.logic.cards;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.logic.initiative;
using Lawfare.scripts.subject;
using Lawfare.scripts.subject.quantities;
using Lawyer = Lawfare.scripts.characters.lawyers.Lawyer;

namespace Lawfare.scripts.interaction;

public partial class Selection : Node
{
    [Signal]
    public delegate void TargetEventChangedEventHandler(GameEventDto gameEvent);
    [Signal]
    public delegate void SourceChangedEventHandler(GodotObject source);
    
    [Signal]
    public delegate void ResolutionEventHandler(Resolution resolution);
    
    [Signal]
    public delegate void ActionResolvedEventHandler(Context context);
    
    [Signal]
    public delegate void TickResolvedEventHandler(Context context);
    
    [Signal]
    public delegate void HandChangedEventHandler(Card[] hand);
    
    [Signal]
    public delegate void QuantitiesChangedEventHandler(Quantities quantities);
    
    [Export]
    private Context _context;
    
    [Export]
    private PropertyConfig _propertyConfig;

    private Card _action;

    private Card Action
    {
        get => _action;
        set
        {
            _action = value;
            var gameEvent = ToActionEvent(_source, null, value);
            EmitSignalTargetEventChanged(new GameEventDto(gameEvent));
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
            if (value is Lawyer lawyer)
            {
                EmitSignalQuantitiesChanged(lawyer.Quantities);
                EmitSignalHandChanged(lawyer.Actions.Select(action => new Card(action)).ToArray());
            }
            else
            {
                EmitSignalQuantitiesChanged(null);
                EmitSignalHandChanged([]);
            }
        }
    }
    
    public void SetSourceLawyer(Lawyer lawyer)
    {
        Source = lawyer;
    }

    public void SetTestimony(Testimony testimony)
    {
        if(Source is not Lawyer lawyer) return;
        var charge = lawyer.Quantities.GetValue(_propertyConfig.Charge);
        if(charge < _propertyConfig.Charge.Maximum) return;

        GD.Print(lawyer.Ultimate.Label);

    }

    public void Secondary()
    {
        Action = null;
    }

    public void Primary(GodotObject godotObject)
    {
        if (godotObject is Card action)
        {
            Action = action;
            if (!action.RequiresTarget)
            {
                TryExecute(null);
            }
            return;
        }

        if (godotObject is not ISubject subject)
        {
            return;
        }
        
        if (Action != null)
        {
            TryExecute(subject);
        }
        else if(subject is Lawyer)
        {
            Source = subject;
            Action = null;
        }
    }

    private void TryExecute(ISubject subject)
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
        if (source == null) return;

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
        EmitSignalActionResolved(_context);

        Tick(actionEvent);
        while (Initiative.GetCurrent(_context.InitiativeTrack) == null)
        {
           Tick(actionEvent);
        }
        _context.UpdateCurrent();
        EmitSignalTickResolved(_context);

        if (source != Initiative.GetCurrent(_context.InitiativeTrack))
        {
            var startTurnEvent = actionEvent with
            {
                Type = EventType.StartTurn,
                Action = null,
            };
            var startTurnResolution = startTurnEvent.Resolve();
            EmitSignalResolution(startTurnResolution);   
        }
    }

    private void Tick(GameEvent actionEvent)
    {
        var tickEvent = actionEvent with
        {
            Type = EventType.Tick,
            Action = null,
        };

        tickEvent.Resolve();
    }
}