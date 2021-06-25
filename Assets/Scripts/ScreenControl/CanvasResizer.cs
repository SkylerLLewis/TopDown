using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasResizer : MonoBehaviour
{
    public Vector2 targetAspect = new Vector2(16, 9);
    void Start() {
        UpdateCanvas();
    }

    // Adjust the camera's height so the desired scene width fits in view
    // even if the screen/window size changes dynamically.
    void UpdateCanvas() {

        // Determine ratios of screen/window & target, respectively.
        float screenRatio = Screen.width / (float)Screen.height;
        float targetRatio = targetAspect.x / targetAspect.y;

        if(screenRatio > targetRatio) {
            // Screen or window is wider than the target: pillarbox.
            foreach (GameObject canvas in GameObject.FindGameObjectsWithTag("UICanvas")) {
                CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
                scaler.matchWidthOrHeight = 1;
            }
        }
    }
    
}
