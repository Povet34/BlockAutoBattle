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
    public float orbitSpeed = 50f; // �˵� ȸ�� �ӵ�
    public float minZoom = 5f; // �ּ� �� �Ÿ�
    public float maxZoom = 20f; // �ִ� �� �Ÿ�
    public float zoomSpeed = 2f; // �� �ӵ�

    private float currentZoom;

    private void Start()
    {
        target = tileGridGenerator.viewTarget.transform;
        currentZoom = cameraDistance;
    }

    private void Update()
    {
        if (target == null) return;

        // �˵� ȸ��
        float horizontalInput = Input.GetAxis("Horizontal");
        transform.RotateAround(target.position, Vector3.up, horizontalInput * orbitSpeed * Time.deltaTime);

        // �� ���
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= scrollInput * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        // ī�޶� ��ġ ���
        Vector3 direction = Quaternion.Euler(cameraAngle, transform.eulerAngles.y, 0) * Vector3.back;
        Vector3 targetPosition = target.position + direction * currentZoom;
        targetPosition.y = Mathf.Max(targetPosition.y, target.position.y + cameraHeight); // Y�� ����

        transform.position = targetPosition;
        transform.LookAt(target);
    }
}