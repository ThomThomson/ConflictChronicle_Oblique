using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class CC_SettingsModel
{
    public string SAVE_FILE_LOCATION { get; set; }
    public string TAG_VISUAL_ONLY { get; set; }
    public string TAG_CAMERA { get; set; }

    public string TERRAIN_DEFAULT_ID { get; set; }
    public float TERRAIN_TOP_HEIGHT { get; set; }
    public int TERRAIN_LAYER_INDEX { get; set; }
    public int LOADING_CHUNKS_PER_FRAME { get; set; }
    public int MAP_CHANGES_SAVE_PER_FRAME { get; set; }

    public float CAMERA_CHECK_SECONDS { get; set; }
    public int VIEW_DISTANCE_X { get; set; }
    public int VIEW_DISTANCE_Z { get; set; }
    public int TILES_PER_CHUNK { get; set; }
    public int DEFAULT_MAP_SIZE_CHUNKS { get; set; }

    public CC_SettingsModel(bool generateNew)
    {
        SAVE_FILE_LOCATION = @"C:\temp\ConflictChronicle\Worlds";
        TAG_VISUAL_ONLY = "CC_VisualOnly";
        TAG_CAMERA = "MainCamera";

        TERRAIN_DEFAULT_ID = "FlatGrass";
        TERRAIN_TOP_HEIGHT = 1000;
        TERRAIN_LAYER_INDEX = 9;
        LOADING_CHUNKS_PER_FRAME = 5;
        MAP_CHANGES_SAVE_PER_FRAME = 1;

        VIEW_DISTANCE_X = 5;
        VIEW_DISTANCE_Z = 5;
        TILES_PER_CHUNK = 10;
        DEFAULT_MAP_SIZE_CHUNKS = 10;
    }
}

