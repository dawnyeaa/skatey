using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class FuckYeah : MonoBehaviour {
    private const float GIZMO_DISK_THICKNESS = 0.01f;
    [Range(-1,1)]
    public float turn = 0;
    public Animator skateanim;

    // public static void DrawGizmoDisk(Vector3 pos, float radius) {
    //     Matrix4x4 oldMatrix = Gizmos.matrix;
    //     Gizmos.color = new Color(0.2f, 0.2f, 0.2f, 0.5f); //this is gray, could be anything
    //     Gizmos.matrix = Matrix4x4.TRS(pos, Quaternion.identity, new Vector3(1, GIZMO_DISK_THICKNESS, 1));
    //     Gizmos.DrawSphere(pos, radius);
    //     Gizmos.matrix = oldMatrix;
    // }

    // Update is called once per frame
    void Update() {
        skateanim.SetFloat("dir", turn);
    }

    void OnDrawGizmos() {
        if (turn==0) {
            Gizmos.DrawRay(transform.position-100*transform.right, transform.right*200);
        }
        else {
            float radius = 0.205f/Mathf.Sin(Mathf.Deg2Rad*turn*8.34f);
            Gizmos.DrawWireSphere(transform.position + (Vector3.forward*radius), Mathf.Abs(radius));
        }
    }
}
