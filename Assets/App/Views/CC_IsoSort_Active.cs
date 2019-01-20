using UnityEngine;

using ConflictChronicle.Controllers;
using System.Collections;

namespace ConflictChronicle.Views
{

    [RequireComponent(typeof(SpriteRenderer))]
    public class CC_IsoSort_Active : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;

        private void Start()
        {
            this.tag = "CC_SortedEntity";
            spriteRenderer = GetComponent<SpriteRenderer>();
            StartCoroutine("ActiveSort");
        }

        private IEnumerator ActiveSort()
        {
            while (true)
            {
                spriteRenderer.sortingOrder = CC_CameraController.spriteSortViaRay(transform.position);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}