using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerController : MyNetworkBehaviour, Damageable, Stunnable {
    private Rigidbody2D rigid;

    public int playerControlNumber;
    public Animator anim;
    public Transform carryTransform;
    public Transform holdTransform;//carry gets moved around (to look up etc). hold point is where the gun is attached
    public Transform HoldTransform { get { return holdTransform; } set { holdTransform = value; } }
    public Transform armorPointTransform;
    public Transform ArmorPointTransform { get { return armorPointTransform; } set { armorPointTransform = value; } }
    public Transform spriteTransform;
    public SpriteRenderer hatRenderer;
    [SyncVar]
    public float speed;
    public float maxSpeed = 100;
    [SyncVar]
    public float jumpSpeed;
    [SyncVar]
    public int maxJumps;
    public int jumpsLeft;
    [SyncVar]
    public bool jumping;
    private float timeWhenLeftGround;
    public float maxTimeForFreeAirJump;//if they jump just after falling off, it acts as if they were on the ground

    public int startingHealth;
    [SyncVar]//[SyncVar(hook = "OnChangeHealth")]
    public int health;
    [SyncVar(hook = "CheckForDeath")]
    public bool dead;
    public Holdable equiped;
    public float throwSpeed;

    float yRot = 0;
    float zRot = 0;

    bool stunned;

    public event intAnnouncement OnHit;
    public event standardAnnouncement OnDeath;
    

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    
    void Start()
    {
        if(hasAuthority)
        {
            yRot = carryTransform.rotation.eulerAngles.y;
            zRot = carryTransform.rotation.eulerAngles.z;
        }
    }

    public override void OnStartLocalPlayer()
    {
        //GetComponentInChildren<SpriteRenderer>().color = Color.blue;
    }
    

    void Update()
    {
        UpdateAnimationParameters();
        if(!hasAuthority)
        {
            return;
        }

        rigid.velocity = new Vector2(rigid.velocity.x, Mathf.Clamp(rigid.velocity.y, -maxSpeed, maxSpeed));
        if(!dead)
        {
            if(!stunned)
            {
                Attacking();
                Jumping();
                Move();
            }
        }
    }

    #region movement
    void UpdateAnimationParameters()
    {
        anim.SetFloat(Tags.velocity_x, Math.Abs(rigid.velocity.x));
        anim.SetFloat(Tags.velocity_y, rigid.velocity.y);
        anim.SetBool(Tags.jumping, jumping);
    }

    void Move()
    {
        float x = MyInputs.GetAxis(playerControlNumber,Tags.horizontal);
        float y = MyInputs.GetAxis(playerControlNumber, Tags.vertical);

        if(x > 0)
            yRot = 0;
        else if(x < 0)
            yRot = 180;

        if(y > 0.7)
            zRot = 90;
        else if(y < -0.7)
            zRot = -90;
        else
            zRot = 0;

        carryTransform.rotation = Quaternion.Euler(0, yRot, zRot);
        spriteTransform.rotation = Quaternion.Euler(0, yRot, 0);
        rigid.velocity = new Vector2(x * speed, rigid.velocity.y);

        
    }
    void Jumping()
    {
        if(MyInputs.GetButtonDown(playerControlNumber, Tags.jump) && jumpsLeft > 0)
        {
            if(jumping && !( Time.time - timeWhenLeftGround < maxTimeForFreeAirJump))//they have been in the air for a while
                jumpsLeft--;
            //jumpsLeft--; whenever you leave the ground a jump is removed automatically
            rigid.velocity = new Vector2(rigid.velocity.x, jumpSpeed);
            
        }
    }
    public void Land()
    {
        jumpsLeft = maxJumps;
        jumping = false;
    }
    public void LeftGround()
    {
        jumpsLeft--;
        jumping = true;
        timeWhenLeftGround = Time.time;
    }
    #endregion

    #region attacking
    void Attacking()
    {
        if(equiped != null)
        {
            if(MyInputs.GetButton(playerControlNumber, Tags.fire))
            {
                CmdFire();
            }
            if(MyInputs.GetButtonDown(playerControlNumber, Tags.Throw))
            {
                CmdThrow();
                equiped = null;
            }
            else if(MyInputs.GetButtonDown(playerControlNumber, Tags.drop))
            {
                CmdDrop();
                equiped = null;
            }
        }
    }
    [Command]
    void CmdFire()
    {
        if(equiped!=null)
            equiped.Fire();
    }
    [Command]
    void CmdDrop()
    {
        equiped.Drop();
        equiped = null;
    }
    [Command]
    void CmdThrow()
    {
        equiped.Throw(throwSpeed);
        equiped = null;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(!hasAuthority)
            return;
        
        if(dead)
            return;
        if(other.tag == Tags.item)
        {
            if(MyInputs.GetButtonDown(playerControlNumber, Tags.fire) && equiped == null)
            {
                Holdable h = other.GetComponent<Holdable>();
                if(!h.IsEquiped)
                {
                    equiped = h;
                    CmdEquip(other.gameObject);
                }
            }
        }
        else if(other.tag == Tags.upgrade)
        {
            if(MyInputs.GetButtonDown(playerControlNumber, Tags.fire))
            {
                CmdPickupUpgrade(other.gameObject);
                
            }
        }
    }
    [Command]
    void CmdEquip(GameObject other)
    {
        equiped = other.GetComponent<Holdable>();
        equiped.Pickup(gameObject, Vector2.zero);

    }
    [Command]
    void CmdPickupUpgrade(GameObject upgrade)
    {
        PickUpAble pickUp = upgrade.GetComponent<PickUpAble>();
        pickUp.Pickup(gameObject, Vector2.zero);
    }

    public void ForceDropWeapon()
    {
        if(equiped != null)
        {
            equiped.Drop();
        }
        equiped = null;
        if(isServer)
            RpcCallForceDrop();
    }
    [ClientRpc]
    void RpcCallForceDrop()
    {
        if(isClientOnly)
            ForceDropWeapon();
    }

    #endregion


    #region Damageable
    public void Damage(int amount)
    {
        print("damage called " + isServer);
        if(!isServer)
            return;

        health -= amount;
        anim.SetTrigger(Tags.hit);
        if(OnHit != null)
            OnHit(health);

        if(health <= 0)
            Kill();
    }

    public void Kill()
    {
        print("kill called " + isServer);
        if(!isServer)
            return;
        
        dead = true;
        health = 0;
        

        if(OnDeath != null)
            OnDeath();
    }

    [ClientRpc]
    void RpcDie()
    {
        if(isLocalPlayer)
        {
            print("RpcDie " + isServer);
            dead = true;
            health = 0;
            spriteTransform.rotation = Quaternion.Euler(0, 0, 90);
        }
        
    }

    void CheckForDeath(bool _dead)
    {
        if(isLocalPlayer)
        {
            print(1);
            dead = _dead;
            if(dead)
            {
                health = 0;
                spriteTransform.rotation = Quaternion.Euler(0, 0, 90);
            }
        }
        
    }

    public void Resurrect()
    {
        dead = false;
        health = 1;
        print("alive");
    }

    

    public void Heal(int amount)
    {
        if(!isServer)
            return;

        if(!dead)
        {
            health += amount;
        }
    }

    public bool Dead
    {  get {return dead;} }

    public int Health
    { get { return health; } }
    #endregion
    
    public void Stun(float time)
    {
        if(!stunned)
            StartCoroutine(Stunned(time));
    }
    IEnumerator Stunned(float time)
    {
        stunned = true;
        anim.SetTrigger(Tags.hit);
        anim.SetFloat(Tags.velocity_x, 0);
        anim.SetFloat(Tags.velocity_y, 0);
        anim.SetBool(Tags.jumping, false);
        yield return new WaitForSeconds(time);
        UpdateAnimationParameters();
        stunned = false;
    }
}
