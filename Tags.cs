using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum CHARACTERTYPES { king, baby, archer, snake }
public enum GAMETYPES { local, lan, online }

public class Tags : MonoBehaviour {
    //inputs
    public const string horizontal = "Horizontal";
    public const string vertical = "Vertical";
    public const string jump = "Jump";
    public const string fire = "Fire";
    public const string Throw = "Throw";
    public const string drop = "Drop";
    public const string escape = "Escape";
    public const string submit = "Submit";
    public const string cancel = "Cancel";

    //tags
    public const string player = "Player";
    public const string floor = "Floor";
    public const string death = "Death";
    public const string item = "Item";
    public const string upgrade = "Upgrade";

    //names
    public const string firePos = "FirePos";
    public const string spawnLocations = "SpawnLocations";

    //animator parameter
    public const string velocity_x = "Velocity_X";
    public const string velocity_y = "Velocity_Y";
    public const string hit = "Hit";
    public const string jumping = "Jumping";
}

public delegate void standardAnnouncement();
public delegate void intAnnouncement(int i);

public interface Damageable
{
    void Kill();
    void Resurrect();

    void Damage(int amount);
    void Heal(int amount);

    bool Dead { get; }
    int  Health { get; }
    
    event intAnnouncement OnHit;
    event standardAnnouncement OnDeath;
}

public interface Holdable
{
    bool IsEquiped { get; }
    void Pickup(GameObject ObjectPlayerController, Vector3 localPosition);
    void Drop();
    void Throw(float speed);
    void Fire();
}

public interface Stunnable
{
    void Stun(float time);
}

public interface PickUpAble
{
    void Pickup(GameObject ObjectPlayerController, Vector3 localPosition);
}





public static class Util
{
    private static System.Random rng = new System.Random();

    public static T GetRandom<T>(this IList<T> list)
    {
        int i = Random.Range(0, list.Count);
        return list[i];
    }
    

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while(n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
