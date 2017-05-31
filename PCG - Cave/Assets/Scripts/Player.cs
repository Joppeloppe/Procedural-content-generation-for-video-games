using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public float speed = 10;

    private Rigidbody rigidBody;
    private Vector3 velocity;

	void Start () {
        rigidBody = GetComponent<Rigidbody>();
	}
	
	void Update () {
        velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * speed;
	}

    void FixedUpdate()
    {
        rigidBody.MovePosition(rigidBody.position + velocity * Time.fixedDeltaTime);
    }
}
