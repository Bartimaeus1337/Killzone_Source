using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraManager : MonoBehaviour
{
    enum VirtualCameras
    {
        NoCamera = -1,
        CockpitCamera = 0,
        FollowCamera
    }

    [SerializeField]
    List<GameObject> _virtualCameras;

    public Transform ActiveCamera { get; private set; }
    public UnityEvent ActiveCameraChanged;

    VirtualCameras CameraKeyPressed
    {
        get
        {
            for (int i = 0; i < _virtualCameras.Count; ++i)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i)) return (VirtualCameras)i;
            }

            return VirtualCameras.NoCamera;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        SetActiveCamera(VirtualCameras.FollowCamera);
    }

    // Update is called once per frame
    void Update()
    {
        //SetActiveCamera(CameraKeyPressed);
    }

    void Awake()
    {
        ActiveCameraChanged = new UnityEvent();
    }

    void SetActiveCamera(VirtualCameras activeCamera)
    {
        if (activeCamera == VirtualCameras.NoCamera)
        {
            return;
        }

        foreach (GameObject cam in _virtualCameras)
        {
            cam.SetActive(cam.tag.Equals(activeCamera.ToString()));
        }
    }

}