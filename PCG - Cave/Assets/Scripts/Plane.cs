using UnityEngine;
using System.Collections;

public class Plane : MonoBehaviour {

    public MeshGenerator meshGen;

    void Start()
    {
        transform.position += new Vector3(0, -meshGen.wallHeight, 0);
    }
}
