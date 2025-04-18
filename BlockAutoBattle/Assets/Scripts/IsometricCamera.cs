using UnityEngine;

[ExecuteInEditMode]
public class IsometricCamera : MonoBehaviour
{
    [SerializeField] TileGridGenerator tileGridGenerator;

    [Header("Isometric View Settings")]
    private Transform target; // ī�޶� ���� Ÿ��
    public float cameraHeight = 10f; // ī�޶� ����
    public float cameraDistance = 10f; // ī�޶� �Ÿ�
    public float cameraAngle = 30f; // ī�޶� ����

    [Header("Orbit Settings")]
    public float orbitSpeed = 100f; // �˵� ȸ�� �ӵ�
    public float minZoom = 5f; // �ּ� �� �Ÿ�
    public float maxZoom = 20f; // �ִ� �� �Ÿ�
    public float zoomSpeed = 2f; // �� �ӵ�

    private float currentZoom;
    private float currentRotationY; // ���� Y�� ȸ�� ��
    private Vector3 lastMousePosition; // ���� �������� ���콺 ��ġ

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

        // ���콺 ������ ��ư�� ������ �� �˵� ȸ��
        if (Input.GetMouseButton(1)) // ���콺 ������ ��ư
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition; // ���콺 �̵��� ���
            currentRotationY += mouseDelta.x * orbitSpeed * Time.deltaTime; // X�� �̵������� Y�� ȸ��
        }

        // �� ���
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= scrollInput * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        // ī�޶� ��ġ ���
        Vector3 direction = Quaternion.Euler(cameraAngle, currentRotationY, 0) * Vector3.back;
        Vector3 targetPosition = target.position + direction * currentZoom;
        targetPosition.y = Mathf.Max(targetPosition.y, target.position.y + cameraHeight); // Y�� ����

        transform.position = targetPosition;
        transform.LookAt(target);

        // ���� ���콺 ��ġ ����
        lastMousePosition = Input.mousePosition;
    }
}
