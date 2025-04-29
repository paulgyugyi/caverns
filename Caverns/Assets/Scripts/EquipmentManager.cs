using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static Stats;

public class EquipmentManager : MonoBehaviour
{
    [SerializeField] private Tilemap equipmentTilemap = null;
    [SerializeField] private Tilemap clusterTilemap = null;
    [SerializeField] private Tile unavailableSlot = null;
    [SerializeField] private Tile emptySlot = null;
    
    [SerializeField] private Tile tileCustom = null;
    [SerializeField] private Tile tileMedKit = null;
    [SerializeField] private Tile tileInformationCrystal = null;
    [SerializeField] private Tile tileSpikes = null;
    [SerializeField] private Tile tileLaserSword = null;
    [SerializeField] private Tile tileEnergyShield = null;
    [SerializeField] private Tile tileInsight = null;
    [SerializeField] private Tile tileTeleportBeacon = null;
    Dictionary<Equipment.EquipmentType, Tile> equipmentTiles = new Dictionary<Equipment.EquipmentType, Tile>();

    [SerializeField] TextMeshProUGUI BrVal = null;
    [SerializeField] TextMeshProUGUI HpVal = null;
    [SerializeField] TextMeshProUGUI GtVal = null;
    [SerializeField] TextMeshProUGUI EpVal = null;
    [SerializeField] TextMeshProUGUI TcVal = null;
    [SerializeField] TextMeshProUGUI AcVal = null;
    [SerializeField] TextMeshProUGUI MjVal = null;

    Character player = null;
    GameManager gameManager = null;

    const string emptySlotTilename = "scan_locator_g";
    const string occupiedSlotTilename = "scan_highlight_g";

    Vector3Int clusterCenterBR = new Vector3Int(-3, 10, 0);
    Vector3Int clusterCenterGT = new Vector3Int(3, 6, 0);
    Vector3Int clusterCenterTC = new Vector3Int(-3, 2, 0);
    Dictionary<Stats.StatType, Vector3Int> clusterCenters = new Dictionary<StatType, Vector3Int>();
    Vector3Int clusterCenterItem = new Vector3Int(7, 9, 0);

    int currentItem = 0;
    [SerializeField] TextMeshProUGUI equipmentInfo = null;
    [SerializeField] TextMeshProUGUI equipmentTitle = null;
    [SerializeField] TextMeshProUGUI equipmentDescription = null;

    Stats.StatType currentAction = Stats.StatType.BR;
    [SerializeField] TextMeshProUGUI actionDescription = null;
    [SerializeField] TextMeshProUGUI actionInfo = null;

    Dictionary<Vector3Int, ClusterHex> tileToClusterHex = new Dictionary<Vector3Int, ClusterHex>();
    Dictionary<ClusterHex, Vector3Int> clusterHexToTile = new Dictionary<ClusterHex, Vector3Int>();

    [SerializeField] Button EquipButtonBR = null;
    [SerializeField] Button EquipButtonTC = null;
    [SerializeField] Button EquipButtonGT = null;
    [SerializeField] Button EquipButtonNone = null;

    bool tileSelected = false;
    List<Vector3Int> selectedTiles = new List<Vector3Int>();

    bool initialized = false;

    void Start()
    {
        InitItemTiles();
        // Get Characters
        GetPlayer();
        UpdateStats();

        clusterCenters[Stats.StatType.BR] = clusterCenterBR;
        clusterCenters[Stats.StatType.GT] = clusterCenterGT;
        clusterCenters[Stats.StatType.TC] = clusterCenterTC;

        InitCluster(clusterCenters[Stats.StatType.BR], player.Inventory.actionClusters[Stats.StatType.BR]);
        InitCluster(clusterCenters[Stats.StatType.GT], player.Inventory.actionClusters[Stats.StatType.GT]);
        InitCluster(clusterCenters[Stats.StatType.TC], player.Inventory.actionClusters[Stats.StatType.TC]);

        UpdateCluster(clusterCenters[Stats.StatType.BR], player.Inventory.actionClusters[Stats.StatType.BR]);
        UpdateCluster(clusterCenters[Stats.StatType.GT], player.Inventory.actionClusters[Stats.StatType.GT]);
        UpdateCluster(clusterCenters[Stats.StatType.TC], player.Inventory.actionClusters[Stats.StatType.TC]);

        if (player.Inventory.allEquipment.Count > 0)
        {
            currentItem = Mathf.Max(0, player.Inventory.allEquipment.Count - 1);
        }
        else
        {
            currentItem = -1;
        }
        HighlightEquipment(currentItem);
        initialized = true;
    }

