using UnityEngine;

using ConflictChronicle.Controllers;

namespace ConflictChronicle.Views
{

    [RequireComponent(typeof(SpriteRenderer))]
    public class CC_IsoSort_Passive : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;

        private void Start() 
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            updateSort();
        }

        public void updateSort()
        {
            spriteRenderer.sortingOrder = CC_CameraController.spriteSort(transform.position);
        }
    }
}