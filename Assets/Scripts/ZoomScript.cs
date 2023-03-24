using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ZoomScript : MonoBehaviour {
    public float sizeChangeAmount; // The amount to change the camera's size by
    public float zoomSpeed; // The speed at which to zoom in/out
    public CinemachineVirtualCamera virtualCamera; // The virtual camera to zoom
    private float targetSize; // The target size of the camera

    public void OnEnable() {
        if(virtualCamera == null) {
            Debug.LogError("Where's the virtual camera? I can't find it! Check the tag");
            enabled = false; return;
        } else {
            virtualCamera.m_Lens.OrthographicSize = 5; // The size of the orthographic camera. 5 seems to be a reasonable default
            targetSize = virtualCamera.m_Lens.OrthographicSize; 
        }
}

    private void Update() {
        Vector2 scrollDelta = Input.mouseScrollDelta;
        if (scrollDelta.y != 0) {
            targetSize -= sizeChangeAmount * scrollDelta.y;
            targetSize = Mathf.Clamp(targetSize, 0.5f, 45f); // Clamp the target size to a reasonable range
        }
        virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(virtualCamera.m_Lens.OrthographicSize, targetSize, zoomSpeed * Time.deltaTime);
    }
}
