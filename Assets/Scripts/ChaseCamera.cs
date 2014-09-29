using UnityEngine;

public class ChaseCamera : MonoBehaviour
{
    public GameObject ChaseGameObject;

	void Update () {
        transform.position = ChaseGameObject.transform.position + new Vector3(0, 5f, -5f);
        transform.LookAt(ChaseGameObject.transform.position);
	}
}
