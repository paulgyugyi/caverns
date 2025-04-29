using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats
{
    public enum StatType { BR, TC, GT, AC, HP, EP, MJ }

    private Dictionary<StatType, int> statValue = new Dictionary<StatType, int>();
    private string[] statNames = new string[7] { "Brawn", "Tech", "Grit", "ArmorClass", "HitPoints", "EnergyPoints", "Mojo" };

    public Stats()
    {
        Init();
    }

    // Construtor the copies existing class
    public Stats(Stats statsToCopy)
    {
        foreach (StatType t in Enum.GetValues(typeof(StatType)))
        {
            statValue[t] = statsToCopy.GetStat(t);
        }
    }

    public void Init()
    {
        foreach (StatType t in Enum.GetValues(typeof(StatType)))
        {
            statValue[t] = 0;
        }
    }

    public int GetStat(StatType statType)
    {
        return statValue[statType];
    }

    public void SetStat(StatType type, int statVal)
    {
        statValue[type] = statVal;
    }

    public void InitBaseStats(int brawn, int tech, int grit)
    {
        SetStat(StatType.BR, brawn);
        SetStat(StatType.TC, tech);
        SetStat(StatType.GT, grit);
        UpdateSecondaryStats();
    }

    public void UpdateSecondaryStats()
    {
        SetStat(StatType.AC, GetStat(StatType.BR) + GetStat(StatType.TC));
        SetStat(StatType.HP, GetStat(StatType.BR) + GetStat(StatType.GT));
        SetStat(StatType.EP, GetStat(StatType.TC) + GetStat(StatType.GT));
        SetStat(StatType.MJ, Mathf.Max(GetStat(StatType.BR), GetStat(StatType.TC), GetStat(StatType.GT)));

    }

    public string ListStats()
    {
        string output = "";
        foreach (StatType t in Enum.GetValues(typeof(StatType)))
        {
            output += " " + t.ToString() + ":" + GetStat(t);
        }
        return output;
    }
}
