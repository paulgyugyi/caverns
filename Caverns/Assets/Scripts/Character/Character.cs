using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Character
{
    protected Stats stats = new Stats();
    protected Stats startingStats;
    protected Inventory inventory;
    protected string name;

    public Stats Stats { get { return stats; } }
    public Stats BaseStats { get { return startingStats; } }
    public Inventory Inventory { get { return inventory; } }
    public string Name { get { return name; } set { name = value; } }

    public enum HitResult { None, Miss, Hit, Crit }

    public virtual void Init(int level)
    {
        name = "Character";
        stats.InitBaseStats(level/3, level/3, level/3);
        startingStats = new Stats(stats);
        inventory = new Inventory(stats);
        Debug.LogWarning("Initialized " + name);
    }

    public void AddBaseStat(Stats.StatType statType, int amount)
    {
        if (statType == Stats.StatType.BR ||
            statType == Stats.StatType.TC ||
            statType == Stats.StatType.GT)
        {
            int oldStat = stats.GetStat(statType);
            oldStat = Mathf.Clamp(oldStat + amount, 0, 16);
            stats.SetStat(statType, oldStat);
            stats.UpdateSecondaryStats();
            startingStats = new Stats(stats);
            inventory.Expand(stats);
        }
        else
        {
            Debug.LogWarning("AddBaseStat only works for base stats, not " + statType);
        }
    }

    public void StartBattle()
    {
        startingStats = new Stats(stats);
    }

    public void StopBattle()
    {
        stats = new Stats(startingStats);
    }

    public List<Enchantment> PerformAction(Stats.StatType action, Character enemy, out HitResult hitResult)
    {
        hitResult = HitResult.None;
        if (inventory.NeedsHit(action))
        {
            // Roll ToHit
            hitResult = RollToHit(action, enemy);
        }
        List<Enchantment> procs = inventory.GetProcs(action, hitResult, stats.GetStat(Stats.StatType.EP));
        return procs;
    }

    public string DescribeActionHit(Stats.StatType statType)
    {
        string description = "";
        List<Enchantment> procs = inventory.GetProcs(statType, HitResult.Hit, 16);
        foreach (var proc in procs)
        {
            description += proc.Describe() + "\n";
        }
        return description;
    }

    HitResult RollToHit(Stats.StatType action, Character enemy)
    {
        int hitRoll = stats.GetStat(action);
        hitRoll += Dice.Roll(2);
        //Debug.Log(name + ": rolled " + hitRoll + " vs. " + enemy.Stats.GetStat(Stats.StatType.AC));
        if (hitRoll == 8)
        {
            return HitResult.Crit;
        }
        else if (hitRoll >= enemy.Stats.GetStat(Stats.StatType.AC))
        {
            return HitResult.Hit;
        }
        else
        {
            return HitResult.Miss;
        }
    }

    public virtual Stats.StatType ChooseAction(Character enemy)
    {
        return Stats.StatType.BR;
    }

    public void ApplyStatChange(Stats.StatType statType, int change)
    {
        int oldStat = stats.GetStat(statType);
        stats.SetStat(statType, Mathf.Min(16, Mathf.Max(0, oldStat + change)));
    }

    public string ApplyProc(Enchantment proc, Character enemy)
    {
        string description = "";
        int change = proc.Multiplier * (Dice.Roll(proc.Dice) + proc.Adjustment);
        if (proc.Damage)
        {
            change *= -1;
        }
        if (proc.Self)
        {
            //Debug.Log(name + ": Buffing self " + proc.Stat + " by " + change + " (" + proc.Multiplier + " * " + proc.Dice + "D4 + " + proc.Adjustment + ")");
            ApplyStatChange(proc.Stat, change);
        }
        else
        {
            //Debug.Log(name + ": Damaging enemy " + proc.Stat + " by " + change + " (" + proc.Multiplier + " * " + proc.Dice + "D4 + " + proc.Adjustment + ")");
            enemy.ApplyStatChange(proc.Stat, change);
        }
        if (change >= 0)
        {
            description += "+";
        }
        description += change + " " + proc.Stat;
        return description;
    }

    public void ApplyProcs(List<Enchantment> procs, Character enemy)
    {
        foreach (Enchantment proc in procs)
        {
            ApplyProc(proc, enemy);
        }
    }

    public void PrintStats()
    {
        Debug.Log(name + " stats: " + stats.ListStats());
    }
}
