using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobeWorldVM : MonoBehaviour {

    public Map Map;

	// Use this for initialization
	void Start () {
		
	}

    private void Awake()
    {
        Map.CreateMapEntities();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
