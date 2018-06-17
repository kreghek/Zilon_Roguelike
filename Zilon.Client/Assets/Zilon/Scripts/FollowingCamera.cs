using System.Linq;
using UnityEngine;

public class FollowingCamera : MonoBehaviour
{

	private ActorVM Target;
	
	// Use this for initialization
	void Start () {
		var actors = FindObjectsOfType<ActorVM>();
		Target = actors.Single(x => !x.IsEnemy);
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.position = Vector3.Lerp(transform.position, 
			Target.transform.position + new Vector3(0, 0, -10),
			Time.deltaTime * 3);
	}
}
