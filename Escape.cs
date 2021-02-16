using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escape : MonoBehaviour {
    
	void Update () {
        if(MyInputs.GetButtonDown(0, Tags.escape))
        {
            Application.Quit();
        }
	}
}
