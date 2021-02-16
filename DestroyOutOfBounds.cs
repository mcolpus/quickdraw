using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOutOfBounds : MonoBehaviour {
    public static float _maxX = 20;
    public static float _minX = -20;
    public static float _maxY = 30;
    public static float _minY = -30;
    public bool SetLevelBounds = false;
    public bool useLevelBounds = true;
    public float maxX = 20;
    public float minX = -20;
    public float maxY = 30;
    public float minY = -30;

    void Awake()
    {
        if(SetLevelBounds)
        {
            _maxX = maxX;
            _minX = minX;
            _maxY = maxY;
            _minY = minY;
        }
    }

    void Start()
    {
        if(useLevelBounds)
        {
            maxX = _maxX;
            minX = _minX;
            maxY = _maxY;
            minY = _minY;
        }
    }

	void Update()
    {
        Vector3 pos = transform.position;
        if(pos.x > maxX || pos.x < minX || pos.y > maxY || pos.y < minY)
            Destroy(gameObject);
    }
}
