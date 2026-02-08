using Godot;
using Lawfare.scripts.logic.@event;

namespace Lawfare.scripts.logic.turn;

public partial class StartTurnHandler : Node
{
    [Signal]
    public delegate void AfterTurnStartedEventHandler();

    [Signal]
    public delegate void ResolutionEventHandler(Resolution resolution);

    [Signal]
    public delegate void TurnStartedEventHandler();

    public void StartTurn()
    {
        RunTriggers();
        EmitSignalTurnStarted();
        EmitSignalAfterTurnStarted();
    }

    private void RunTriggers()
    {
        // foreach (var space in _registry.Spaces)
        // {
        //     var gameEvent = new GameEvent
        //     {
        //         Type = EventType.StartTurn,
        //         Space = space
        //     };
        //     var resolution = gameEvent.Resolve();
        //     EmitSignalResolution(resolution);
        // }
    }
}