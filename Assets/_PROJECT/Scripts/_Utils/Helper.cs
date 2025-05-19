using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PhantomPixelStudio.Utility
{
    /// <summary>
    /// Provides helper methods for various tasks.
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Converts screen coordinates to world coordinates based on the given camera.
        /// </summary>
        /// <param name="cameraMain1">The camera used for conversion.</param>
        /// <param name="screenPosition">The screen position to be converted.</param>
        /// <returns>The corresponding world coordinates.</returns>
        public static Vector3 ConvertScreenToWorldCoords(Camera cameraMain1, Vector2 screenPosition)
        {
            Vector3 screenCoordinates = new Vector3(screenPosition.x, screenPosition.y, cameraMain1.nearClipPlane);
            Vector3 worldCoordinates = cameraMain1.ScreenToWorldPoint(screenCoordinates);
            worldCoordinates.z = 0f;
            return worldCoordinates;
        }


        /// <summary>
        /// Generates a random float value between the specified minimum and maximum values.
        /// </summary>
        /// <param name="_minValue">The minimum value of the range.</param>
        /// <param name="_maxValue">The maximum value of the range.</param>
        /// <returns>A random float value between _minValue (inclusive) and _maxValue (exclusive).</returns>
        public static float GetRandomValue(float _minValue, float _maxValue)
        {
            return Random.Range(_minValue, _maxValue);
        }


        ///<summary>
        ///As you rotate, once you are within the FOVAngle's degrees within the target, this becomes true.
        ///Credit: https://answers.unity.com/questions/503934/chow-to-check-if-an-object-is-facing-another.html
        /// </summary>
        /// <param name="looker">object that is rotating</param>
        /// <param name="targetPos">position this is rotating towards</param>
        /// <param name="FOVAngle">angle offset amount from the target rotation angle</param>
        /// <returns></returns>
        public static bool IsLookingAtObject(Transform looker, Vector3 targetPos, float FOVAngle)
        {
            float checkAngle = Mathf.Min(FOVAngle, 359.9999f) / 2;
            float dot = Vector3.Dot(looker.up, (targetPos - looker.position).normalized);
            float viewAngle = (1 - dot) * 90;
            return viewAngle <= checkAngle;
        }

        /// <summary>
        /// Returns a random direction as a normalized Vector3.
        /// </summary>
        /// <returns>A randomly generated direction as a normalized Vector3.</returns>
        public static Vector3 GetRandomDirection()
        {
            return new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
        }

        /// <summary>
        /// Generates a random point on the screen based on the camera's viewport.
        /// </summary>
        /// <param name="cam">The camera used to calculate the point on the screen.</param>
        /// <returns>A random point on the screen as a Vector2.</returns>
        public static Vector2 GetRandomPointOnScreen(Camera cam)
        {
            return cam.ViewportToWorldPoint(new Vector2(UnityEngine.Random.value, UnityEngine.Random.value));
        }


        /// <summary>
        /// Gets the minimum screen bounds in world coordinates.
        /// </summary>
        /// <returns>The minimum screen bounds as a Vector3.</returns>
        public static Vector3 GetScreenBoundsMin()
        {
            return Camera.main.ViewportToWorldPoint(new Vector3(0, 0));
        }

        /// <summary>
        /// Gets the maximum point of the screen bounds in world space.
        /// </summary>
        /// <returns>The maximum point of the screen bounds as a Vector3.</returns>
        public static Vector3 GetScreenBoundsMax()
        {
            return Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f));
        }

        /// <summary>
        /// Creates a text object in world space.
        /// </summary>
        /// <param name="text">The text to be displayed.</param>
        /// <param name="parent">The parent transform of the text object. Default value is null.</param>
        /// <param name="localPosition">The local position of the text object. Default value is Vector3.zero.</param>
        /// <param name="fontSize">The font size of the text object. Default value is 40.</param>
        /// <param name="color">The color of the text object. Default value is Color.white.</param>
        /// <param name="textAnchor">The anchor point of the text object. Default value is TextAnchor.UpperLeft.</param>
        /// <param name="textAlignment">The text alignment of the text object. Default value is TextAlignment.Left.</param>
        /// <param name="sortingOrder">The sorting order of the text object. Default value is 0.</param>
        /// <returns>The created TextMesh object.</returns>
        public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), int fontSize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = 0)
        {
            color ??= Color.white;
            return CreateWorldText(parent, text, localPosition, fontSize, (Color) color, textAnchor, textAlignment, sortingOrder);
        }

        /// <summary>
        /// Create a world text object with specified parameters.
        /// </summary>
        /// <param name="parent">The parent transform to which the text object will be attached.</param>
        /// <param name="text">The text to be displayed.</param>
        /// <param name="localPosition">The local position of the text object.</param>
        /// <param name="fontSize">The font size of the text.</param>
        /// <param name="color">The color of the text.</param>
        /// <param name="textAnchor">The anchor point of the text alignment.</param>
        /// <param name="textAlignment">The alignment of the text within the text object.</param>
        /// <param name="sortingOrder">The sorting order of the text object when rendered.</param>
        /// <returns>The created TextMesh object.</returns>
        public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
        {
            GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
            Transform transform = gameObject.transform;
            transform.SetParent(parent, false);
            transform.localPosition = localPosition;
            TextMesh textMesh = gameObject.GetComponent<TextMesh>();
            textMesh.anchor = textAnchor;
            textMesh.alignment = textAlignment;
            textMesh.text = text;
            textMesh.fontSize = fontSize;
            textMesh.color = color;
            textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
            return textMesh;
        }

        /// <summary>
        /// Gets a spawn point that is off the screen based on a random percentage value.
        /// </summary>
        /// <returns>A Vector2 representing the spawn point.</returns>
        public static Vector2 GetSpawnPointOffScreen()
        {
            float _percentage = GetRandomValue(0, 100f);
            return _percentage switch
            {
                < 25 =>
                    //Left Side
                    new Vector2(-10f, Random.Range(-10f, 10f)),
                >= 25 and < 50 =>
                    //top side
                    new Vector2(Random.Range(-10f, 10f), 10f),
                >= 50 and < 75 =>
                    //right side
                    new Vector2(10f, Random.Range(-10f, 10f)),
                > 75 =>
                    //bottom
                    new Vector2(Random.Range(-10f, 10f), -10f),
                _ => Vector2.zero
            };
        }

        //can't add this to GamerStruggles since we dont have access to UnityEngine.math ????
        // #region Remap Values
        //
        // /// <summary>
        // /// Returns the result of a non-clamping linear remapping of a value x from source range [a,b] to the destination range [c,d].
        // /// </summary>
        // /// <param name="a">The first endpoint of the source range [a,b]</param>
        // /// <param name="b">The second endpoint of the source range [a,b]</param>
        // /// <param name="c">The first endpoint of the destination range [c,d]</param>
        // /// <param name="d">The second endpoint of the destination range [c,d]</param>
        // /// <param name="x">The value to remap from the source to destination range</param>
        // /// <returns>The remap of input x from the source range to the destination range.</returns>
        // public static float RemapValues(float sourceMin, float sourceMax, float destinationMin, float destinationMax, float currentValue)
        // {
        //     var newValue = math.remap(sourceMin, sourceMax, destinationMin, destinationMax, currentValue);
        //     return newValue;
        // }
        //
        // #endregion

        /// <summary>
        /// Determines if a random chance succeeds based on the given probability.
        /// </summary>
        /// <param name="_chance">The probability of the chance, ranging from 0 to 100.</param>
        /// <returns>True if the random chance is less than or equal to the given probability, otherwise false.</returns>
        public static bool TryGetChance(float _chance)
        {
            if (_chance < 0 || _chance > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(_chance), "Chance must be between 0 and 100.");
            }

            return Random.Range(0f, 100f) <= _chance;
        }

        /// <summary>
        /// Increases the simulation speed of a particle system over time using DOTween.
        /// </summary>
        /// <param name="_vfx">The ParticleSystem to adjust the simulation speed for.</param>
        /// <param name="_targetSimSpeed">The target simulation speed to reach.</param>
        /// <param name="_timeToTargetSimSpeed">The time it takes to reach the target simulation speed.</param>
        public static void IncreaseParticleSystemSimSpeedOverTime(ParticleSystem _vfx, float _targetSimSpeed, float _timeToTargetSimSpeed)
        {
            if (_vfx == null || DOTween.instance == null)
                return;

            var _main = _vfx.main;
            DOTween.To(() => _vfx.main.simulationSpeed, x => _main.simulationSpeed = x, _targetSimSpeed, _timeToTargetSimSpeed);
        }
    }
}
