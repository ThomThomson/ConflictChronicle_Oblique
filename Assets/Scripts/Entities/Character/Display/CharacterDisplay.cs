using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ConflictChronicle {

    public class CharacterDisplay : MonoBehaviour {
        private SettingsController settingsController;
        private AssetController assetController;
        private CameraController cameraController;

        // S t a t e
        private CC_Available_CharacterType _CharacterType;
        private CC_Character_Type _currentCharacterType;
        private CC_CameraDirection lastCameraDirection = null;
        private int _facingDirection = 0;
        private float _lastVelocity = 0;

        // S p r i t e s
        private SpriteRenderer headSpriteRenderer;
        private SpriteRenderer chestSpriteRenderer;
        private SpriteRenderer stomachSpriteRenderer;
        private SortingGroup leftArmGroup;
        private SortingGroup rightArmGroup;
        private SortingGroup leftLegGroup;
        private SortingGroup rightLegGroup;
        private SortingGroup characterGroup;

        [SerializeField] private GameObject spriteParent;

        [SerializeField] private GameObject characterObject;
        [SerializeField] private GameObject headObject;
        [SerializeField] private GameObject chestObject;
        [SerializeField] private GameObject stomachObject;
        [SerializeField] private GameObject armRightObject;
        [SerializeField] private GameObject armLeftObject;
        [SerializeField] private GameObject legRightObject;
        [SerializeField] private GameObject legLeftObject;

        [SerializeField] private CharacterControlDirect characterControlDirect;
        [SerializeField] private Animator animator;

        // A n i m a t i o n
        private float speed = 0;

        // P r o p e r t i e s
        public int FacingDirection {
            get { return _facingDirection; }
            set {
                if (value == _facingDirection) { return; }
                _facingDirection = value;
                updateFacings ();
            }
        }

        public CC_Available_CharacterType CharacterType {
            get { return _CharacterType; }
            set {
                _CharacterType = value;
                Debug.Log ("Character Type Set to: " + _CharacterType.ToString ());

                _currentCharacterType = assetController.characterTypes[_CharacterType.ToString ()];
                updateFacings ();
            }
        }

        public void Awake () {
            headSpriteRenderer = headObject.GetComponent<SpriteRenderer> ();
            chestSpriteRenderer = chestObject.GetComponent<SpriteRenderer> ();
            stomachSpriteRenderer = stomachObject.GetComponent<SpriteRenderer> ();
            characterGroup = characterObject.GetComponent<SortingGroup> ();
            leftArmGroup = armLeftObject.GetComponent<SortingGroup> ();
            rightArmGroup = armRightObject.GetComponent<SortingGroup> ();
            leftLegGroup = legLeftObject.GetComponent<SortingGroup> ();
            rightLegGroup = legRightObject.GetComponent<SortingGroup> ();
        }

        public void InjectDependencies (SettingsController settingsController, AssetController assetController, CameraController cameraController) {
            this.settingsController = settingsController;
            this.assetController = assetController;
            this.cameraController = cameraController;
            CharacterType = settingsController.CharacterDefaultType;
        }

        public void Update () {
            if (!characterControlDirect || !cameraController || !assetController) {
                return;
            }

            if (cameraController.CameraDirection != lastCameraDirection) {
                this.transform.rotation = cameraController.CameraDirection.CameraRotation;
                lastCameraDirection = cameraController.CameraDirection;
            }

            Vector2 facingVector = characterControlDirect.get2dMovement ();
            float currentVelocity = characterControlDirect.getVelocity ();
            if (characterControlDirect.isMovingUnderOwnForce ()) {
                _lastVelocity = currentVelocity;
                animator.SetFloat ("MovementSpeed", currentVelocity / characterControlDirect.getTopSpeed ());
            } else {
                animator.SetFloat ("MovementSpeed", 0);
            }
            FacingDirection = setFacingBasedOnHeading (characterControlDirect.getHeading (), lastCameraDirection);
            // characterGroup.sortingOrder = cameraController.spriteSort (characterObject.transform.position, this.characterControlDirect.Grounded);
        }

        private int setFacingBasedOnHeading (CC_CompassHeading heading, CC_CameraDirection cameraDirection) {
            int cameraDirectionOffset = CC_CameraDirection.CameraDirectionsInOrder.IndexOf (cameraDirection);
            int headingIndex = Array.IndexOf (CC_CompassUtil.headingsInOrder, heading);
            int test = ((headingIndex + CC_CompassUtil.headingsInOrder.Length) + cameraDirectionOffset) % CC_CompassUtil.headingsInOrder.Length;
            return test;

            // int offset = 0;
            // if (cameraDirection == CC_CameraDirection.EAST_FACING) {
            //     offset = 1;
            // } else if (cameraDirection == CC_CameraDirection.SOUTH_FACING) {
            //     offset = 2;
            // } else if (cameraDirection == CC_CameraDirection.WEST_FACING) {
            //     offset = 3;
            // }
            // switch (heading) {
            //     case CC_CompassHeading.SOUTH_EAST:
            //         return ((0 + CC_CompassUtil.headingsInOrder.Length) + offset) % CC_CompassUtil.headingsInOrder.Length;
            //     case CC_CompassHeading.NORTH_EAST:
            //         return ((1 + CC_CompassUtil.headingsInOrder.Length) + offset) % CC_CompassUtil.headingsInOrder.Length;
            //     case CC_CompassHeading.NORTH_WEST:
            //         return ((2 + CC_CompassUtil.headingsInOrder.Length) + offset) % CC_CompassUtil.headingsInOrder.Length;
            //     case CC_CompassHeading.SOUTH_WEST:
            //         return ((3 + CC_CompassUtil.headingsInOrder.Length) + offset) % CC_CompassUtil.headingsInOrder.Length;
            // }
            // return 0;
        }

        private void updateFacings () {
            if (FacingDirection % 2 == 0) {
                armRightObject.transform.localPosition = _currentCharacterType.nearShoulderLocation;
                legRightObject.transform.localPosition = _currentCharacterType.nearHipLocation;
                armLeftObject.transform.localPosition = _currentCharacterType.farShoulderLocation;
                legLeftObject.transform.localPosition = _currentCharacterType.farHipLocation;
            } else {
                armRightObject.transform.localPosition = _currentCharacterType.farShoulderLocation;
                legRightObject.transform.localPosition = _currentCharacterType.farHipLocation;
                armLeftObject.transform.localPosition = _currentCharacterType.nearShoulderLocation;
                legLeftObject.transform.localPosition = _currentCharacterType.nearHipLocation;
            }
            if (FacingDirection == 0 || FacingDirection == 3) {
                headSpriteRenderer.sprite = _currentCharacterType.headFrontSprite;
                headSpriteRenderer.sortingOrder = 2;
                stomachSpriteRenderer.sprite = _currentCharacterType.stomachFrontSprite;
                chestSpriteRenderer.sprite = _currentCharacterType.chestFrontSprite;
            } else {
                headSpriteRenderer.sprite = _currentCharacterType.headBackSprite;
                headSpriteRenderer.sortingOrder = -2;
                stomachSpriteRenderer.sprite = _currentCharacterType.stomachBackSprite;
                chestSpriteRenderer.sprite = _currentCharacterType.chestBackSprite;
            }
            if (FacingDirection == 0 || FacingDirection == 1) {
                rightArmGroup.sortingOrder = 3;
                rightLegGroup.sortingOrder = -1;
                leftArmGroup.sortingOrder = -3;
                leftLegGroup.sortingOrder = -2;
                rightArmGroup.transform.localScale = Vector3.one;
                rightLegGroup.transform.localScale = Vector3.one;
                leftArmGroup.transform.localScale = new Vector3 (1, 0.85f, 1);
                leftLegGroup.transform.localScale = new Vector3 (1, 0.9f, 1);
                if (cameraController.Rotating) {
                    this.spriteParent.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, 0));
                } else {
                    LeanTween.rotateLocal (this.spriteParent, new Vector3 (0, 0, 0), 0.25f).setEaseInOutExpo ();
                }
            } else {
                rightArmGroup.sortingOrder = -3;
                rightLegGroup.sortingOrder = -2;
                leftArmGroup.sortingOrder = 3;
                leftLegGroup.sortingOrder = -1;
                rightArmGroup.transform.localScale = new Vector3 (1, 0.85f, 1);
                rightLegGroup.transform.localScale = new Vector3 (1, 0.9f, 1);
                leftArmGroup.transform.localScale = Vector3.one;
                leftLegGroup.transform.localScale = Vector3.one;
                if (cameraController.Rotating) {
                    this.spriteParent.transform.localRotation = Quaternion.Euler (new Vector3 (0, 180, 0));
                } else {
                    LeanTween.rotateLocal (this.spriteParent, new Vector3 (0, 180, 0), 0.25f).setEaseInOutExpo ();
                }
            }
        }
    }
}