using UnityEngine;
using System.Collections;

public class Apple : MonoBehaviour 
{
	private Transform _transform; 

	// Use this for initialization
	void Start () 
	{
		_transform = this.GetComponent<Transform>();
		StartCoroutine(DestroyMe());
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 pos = _transform.localPosition;
		pos.x += 0.1f;
		_transform.localPosition = pos;
	}

	IEnumerator DestroyMe()
	{
		yield return new WaitForSeconds(1f);

		GameObject.Destroy(this.gameObject);
	}
}
