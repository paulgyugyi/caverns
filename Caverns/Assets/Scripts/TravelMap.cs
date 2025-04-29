using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TravelMap : MonoBehaviour
{
    [SerializeField] private Tilemap scannerMap = null;
    [SerializeField] private Tilemap fogMap = null;
    [SerializeField] private Tilemap terrainMap = null;
    [SerializeField] private Tile scannerLocation = null;
    [SerializeField] private Tile scannerLocationLock = null;
    [SerializeField] private Tile scannerHighlight = null;
    [SerializeField] TextMeshProUGUI hexDescription = null;

    // Encounter info
    [SerializeField] private GameObject encounterIcon = null;
    [SerializeField] private TextMeshProUGUI encounterDescription = null;
    [SerializeField] private GameObject encounterButton = null;
    [SerializeField] private GameObject exploreButton = null;
    [SerializeField] private GameObject retreatButton = null;

    // Status info
    [SerializeField] private TextMeshProUGUI suppliesDescription = null;
    [SerializeField] private TextMeshProUGUI questDescription = null;

    [SerializeField] GameObject playerTracker = null;


    const string scanHighlightTilename = "scan_highlight_g";
    const string scanLocatorTilename = "scan_locator_g";
    const string scanEncounterTilename = "scan_locator";
    const string entranceTilename = "scifi_port";

    Vector3Int currentTile;

    Dictionary<string, string> hexDescriptions = new Dictionary<string, string>();

    Dictionary<Vector3Int, Encounter> locationEncounters = new Dictionary<Vector3Int, Encounter>();
    public Encounter currentEncounter = null;

    public int supplies;
    public int maxSupplies = 25;

    GameManager gameManager = null;

    public Quest mainQuest = null;
    public List<Quest> sideQuests = new List<Quest>();
    public int questsCompleted = 0;



    // Start is called before the first frame update
    void Start()
    {
        //Debug.LogWarning("Travel Start()");
        InitializeHexDescriptions();
        InitializeLocationEncounters();

        questsCompleted = 0; 
        mainQuest = new Quest(questsCompleted);

        currentTile.x = 0;
        currentTile.y = 0;
        currentTile.z = 0;
        
        supplies = maxSupplies;

        MoveLocation(currentTile);

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (gameManager == null)
        {
            Debug.LogWarning("Missing GameManager");
        }
        StartCoroutine(MoveTracker());
    }

    IEnumerator MoveTracker()
    {
        while (true)
        {
            Vector3 startPosition = playerTracker.transform.position;
            Vector3 finalPosition = scannerMap.CellToWorld(currentTile);
            if (Vector3.Distance(startPosition, finalPosition) > 0)
            {
                //Debug.Log("Moving playerTracker from " + startPosition + " to " + finalPosition);
                float elapsedTime = 0f;
                while (Vector3.Distance(playerTracker.transform.position, finalPosition) > 0)
                {
                    elapsedTime += Time.deltaTime;
                    playerTracker.transform.position = Vector3.Lerp(startPosition, finalPosition, 2.0f * elapsedTime);
                    //Debug.Log("Moved playerTracker to " + playerTracker.transform.position);
                    yield return null;
                }
            }
            yield return new WaitForSeconds(0.25f);
        }
    }
    public void GetNextQuest()
    {
        questsCompleted++;
        mainQuest = new Quest(questsCompleted);
    }

    public void EndEncounter()
    {
        MoveLocation(currentTile);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager == null || gameManager.State != GameManager.GameState.Travel)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Vector3Int tileLocation = scannerMap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            tileLocation.z = 0;
            //Debug.Log(tileLocation);
            Tile tile = scannerMap.GetTile<Tile>(tileLocation);
            if (tile != null && tile.name == scanHighlightTilename)
            {
                MoveLocation(tileLocation);
            }
        }

    }

    void RevealNeighbors(Vector3Int center, bool highlight)
    {
        List<Vector3Int> offsets = Hexmap.GetNeighbors(center);
        foreach (Vector3Int offset in offsets)
        {
            RevealTile(offset, highlight);
        }
    }

    void ResetNeighbors(Vector3Int center)
    {
        List<Vector3Int> offsets = Hexmap.GetNeighbors(center);
        foreach (Vector3Int offset in offsets)
        {
            ResetTile(offset);
        }
    }

    void RevealTile(Vector3Int tileLocation, bool highlight)
    {
        if (tileLocation.y < 0)
        {
            return;
        }
        Tile tile = terrainMap.GetTile<Tile>(tileLocation);
        if (tile.name == "stone_07" || !highlight)
        {
            //Debug.Log("Setting scanner to null at " + tileLocation);
            scannerMap.SetTile(tileLocation, null);
            return;
        }
        //Debug.Log("Setting scanner to highlight at " + tileLocation);
        scannerMap.SetTile(tileLocation, scannerHighlight);
    }

    void ResetTile(Vector3Int tileLocation)
    {
        if (tileLocation.y < 0)
        {
            return;
        }
        Tile tile = scannerMap.GetTile<Tile>(tileLocation);
        if (tile != null && ((tile.name == scanLocatorTilename) || (tile.name == scanEncounterTilename) || (tile.name == scanHighlightTilename)))
        {
            scannerMap.SetTile(tileLocation, null);
        }
    }

    // The supply mechanic is depricated
    public void AdjustSupplies(int amt)
    {
        //supplies += amt;
        supplies = Mathf.Clamp(supplies, 0, maxSupplies);
        if (supplies <= 0)
        {
            suppliesDescription.text = "Out of Supplies! Return to Base!";

        }
        else
        {
            suppliesDescription.text = "Supplies: " + supplies;
        }
    }

    void MoveLocation(Vector3Int newCenter)
    {
        // Remove old Icons
        ResetTile(currentTile);
        ResetNeighbors(currentTile);

        AdjustSupplies(-1);

        // Update location
        currentTile = newCenter;

        if (locationEncounters.ContainsKey(currentTile))
        {
            currentEncounter = locationEncounters[currentTile];
        } else
        {
            currentEncounter = null;
        }
        // reveal terrain
        fogMap.SetTile(currentTile, null);
        // Set new icons
        if (locationEncounters.ContainsKey(currentTile) && locationEncounters[currentTile].robots.Count > 0)
        {
            // locked
            scannerMap.SetTile(currentTile, scannerLocationLock);
            RevealNeighbors(currentTile, false);
            encounterIcon.SetActive(true);
            encounterDescription.text = "Enemy Attack !!!\nYou cannot move further until the enemy is defeated.";
            encounterButton.SetActive(true);
            exploreButton.SetActive(false);
            retreatButton.SetActive(true);
            // auto combat
            gameManager.State = GameManager.GameState.Encounter;
        }
        else
        {
            scannerMap.SetTile(currentTile, scannerLocation);
            RevealNeighbors(currentTile, supplies > 0);
            encounterIcon.SetActive(false);
            encounterDescription.text = "Click on a green circle to move to a new location.";
            encounterButton.SetActive(false);
            retreatButton.SetActive(true);
            if (locationEncounters.ContainsKey(currentTile) && locationEncounters[currentTile].loot.Count > 0)
            {
                // Exploring is deprecated - jump right into combat instead
                //exploreButton.SetActive(true);
                // auto combat
                if (gameManager != null)
                {
                    gameManager.State = GameManager.GameState.Encounter;
                }
            }
            else
            {
                exploreButton.SetActive(false);
            }
        }

        // Update map legend
        Vector3Int legendLocation = new Vector3Int(5, 11, 0);
        Tile legendTile = terrainMap.GetTile<Tile>(currentTile);
        terrainMap.SetTile(legendLocation, legendTile);

        hexDescription.text = hexDescriptions[legendTile.name];

        questDescription.text = mainQuest.DescribeQuest();
    }

    void InitializeHexDescriptions()
    {
        hexDescriptions["scifi_port"] = "Base - Cavern Entrance";
        hexDescriptions["mars_07"] = "Tunnel";
        hexDescriptions["scifi_energy"] = "Factory Ruins";
        hexDescriptions["scifi_hangar"] = "Storage Units";
        hexDescriptions["scifi_foliage"] = "Overgrown Terrain";
        hexDescriptions["scifi_tower"] = "Research Tower";
    }

    void InitializeLocationEncounters()
    {
        locationEncounters[new Vector3Int(0, 0, 0)] = new Encounter(Encounter.EncounterType.Base); // Cavern Entrance
        locationEncounters[new Vector3Int(-1, 2, 0)] = new Encounter(Encounter.EncounterType.Factory);  // Factory Ruins
        locationEncounters[new Vector3Int(-2, 4, 0)] = new Encounter(Encounter.EncounterType.Storage); // Storage Units
        locationEncounters[new Vector3Int(0, 4, 0)] = new Encounter(Encounter.EncounterType.Overgrown); // Overgrown Terrain
        locationEncounters[new Vector3Int(-3, 6, 0)] = new Encounter(Encounter.EncounterType.Overgrown); // Overgrown Terrain
        locationEncounters[new Vector3Int(-1, 6, 0)] = new Encounter(Encounter.EncounterType.Storage); // Storage Units
        locationEncounters[new Vector3Int(1, 6, 0)] = new Encounter(Encounter.EncounterType.Factory); // Factory Ruins
        locationEncounters[new Vector3Int(-2, 8, 0)] = new Encounter(Encounter.EncounterType.Storage); // Storage Units
        locationEncounters[new Vector3Int(0, 8, 0)] = new Encounter(Encounter.EncounterType.Overgrown); // Overgrown Terrain
        locationEncounters[new Vector3Int(-1, 10, 0)] = new Encounter(Encounter.EncounterType.Factory); // Factory Ruins
        locationEncounters[new Vector3Int(-2, 12, 0)] = new Encounter(Encounter.EncounterType.ResearchTower); // Research Tower
    }

    public void BattleButton()
    {
        //Debug.Log("BattleButton: encounter robot count: " + currentEncounter.robots.Count);
        gameManager.State = GameManager.GameState.Encounter;
    }

    public void RetreatButton()
    {
        //Debug.Log("Retreating to Base");
        MoveLocation(Vector3Int.zero);
        gameManager.State = GameManager.GameState.Encounter;
    }

    public void ExploreButton()
    {
        //Debug.Log("ExploreButton: encounter robot count: " + currentEncounter.robots.Count);
        gameManager.State = GameManager.GameState.Encounter;
    }
}
