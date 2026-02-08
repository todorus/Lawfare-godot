using Godot;
using Lawfare.scripts.logic.@event;

namespace Lawfare.scripts.logic.turn;

public partial class EndTurnHandler : Node
{
    [Signal]
    public delegate void ResolutionEventHandler(Resolution resolution);

    [Signal]
    public delegate void TurnEndedEventHandler();

    [Signal]
    public delegate void TurnStartedEventHandler();

    public void EndTurn()
    {
        Tick();
        RunEndTurnTriggers();

        EmitSignalTurnEnded();
        EmitSignalTurnStarted();
    }

    private void Tick()
    {
        // _registry.Spaces.SelectMany(space => space.Cards)
        //     .Where(cardInstance => cardInstance.Type == CardType.Temporary).ToList()
        //     .ForEach(cardInstance => cardInstance.Ticks++);
    }

    private void RunEndTurnTriggers()
    {
        // foreach (var space in _registry.Spaces)
        // {
        //     var gameEvent = new GameEvent
        //     {
        //         Type = EventType.EndTurn,
        //         Space = space,
        //     };
        //     var resolution = gameEvent.Resolve();
        //     EmitSignalResolution(resolution);
        // }
    }
}