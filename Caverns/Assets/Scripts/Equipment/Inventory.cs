using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Inventory
{
    public List<Equipment> allEquipment = new List<Equipment>();
    public Dictionary<Equipment, EquipmentCluster> readyEquipment = new Dictionary<Equipment, EquipmentCluster>();
    public Dictionary<Stats.StatType, EquipmentCluster> actionClusters = new Dictionary<Stats.StatType, EquipmentCluster>();

    public Inventory(Stats stats)
    {
        Init(stats);
    }

    public void Init(Stats stats)
    {
        actionClusters[Stats.StatType.BR] = new EquipmentCluster();
        actionClusters[Stats.StatType.BR].actionStat = Stats.StatType.BR;
        actionClusters[Stats.StatType.TC] = new EquipmentCluster();
        actionClusters[Stats.StatType.TC].actionStat = Stats.StatType.TC;
        actionClusters[Stats.StatType.GT] = new EquipmentCluster();
        actionClusters[Stats.StatType.GT].actionStat = Stats.StatType.GT;
        Expand(stats);
    }

    public void Expand(Stats stats)
    {
        actionClusters[Stats.StatType.BR].Init(stats.GetStat(Stats.StatType.BR) - 1);
        actionClusters[Stats.StatType.TC].Init(stats.GetStat(Stats.StatType.TC) - 1);
        actionClusters[Stats.StatType.GT].Init(stats.GetStat(Stats.StatType.GT) - 1);

    }

    public void AddEquipment(Equipment equipment, Stats.StatType statType)
    {
        AddEquipment(equipment);
        EquipEquipment(allEquipment.IndexOf(equipment), statType);
    }

    public void RemoveEquipment(Equipment equipment)
    {
        EquipmentCluster equipmentCluster = FindEquipmentCluster(equipment);
        if (equipmentCluster != null)
        {
            //Debug.Log("Removing " + equipment.Name + " from cluster " + equipmentCluster.actionStat);
            Stats.StatType statType = equipmentCluster.actionStat;
            actionClusters[statType].Remove(equipment);
            readyEquipment.Remove(equipment);
        }
        //Debug.Log("Removing " + equipment.Name);
        allEquipment.Remove(equipment);
    }

    public void AddEquipment(Equipment equipment)
    {
        allEquipment.Add(equipment);
    }

    public bool EquipEquipment(int item, Stats.StatType statType)
    {
        //Debug.Log("Equipping item " + item);
        Equipment equipment = allEquipment[item];
        if (actionClusters[statType].AutoAdd(equipment))
        {
            readyEquipment[equipment] = actionClusters[statType];
            return true;
        }
        return false;
    }

    public void AddAndAutoEquip(Equipment equipment)
    {
        AddEquipment(equipment);
        int item = allEquipment.IndexOf(equipment);
        List<Stats.StatType> affinities = equipment.GetAffinities();
        // new code
        bool equipped = false;
        foreach (Stats.StatType stat in affinities)
        {
            equipped = EquipEquipment(item, stat);
            if (equipped)
            {
                break;
            }
        }
        if (!equipped)
        {
            Debug.Log("Nowhere to equip " + equipment.Name);
        }
    }

    public void UnequipEquipment(int item)
    {
        Equipment equipment = allEquipment[item];
        EquipmentCluster equipmentCluster = FindEquipmentCluster(equipment);
        Stats.StatType statType = equipmentCluster.actionStat;
        actionClusters[statType].Remove(equipment);
        readyEquipment.Remove(equipment);
    }


    public EquipmentCluster FindEquipmentCluster(Equipment equipment)
    {
        EquipmentCluster equipmentCluster = null;
        if (readyEquipment.TryGetValue(equipment, out equipmentCluster))
        {
            return equipmentCluster;
        }
        return null;
    }

    public List<Enchantment> GetProcs(Stats.StatType statType, Character.HitResult hitResult, int energy)
    {
        List<Enchantment> procs = new List<Enchantment>();
        // get appropriate cluster
        EquipmentCluster actionCluster = actionClusters[statType];
        // iterate over all items and collect enchantment procs
        foreach (Equipment equipment in actionCluster.GetEquipment().Keys)
        {
            if (equipment.EnergyCost() > energy)
            {
                Debug.Log("Not Procing " + equipment.Name + " due to lack of energy");
                continue;
            }
            energy -= equipment.EnergyCost();
            foreach (Enchantment enchantment in equipment.GetEnchantments())
            {
                if (enchantment.MustHit && hitResult == Character.HitResult.Miss)
                {
                    continue;
                }
                procs.Add(enchantment);
            }
        }
        return procs;
    }

    public bool NeedsHit(Stats.StatType statType)
    {
        // get appropriate cluster
        EquipmentCluster actionCluster = actionClusters[statType];
        // iterate over all items and collect enchantment procs
        foreach (Equipment equipment in actionCluster.GetEquipment().Keys)
        {
            foreach (Enchantment enchantment in equipment.GetEnchantments())
            {
                if (enchantment.MustHit)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public int EnergyCost(Stats.StatType statType)
    {
        int energyCost = 0;
        // get appropriate cluster
        EquipmentCluster actionCluster = actionClusters[statType];
        // iterate over all items and collect enchantment procs
        foreach (Equipment equipment in actionCluster.GetEquipment().Keys)
        {
            energyCost += equipment.EnergyCost();
        }
        return energyCost;
    }

}
