using System;
using System.Collections.Generic;
using System.Linq;
using Lawfare.scripts.board.dice;
using Lawfare.scripts.board.factions;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.dice;

public class DicePool
{
    private readonly Faction _faction;
    private readonly Dictionary<Skill, SkillPool> _pools = new();

    public DicePool(Faction faction, SkillPool[] skillPools)
    {
        _faction = faction;
        foreach (var pool in skillPools) AddPool(pool);
    }

    public DiceRoll[] RollAll()
    {
        return _pools.Values
            .Select(pool => Roll(pool.Skill))
            .ToArray();
    }

    public DiceRoll Roll(Skill skill)
    {
        var pool = GetPool(skill);
        var results = new int[pool.Dice]
            .Select(dice => Random.Shared.Next(1, 6))
            .ToArray();
        return new DiceRoll(_faction, skill, results, pool.Successes);
    }

    private void AddPool(SkillPool pool)
    {
        var existingPool = GetPool(pool.Skill);
        existingPool.Dice += pool.Dice;
        existingPool.Successes += pool.Successes;
        _pools[pool.Skill] = existingPool;
    }

    private SkillPool GetPool(Skill skill)
    {
        if (!_pools.ContainsKey(skill))
            _pools[skill] = new SkillPool
            {
                Skill = skill,
                Dice = 0,
                Successes = 0
            };
        return _pools[skill];
    }
}