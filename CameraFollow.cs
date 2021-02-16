using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public float minZoom;//zoom is half of vertical distance
    public float maxZoom;
    
    //sets bounds on what can be put in frame
    public Vector2 boundsX;
    public Vector2 boundsY;
    public float border;

    public float lerpSpeed;
    public float DiffBeforeChange;

    public List<Transform> players;
    private Camera cam;

    float aspect = 0;
    float minX = 0, maxX = 0, minY = 0, maxY = 0;
    public bool running = false;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }
    void Start()
    {
        aspect = cam.aspect;
        GameController.OnPlayersEnter += FindPlayers;
    }

    void FindPlayers()
    {
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag(Tags.player))
        {
            players.Add(obj.transform);
        }
        running = true;
    }

    void Update()
    {
        if(!running) return;

        minX = 0;
        maxX = 0;
        minY = 0;
        maxY = 0;
        foreach(Transform t in players)
        {
            if(t.position.x < minX) minX = t.position.x;
            if(t.position.y < minY) minY = t.position.y;
            if(t.position.x > maxX) maxX = t.position.x;
            if(t.position.y > maxY) maxY = t.position.y;
        }
        minX = Mathf.Clamp(minX - border, boundsX.x, boundsX.y);
        maxX = Mathf.Clamp(maxX + border, boundsX.x, boundsX.y);
        minY = Mathf.Clamp(minY - border, boundsY.x, boundsY.y);
        maxY = Mathf.Clamp(maxY + border, boundsY.x, boundsY.y);

        float zoom = Mathf.Max((maxX - minX) / aspect, (maxY - minY));
        zoom /= 2f;
        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
        Vector3 targetPos = new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f, -10);

        if((maxX - minX) / aspect > (maxY - minY))
        {
            //players are more spread out side ways
            if(boundsY.x + zoom< boundsY.y - zoom)
                targetPos.y = Mathf.Clamp(targetPos.y, boundsY.x + zoom, boundsY.y - zoom);
        }
        else
        {
            if(boundsX.x + zoom * aspect < boundsX.y - zoom * aspect)
                targetPos.x = Mathf.Clamp(targetPos.x, boundsX.x + zoom*aspect, boundsX.y - zoom*aspect);
        }
        
        
        if(Vector2.Distance(cam.transform.position,targetPos) > DiffBeforeChange)
            cam.transform.position = Vector3.Lerp(cam.transform.position, targetPos, lerpSpeed);

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, zoom, lerpSpeed);
    }

}
