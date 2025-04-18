using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TetrisBlockPlacer : MonoBehaviour
{
    public List<TetrisBlock> tetrisBlocks; // ����� ��Ʈ���� ��� ����Ʈ
    public GameObject cubePrefab; // ť�� ������
    public float gridSize = 1f; // �׸��� ũ��
    public TileGridGenerator gridDrawer; // GridLinesDrawer ����

    private TetrisBlock currentBlock; // ���� ���õ� ��Ʈ���� ���
    private GameObject ghostBlock; // ��Ʈ ���
    private Renderer[] ghostRenderers;

    [SerializeField] private Material ghostCubeMaterial; // ��Ʈ ť�� ��Ƽ����
    [SerializeField] private Material placeCubeMaterial; // ��ġ�� ť�� ��Ƽ����

    private Ray debugRay; // ������ ����
    private Vector3? hitPoint; // ���̰� �ε��� ����

    void Start()
    {
        if (tetrisBlocks == null || tetrisBlocks.Count == 0)
        {
            Debug.LogError("TetrisBlocks list is null or empty. Please assign TetrisBlock objects in the inspector.");
            return;
        }

        SelectRandomBlock();
    }

    void Update()
    {
        if (ghostBlock != null)
        {
            // ��Ʈ ��� ��ġ ������Ʈ
            UpdateGhostBlock();

            // ���콺 Ŭ�� �� ��� ��ġ
            if (Input.GetMouseButtonDown(0))
            {
                PlaceBlock();
                SelectRandomBlock(); // ��� ��ġ �� ���ο� ��� ����
            }
        }
    }

    void SelectRandomBlock()
    {
        if (tetrisBlocks.Count == 0)
        {
            Debug.Log("No more blocks available.");
            return;
        }

        // �������� ��� ����
        int randomIndex = Random.Range(0, tetrisBlocks.Count);
        currentBlock = tetrisBlocks[randomIndex];

        // ���� ��Ʈ ��� ����
        if (ghostBlock != null)
        {
            Destroy(ghostBlock);
        }

        // ���ο� ��Ʈ ��� ����
        CreateGhostBlock();
    }

    void CreateGhostBlock()
    {
        // ���� ��Ʈ ��� ����
        if (ghostBlock != null)
        {
            Destroy(ghostBlock);
            ghostBlock = null; // ���� ����
        }

        // ��Ʈ ��� ����
        ghostBlock = new GameObject("GhostBlock");
        ghostRenderers = new Renderer[currentBlock.cubePositions.Length];

        for (int i = 0; i < currentBlock.cubePositions.Length; i++)
        {
            GameObject ghostCube = Instantiate(cubePrefab, ghostBlock.transform);
            ghostCube.transform.localPosition = currentBlock.cubePositions[i];

            // ��Ʈ ����� ��Ƽ���� ����
            Renderer renderer = ghostCube.GetComponent<Renderer>();
            if (ghostCubeMaterial != null)
            {
                renderer.material = ghostCubeMaterial; // ghostCubeMaterial ����
            }
            ghostRenderers[i] = renderer;

            // ��Ʈ ����� ��� �ݶ��̴� ��Ȱ��ȭ
            Collider[] colliders = ghostCube.GetComponents<Collider>();
            foreach (Collider collider in colliders)
            {
                collider.enabled = false;
            }
        }

        // ��Ʈ ����� ��ġ �ʱ�ȭ
        ghostBlock.transform.position = Vector3.zero;
    }

    private Vector3 CalculateGridPosition(Vector3 position)
    {
        // Grid ��ǥ ���
        int gridX = Mathf.RoundToInt(position.x / gridSize);
        int gridY = Mathf.RoundToInt(position.y / gridSize);
        int gridZ = Mathf.RoundToInt(position.z / gridSize);

        return new Vector3(
            gridX * gridSize,
            gridY * gridSize,
            gridZ * gridSize
        );
    }

    private void UpdateGhostBlock()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        debugRay = ray; // ������ ���� ����
        hitPoint = null; // �ʱ�ȭ

        Vector3 snappedPosition = Vector3.zero;
        bool isValidPosition = false;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // �ε��� ������Ʈ�� �߽� ��ǥ�� �������� �׸��� ��ǥ ���
            Vector3 hitObjectCenter = hit.collider.transform.position;
            Vector3 gridPosition = CalculateGridPosition(hitObjectCenter);

            // Normal �������� �̵�
            Vector3 offsetPosition = gridPosition + hit.normal * gridSize;

            // �׸��� ���� ����
            snappedPosition = CalculateGridPosition(offsetPosition);
            isValidPosition = true;

            // �浹 ���� ����
            hitPoint = hit.point;
        }
        else
        {
            // Ray�� �ƹ��͵� �ε����� ���� ���, Grid ��� ���� ��Ʈ ��� ��ġ
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // Y=0 ���
            if (groundPlane.Raycast(ray, out float distance))
            {
                Vector3 pointOnPlane = ray.GetPoint(distance);
                snappedPosition = CalculateGridPosition(pointOnPlane);
                isValidPosition = true;

                // �浹 ���� ����
                hitPoint = pointOnPlane;
            }
        }

        if (isValidPosition)
        {
            // ��Ʈ ��� ��ġ ������Ʈ
            ghostBlock.transform.position = snappedPosition;

            // ��ħ ���� Ȯ��
            bool isOverlapping = IsBlockOverlapping(snappedPosition);

            // ��Ʈ ��� ���� ����
            foreach (Renderer renderer in ghostRenderers)
            {
                renderer.material.color = isOverlapping
                    ? new Color(1, 0, 0, 0.5f) // ������ (������)
                    : new Color(0, 1, 0, 0.5f); // �ʷϻ� (������)
            }
        }
    }

    private bool IsBlockOverlapping(Vector3 position)
    {
        foreach (Vector3 offset in currentBlock.cubePositions)
        {
            // ���� ����� �� ť���� ���� ��ġ ���
            Vector3 cubePosition = position + offset;

            // �ش� ��ġ�� ��Ȯ�� ������ ��ġ�� �ִ� ��ġ�� ����� �ִ��� Ȯ��
            Collider[] colliders = Physics.OverlapBox(
                cubePosition,
                Vector3.one * (gridSize / 2), // �׸��� ũ�� �������� �ڽ� ũ�� ����
                Quaternion.identity
            );

            foreach (Collider collider in colliders)
            {
                // ��Ʈ ����� �����ϰ�, ��Ȯ�� ������ ��ġ�� �ִ��� Ȯ��
                if (collider.gameObject != ghostBlock && collider.transform.position == cubePosition)
                {
                    return true; // ������ ��ģ ���
                }
            }
        }

        return false; // ������ ��ġ�� ����� ���� ���
    }

    private bool PlaceBlock()
    {
        Vector3 position = ghostBlock.transform.position;

        if (IsBlockOverlapping(position))
        {
            Debug.Log("Cannot place block: position is already occupied.");
            return false;
        }

        // ��� ��ġ
        PlaceBlockAtPosition(position);
        return true;
    }

    private void PlaceBlockAtPosition(Vector3 position)
    {
        // ���� ��� ��ġ
        foreach (Vector3 offset in currentBlock.cubePositions)
        {
            // �߽����� �������� ť���� ���� ��ġ ���
            Vector3 cubePosition = position + offset;
            GameObject placedCube = Instantiate(cubePrefab, cubePosition, Quaternion.identity);

            // ��ġ�� ť���� ��Ƽ���� ����
            Renderer renderer = placedCube.GetComponent<Renderer>();
            if (placeCubeMaterial != null)
            {
                renderer.material = placeCubeMaterial; // placeCubeMaterial ����
            }

            // ��ġ�� ť���� ��� �ݶ��̴� Ȱ��ȭ
            Collider[] colliders = placedCube.GetComponents<Collider>();
            foreach (Collider collider in colliders)
            {
                collider.enabled = true;
            }
        }
    }

    private void OnDrawGizmos()
    {
        // ���� �����
        if (debugRay.origin != Vector3.zero)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(debugRay.origin, debugRay.direction * 100f); // ���� �׸���
        }

        // �浹 ���� �����
        if (hitPoint.HasValue)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(hitPoint.Value, 0.2f); // �浹 ������ ��ü �׸���
        }
    }
}
