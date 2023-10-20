using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform attachedPlayer;
    Camera thisCamera;
    public float blendAmount = 0.05f;
    public float boxSizeX = 2;
    public float boxSizeY = 0;

    // Start is called before the first frame update
    void Start()
    {
        thisCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPos = attachedPlayer.transform.position;
        Vector3 cameraPos = transform.position;
        float camX, camY;
        camX = cameraPos.x;
        camY = cameraPos.y;
        float screenX0, screenX1, screenY0, screenY1;
        float box_x0, box_x1, box_y0, box_y1;
        box_x0 = playerPos.x - boxSizeX;
        box_x1 = playerPos.x + boxSizeX;
        box_y0 = playerPos.y - boxSizeY;
        box_y1 = playerPos.y + boxSizeY;
        Vector3 bottomLeft = thisCamera.ViewportToWorldPoint(new Vector3(0, 0,
       0));
        Vector3 topRight = thisCamera.ViewportToWorldPoint(new Vector3(1, 1, 0));
        screenX0 = bottomLeft.x;
        screenX1 = topRight.x;
        if (box_x0 < screenX0)
            camX = playerPos.x + 0.5f * (screenX1 - screenX0) - boxSizeX;
        else if (box_x1 > screenX1)
            camX = playerPos.x - 0.5f * (screenX1 - screenX0) + boxSizeX;
        screenY0 = bottomLeft.y;
        screenY1 = topRight.y;
        if (box_y0 < screenY0)
            camY = playerPos.y + 0.5f * (screenY1 - screenY0) - boxSizeY;
        else if (box_y1 > screenY1)
            camY = playerPos.y - 0.5f * (screenY1 - screenY0) + boxSizeY;
        transform.position = new Vector3(camX, camY, cameraPos.z);
    }
}
