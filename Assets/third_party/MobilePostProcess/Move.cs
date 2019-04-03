
using UnityEngine;

public class Move : MonoBehaviour {
    Vector3 motion = new Vector3(0f, 0f, 1f);
    //MobilePostProcessingSingleton mpps; // edit by Will
	// Use this for initialization
	void Awake () {
        //mpps = MobilePostProcessingSingleton.Instance;
    }
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.position += motion * 0.02f;
	}
}
