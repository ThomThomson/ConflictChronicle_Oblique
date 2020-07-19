using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConflictChronicle {

    public class CC_Character_Animation_Util : MonoBehaviour {
        // E d i t o r
        [SerializeField][Range (0, 3)] private int facing_direction;
        [SerializeField] private CC_Available_CharacterType character_type;

        private CharacterDisplay character_Display;

        public void Start () {
            character_Display = GetComponent<CharacterDisplay> ();
            // character_Display.CharacterType = character_type;

        }

        // C h a n g e  D e t e c t i o n
        void OnValidate () {
            if (character_Display == null) { return; }
            if (facing_direction != character_Display.FacingDirection) {
                character_Display.FacingDirection = facing_direction;
            }

            if (character_type != character_Display.CharacterType) {
                character_Display.CharacterType = character_type;
            }
        }
    }
}