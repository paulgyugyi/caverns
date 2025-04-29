using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject TravelScreen = null;
    [SerializeField] private GameObject EncounterScreen = null;
    [SerializeField] private GameObject InventoryScreen = null;
    [SerializeField] private DynamicMusic dynamicMusic = null;


    Character player;

    public Character Player { get { return player; } }

    public enum GameState { Travel, Encounter, Inventory }
    GameState state = GameState.Encounter;

    public GameState State { get { return state; } set { SetGameState(value); } }

    private void Awake()
    {
        player = new PlayerCharacter();
        player.Init(15);
    }

    // Start is called before the first frame update
    void Start()
    {
        //Debug.LogWarning("GameManager start");
        SetGameState(GameState.Encounter);
    }

    private void SetGameState(GameState newState)
    {
        //Debug.Log("Switching game state from " + this.state + " to " + newState);
        if (state == newState)
        {
            return;
        }
        switch (newState)
        {
            case GameState.Travel:
                dynamicMusic.Context = DynamicMusic.MusicContext.PlayerTravel;
                if (this.state == GameState.Encounter)
                {
                    TravelScreen.GetComponent<TravelMap>().EndEncounter();
                }
                TravelScreen.SetActive(true);
                EncounterScreen.SetActive(false);
                InventoryScreen.SetActive(false);
                break;
            case GameState.Encounter:
                if (TravelScreen.GetComponent<TravelMap>().currentEncounter == null || 
                    TravelScreen.GetComponent<TravelMap>().currentEncounter.encounterType == Encounter.EncounterType.Base)
                    {
                        dynamicMusic.Context = DynamicMusic.MusicContext.PlayerTravel;
                }
                else
                {
                    dynamicMusic.Context = DynamicMusic.MusicContext.PlayerAttack;
                }
                TravelScreen.SetActive(true);
                EncounterScreen.SetActive(true);
                InventoryScreen.SetActive(false);
                break;
            case GameState.Inventory:
                dynamicMusic.Context = DynamicMusic.MusicContext.PlayerIdle;
                TravelScreen.SetActive(true);
                EncounterScreen.SetActive(true);
                InventoryScreen.SetActive(true);
                break;
        }
        this.state = newState;
    }

    void Update()
    {
        
    }
}