    void GetPlayer()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (gameManager != null)
        {
            player = gameManager.Player;
        }
        else
        {
            Debug.LogWarning("Missing GameManager");
        }
    }

    void Update()
    {
        if (gameManager.State != GameManager.GameState.Inventory)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Vector3Int tileLocation = clusterTilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            tileLocation.z = 0;
            Tile tile = clusterTilemap.GetTile<Tile>(tileLocation);
            if (tile != null && (tile.name == emptySlotTilename))
            {
                UpdateInfo(tileLocation);
            }
        }
    }

    private void OnEnable()
    {
        if (initialized)
        {
            UpdateCluster(clusterCenters[Stats.StatType.BR], player.Inventory.actionClusters[Stats.StatType.BR]);
            UpdateCluster(clusterCenters[Stats.StatType.GT], player.Inventory.actionClusters[Stats.StatType.GT]);
            UpdateCluster(clusterCenters[Stats.StatType.TC], player.Inventory.actionClusters[Stats.StatType.TC]);

            if (player != null && player.Inventory.allEquipment.Count > 0)
            {
                // Jump to last item - most likely to be newly added and not yet equipped.
                currentItem = Mathf.Max(0, player.Inventory.allEquipment.Count - 1);
            }
            else
            {
                currentItem = -1;
            }
            //Debug.Log("Setting current item to " + currentItem);
            HighlightEquipment(currentItem);
        }
    }

    void InitItemTiles()
    {
        //Custom, MedKit, InformationCrystal, VictoryMedal, Supplies, Spikes, LaserSword,
        //    EnergyShield, Insight, TeleportBeacon,
        //    RobotClaw, ShockWave, RebootCircuit, PlasmaTorch
        equipmentTiles[Equipment.EquipmentType.Custom] = tileCustom;
        equipmentTiles[Equipment.EquipmentType.MedKit] = tileMedKit;
        equipmentTiles[Equipment.EquipmentType.InformationCrystal] = tileInformationCrystal;
        equipmentTiles[Equipment.EquipmentType.VictoryMedal] = tileCustom;
        equipmentTiles[Equipment.EquipmentType.Supplies] = tileCustom;
        equipmentTiles[Equipment.EquipmentType.Spikes] = tileSpikes;
        equipmentTiles[Equipment.EquipmentType.LaserSword] = tileLaserSword;
        equipmentTiles[Equipment.EquipmentType.EnergyShield] = tileEnergyShield;
        equipmentTiles[Equipment.EquipmentType.Insight] = tileInsight;
        equipmentTiles[Equipment.EquipmentType.TeleportBeacon] = tileTeleportBeacon;
        equipmentTiles[Equipment.EquipmentType.PlasmaRifle] = tileCustom;
        equipmentTiles[Equipment.EquipmentType.RobotClaw] = tileCustom;
        equipmentTiles[Equipment.EquipmentType.ShockWave] = tileCustom;
        equipmentTiles[Equipment.EquipmentType.RebootCircuit] = tileCustom;
        equipmentTiles[Equipment.EquipmentType.PlasmaTorch] = tileCustom;
    }


    void UpdateInfo(Vector3Int tileLocation)
    {
        Equipment equipment = null;

         // Highlight selected equipment
        if (tileToClusterHex.TryGetValue(tileLocation, out ClusterHex clusterHex))
        {
            //Debug.Log("Selected cluster " + clusterHex.cluster.center + " slot " + clusterHex.hex);
            EquipmentCluster actionCluster = clusterHex.cluster as EquipmentCluster;
            if (actionCluster.hexEquipment.TryGetValue(clusterHex.hex, out equipment))
            {
                currentItem = player.Inventory.allEquipment.IndexOf(equipment);
                HighlightEquipment(currentItem);
            }
        }
    }

    void ClearHighlight()
    {
        if (tileSelected)
        {
            foreach (Vector3Int tile in selectedTiles)
            {
                clusterTilemap.SetTile(tile, emptySlot);
            }
        }
        tileSelected = false;
        for (int i = 0; i < Hexmap.ClusterSize; i++)
        {
            Vector3Int equipmentTile = Hexmap.GetNeighbor(clusterCenterItem, i);
            equipmentTilemap.SetTile(equipmentTile, null);
        }
        selectedTiles.Clear();
    }

    void HighlightEquipment(int equipmentIndex)
    {
        ClearHighlight();

        EquipButtonBR.interactable = false;
        EquipButtonTC.interactable = false;
        EquipButtonGT.interactable = false;
        EquipButtonNone.interactable = false;

        if (equipmentIndex == -1)
        {
            equipmentTitle.text = "ITEM";
            equipmentDescription.text = "";
            equipmentInfo.text = "";
            tileSelected = false;

            currentAction = Stats.StatType.BR;
            actionDescription.text = "";
            actionInfo.text = "";

            for (int space = 0; space < Hexmap.ClusterSize; space++)
            {
                Vector3Int equipmentTile = Hexmap.GetNeighbor(clusterCenterItem, space);
                clusterTilemap.SetTile(equipmentTile, null);
            }            
            return;
        }

        Equipment equipment = player.Inventory.allEquipment[equipmentIndex];
        EquipmentCluster equipmentCluster = player.Inventory.FindEquipmentCluster(equipment);


        // Highlight selected equipment
        equipmentTitle.text = "ITEM (" + (equipmentIndex + 1) + " of " + player.Inventory.allEquipment.Count + ")";
        equipmentDescription.text = equipment.Name;
        equipmentInfo.text = equipment.DescribeEnchantments();

        if (equipmentCluster == null)
        {
            tileSelected = false;
            currentAction = Stats.StatType.BR;
            actionDescription.text = "";
            actionInfo.text = "";
            // enable equip buttons based on enchantment affinity
            List<Stats.StatType> affinities = equipment.GetAffinities();
            if (affinities.Contains(Stats.StatType.BR))
            {
                EquipButtonBR.interactable = true;
            }
            if (affinities.Contains(Stats.StatType.TC))
            {
                EquipButtonTC.interactable = true;
            }
            if (affinities.Contains(Stats.StatType.GT))
            {
                EquipButtonGT.interactable = true;
            }
            // highlight item in item area
            bool[] spaceUsed = equipment.Shape.validHexes;
            for (int space = 0; space < Hexmap.ClusterSize; space++)
            {
                Vector3Int equipmentTile = Hexmap.GetNeighbor(clusterCenterItem, space);
                if (spaceUsed[space])
                {
                    clusterTilemap.SetTile(equipmentTile, unavailableSlot);
                }
                else
                {
                    clusterTilemap.SetTile(equipmentTile, null);
                }
            }
            equipmentTilemap.SetTile(clusterCenterItem, equipmentTiles[equipment.Type]);
        }
        else
        {
            EquipButtonNone.interactable = true;
            int equipmentCenter = equipmentCluster.GetEquipment()[equipment];
            Vector3Int equipmentCenterTile = Hexmap.GetNeighbor(equipmentCluster.center, equipmentCenter);

            bool[] spaceUsed = equipment.Shape.validHexes;
            for (int space = 0; space < Hexmap.ClusterSize; space++)
            {
                if (spaceUsed[space])
                {
                    // determine action cluster slot corresponding to equipment space
                    Vector3Int slotTile = Hexmap.GetNeighbor(equipmentCenterTile, space);
                    //Debug.Log("Highlighting space " + space + " tile " + slotTile);
                    selectedTiles.Add(slotTile);
                    tileSelected = true;
                    clusterTilemap.SetTile(slotTile, unavailableSlot);
                }
                // Update on item description dialog
                Vector3Int equipmentTile = Hexmap.GetNeighbor(clusterCenterItem, space);
                if (spaceUsed[space])
                {
                    clusterTilemap.SetTile(equipmentTile, unavailableSlot);
                }
                else
                {
                    clusterTilemap.SetTile(equipmentTile, null);
                }
            }
            equipmentTilemap.SetTile(clusterCenterItem, equipmentTiles[equipment.Type]);

            // Update combined action effects
            currentAction = equipmentCluster.actionStat;
            actionDescription.text = currentAction + " Action Combined Effects:";
            actionInfo.text = player.DescribeActionHit(currentAction);
        }
    }


    void UpdateCluster(Vector3Int center, EquipmentCluster actionCluster)
    {
        for (int i = 0; i < 7; i++)
        {
            Vector3Int tileLocation = Hexmap.GetNeighbor(center, i);
            if (actionCluster.validHexes[i])
            {
                clusterTilemap.SetTile(tileLocation, emptySlot);
                //Debug.Log("Clearing tile " + tileLocation + " in cluster " + center + " stat " + actionCluster.actionStat);
                equipmentTilemap.SetTile(tileLocation, null);
            }
        }
        foreach ((Equipment equipment, int foo) in actionCluster.allEquipment)
        {
            int equipmentCenter = actionCluster.GetEquipment()[equipment];
            Vector3Int equipmentCenterTile = Hexmap.GetNeighbor(actionCluster.center, equipmentCenter);
            //Debug.Log("Drawing tile " + equipmentCenterTile + " in cluster " + center + " stat " + actionCluster.actionStat);
            equipmentTilemap.SetTile(equipmentCenterTile, equipmentTiles[equipment.Type]);
        }
    }

    void InitCluster(Vector3Int center, EquipmentCluster actionCluster)
    {
        actionCluster.center = center;
        for (int i = 0; i < Hexmap.ClusterSize; i++)
        {
            Vector3Int tileLocation = Hexmap.GetNeighbor(center, i);
            ClusterHex clusterHex = new ClusterHex(actionCluster, i);
            tileToClusterHex[tileLocation] = clusterHex;
            clusterHexToTile[clusterHex] = tileLocation;
        }
    }

    void UpdateStats()
    {
        foreach (StatType t in Enum.GetValues(typeof(StatType)))
        {
            UpdateStat(t);
        }

    }

    void UpdateStat(Stats.StatType statType)
    {
        string val = player.Stats.GetStat(statType).ToString();
        switch (statType)
        {
            case Stats.StatType.BR:
                BrVal.text = val;
                break;
            case Stats.StatType.HP:
                HpVal.text = val;
                break;
            case Stats.StatType.GT:
                GtVal.text = val;
                break;
            case Stats.StatType.EP:
                EpVal.text = val;
                break;
            case Stats.StatType.TC:
                TcVal.text = val;
                break;
            case Stats.StatType.AC:
                AcVal.text = val;
                break;
            case Stats.StatType.MJ:
                MjVal.text = val;
                break;
        }
    }

    public void ButtonNext()
    {
        currentItem++;
        if (currentItem >= player.Inventory.allEquipment.Count)
        {
            currentItem = 0;
        }
        HighlightEquipment(currentItem);
    }

    public void ButtonPrev()
    {
        currentItem--;
        if (currentItem < 0)
        {
            currentItem = player.Inventory.allEquipment.Count - 1;
        }
        HighlightEquipment(currentItem);
    }

    public void ButtonReturn()
    {
        gameManager.State = GameManager.GameState.Encounter;
    }

    public void ButtonEquipBR()
    {
        player.Inventory.EquipEquipment(currentItem, Stats.StatType.BR);
        UpdateCluster(clusterCenters[Stats.StatType.BR], player.Inventory.actionClusters[Stats.StatType.BR]);
        HighlightEquipment(currentItem);
    }
    public void ButtonEquipTC()
    {
        //Debug.Log("Equiping item " + currentItem);
        player.Inventory.EquipEquipment(currentItem, Stats.StatType.TC);
        UpdateCluster(clusterCenters[Stats.StatType.TC], player.Inventory.actionClusters[Stats.StatType.TC]);
        HighlightEquipment(currentItem);
    }
    public void ButtonEquipGT()
    {
        player.Inventory.EquipEquipment(currentItem, Stats.StatType.GT);
        UpdateCluster(clusterCenters[Stats.StatType.GT], player.Inventory.actionClusters[Stats.StatType.GT]);
        HighlightEquipment(currentItem);
    }

    public void ButtonEquipNone()
    {
        Unequip(currentItem);
    }

    public void ButtonEquipAll()
    {
        foreach (Equipment equipment in player.Inventory.allEquipment)
        {
            EquipmentCluster equipmentCluster = player.Inventory.FindEquipmentCluster(equipment);
            if (equipmentCluster == null)
            {
                int item = player.Inventory.allEquipment.IndexOf(equipment);
                List<Stats.StatType> affinities = equipment.GetAffinities();
                if (affinities.Contains(Stats.StatType.BR))
                {
                    //Debug.Log("Equipping " + equipment.Name);
                    player.Inventory.EquipEquipment(item, Stats.StatType.BR);
                    UpdateCluster(clusterCenters[Stats.StatType.BR], player.Inventory.actionClusters[Stats.StatType.BR]);
                    currentItem = item;
                    HighlightEquipment(currentItem);
                }
                else if (affinities.Contains(Stats.StatType.TC))
                {
                    //Debug.Log("Equipping " + equipment.Name);
                    player.Inventory.EquipEquipment(item, Stats.StatType.TC);
                    UpdateCluster(clusterCenters[Stats.StatType.TC], player.Inventory.actionClusters[Stats.StatType.TC]);
                    currentItem = item;
                    HighlightEquipment(currentItem);
                }
                else if (affinities.Contains(Stats.StatType.GT))
                {
                    //Debug.Log("Equipping " + equipment.Name);
                    player.Inventory.EquipEquipment(item, Stats.StatType.GT);
                    UpdateCluster(clusterCenters[Stats.StatType.GT], player.Inventory.actionClusters[Stats.StatType.GT]);
                    currentItem = item;
                    HighlightEquipment(currentItem);
                }
            }
        }
     }


    void Unequip(int item)
    {
        Equipment equipment = player.Inventory.allEquipment[item];
        EquipmentCluster equipmentCluster = player.Inventory.FindEquipmentCluster(equipment);
        if (equipmentCluster != null)
        {
            //Debug.Log("Unequipping " + equipment.Name + " from cluster " + equipmentCluster.center + " stat " + equipmentCluster.actionStat);
            player.Inventory.UnequipEquipment(item);
            UpdateCluster(clusterCenters[equipmentCluster.actionStat], player.Inventory.actionClusters[equipmentCluster.actionStat]);
            currentItem = item;
            HighlightEquipment(currentItem);
        }
    }

}
