using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetect : MonoBehaviour {
    private PlayerController controller;
    private int playerControllerNumber;
    public int normal;
    public int dropThroughFloor;
    public MyAudio.AUDIOCLIPS landAudio;
    public float minSpeedForSoundEffect;

    private float previousYValue;

    void Awake()
    {
        controller = transform.parent.GetComponent<PlayerController>();
    }

    void Start()
    {
        playerControllerNumber = controller.playerControlNumber;
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if(other.tag == Tags.floor)
        {
            controller.Land();
        }
    }
    public void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == Tags.floor)
        {
            controller.LeftGround();
        }
    }

    void Update()
    {
        float y = MyInputs.GetAxisRaw(playerControllerNumber, Tags.vertical);
        if(!falling && y < -0.85 && previousYValue>-0.75)
        {
            StartCoroutine(FallThroughFloor());
        }
        if(MyInputs.GetButtonDown(playerControllerNumber, Tags.jump))
        {
            
        }

        previousYValue = y;
    }

    bool falling;
    IEnumerator FallThroughFloor()
    {
        falling = true;
        gameObject.layer = dropThroughFloor;
        yield return new WaitForSeconds(0.2f);
        gameObject.layer = normal;
        falling = false;
    }
	

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.relativeVelocity.magnitude > minSpeedForSoundEffect)
        {
            MyAudio.current.PlayClip(landAudio);
        }
    }
}
