using UnityEngine;
using System.Collections;

public class SkyboxChanger : MonoBehaviour {
    float changeHeight = 50.0f;
    public Transform rocket;
    float xFade = 0.0f;
    public Skybox spaceBox;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if(rocket.position.y > changeHeight && xFade < 1.0f)
        {
            xFade += Time.deltaTime / 10.0f;
        }

        spaceBox.material.SetFloat("_XFade", xFade);

    }
}
