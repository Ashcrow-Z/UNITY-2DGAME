using UnityEngine;

public class MainMenuInit : MonoBehaviour
{
    void Start()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.orthographic = true;
            cam.orthographicSize = 5f;
            cam.transform.position = new Vector3(0, 0, -10);
            cam.backgroundColor = new Color(0.06f, 0.06f, 0.1f);
        }
    }
}
