using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class EncounterManager : MonoBehaviour
{
    [SerializeField] GameObject robotHUD = null;
    [SerializeField] GameObject robotSprites = null;
    [SerializeField] GameObject playerHUD = null;
    [SerializeField] GameObject playerSprites = null;
    [SerializeField] GameObject[] playerImages = new GameObject[2];
    [SerializeField] GameObject[] robotImages = new GameObject[6];

    [SerializeField] private Tilemap playerHUDTilemap = null;
    [SerializeField] private Tilemap robotHUDTilemap = null;

    private GameObject robotAliveSprite = null;
    private GameObject robotDeadSprite = null;
    private GameObject playerAliveSprite = null;
    private GameObject playerDeadSprite = null;

    [SerializeField] TextMeshProUGUI PlayerBrVal = null;
    [SerializeField] TextMeshProUGUI PlayerHpVal = null;
    [SerializeField] TextMeshProUGUI PlayerGtVal = null;
    [SerializeField] TextMeshProUGUI PlayerEpVal = null;
    [SerializeField] TextMeshProUGUI PlayerTcVal = null;
    [SerializeField] TextMeshProUGUI PlayerAcVal = null;
    [SerializeField] TextMeshProUGUI PlayerMjVal = null;
    [SerializeField] Button PlayerBrawnAction = null;
    [SerializeField] Button PlayerTechAction = null;
    [SerializeField] Button PlayerGritAction = null;

    [SerializeField] TextMeshProUGUI RobotBrVal = null;
    [SerializeField] TextMeshProUGUI RobotHpVal = null;
    [SerializeField] TextMeshProUGUI RobotGtVal = null;
    [SerializeField] TextMeshProUGUI RobotEpVal = null;
    [SerializeField] TextMeshProUGUI RobotTcVal = null;
    [SerializeField] TextMeshProUGUI RobotAcVal = null;
    [SerializeField] TextMeshProUGUI RobotMjVal = null;
    [SerializeField] Button RobotBrawnAction = null;
    [SerializeField] Button RobotTechAction = null;
    [SerializeField] Button RobotGritAction = null;

    [SerializeField] TextMeshProUGUI PlayerBuff = null;
    [SerializeField] TextMeshProUGUI PlayerDamage = null;
    [SerializeField] TextMeshProUGUI RobotBuff = null;
    [SerializeField] TextMeshProUGUI RobotDamage = null;

    [SerializeField] GameObject npcHUD = null;
    [SerializeField] TextMeshProUGUI npcDialog = null;

    [SerializeField] TextMeshProUGUI LootDescription = null;

    [SerializeField] GameObject exploreHUD = null;
    [SerializeField] Button InventoryButton = null;
    [SerializeField] Button SearchButton = null;
    [SerializeField] Button LeaveButton = null;
    [SerializeField] TextMeshProUGUI InventoryButtonText = null;
    [SerializeField] TextMeshProUGUI SearchButtonText = null;
    [SerializeField] TextMeshProUGUI LeaveButtonText = null;

    [SerializeField] TravelMap travelMap = null;

    [SerializeField] SpriteRenderer encounterBackground = null;
    [SerializeField] Sprite encounterBase = null;
    [SerializeField] Sprite encounterFactory = null;
    [SerializeField] Sprite encounterStorage = null;
    [SerializeField] Sprite encounterResearch = null;
    [SerializeField] Sprite encounterOther = null;
    [SerializeField] TextMeshProUGUI encounterTitle = null;

    [SerializeField] TextMeshProUGUI actionName = null;
    [SerializeField] TextMeshProUGUI equipmentName = null;
    [SerializeField] TextMeshProUGUI enchantmentName = null;


    [SerializeField] DynamicMusic dynamicMusic = null;

    const string backgroundHUDTilename = "scan_hex";

    Character player = null;
    Character robot = null;
    GameManager gameManager = null;

    Stats.StatType playerAction = Stats.StatType.BR;
    Stats.StatType robotAction = Stats.StatType.BR;

    public Encounter activeEncounter = null;

    private const float veryLongDelay = 2.0f;
    private const float longDelay = 0.8f;
    private const float shortDelay = 0.2f;

    enum EncounterState { Start, PlayerActionNeeded, PlayerActionChosen, PlayerCheck, RobotActionNeeded, RobotActionChosen, RobotCheck, Finish};
    EncounterState encounterState;

    void Start()
    {
        //Debug.LogWarning("=-= Encounter Start()");
        // Get Characters
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (gameManager != null)
        {
            player = gameManager.Player;
        }
        else
        {
            Debug.LogWarning("Missing GameManager");
        }
        InitializeEncounter();
    }

    private void OnEnable()
    {
        if (player != null)
        {
            InitializeEncounter();
        }
    }

    public void InitializeEncounter()
    {
        foreach (GameObject characterSprite in playerImages)
        {
            characterSprite.SetActive(false);
        }
        foreach (GameObject characterSprite in robotImages)
        {
            characterSprite.SetActive(false);
        }

        playerDeadSprite = playerImages[0];
        playerAliveSprite = playerImages[1];

        playerAliveSprite.SetActive(true);
        playerDeadSprite.SetActive(false);
        UpdatePlayerHUD();
        activeEncounter = travelMap.currentEncounter;
        exploreHUD.SetActive(true);
        SearchButton.interactable = false;
        InventoryButton.interactable = false;
        LeaveButton.interactable = false;
        if (activeEncounter != null)
        {
            encounterTitle.text = "Encounter: " + activeEncounter.encounterType;
            if (activeEncounter.encounterType == Encounter.EncounterType.Base)
            {
                SearchButtonText.text = "VISIT H.Q.";
                InventoryButtonText.text = "INVENTORY";
                LeaveButtonText.text = "ENTER CAVE";
                encounterBackground.sprite = encounterBase;
            }
            else if (activeEncounter.encounterType == Encounter.EncounterType.Factory)
            {
                InventoryButtonText.text = "INVENTORY";
                SearchButtonText.text = "SEARCH";
                LeaveButtonText.text = "LEAVE";
                encounterBackground.sprite = encounterFactory;
                exploreHUD.SetActive(false);
            }
            else if (activeEncounter.encounterType == Encounter.EncounterType.Storage)
            {
                InventoryButtonText.text = "INVENTORY";
                SearchButtonText.text = "SEARCH";
                LeaveButtonText.text = "LEAVE";
                encounterBackground.sprite = encounterStorage;
                exploreHUD.SetActive(false);
            }
            else if (activeEncounter.encounterType == Encounter.EncounterType.ResearchTower)
            {
                InventoryButtonText.text = "INVENTORY";
                SearchButtonText.text = "SEARCH";
                LeaveButtonText.text = "LEAVE";
                encounterBackground.sprite = encounterResearch;
                exploreHUD.SetActive(false);
            }
            else
            {
                InventoryButtonText.text = "INVENTORY";
                SearchButtonText.text = "SEARCH";
                LeaveButtonText.text = "LEAVE";
                encounterBackground.sprite = encounterOther;
                exploreHUD.SetActive(false);
            }
        }
        if (activeEncounter != null && activeEncounter.robots.Count > 0)
        {
            robot = activeEncounter.robots[0];
        } else
        {
            robot = null;
        }
        if (robot == null)
        {
            robotDeadSprite = robotImages[0];
            robotAliveSprite = robotImages[1];
            robotAliveSprite.SetActive(false);
            robotDeadSprite.SetActive(false);
            robotHUD.SetActive(false);
            robotSprites.SetActive(false);
            playerHUD.SetActive(false);
            if (activeEncounter.encounterType == Encounter.EncounterType.Base && travelMap.questsCompleted == 0)
            {
                SearchButton.interactable = false;
                LeaveButton.interactable = false;
            }
            else
            {
                if (activeEncounter.loot.Count > 0 || activeEncounter.encounterType == Encounter.EncounterType.Base)
                {
                    SearchButton.interactable = true;
                }
                InventoryButton.interactable = true;
                LeaveButton.interactable = true;
            }
            encounterState = EncounterState.Finish;
        }
        else
        {
            if (robot.Stats.GetStat(Stats.StatType.MJ) == 8)
            {
                robotDeadSprite = robotImages[4];
                robotAliveSprite = robotImages[5];
            }
            else if (robot.Stats.GetStat(Stats.StatType.MJ) >= 6)
            {
                robotDeadSprite = robotImages[2];
                robotAliveSprite = robotImages[3];

            }
            else
            {
                robotDeadSprite = robotImages[0];
                robotAliveSprite = robotImages[1];
            }

            robotAliveSprite.SetActive(true);
            robotDeadSprite.SetActive(false);
            robotHUD.SetActive(true);
            robotSprites.SetActive(true);

            UpdateRobotHUD();
            playerHUD.SetActive(true);
            encounterState = EncounterState.Start;
        }
        StartCoroutine(RunEncounter());
    }

    void CheckInventoryRequest()
    {
        if (Input.GetMouseButtonDown(0))
        {
            {
                Vector3Int tileLocation = playerHUDTilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                tileLocation.z = 0;
            }
            {
                Vector3Int tileLocation = robotHUDTilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                tileLocation.z = 0;
                Tile tile = robotHUDTilemap.GetTile<Tile>(tileLocation);
                if (tile != null && (tile.name == backgroundHUDTilename))
                {
                    Debug.Log("Display Robot Inventory");
                }
            }
        }
    }

    public void ShowPlayerInventory()
    {
        gameManager.State = GameManager.GameState.Inventory;
        LeaveButton.interactable = true;
    }

    void Update()
    {
        if (gameManager.State != GameManager.GameState.Encounter)
        {
            return;
        }
        CheckInventoryRequest();
    }

    void DisplayProc(bool isPlayer, bool isBuff, string desc)
    {
        PlayerBuff.text = "";
        PlayerDamage.text = "";
        RobotBuff.text = "";
        RobotDamage.text = "";
        if (isPlayer)
        {
            if (isBuff)
            {
                PlayerBuff.text = desc;
            }
            else
            {
                PlayerDamage.text = desc;
            }
        }
        else
        {
            if (isBuff)
            {
                RobotBuff.text = desc;
            }
            else
            {
                RobotDamage.text = desc;
            }
        }
    }
    IEnumerator RunEncounter()
    {
        int rounds = 0;
        int maxRounds = 20;

        if (activeEncounter.npcHints != "")
        {
            npcDialog.text = activeEncounter.npcHints;
            npcHUD.SetActive(true);
            yield return new WaitForSeconds(shortDelay);
            while (npcHUD.activeSelf)
            {
                // wait for dialog to close
                yield return new WaitForSeconds(shortDelay);
            }
            activeEncounter.npcHints = "";
        }

        while (encounterState != EncounterState.Finish)
        {
            switch (encounterState)
            {
                case EncounterState.Start:
                    player.StartBattle();
                    robot.StartBattle();
                    // Highlight Player Icon
                    encounterState = EncounterState.PlayerActionNeeded;
                    break;
                case EncounterState.PlayerActionNeeded:
                    if (player.Stats.GetStat(Stats.StatType.HP) > 7)
                    {
                        dynamicMusic.musicContext = DynamicMusic.MusicContext.PlayerAttack;
                    } 
                    else
                    {
                        dynamicMusic.musicContext = DynamicMusic.MusicContext.PlayerUnhealthy;
                    }
                    yield return new WaitForSeconds(longDelay);
                    // Enable buttons
                    PlayerBrawnAction.interactable = true;
                    PlayerTechAction.interactable = true;
                    PlayerGritAction.interactable = true;
                    InventoryButton.interactable = true;
                    LeaveButton.interactable = true;
                    // Wait for button press
                    yield return new WaitForSeconds(longDelay);
                    // Animate Player Icon
                    break;
                case EncounterState.PlayerActionChosen:
                    actionName.text = player.Name + " " + playerAction + " Action";
                    Vector3 playerLocation = playerSprites.transform.position;
                    playerSprites.transform.position = playerLocation + new Vector3( 1, 0 , 0);
                    PlayerBrawnAction.interactable = false;
                    PlayerTechAction.interactable = false;
                    PlayerGritAction.interactable = false;
                    InventoryButton.interactable = false;
                    LeaveButton.interactable = false;
                    Character.HitResult playerHit = Character.HitResult.None;
                    List<Enchantment> playerProcs = player.PerformAction(playerAction, robot, out playerHit);
                    if (playerHit == Character.HitResult.Crit)
                    {
                        DisplayProc(false, true, "Crit");
                    }
                    else if (playerHit == Character.HitResult.Hit)
                    {
                        DisplayProc(false, true, "Hit");
                    }
                    else if (playerHit == Character.HitResult.Miss)
                    {
                        DisplayProc(false, false, "Miss");
                    }
                    if (playerHit != Character.HitResult.None)
                    {
                        yield return new WaitForSeconds(longDelay);
                        DisplayProc(true, true, "");
                        yield return new WaitForSeconds(shortDelay);
                    }
                    foreach (Enchantment proc in playerProcs)
                    {
                        equipmentName.text = proc.Equipment.Name;
                        enchantmentName.text = proc.Name;
                        string desc = player.ApplyProc(proc, robot);
                        DisplayProc(proc.Self, !proc.Damage, desc);
                        UpdateHUDs();
                        yield return new WaitForSeconds(longDelay);
                        DisplayProc(proc.Self, !proc.Damage, "");
                        equipmentName.text = "";
                        enchantmentName.text = "";
                        yield return new WaitForSeconds(shortDelay);
                    }
                    encounterState = EncounterState.PlayerCheck;
                    playerSprites.transform.position = playerLocation;
                    actionName.text = "";
                    yield return new WaitForSeconds(shortDelay);
                    break;
                case EncounterState.PlayerCheck:
                    if (player.Stats.GetStat(Stats.StatType.HP) <= 0)
                    {
                        playerAliveSprite.SetActive(false);
                        playerDeadSprite.SetActive(true);
                        encounterState = EncounterState.Finish;

                    }
                    else if (robot.Stats.GetStat(Stats.StatType.HP) <= 0)
                    {
                        robotAliveSprite.SetActive(false);
                        robotDeadSprite.SetActive(true);
                        activeEncounter.robots.Clear();
                        encounterState = EncounterState.Finish;
                    }
                    else
                    {
                        encounterState = EncounterState.RobotActionNeeded;
                    }
                    break;
                case EncounterState.RobotActionNeeded:
                    if (robot.Stats.GetStat(Stats.StatType.HP) > 7)
                    {
                        dynamicMusic.musicContext = DynamicMusic.MusicContext.EnemyAttack;
                    } else
                    {
                        dynamicMusic.musicContext = DynamicMusic.MusicContext.EnemyUnhealthy;
                    }
                    yield return new WaitForSeconds(longDelay);
                    // Highlight Robot
                    // Animate buttons
                    robotAction = robot.ChooseAction(player);
                    switch (robotAction)
                    {
                        case Stats.StatType.BR:
                            RobotBrawnAction.interactable = true;
                            break;
                        case Stats.StatType.TC:
                            RobotTechAction.interactable = true;
                            break;
                        case Stats.StatType.GT:
                            RobotGritAction.interactable = true;
                            break;
                    }
                    yield return new WaitForSeconds(longDelay);
                    encounterState = EncounterState.RobotActionChosen;
                    break;
                case EncounterState.RobotActionChosen:
                    actionName.text = robot.Name + " " + robotAction + " Action";
                    Vector3 robotLocation = robotSprites.transform.position;
                    robotSprites.transform.position = robotLocation + new Vector3(-1, 0, 0);
                    Character.HitResult robotHit = Character.HitResult.Miss;
                    List<Enchantment> robotProcs = robot.PerformAction(robotAction, player, out robotHit);
                    if (robotHit == Character.HitResult.Crit)
                    {
                        DisplayProc(true, false, "Crit");
                    }
                    else if (robotHit == Character.HitResult.Hit)
                    {
                        DisplayProc(true, false, "Hit");
                    }
                    else if (robotHit == Character.HitResult.Miss)
                    {
                        DisplayProc(true, true, "Miss");
                    }
                    if (robotHit != Character.HitResult.None)
                    {
                        yield return new WaitForSeconds(longDelay);
                        DisplayProc(true, true, "");
                        yield return new WaitForSeconds(shortDelay);
                    }
                    foreach (Enchantment proc in robotProcs)
                    {
                        equipmentName.text = proc.Equipment.Name;
                        enchantmentName.text = proc.Name;
                        string desc = robot.ApplyProc(proc, player);
                        DisplayProc(!proc.Self, !proc.Damage, desc);
                        UpdateHUDs();
                        yield return new WaitForSeconds(longDelay);
                        equipmentName.text = "";
                        enchantmentName.text = "";
                        DisplayProc(!proc.Self, !proc.Damage, "");
                        yield return new WaitForSeconds(shortDelay);
                    }
                    RobotBrawnAction.interactable = false;
                    RobotTechAction.interactable = false;
                    RobotGritAction.interactable = false;
                    robotSprites.transform.position = robotLocation;
                    encounterState = EncounterState.RobotCheck;
                    actionName.text = "";
                    break;
                case EncounterState.RobotCheck:
                    rounds++;
                    if (rounds > maxRounds)
                    {
                        player.ApplyStatChange(Stats.StatType.HP, -100);
                        UpdatePlayerHUD();
                        encounterState = EncounterState.Finish;
                    }
                    else if (player.Stats.GetStat(Stats.StatType.HP) <= 0)
                    {
                        playerAliveSprite.SetActive(false);
                        playerDeadSprite.SetActive(true);
                        encounterState = EncounterState.Finish;

                    }
                    else if (robot.Stats.GetStat(Stats.StatType.HP) <= 0)
                    {
                        robotAliveSprite.SetActive(false);
                        robotDeadSprite.SetActive(true);
                        activeEncounter.robots.Clear();
                        encounterState = EncounterState.Finish;
                    }
                    else
                    {
                        encounterState = EncounterState.PlayerActionNeeded;
                    }
                    break;
            }
            yield return null;
        }
        if (robot != null)
        {
            if (rounds > maxRounds)
            {
                DisplayProc(true, false, "Timeout");
                yield return new WaitForSeconds(longDelay);
                DisplayProc(true, true, "");
                yield return new WaitForSeconds(shortDelay);
                DisplayProc(true, false, "-100 HP");
                yield return new WaitForSeconds(longDelay);
                DisplayProc(true, true, "");
                yield return new WaitForSeconds(shortDelay);
            }
            if (robot.Stats.GetStat(Stats.StatType.HP) <= 0)
            {
                DisplayProc(true, true, "Victory");
            }
            else
            {
                DisplayProc(true, false, "Defeat");
            }
            yield return new WaitForSeconds(longDelay);
            DisplayProc(true, true, "");
            yield return new WaitForSeconds(shortDelay);

            player.StopBattle();
            robot.StopBattle();
        }
        if ((activeEncounter.robots.Count == 0 && activeEncounter.loot.Count > 0) ||
            activeEncounter.encounterType == Encounter.EncounterType.Base)
        {
            SearchButton.interactable = true;
        }
        else
        {
            SearchButton.interactable = false;
        }
        if (activeEncounter.encounterType != Encounter.EncounterType.Base)
        {
            InventoryButton.interactable = true;
            LeaveButton.interactable = true;
            dynamicMusic.musicContext = DynamicMusic.MusicContext.PlayerIdle;
        }
        else
        {
            if (travelMap.questsCompleted == 0)
            {
                LeaveButton.interactable = false;
            }
            else
            {
                LeaveButton.interactable = true;
            }
            dynamicMusic.musicContext = DynamicMusic.MusicContext.PlayerTravel;
        }
        if (SearchButton.interactable)
        {
            if (activeEncounter.encounterType != Encounter.EncounterType.Base)
            {
                ButtonSearch();
            }
        }
        else
        {
            // kick 'em out if dead or no loot
            if (activeEncounter.encounterType != Encounter.EncounterType.Base)
            {
                ButtonLeave();
            }

        }
    }

    void UpdateQuest()
    {
        // Update quest
        foreach (Encounter encounter in travelMap.mainQuest.Encounters)
        {
            if (encounter.encounterType == activeEncounter.encounterType)
            {
                travelMap.mainQuest.Encounters.Remove(encounter);
                break;
            }
        }
        Equipment itemToRemove = null;
        foreach (Equipment item in travelMap.mainQuest.Items)
        {
            foreach (Equipment equipment in player.Inventory.allEquipment)
            {
                if (equipment.Type == item.Type)
                {
                    itemToRemove = item;
                    break;
                }
            }
        }
        travelMap.mainQuest.Items.Remove(itemToRemove);
    }


    public void ButtonSearch()
    {
        SearchButton.interactable = false;
        InventoryButton.interactable = false;
        LeaveButton.interactable = false;
        if (activeEncounter.encounterType == Encounter.EncounterType.Base)
        {
            StartCoroutine(TurnInQuest());
        }
        else
        {
            StartCoroutine(SearchAndLeave());
        }
    }

    IEnumerator SearchAndLeave()
    {
        yield return SearchArea();
        ButtonLeave();
    }

    IEnumerator SearchArea()
    {
        foreach (Equipment equipment in activeEncounter.loot)
        {
            LootDescription.text = "Found " + equipment.Name;
            if (equipment.Type == Equipment.EquipmentType.Supplies)
            {
                travelMap.AdjustSupplies(5);
            }
            else
            {
                player.Inventory.AddAndAutoEquip(equipment);
            }
            yield return new WaitForSeconds(longDelay);
            LootDescription.text = "";
            yield return new WaitForSeconds(shortDelay);
        }
        activeEncounter.loot.Clear();
        LootDescription.text = "";
        UpdateQuest();
        InventoryButton.interactable = true;
        LeaveButton.interactable = true;
    }
    IEnumerator TurnInQuest()
    {
        bool onboarding = false;

        if (travelMap.questsCompleted == 0)
        {
            onboarding = true;
        }
        if (travelMap.mainQuest.Encounters.Count == 0 && travelMap.mainQuest.Items.Count == 0)
        {
            if (travelMap.mainQuest.NpcResolve != "")
            {
                npcDialog.text = travelMap.mainQuest.NpcResolve;
                npcHUD.SetActive(true);
                while (npcHUD.activeSelf)
                {
                    // wait for dialog to close
                    yield return new WaitForSeconds(shortDelay);
                }
            }
            foreach (Equipment equipment in travelMap.mainQuest.ItemsOrig)
            {
                Equipment.EquipmentType equipmentType = equipment.Type;
                Equipment matchingEquipment = null;
                foreach (Equipment playerEquipment in player.Inventory.allEquipment)
                {
                    if (playerEquipment.Type == equipmentType)
                    {
                        matchingEquipment = playerEquipment;
                        break;
                    }
                }
                if (matchingEquipment != null)
                {
                    player.Inventory.RemoveEquipment(matchingEquipment);
                }
                else
                {
                    Debug.LogWarning("Quest item not in inventory.");
                }
            }

            foreach (Equipment equipment in travelMap.mainQuest.Rewards)
            {
                player.Inventory.AddEquipment(equipment);
            }
            travelMap.mainQuest.Rewards.Clear();
            travelMap.GetNextQuest();
            npcDialog.text = travelMap.mainQuest.NpcSetup;
            npcHUD.SetActive(true);
            while (npcHUD.activeSelf)
            {
                // wait for dialog to close
                yield return new WaitForSeconds(shortDelay);
            }
        }
        else
        {
            npcDialog.text = "How is the mission going?\nCome back when it is complete.";
            npcHUD.SetActive(true);
            while (npcHUD.activeSelf)
            {
                // wait for dialog to close
                yield return new WaitForSeconds(shortDelay);
            }
        }
        if (!onboarding)
        {
            LeaveButton.interactable = true;
        }
        InventoryButton.interactable = true;
    }

    public void ButtonLeave()
    {
        if (encounterState != EncounterState.Finish)
        {
            // Clean up before exit
            player.StopBattle();
            robot.StopBattle();
        }
        UpdateQuest();
        gameManager.State = GameManager.GameState.Travel;
    }

    void UpdateHUDs()
    {
        UpdatePlayerHUD();
        UpdateRobotHUD();

    }

    void HighlightState(TextMeshProUGUI text, int stat, int baseStat)
    {
        if (stat > baseStat)
        {
            text.color = Color.green;
        }
        else if (stat < baseStat)
        {
            text.color = Color.red;
        }
        else
        {
            text.color = Color.white;
        }
    }
    void UpdatePlayerHUD()
    {
        if (player != null)
        {
            PlayerBrVal.text = player.Stats.GetStat(Stats.StatType.BR).ToString();
            HighlightState(PlayerBrVal, player.Stats.GetStat(Stats.StatType.BR), player.BaseStats.GetStat(Stats.StatType.BR));
            PlayerHpVal.text = player.Stats.GetStat(Stats.StatType.HP).ToString();
            HighlightState(PlayerHpVal, player.Stats.GetStat(Stats.StatType.HP), player.BaseStats.GetStat(Stats.StatType.HP));
            PlayerGtVal.text = player.Stats.GetStat(Stats.StatType.GT).ToString();
            HighlightState(PlayerGtVal, player.Stats.GetStat(Stats.StatType.GT), player.BaseStats.GetStat(Stats.StatType.GT));
            PlayerEpVal.text = player.Stats.GetStat(Stats.StatType.EP).ToString();
            HighlightState(PlayerEpVal, player.Stats.GetStat(Stats.StatType.EP), player.BaseStats.GetStat(Stats.StatType.EP));
            PlayerTcVal.text = player.Stats.GetStat(Stats.StatType.TC).ToString();
            HighlightState(PlayerTcVal, player.Stats.GetStat(Stats.StatType.TC), player.BaseStats.GetStat(Stats.StatType.TC));
            PlayerAcVal.text = player.Stats.GetStat(Stats.StatType.AC).ToString();
            HighlightState(PlayerAcVal, player.Stats.GetStat(Stats.StatType.AC), player.BaseStats.GetStat(Stats.StatType.AC));
            PlayerMjVal.text = player.Stats.GetStat(Stats.StatType.MJ).ToString();
            HighlightState(PlayerMjVal, player.Stats.GetStat(Stats.StatType.MJ), player.BaseStats.GetStat(Stats.StatType.MJ));
        }

        PlayerBrawnAction.interactable = false;
        PlayerTechAction.interactable = false;
        PlayerGritAction.interactable = false;
    }

    void UpdateRobotHUD()
    {
        if (robot != null)
        {
            RobotBrVal.text = robot.Stats.GetStat(Stats.StatType.BR).ToString();
            HighlightState(RobotBrVal, robot.Stats.GetStat(Stats.StatType.BR), robot.BaseStats.GetStat(Stats.StatType.BR));
            RobotHpVal.text = robot.Stats.GetStat(Stats.StatType.HP).ToString();
            HighlightState(RobotHpVal, robot.Stats.GetStat(Stats.StatType.HP), robot.BaseStats.GetStat(Stats.StatType.HP));
            RobotGtVal.text = robot.Stats.GetStat(Stats.StatType.GT).ToString();
            HighlightState(RobotGtVal, robot.Stats.GetStat(Stats.StatType.GT), robot.BaseStats.GetStat(Stats.StatType.GT));
            RobotEpVal.text = robot.Stats.GetStat(Stats.StatType.EP).ToString();
            HighlightState(RobotEpVal, robot.Stats.GetStat(Stats.StatType.EP), robot.BaseStats.GetStat(Stats.StatType.EP));
            RobotTcVal.text = robot.Stats.GetStat(Stats.StatType.TC).ToString();
            HighlightState(RobotTcVal, robot.Stats.GetStat(Stats.StatType.TC), robot.BaseStats.GetStat(Stats.StatType.TC));
            RobotAcVal.text = robot.Stats.GetStat(Stats.StatType.AC).ToString();
            HighlightState(RobotAcVal, robot.Stats.GetStat(Stats.StatType.AC), robot.BaseStats.GetStat(Stats.StatType.AC));
            RobotMjVal.text = robot.Stats.GetStat(Stats.StatType.MJ).ToString();
            HighlightState(RobotMjVal, robot.Stats.GetStat(Stats.StatType.MJ), robot.BaseStats.GetStat(Stats.StatType.MJ));
        }

        RobotBrawnAction.interactable = false;
        RobotTechAction.interactable = false;
        RobotGritAction.interactable = false;
    }

    public void ButtonPlayerBrawnAction()
    {
        if (encounterState == EncounterState.PlayerActionNeeded)
        {
            playerAction = Stats.StatType.BR;
            encounterState = EncounterState.PlayerActionChosen;
            //Debug.Log("Player Brawn Attack");
        }
    }
    public void ButtonPlayerTechAction()
    {
        if (encounterState == EncounterState.PlayerActionNeeded)
        {
            playerAction = Stats.StatType.TC;
            encounterState = EncounterState.PlayerActionChosen;
            //Debug.Log("Player Tech Attack");
        }
    }
    public void ButtonPlayerGritAction()
    {
        if (encounterState == EncounterState.PlayerActionNeeded)
        {
            playerAction = Stats.StatType.GT;
            encounterState = EncounterState.PlayerActionChosen;
            //Debug.Log("Player Grit Attack");
        }
    }
    public void ButtonNpc()
    {
        npcHUD.SetActive(false);
    }

}
