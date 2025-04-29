using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    protected string name = "";
    protected List<Encounter> encounters = new List<Encounter>();
    protected List<Equipment> items = new List<Equipment>();
    protected List<Equipment> rewards = new List<Equipment>();
    protected string npcSetup = "";
    protected string npcResolve = "";
    protected List<Encounter> encountersOrig = new List<Encounter>();
    protected List<Equipment> itemsOrig = new List<Equipment>();
    protected List<Equipment> rewardsOrig = new List<Equipment>();

    public string Name { get { return name; } set {  name = value; } }
    public List<Encounter> Encounters { get { return encounters; } set { encounters = value; } }
    public List<Equipment> Items { get {  return items; } set { items = value; } }
    public List<Equipment> ItemsOrig { get { return itemsOrig; } set { itemsOrig = value; } }
    public List<Equipment> Rewards { get {  return rewards; } set {  rewards = value; } }
    public string NpcSetup { get { return npcSetup; } }
    public string NpcResolve { get { return npcResolve; } }

    public Quest(int questNumber)
    {
        switch (questNumber)
        {
            case 0:
                name = "Get a mission from H.Q.";
                npcSetup = "Go to Titan and meet Commander Cassini at the base";
                npcResolve = "Welcome to Titan H.Q.\nI'm Commander Cassini. You must be new here.\n\nIf you are looking for work, I have a mission available.\n\nHere, you will need this LASERSWORD, ENERGYSHIELD, and INSIGHT module.";
                rewards.Add(new Equipment(Equipment.EquipmentType.Insight));
                rewards.Add(new Equipment(Equipment.EquipmentType.EnergyShield));
                rewards.Add(new Equipment(Equipment.EquipmentType.LaserSword));
                break;
            case 1:
                name = "Retrieve Scientific Research";
                encounters.Add(new Encounter(Encounter.EncounterType.ResearchTower));
                items.Add(new Equipment(Equipment.EquipmentType.InformationCrystal));
                rewards.Add(new Equipment(Equipment.EquipmentType.VictoryMedal));
                npcSetup = "We need someone to go down into the caverns, find the abandoned RESEARCH TOWER, and bring back an INFORMATION CRYSTAL.\n\nYou should use the INVENTORY button to assign your equipment before you ENTER THE CAVE.";
                npcResolve = "Welcome back. I see you found the INFORMATION CRYSTAL.\nHere, take this VICTORY MEDAL.";
                break;
            case 2:
            default:
                name = "You Win! ESC to Exit";
                npcSetup = "Congratulations! You have finished all the missions I have right now.\n\nYou can use the ESC key to return to the main menu.";
                npcResolve = "";
                break;
        }
        encountersOrig = new List<Encounter>(encounters);
        itemsOrig = new List<Equipment>(items);
        rewardsOrig = new List<Equipment>(rewards);
    }

    public string DescribeQuest()
    {
        string description = "" + Name + "\n";
        if (encountersOrig.Count > 0)
        {
            if (encounters.Count > 0)
            {
                description += "__ ";
            }
            else
            {
                description += "\U0001f600 ";
            }
            description += "Find the ";
            foreach (Encounter encounter in encountersOrig)
            {
                description += encounter.encounterType + " ";
            }
            description += "\n";
        }
        if (itemsOrig.Count > 0)
        {
            if (items.Count > 0)
            {
                description += "__ ";
            }
            else
            {
                description += "\U0001f600 ";
            }
            description += "Recover the ";
            foreach (Equipment equipment in itemsOrig)
            {
                description += equipment.Name + " ";
            }
            description += "\n";
        }
        description += "__ " + "Return to H.Q.\n";
        if (rewards.Count > 0)
        {
            description += "Reward: ";
            foreach (Equipment equipment in rewardsOrig)
            {
                description += equipment.Name + " ";
            }
        }
        return description;
    }
}
