using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using Pathfinding;

public class Main : MonoBehaviour
{
    public CC_AssetMap ConflictChronicleAssets;
    private string savedSettingsLocation;
    private string selectedWorldLocation;
    private CC_MapController mapController;
    private CC_CameraFollow followScript;
    private GameObject focusPoint;
    private int mapX;
    private int worldMaxX = 3;
    private int mapY;
    private int worldMaxY = 3;

    void Start()
    {
        string[] worlds = ListWorlds();
        selectedWorldLocation = worlds[0];
        startGame();
    }

    // Testing updates
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Saving...");
            mapController.SaveToJSON();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space");
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("Next X");
            if(mapX < worldMaxX)
            {
                mapX++;
                loadMap();
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log("Previous X");
            if(mapX > 0)
            {
                mapX--;
                loadMap();
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log("Next Y");
            if(mapY < worldMaxY)
            {
                mapY++;
                loadMap();
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Debug.Log("Previous Y");
            if(mapY > 0)
            {
                mapY--;
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
        mapX = 0;
        mapY = 0;
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
        Debug.Log("Loading Map: " + mapX + " " + mapY);
        Time.timeScale = 0;
        CC_MapModel map = CC_MapModelUtil.LoadMapFromFile(mapX, mapY, selectedWorldLocation);
        mapController.LoadMapIntoScene(map, selectedWorldLocation);
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
        focusPoint.transform.position = new Vector3(3, 1, 3);
        followScript.player = focusPoint;
        Time.timeScale = 1;
    }
}

