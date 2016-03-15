using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LaunchpadController : MonoBehaviour {
    public List<Transform> arms;
    public Transform bigArm;
    bool launchSequenceStarted = true;
    float timer = 0.0f;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if(launchSequenceStarted)
        {
            timer += Time.deltaTime / 5.0f;

            float index = 0;
            foreach (Transform t in arms)
            {
                index+=0.4f;
                float angle = Mathf.LerpAngle(16.45f, 60.0f, timer / index);
                t.localRotation = Quaternion.Euler(Vector3.up * angle);

            }

            float angleBigArm = Mathf.LerpAngle(359.9569f, -40.0f, timer / 5.0f);
            bigArm.localRotation = Quaternion.Euler(Vector3.up * angleBigArm);
            
        }
	}
}
