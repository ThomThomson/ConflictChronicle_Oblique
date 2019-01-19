

using UnityEngine;

namespace ConflictChronicle.Views
{
    public class ChunkObjectView
    {
        public GameObject chunkObjectReference { get; set; }
        public string chunkObjectID { get; set; }

        public ChunkObjectView(GameObject chunkObjectReference, string chunkObjectID)
        {
            this.chunkObjectID = chunkObjectID;
            this.chunkObjectReference = chunkObjectReference;
        }
    }
}