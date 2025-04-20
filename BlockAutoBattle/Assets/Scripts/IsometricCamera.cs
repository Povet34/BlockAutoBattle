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
    public float orbitSpeed = 100f; // 궤도 회전 속도
    public float minZoom = 5f; // 최소 줌 거리
    public float maxZoom = 20f; // 최대 줌 거리
    public float zoomSpeed = 2f; // 줌 속도

    private float currentZoom;
    private float currentRotationY; // 현재 Y축 회전 값
    private Vector3 lastMousePosition; // 이전 프레임의 마우스 위치

    private void Start()
    {
        if (tileGridGenerator == null)
        {
            Debug.LogError("TileGridGenerator is not assigned.");
            return;
        }

        target = tileGridGenerator.viewTarget.transform;
        currentZoom = cameraDistance;
        currentRotationY = transform.eulerAngles.y;
    }

    private void Update()
    {
        if (target == null) return;

        // 마우스 오른쪽 버튼을 눌렀을 때 궤도 회전
        if (Input.GetMouseButton(1)) // 마우스 오른쪽 버튼
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition; // 마우스 이동량 계산
            currentRotationY += mouseDelta.x * orbitSpeed * Time.deltaTime; // X축 이동량으로 Y축 회전
        }

        // 줌 기능
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= scrollInput * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        // 카메라 위치 계산
        Vector3 direction = Quaternion.Euler(cameraAngle, currentRotationY, 0) * Vector3.back;
        Vector3 targetPosition = target.position + direction * currentZoom;
        targetPosition.y = Mathf.Max(targetPosition.y, target.position.y + cameraHeight); // Y축 제한

        transform.position = targetPosition;
        transform.LookAt(target);

        // 현재 마우스 위치 저장
        lastMousePosition = Input.mousePosition;
    }
}
