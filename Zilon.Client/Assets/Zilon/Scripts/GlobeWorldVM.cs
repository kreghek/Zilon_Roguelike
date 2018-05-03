using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zilon.Logic.Services;
using Zilon.Logic.Services.Client;

public class GlobeWorldVM : MonoBehaviour {

    public Map Map;
    public SchemeLocator SchemeLocator;

    public GlobeWorldVM() {
        
    }

	// Use this for initialization
	void Start () {
		
	}

    private void Awake()
    {
        var schemeService = new SchemeService(SchemeLocator);
        Map.CreateMapEntities(schemeService, "main");
    }

    // Update is called once per frame
    void Update () {
		
	}
}
