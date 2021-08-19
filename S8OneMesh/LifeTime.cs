using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeTime : MonoBehaviour {

    public float _duration=5;
	void Awake () {
        Destroy(this.gameObject, _duration);
	}
	
}
