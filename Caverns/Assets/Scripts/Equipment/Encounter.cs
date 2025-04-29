using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encounter
{
    public enum EncounterType { Base, Factory, ResearchTower, Overgrown, Storage }

    public List<RobotCharacter> robots = new List<RobotCharacter>();
    public List<Equipment> loot = new List<Equipment>();
    public EncounterType encounterType;
    public string npcHints = "";

    public Encounter(EncounterType encounterType)
    {
        this.encounterType = encounterType;
        switch (encounterType)
        {
            case EncounterType.Base:
                npcHints = "Come see me at the H.Q.";
                break;
            case EncounterType.Factory:
                npcHints = "Factory robots are weak. Use your BRAWN action to hit them with your sword.";
                robots.Add(new RobotCharacter());
                robots[0].Init(13);
                loot.Add(new Equipment(Equipment.EquipmentType.MedKit));
                break;
            case EncounterType.ResearchTower:
                npcHints = "Research robots are dangerous. Use your TECH action to raise your shields twice, then GRIT three times to lower their AC and heal, then BRAWN attacks.";
                robots.Add(new RobotCharacter());
                robots[0].Init(20);
                loot.Add(new Equipment(Equipment.EquipmentType.InformationCrystal));
                break;
            case EncounterType.Overgrown:
                npcHints = "";
                //loot.Add(new Equipment(Equipment.EquipmentType.Supplies));
                break;
            case EncounterType.Storage:
                npcHints = "Storage robots are tough. Use your GRIT action to lower their AC, then BRAWN actions to take them down.";
                robots.Add(new RobotCharacter());
                robots[0].Init(16);
                loot.Add(new Equipment(Equipment.EquipmentType.Spikes));
                break;
        }
    }
}
