using UnityEngine;

public class BulletDrive : MonoBehaviour
{

	public GameObject StartObject;
	public GameObject FinishObject;
	public float Speed = 1;

	void Start()
	{
		transform.position = StartObject.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = Vector3.MoveTowards(transform.position,
			FinishObject.transform.position,
			Speed * Time.deltaTime);

		var distance = (transform.position - FinishObject.transform.position).magnitude;
		if (distance <= 0.1f)
		{
			Destroy(gameObject);
		}
	}
}
