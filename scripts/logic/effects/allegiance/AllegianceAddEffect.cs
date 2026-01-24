using System.Collections.Generic;
using Godot;
using Lawfare.scripts.board.factions;
using Lawfare.scripts.logic.effects.allegiance.factions;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.logic.modifiers;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects.allegiance;

[GlobalClass]
public partial class AllegianceAddEffect : Effect
{
    [Export] private FactionProvider _factionProvider;

    protected override IChange[] StageInternal(GameEvent context, ISubject subject)
    {
        return
        [
            new AllegianceAddChange
            (
                space: context.Space,
                faction: _factionProvider.GetFaction(context),
                subject: subject
            )
        ];
    }

    public struct AllegianceAddChange(
        ISubject space,
        ISubject subject,
        Faction faction)
        : IChange
    {
        public ISubject Space => space;
        public readonly ISubject Subject = subject;
        public readonly Faction Faction = faction;

        public IReadOnlyList<IModification> Modifications => new List<IModification>();

        public IChange Apply()
        {
            if (Subject == null)
            {
                GD.PushWarning("AllegianceAddChange.Apply: Subject is null");
                return this;
            }

            Subject.Allegiances.Add(Faction);
            return this;
        }
    }
}