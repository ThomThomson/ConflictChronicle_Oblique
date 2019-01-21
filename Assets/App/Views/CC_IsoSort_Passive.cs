using UnityEngine;

using ConflictChronicle.Controllers;
using UnityEngine.Rendering;

namespace ConflictChronicle.Views
{
    public class CC_IsoSort_Passive : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private SortingGroup sortGroup;

        private void Start() 
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            sortGroup = GetComponent<SortingGroup>();
            updateSort();
        }

        public void updateSort()
        {
            if(spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = CC_CameraController.spriteSort(transform.position);
            }
            if(sortGroup != null)
            {
                sortGroup.sortingOrder = CC_CameraController.spriteSort(transform.position);
            }
        }
    }
}