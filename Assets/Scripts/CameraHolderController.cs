using UnityEngine;

public class CameraHolderController : MonoBehaviour {
    public Transform cameraPosition;

    void Update() {
        transform.position = cameraPosition.position;
    }
}