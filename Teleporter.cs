using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour {
    //effect rigidbodies which have no parents
    public Teleporter target;
    public bool checkForSwapOnEnter = true;
    public bool retainRelativePosition;

    private List<Transform> checking = new List<Transform>();
    public enum AXISDIRECTION {none, both, positive, negative };
    public AXISDIRECTION Xaxis;
    public AXISDIRECTION Yaxis;
    

    IEnumerator CheckForMove(Transform t)
    {
        Vector2 startPos = t.position;
        Vector2 nowPos = t.position;
        while(checking.Contains(t))
        {
            nowPos = t.position;
            if(SwappedSides(nowPos, startPos))
            {
                checking.Remove(t);
                target.TeleportReceive(t, t.position - transform.position);
                yield break;
            }
            
            startPos = t.position;
            yield return null;
        }
        
    }
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D r = other.GetComponentInParent<Rigidbody2D>();
        if(r != null  && r.transform.parent == null)
        {
            if(!checkForSwapOnEnter)
            {
                target.TeleportReceive(r.transform, r.transform.position - transform.position);
            }
            else
            {
                if(!checking.Contains(r.transform))
                {
                    checking.Add(r.transform);
                }
                StartCoroutine(CheckForMove(r.transform));
            }
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        Rigidbody2D r = other.GetComponentInParent<Rigidbody2D>();
        if(checking.Contains(r.transform))
        {
            checking.Remove(r.transform);
        }
    }
    

    public void TeleportReceive(Transform t, Vector2 relativePosition)
    {
        t.position = transform.position;
        if(retainRelativePosition)
            t.position += (Vector3)relativePosition;
    }
    

    bool SwappedSides(Vector2 now, Vector2 previous)
    {
        Vector2 pos = transform.position;
        if(Xaxis == AXISDIRECTION.none && Yaxis == AXISDIRECTION.none)
            return false;
        
        bool x = false;
        bool y = false;
        if(Xaxis == AXISDIRECTION.both)
        {
            x = Mathf.Sign(now.x - pos.x) != Mathf.Sign(previous.x - pos.x);
        }
        else if(Xaxis == AXISDIRECTION.positive)
        {
            x = (now.x - pos.x)>0 && (previous.x - pos.x) < 0;
        }
        else if(Xaxis == AXISDIRECTION.negative)
        {
            x = (now.x - pos.x) < 0 && (previous.x - pos.x) > 0;
        }

        if(Yaxis == AXISDIRECTION.both)
        {
            y = Mathf.Sign(now.y - pos.y) != Mathf.Sign(previous.y - pos.y);
        }
        else if(Yaxis == AXISDIRECTION.positive)
        {
            y = (now.y - pos.y) > 0 && (previous.y - pos.y) < 0;
        }
        else if(Yaxis == AXISDIRECTION.negative)
        {
            y = (now.y - pos.y) < 0 && (previous.y - pos.y) > 0;
        }


        return x || y;
    }
}
