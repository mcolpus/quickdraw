using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MovingPlatform : MyNetworkBehaviour {
    public List<Vector2> path;
    public bool relativePath;
    public int positionIndex;//first target
    public enum MODES { linear, lerp, rotateContinual}
    public MODES mode;
    public float speed;
    public float timeBetweenMovements;

    private Vector2 startPos;

    void Start()
    {
        startPos = transform.position;
        if(isServer)
        {
            StartCoroutine(Run());
        }
    }
    [ClientRpc]
    void RpcRun()
    {
        if(isClientOnly)
            StartCoroutine(Run());
    }

    [ClientRpc]
    void RpcUpdatePosition(int newPositionIndex, Vector2 currentPos)
    {
        //called by server at the end of every cycle to ensure everyone is up to data
        if(isClientOnly)
        {
            StopAllCoroutines();
            transform.position = currentPos;
            positionIndex = newPositionIndex;
            StartCoroutine(Run());
        }
    }

    IEnumerator Run()
    {
        //sync time
        if(isServer)
        {
            yield return new WaitForSeconds(1);
            RpcRun();
        }

        if(mode == MODES.rotateContinual)
        {
            yield return new WaitForSeconds(timeBetweenMovements);
            yield return RotateForever();
        }
        else if(mode == MODES.linear)
        {
            while(true)
            {
                Vector2 target = path[positionIndex];
                if(relativePath)
                    target += startPos;

                yield return LinearTo(target);

                yield return new WaitForSeconds(timeBetweenMovements);
                positionIndex = (positionIndex + 1) % path.Count;
                if(isServer)
                    RpcUpdatePosition(positionIndex, transform.position);
            }
        }
        else if(mode == MODES.lerp)
        {
            while(true)
            {
                Vector2 target = path[positionIndex];
                if(relativePath)
                    target += startPos;

                yield return LerpTo(target);

                yield return new WaitForSeconds(timeBetweenMovements);
                positionIndex = (positionIndex + 1) % path.Count;
            }
        }
    }

    IEnumerator LinearTo(Vector2 target)
    {
        while(true)
        {
            Vector2 dir = target - (Vector2) transform.position;
            if(dir.magnitude < speed)
            {
                transform.position = target;
                yield break;
            }
            dir.Normalize();
            dir *= speed;
            transform.position += (Vector3) dir;

            yield return null;
        }
    }

    IEnumerator LerpTo(Vector2 target)
    {
        while(true)
        {
            Vector2 dir = target - (Vector2)transform.position;
            if(dir.magnitude < 0.1f)
            {
                transform.position = target;
                yield break;
            }
            transform.position = Vector2.Lerp(transform.position, target, speed);

            yield return null;
        }
    }
	
    IEnumerator RotateForever()
    {
        while(true)
        {
            transform.Rotate(new Vector3(0, 0, speed));
            yield return null;
        }
    }
}
