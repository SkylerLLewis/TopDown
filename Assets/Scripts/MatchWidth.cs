using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class MatchWidth : MonoBehaviour {

    // Set this to the in-world distance between the left & right edges of your scene.
    public float sceneWidth = 10;
    Camera _camera;
    // Set this to your target aspect ratio, eg. (16, 9) or (4, 3).
    public Vector2 targetAspect = new Vector2(16, 9);
    void Start() {
        _camera = GetComponent<Camera>();
        UpdateRes();
    }

    // Adjust the camera's height so the desired scene width fits in view
    // even if the screen/window size changes dynamically.
    void UpdateRes() {

        // Determine ratios of screen/window & target, respectively.
        float screenRatio = Screen.width / (float)Screen.height;
        float targetRatio = targetAspect.x / targetAspect.y;

        if(Mathf.Approximately(screenRatio, targetRatio)) {
            // Screen or window is the target aspect ratio: use the whole area.
            float unitsPerPixel = sceneWidth / Screen.width;
            float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;
            _camera.orthographicSize = desiredHalfHeight;
        } else if(screenRatio > targetRatio) {
            // Screen or window is wider than the target: pillarbox.
            float normalizedWidth = targetRatio / screenRatio;
            float barThickness = (1f - normalizedWidth)/2f;

            float unitsPerPixel = sceneWidth / (Screen.width + normalizedWidth);
            float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;
            _camera.orthographicSize = desiredHalfHeight;
        } else {
            // Screen or window is narrower than the target: letterbox.
            float unitsPerPixel = sceneWidth / Screen.width;
            float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;
            _camera.orthographicSize = desiredHalfHeight;
        }
    }
}
