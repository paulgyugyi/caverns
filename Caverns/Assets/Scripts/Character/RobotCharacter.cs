using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotCharacter : Character
{
    public override void Init(int level)
    {
        name = "Robot";
        if (level == 13)
        {
            stats.InitBaseStats(5, 4, 4);
            inventory = new Inventory(stats);
            inventory.AddEquipment(new Equipment(Equipment.EquipmentType.RobotClaw), Stats.StatType.BR);
        }
        else if (level == 16)
        {
            stats.InitBaseStats(4, 6, 6);
            inventory = new Inventory(stats);
            inventory.AddEquipment(new Equipment(Equipment.EquipmentType.ShockWave), Stats.StatType.BR);
            inventory.AddEquipment(new Equipment(Equipment.EquipmentType.RebootCircuit), Stats.StatType.GT);
        }
        else if (level == 20)
        {
            stats.InitBaseStats(8, 7, 5);
            inventory = new Inventory(stats);
            inventory.AddEquipment(new Equipment(Equipment.EquipmentType.ShockWave), Stats.StatType.BR);
            inventory.AddEquipment(new Equipment(Equipment.EquipmentType.PlasmaTorch), Stats.StatType.TC);
            inventory.AddEquipment(new Equipment(Equipment.EquipmentType.RebootCircuit), Stats.StatType.GT);
        }
        else
        {
            stats.InitBaseStats(level / 3, level / 3,  level / 3);
            inventory = new Inventory(stats);
            inventory.AddEquipment(new Equipment(Equipment.EquipmentType.RobotClaw), Stats.StatType.BR);
        }
        startingStats = new Stats(stats);
        //Debug.LogWarning("Initialized " + name);
    }

    public override Stats.StatType ChooseAction(Character enemy)
    {
        if (stats.GetStat(Stats.StatType.HP) < 8)
        {
            //Debug.Log(name + ": Heal costs " + inventory.EnergyCost(Stats.StatType.GT));
            if ((inventory.actionClusters[Stats.StatType.GT].allEquipment.Count > 0) &&
                (stats.GetStat(Stats.StatType.EP) >= inventory.EnergyCost(Stats.StatType.GT)))
            {
                return Stats.StatType.GT;
            }
        }
        if (enemy.Stats.GetStat(Stats.StatType.AC) - stats.GetStat(Stats.StatType.BR) > 6)
        {
            if ((inventory.actionClusters[Stats.StatType.TC].allEquipment.Count > 0) &&
                (stats.GetStat(Stats.StatType.EP) >= inventory.EnergyCost(Stats.StatType.TC)))
            {
                return Stats.StatType.TC;
            }

        }
        return Stats.StatType.BR;
    }

}
