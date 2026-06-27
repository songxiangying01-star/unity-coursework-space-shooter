using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform targetTr;
    private Transform camTr;

    [Range(2.0f, 20.0f)]
    public float distance = 2.0f;

    [Range(0.0f, 10.0f)]
    public float height = 3.0f;

    // 반응 속도
    public float damping = 1.0f;
    public float targetOffset = 2.0f;
    private Vector3 velocity = Vector3.zero;

    void Start() {
        camTr = GetComponent<Transform>();
    }

    void LateUpdate() {
        Vector3 pos = targetTr.position
                        + (-targetTr.forward * distance)
                        + (Vector3.up * height);
        
        camTr.position = Vector3.SmoothDamp(camTr.position, pos, ref velocity, damping);

        camTr.LookAt(targetTr.position + (targetTr.up * targetOffset));    
    }
}
