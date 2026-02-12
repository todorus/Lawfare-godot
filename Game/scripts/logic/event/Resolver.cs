using System;
using System.Linq;
using Lawfare.scripts.board.dice;
using Lawfare.scripts.dice;
using Lawfare.scripts.logic.effects;
using Lawfare.scripts.logic.initiative;
using Lawfare.scripts.subject;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.logic.@event;

public static class Resolver
{
    public static Resolution Resolve(this GameEvent gameEventData)
    {
        var diceRolls = RollDice(gameEventData);
        gameEventData = gameEventData with { DiceRolls = diceRolls };

        var triggered = gameEventData.StageTriggers();
        var actioned = gameEventData.StageAction();
        var ticked = gameEventData.StageTick();
        var staged = triggered
            .Concat(actioned)
            .Concat(ticked)
            .ToArray();

        // TODO change this to outcome modification
        // var modified = staged.Select(change => change.Modify(gameEventData)).ToArray();
        // var actual = modified.Apply();
        var changes = staged.SelectMany(changeGroup => changeGroup.Diffs).ToArray();
        var actual = changes.Apply();

        return new Resolution
        (
            actual,
            diceRolls
        );
    }

    private static DiceRoll[] RollDice(GameEvent gameEventData)
    {
        if (!gameEventData.ShouldRollDice()) return Array.Empty<DiceRoll>();
        var dicePools = gameEventData.GetDicePools(gameEventData.Faction);
        var diceRolls = dicePools.RollAll();
        return diceRolls;
    }

    private static ChangeGroup[] StageTriggers(this GameEvent gameEventData)
    {
        if (!gameEventData.ShouldRunTriggers()) return Array.Empty<ChangeGroup>();

        return gameEventData.Triggers?
                   .Where(triggerHost => triggerHost.Trigger.Applies(gameEventData with
                   {
                       Host = triggerHost.Host, Faction = triggerHost.Host.Allegiances.Primary
                   }))
                   .SelectMany(triggerHost => triggerHost.Trigger.Stage(gameEventData with
                   {
                       Host = triggerHost.Host, Faction = triggerHost.Host.Allegiances.Primary
                   }))
                   .ToArray()
               ?? Array.Empty<ChangeGroup>();
    }

    private static ChangeGroup[] StageAction(this GameEvent gameEventData)
    {
        if (!gameEventData.ShouldRunAction()) return [];

        if (gameEventData.Action == null || !gameEventData.Action.Applies(gameEventData)) return [];
        return gameEventData.Action.Stage(gameEventData with { Faction = gameEventData.Source?.Allegiances?.Primary });
    }

    private static ChangeGroup[] StageTick(this GameEvent gameEventData)
    {
        if (!gameEventData.ShouldTick()) return [];

        // Advance initiative track by 1 tick.
        var diffs = Initiative.Tick(gameEventData.Context).Cast<IDiff>().ToArray();
        return [diffs.ToChangeGroup()];
    }

    public static int Read(this ISubject subject, Property property, GameEvent gameEventData)
    {
        // TODO: Implement modifiers based on eventData
        return subject.Quantities.GetValue(property);
    }

    public static int Available(this ISubject subject, Property property, GameEvent gameEventData)
    {
        var read = subject.Read(property, gameEventData);
        return Math.Max(read - subject.Minimum(property), property.Minimum);
    }

    public static IDiff Modify(this IDiff change, GameEvent gameEventData)
    {
        // TODO: Implement modifiers based on eventData
        return change;
    }

    public static IDiff[] Apply(this IDiff[] changes)
    {
        return changes.Select(change => change.Apply()).ToArray();
    }
}