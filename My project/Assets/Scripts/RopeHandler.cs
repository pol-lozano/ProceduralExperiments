using UnityEngine;

public class RopeHandler : MonoBehaviour
{
    [SerializeField] private Transform p0, p1, p2;

    [Header("Rope Settings")]
    [SerializeField] private int length = 10;
    [SerializeField] private float damping = 12;
    [SerializeField] private float bounce = .93f;
    private Vector3 oldPosition;
    private Vector3 centerPoint;
    private Vector3 ropeSlack;

    [Header("Bezier Renderer")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int numPoints = 100;
    private Vector3[] positions;

    private void Start()
    {
        oldPosition = p1.position;
        positions = new Vector3[numPoints];
        lineRenderer.positionCount = numPoints;
    }

    private void Update()
    {
        UpdatePhysics();
        DrawBezier();
    }

    private void UpdatePhysics()
    {
        centerPoint = (p0.position + p2.position) / 2;
        ropeSlack = Vector3.down * Mathf.Max(0, length - Vector3.Distance(p0.position, p2.position));
        var targetPosition = centerPoint + ropeSlack;

        var currentPosition = p1.position;
        p1.position = Vector3.Lerp(p1.position, targetPosition, (1 - bounce) * damping * Time.deltaTime);
        p1.position = (1 + bounce) * p1.position - bounce * oldPosition;
        oldPosition = currentPosition;
    }

    private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 a, Vector3 b, Vector3 c)
    {
        return Vector3.Lerp(Vector3.Lerp(a, b, t), Vector3.Lerp(b, c, t), t);
    }

    private void DrawBezier()
    {
        for (int i = 1; i <= numPoints; i++)
        {
            float t = i / (float)numPoints;
            positions[i - 1] = CalculateQuadraticBezierPoint(t, p0.position, p1.position, p2.position);
        }
        lineRenderer.SetPositions(positions);
    }

    private void OnDrawGizmos()
    {
        if (p0 == null || p1 == null || p2 == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(centerPoint + ropeSlack, .2f);
        Gizmos.DrawRay(p0.position, p2.position - p0.position);
        Gizmos.DrawRay(centerPoint, ropeSlack);

        Gizmos.color = Color.white;
        Gizmos.DrawSphere(p1.position, .15f);
        Gizmos.DrawRay(p1.position, centerPoint + ropeSlack - p1.position);
    }
}
