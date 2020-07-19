using System;
using System.Collections.Generic;
using UnityEngine;

namespace ConflictChronicle {
    public class CC_InputDirection {

        public static Dictionary<String, Vector2> inputDirections = getInputDirections ();

        private static Dictionary<String, Vector2> getInputDirections () {
            Dictionary<String, Vector2> inputDirections = new Dictionary<String, Vector2> ();
            inputDirections.Add ("UP", new Vector2 (-0.3f, 0.9f));
            inputDirections.Add ("RIGHT", new Vector2 (1, 0));
            inputDirections.Add ("DOWN", new Vector2 (0.3f, -0.9f));
            inputDirections.Add ("LEFT", new Vector2 (-1, 0));
            return inputDirections;
        }

        public static Vector2 transformInputDirection (Vector2 controlDirection) {
            controlDirection = Vector2.ClampMagnitude (controlDirection, 1);
            float lengthOfControlVector = Mathf.Sqrt (Mathf.Pow (controlDirection.x, 2) + Mathf.Pow (controlDirection.y, 2));
            if (lengthOfControlVector == 0) { return controlDirection; }
            float upAngle = 108.4f;

            Vector2 newInputDirection = Vector2.zero;
            if (controlDirection.x >= 0 && controlDirection.y >= 0) { // top right
                float controlQuadrantPercent = (Mathf.Asin (controlDirection.y / lengthOfControlVector) * Mathf.Rad2Deg) / 90;
                float newAngle = (controlQuadrantPercent * upAngle) / Mathf.Rad2Deg;
                newInputDirection = new Vector2 (Mathf.Cos (newAngle) * lengthOfControlVector, Mathf.Sin (newAngle) * lengthOfControlVector);

            } else if (controlDirection.x < 0 && controlDirection.y > 0) { // top left

                float controlQuadrantPercent = ((180 - (Mathf.Asin (controlDirection.y / lengthOfControlVector) * Mathf.Rad2Deg)) - 90) / 90;
                float newAngle = (controlQuadrantPercent * 71.6f + upAngle) / Mathf.Rad2Deg;
                newInputDirection = new Vector2 (Mathf.Cos (newAngle) * lengthOfControlVector, Mathf.Sin (newAngle) * lengthOfControlVector);

            } else if (controlDirection.x <= 0 && controlDirection.y <= 0) { // bottom left

                float controlQuadrantPercent = ((Mathf.Asin (-controlDirection.y / lengthOfControlVector) * Mathf.Rad2Deg)) / 90;
                float newAngle = (controlQuadrantPercent * upAngle + 180) / Mathf.Rad2Deg;
                newInputDirection = new Vector2 (Mathf.Cos (newAngle) * lengthOfControlVector, Mathf.Sin (newAngle) * lengthOfControlVector);

            } else if (controlDirection.x > 0 && controlDirection.y < 0) { // bottom right

                float controlQuadrantPercent = ((180 - (Mathf.Asin (-controlDirection.y / lengthOfControlVector) * Mathf.Rad2Deg)) - 90) / 90;
                float newAngle = (controlQuadrantPercent * 71.6f + upAngle + 180) / Mathf.Rad2Deg;
                newInputDirection = new Vector2 (Mathf.Cos (newAngle) * lengthOfControlVector, Mathf.Sin (newAngle) * lengthOfControlVector);

            }
            return newInputDirection;
        }
    }
}