using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ConflictChronicle.Views
{

    public class CC_IsoCollider : MonoBehaviour
    {

        public SpriteRenderer GovernedSprite;
        public SortingGroup GovernedGroup;
        public bool useGroup;

        public int getOrder()
        {
            if (GovernedGroup != null && useGroup)
            {
                return GovernedGroup.sortingOrder;
            }
            if (GovernedSprite != null && !useGroup)
            {
                return GovernedSprite.sortingOrder;
            }
            else
            {
                throw new ArgumentException(name + " CC_IsoCollider not properly set up. ");
            }
        }

    }
}