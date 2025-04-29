using System.Collections;
using System.Collections.Generic;
using static Enchantment;

public class Equipment
{
    int size;
    string name;
    bool needsEnergy;
    Cluster shape;
    EquipmentType equipmentType;

    public enum EquipmentType
    {
        Custom, MedKit, InformationCrystal, VictoryMedal, Supplies, Spikes, LaserSword,
        EnergyShield, Insight, TeleportBeacon, PlasmaRifle,
        RobotClaw, ShockWave, RebootCircuit, PlasmaTorch
    }

    public int Size { get { return size; } set { size = value; shape.Init(size); } }
    public Cluster Shape { get { return shape; } set { shape = value; } }
    public string Name { get { return name; } set { name = value; } }
    public bool NeedsEnergy { get { return needsEnergy; } set { needsEnergy = value; } }
    public EquipmentType Type { get { return equipmentType; } set { equipmentType = value; } }

    List<Enchantment> enchantments = new List<Enchantment>();

    public Equipment(EquipmentType equipmentType = EquipmentType.Custom)
    {
        Init(equipmentType);
    }

    public void AddEnchantment(Enchantment enchantment)
    {
        enchantments.Add(enchantment);
    }

    public List<Enchantment> GetEnchantments()
    {
        return enchantments;
    }

    public string DescribeEnchantments()
    {
        string description = "";
        foreach (var enchantment in enchantments)
        {
            description += enchantment.Describe() + "\n";
        }
        return description;
    }

    public int EnergyCost()
    {
        int energyCost = 0;
        foreach (Enchantment enchantment in enchantments)
        {
            energyCost += enchantment.EnergyCost();
        }
        return energyCost;
    }

    public List<Stats.StatType> GetAffinities()
    {
        List<Stats.StatType> affinities = new List<Stats.StatType>();
        foreach (var enchantment in enchantments)
        {
            if (enchantment.Multiplier == 0)
            {
                // Just a description for UI.
                continue;
            }
            if (!affinities.Contains(enchantment.Affinity)) {
                affinities.Add(enchantment.Affinity);
            }
        }
        return affinities;

    }

    public void Init(EquipmentType equipmentType)
    {
        this.equipmentType = equipmentType;
        switch (equipmentType)
        {
            case EquipmentType.Custom:
                size = 1;
                name = "item";
                needsEnergy = false;
                shape = new Cluster(size);
                break;
            case EquipmentType.MedKit:
                size = 1;
                name = "Med Kit";
                needsEnergy = false;
                shape = new Cluster(size);
                AddEnchantment(new Enchantment(EnchantmentType.Heal, new List<int> { 1, 0, 2 }, this));
                break;
            case EquipmentType.InformationCrystal:
                size = 1;
                name = "Information Crystal";
                needsEnergy = false;
                shape = new Cluster(size);
                break;
            case EquipmentType.VictoryMedal:
                size = 1;
                name = "Victory Medal";
                needsEnergy = false;
                shape = new Cluster(size);
                break;
            case EquipmentType.Supplies:
                size = 1;
                name = "5 Supplies";
                needsEnergy = false;
                shape = new Cluster(size);
                break;
            case EquipmentType.Spikes:
                size = 1;
                name = "Spikes";
                needsEnergy = false;
                shape = new Cluster(size);
                AddEnchantment(new Enchantment(EnchantmentType.Damage, new List<int> { 1, 0, 2 }, this));
                Enchantment enemyDrain = new Enchantment(EnchantmentType.Drain, new List<int> { 1, 0, 1 }, this);
                enemyDrain.Self = false;
                enemyDrain.MustHit = true;
                AddEnchantment(enemyDrain);
                break;
            case EquipmentType.LaserSword:
                size = 2;
                name = "Laser Sword";
                needsEnergy = false;
                shape = new Cluster(size);
                AddEnchantment(new Enchantment(EnchantmentType.Damage, new List<int> { 1, 2, 0 }, this));
                break;
            case EquipmentType.EnergyShield:
                size = 3;
                name = "Energy Shield";
                needsEnergy = true;
                shape = new Cluster(size);
                AddEnchantment(new Enchantment(EnchantmentType.Block, new List<int> { 1, 0, 3 }, this));
                AddEnchantment(new Enchantment(EnchantmentType.Drain, new List<int> { 1, 0, 4 }, this));
                break;
            case EquipmentType.Insight:
                size = 1;
                name = "Insight";
                needsEnergy = false;
                shape = new Cluster(size);
                AddEnchantment(new Enchantment(EnchantmentType.Aim, new List<int> { 1, 0, 2 }, this));
                break;
            case EquipmentType.TeleportBeacon:
                size = 1;
                name = "Teleport Beacon";
                needsEnergy = false;
                shape = new Cluster(size);
                Enchantment description = new Enchantment(EnchantmentType.Custom, new List<int> { 0, 0, 0 }, this);
                description.Name = "Allows teleporting back to Base";
                description.Multiplier = 0;
                AddEnchantment(description);
                break;
            case EquipmentType.PlasmaRifle:
                size = 3;
                name = "Plasma Rifle";
                needsEnergy = true;
                shape = new Cluster(1);
                shape.validHexes[1] = true;
                shape.validHexes[4] = true;
                AddEnchantment(new Enchantment(EnchantmentType.Damage, new List<int> { 1, 3, 0 }, this));
                AddEnchantment(new Enchantment(EnchantmentType.Drain, new List<int> { 1, 0, 1 }));
                break;
            case EquipmentType.RobotClaw:
                size = 1;
                name = "Robot Claw";
                needsEnergy = false;
                shape = new Cluster(size);
                AddEnchantment(new Enchantment(EnchantmentType.Damage, new List<int> { 1, 1, 0 }, this));
                break;
            case EquipmentType.ShockWave:
                size = 1;
                name = "Shockwave";
                needsEnergy = false;
                shape = new Cluster(size);
                AddEnchantment(new Enchantment(EnchantmentType.Damage, new List<int> { 1, 1, 2 }, this));
                break;
            case EquipmentType.RebootCircuit:
                size = 1;
                name = "Reboot Circuit";
                needsEnergy = true;
                shape = new Cluster(size);
                AddEnchantment(new Enchantment(EnchantmentType.Heal, new List<int> { 1, 3, 0 }, this));
                AddEnchantment(new Enchantment(EnchantmentType.Drain, new List<int> { 1, 0, 7 }, this));
                break;
            case EquipmentType.PlasmaTorch:
                size = 3;
                name = "Plasma Torch";
                needsEnergy = true;
                shape = new Cluster(size);
                AddEnchantment(new Enchantment(EnchantmentType.Aim, new List<int> { 1, 0, 2 }, this));
                AddEnchantment(new Enchantment(EnchantmentType.Drain, new List<int> { 1, 0, 2 }, this));
                break;

        }
    }


}
