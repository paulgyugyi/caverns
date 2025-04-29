using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment
{
    Stats.StatType stat = Stats.StatType.HP;
    Stats.StatType affinity = Stats.StatType.HP;
    bool self = false;
    bool mustHit = false;
    bool damage = true;
    int dice = 0;
    int multiplier = 1;
    int adjustment = 0;
    string name = "";
    Equipment equipment = null;
    EnchantmentType enchantmentType;

    public enum EnchantmentType
    {
        Custom,
        Heal, Damage,
        Charge, Drain,
        Block, Aim
    }

    public Stats.StatType Stat { get { return stat; } set { stat = value; } }
    public Stats.StatType Affinity { get { return affinity; } set { affinity = value; } }
    public int Dice { get { return dice; } set { dice = value; } }
    public int Multiplier { get { return multiplier; } set { multiplier = value; } }
    public int Adjustment { get { return adjustment; } set { adjustment = value; } }
    public bool Self { get { return self; } set { self = value; } }
    public bool MustHit { get { return mustHit; } set { mustHit = value; } }
    public bool Damage { get { return damage; } set { damage = value; } }
    public string Name { get { return name; } set { name = value; } }
    public Equipment Equipment { get { return equipment; } set { equipment = value; } }
    public EnchantmentType Type { get { return enchantmentType; } set { enchantmentType = value; } }


    public Enchantment(EnchantmentType enchantmentType = EnchantmentType.Custom, List<int> diceOpt = null, Equipment equipment = null)
    {
        Init(enchantmentType, diceOpt);
        this.equipment = equipment;
    }

    public int EnergyCost()
    {
        if ((stat == Stats.StatType.EP) && damage)
        {
            if (dice != 0)
            {
                Debug.LogWarning("EP enchantments must have dice == 0");
            }
            return multiplier * adjustment;
        }
        return 0;
    }
    public string Describe()
    {
        string description = "" + name;
        if (multiplier == 0)
        {
            return description;
        }
        description += ": ";
        if (mustHit)
        {
            description += "On hit: ";
        }
        if (dice > 0)
        {
            if (Mathf.Abs(multiplier) != 1)
            {
                description += multiplier + "* ( ";
            }
            if (dice > 1)
            {
                description += dice;
            }
            description += "D4 ";
        }
        if (adjustment != 0)
        {
            if (adjustment > 0)
            {
                description += "+";
            }
            description += adjustment + " ";
        }
        if (Mathf.Abs(multiplier) != 1)
        {
            description += ") ";
        }
        if (damage)
        {
            description += "damage ";
        }
        description += "to ";
        if (!self)
        {
            description += "enemy ";
        }
        description += stat;

        return description;
    }

    public void Init(EnchantmentType enchantmentType, List<int> diceOpt)
    {
        this.enchantmentType = enchantmentType;
        switch (enchantmentType)
        {
            case EnchantmentType.Custom:
                stat = Stats.StatType.HP;
                affinity = Stats.StatType.BR;
                self = false;
                mustHit = false;
                damage = true;
                dice = 0;
                multiplier = 1;
                adjustment = 0;
                name = "Custom";
                break;
            case EnchantmentType.Heal:
                stat = Stats.StatType.HP;
                affinity = Stats.StatType.GT;
                self = true;
                mustHit = false;
                damage = false;
                multiplier = diceOpt[0];
                dice = diceOpt[1];
                adjustment = diceOpt[2];
                name = "Heal";
                break;
            case EnchantmentType.Damage:
                stat = Stats.StatType.HP;
                affinity = Stats.StatType.BR;
                self = false;
                mustHit = true;
                damage = true;
                multiplier = diceOpt[0];
                dice = diceOpt[1];
                adjustment = diceOpt[2];
                name = "Damage";
                break;
            case EnchantmentType.Charge:
                stat = Stats.StatType.EP;
                affinity = Stats.StatType.TC;
                self = true;
                mustHit = false;
                damage = false;
                multiplier = diceOpt[0];
                dice = diceOpt[1];
                adjustment = diceOpt[2];
                name = "Charge";
                break;
            case EnchantmentType.Drain:
                stat = Stats.StatType.EP;
                affinity = Stats.StatType.TC;
                self = true;
                mustHit = false;
                damage = true;
                multiplier = diceOpt[0];
                dice = diceOpt[1];
                adjustment = diceOpt[2];
                name = "Drain";
                break;
            case EnchantmentType.Block:
                stat = Stats.StatType.AC;
                affinity = Stats.StatType.TC;
                self = true;
                mustHit = false;
                damage = false;
                multiplier = diceOpt[0];
                dice = diceOpt[1];
                adjustment = diceOpt[2];
                name = "Block";
                break;
            case EnchantmentType.Aim:
                stat = Stats.StatType.AC;
                affinity = Stats.StatType.GT;
                self = false;
                mustHit = false;
                damage = true;
                multiplier = diceOpt[0];
                dice = diceOpt[1];
                adjustment = diceOpt[2];
                name = "Track";
                break;
        }
    }
}
