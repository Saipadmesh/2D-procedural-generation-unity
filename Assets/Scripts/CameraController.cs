using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform Player;
    public Vector2 offset;
    
    public TileGrid grid;
    private Vector3 max;
    public Camera cam;
    private float halfWidth;
    private float halfHeight;
    public Transform tilemap;

    private void Start()
    {
        halfHeight = cam.orthographicSize; //* ((float)Screen.width / Screen.height);
        halfWidth = cam.aspect * halfHeight;
        max.x = tilemap.lossyScale.x*grid.Width - halfWidth;
        max.y = tilemap.lossyScale.y*grid.Height - halfHeight;
        max.z = transform.position.z;
        
        
    }

    // Update is called once per frame
    void Update()
    {
        float posX = Player.position.x + offset.x;
        float posY = Player.position.y + offset.y;
        float moveX = Mathf.Clamp(posX, 0f+halfWidth, max.x);
        float moveY = Mathf.Clamp(posY, 0f+halfHeight, max.y);
        transform.position = new Vector3(moveX, moveY, transform.position.z);
    }
}
