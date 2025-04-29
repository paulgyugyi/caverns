using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class PlayerCharacter : Character
{
    bool initialized = false;

    public override void Init(int level)
    {
        if (initialized)
        {
            Debug.LogWarning("Already intialized!");
        }
        initialized = true;
        name = "Player";
        stats.InitBaseStats(level / 3, level / 3, level / 3);
        startingStats = new Stats(stats);
        inventory = new Inventory(stats);
        AddStartingPlayerEquipment();
        //Debug.LogWarning("Initialized " + name);
    }

    public override Stats.StatType ChooseAction(Character enemy)
    {
        int choice = Random.Range(0, 5);
        Stats.StatType action = Stats.StatType.BR;
        switch (choice)
        {
            case 0:
                action = Stats.StatType.BR;
                break;
            case 1:
            case 2:
                if (stats.GetStat(Stats.StatType.EP) > 6)
                {
                    action = Stats.StatType.TC;
                }
                else
                {
                    action = Stats.StatType.BR;
                }
                break;
            case 3:
            case 4:
                if (enemy.Stats.GetStat(Stats.StatType.AC) > 8)
                {
                    action = Stats.StatType.GT;
                }
                else
                {
                    action = Stats.StatType.BR;
                }
                break;
        }
        return action;
    }

    public void AddStartingPlayerEquipment()
    {
    }


}
