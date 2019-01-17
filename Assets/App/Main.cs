using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using Pathfinding;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public CC_AssetMap ConflictChronicleAssets;
    public Text debugText;


    private string savedSettingsLocation;
    private string selectedWorldLocation;
    private CC_MapController mapController;
    private CC_CameraFollow followScript;
    private GameObject focusPoint;
    private int mapRow;
    private int worldMaxRow = 3;
    private int mapCol;
    private int worldMaxCol = 3;

    void Start()
    {
        string[] worlds = ListWorlds();
        selectedWorldLocation = worlds[0];
        startGame();
    }

    // Testing updates
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3? point = CC_CameraUtil.screenToWorldCoordinatesOnGround(Input.mousePosition);
            if(point.HasValue) { mapController.SpawnWorldObject((Vector3)point, "TreeStump"); }
        }
        if(Input.GetMouseButtonDown(1))
        {
            Vector3? point = CC_CameraUtil.screenToWorldCoordinatesOnGround(Input.mousePosition);
            if(point.HasValue) { mapController.RemoveWorldObject((Vector3)point); }
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            mapController.SaveToDisk();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if(mapRow < worldMaxRow - 1)
            {
                mapRow++;
                loadMap();
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if(mapRow > 0)
            {
                mapRow--;
                loadMap();
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if(mapCol < worldMaxCol - 1)
            {
                mapCol++;
                loadMap();
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if(mapCol > 0)
            {
                mapCol--;
                loadMap();
            }
        }
    }

    private string[] ListWorlds()
    {
        if (!Directory.Exists(CC_SettingsController.gameSettings.SAVE_FILE_LOCATION))
        {
            Directory.CreateDirectory(CC_SettingsController.gameSettings.SAVE_FILE_LOCATION);
        }
        string[] worlds = Directory.GetDirectories(CC_SettingsController.gameSettings.SAVE_FILE_LOCATION);
        if (worlds.Length == 0)
        {
            Directory.CreateDirectory(CC_SettingsController.gameSettings.SAVE_FILE_LOCATION + @"\defaultWorld\maps");
            return new string[] { CC_SettingsController.gameSettings.SAVE_FILE_LOCATION + @"\defaultWorld" };
        }
        else
        {
            return worlds;
        }
    }

    private void startGame()
    {
        Time.timeScale = 0;
        // TODO: save these in the WORLD settings file in the world folder;
        mapRow = 0;
        mapCol = 0;
        if (this.ConflictChronicleAssets == null)
        {
            throw new MissingReferenceException("Asset Map not attached to main game object!");
        }
        
        GameObject camera = GameObject.FindGameObjectWithTag("CC_Camera");
        followScript = camera.GetComponent<CC_CameraFollow>();
        camera.name = "CC_Game_Camera";

        GameObject worldObject = new GameObject();
        worldObject.name = "CC_World";
        mapController = worldObject.AddComponent<CC_MapController>();

        focusPoint = Instantiate(this.ConflictChronicleAssets.cameraFollowPoint);
        focusPoint.name = "CC_PlayerPointOfInterest";
        loadMap();
    }

    private void loadMap()
    {
        debugText.text = "Viewing Map: " + mapRow + " " + mapCol;
        //Debug.Log("Loading Map: " + mapRow + " " + mapCol);
        Time.timeScale = 0;
        CC_MapModel map = CC_MapModelUtil.LoadMapFromFile(mapRow, mapCol, selectedWorldLocation);
        mapController.LoadMapIntoScene(map, selectedWorldLocation, mapRow, mapCol);
        // Pathfinding....
        GridGraph graph = (GridGraph)AstarPath.active.data.graphs[0];
        int width = map.mapTerrain[0].Count * CC_SettingsController.gameSettings.TILES_PER_CHUNK;
        int depth = map.mapTerrain.Count * CC_SettingsController.gameSettings.TILES_PER_CHUNK;
        graph.SetDimensions(width, depth, 1f);
        graph.center = new Vector3(
            (map.mapTerrain[0].Count * CC_SettingsController.gameSettings.TILES_PER_CHUNK) / 2,
            -0.1f,
            (map.mapTerrain.Count * CC_SettingsController.gameSettings.TILES_PER_CHUNK) / 2
        );
        // make spawn points in each map later....
        focusPoint.transform.position = new Vector3(3, 0, 3);
        followScript.player = focusPoint;
        Time.timeScale = 1;
    }
}
