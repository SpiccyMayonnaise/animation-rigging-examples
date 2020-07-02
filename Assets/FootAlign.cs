using UnityEngine;

public class FootAlign : MonoBehaviour {
    [SerializeField] private Transform target;
    [SerializeField] private Transform footBone;

    [SerializeField] private float raycastStart;
    [SerializeField] private float maxDistance;
    [SerializeField] private float distanceFromGround;

    [SerializeField] private LayerMask ignore;

    private void Update() {
        Vector3 startPos = target.position + Vector3.up * raycastStart;

        Ray ray = new Ray(startPos, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance, ~ignore)) {
            Debug.DrawRay(hit.point, hit.normal, Color.green);
            Debug.DrawLine(startPos, hit.point, Color.red);

            float angle = Vector3.Angle(Vector3.up, hit.normal);

            float verticalModifier = 1 + Mathf.Tan(angle * Mathf.Deg2Rad);

            Debug.Log(verticalModifier);


            target.rotation = Quaternion.LookRotation(target.forward, hit.normal);

            target.position = hit.point + Vector3.up * (verticalModifier * distanceFromGround);
        }
    }
}
