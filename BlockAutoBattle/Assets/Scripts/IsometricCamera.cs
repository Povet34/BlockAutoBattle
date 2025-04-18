using UnityEngine;

[ExecuteInEditMode]
public class IsometricCamera : MonoBehaviour
{
    [SerializeField] TileGridGenerator tileGridGenerator;

    [Header("Isometric View Settings")]
    private Transform target; // 카메라가 따라갈 타겟
    public float cameraHeight = 10f; // 카메라 높이
    public float cameraDistance = 10f; // 카메라 거리
    public float cameraAngle = 30f; // 카메라 각도

    [Header("Orbit Settings")]
    public float orbitSpeed = 50f; // 궤도 회전 속도
    public float minZoom = 5f; // 최소 줌 거리
    public float maxZoom = 20f; // 최대 줌 거리
    public float zoomSpeed = 2f; // 줌 속도

    private float currentZoom;

    private void Start()
    {
        target = tileGridGenerator.viewTarget.transform;
        currentZoom = cameraDistance;
    }

    private void Update()
    {
        if (target == null) return;

        // 궤도 회전
        float horizontalInput = Input.GetAxis("Horizontal");
        transform.RotateAround(target.position, Vector3.up, horizontalInput * orbitSpeed * Time.deltaTime);

        // 줌 기능
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= scrollInput * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        // 카메라 위치 계산
        Vector3 direction = Quaternion.Euler(cameraAngle, transform.eulerAngles.y, 0) * Vector3.back;
        Vector3 targetPosition = target.position + direction * currentZoom;
        targetPosition.y = Mathf.Max(targetPosition.y, target.position.y + cameraHeight); // Y축 제한

        transform.position = targetPosition;
        transform.LookAt(target);
    }
}