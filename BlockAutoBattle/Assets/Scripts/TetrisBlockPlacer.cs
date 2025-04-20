using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TetrisBlockPlacer : MonoBehaviour
{
    [SerializeField] Camera cam;
    public List<TetrisBlock> tetrisBlocks; // ����� ��Ʈ���� ��� ����Ʈ
    public GameObject cubePrefab; // ť�� ������
    public float gridSize = 1f; // �׸��� ũ��
    public GridLinesDrawer gridDrawer; // GridLinesDrawer ����

    private TetrisBlock currentBlock; // ���� ���õ� ��Ʈ���� ���
    private GameObject ghostBlock; // ��Ʈ ���
    private Renderer[] ghostRenderers;

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
            if (currentBlock == null)
            {
                Debug.LogError("CurrentBlock is null. Ensure that a valid TetrisBlock is sel��ected.");
                return;
            }
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

            // ��Ʈ ����� ������ ��Ʈ���� ��ϰ� �����ϰ� ���� (������)
            Renderer renderer = ghostCube.GetComponent<Renderer>();
            renderer.material.color = new Color(
                currentBlock.blockColor.r,
                currentBlock.blockColor.g,
                currentBlock.blockColor.b,
                0.5f // ������
            );
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

    bool IsBlockOverlapping(Vector3 position)
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

    void UpdateBlockPosition(bool isPlacing = false)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Vector3 snappedPosition = Vector3.zero;
        bool isValidPosition = false;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Ray�� �ε��� ��ġ�� �������� ��� ��ġ
            Vector3 hitPosition = hit.point;

            // �߽����� �������� �׸��� ���� ����
            Vector3 center = currentBlock.GetCenter();
            int gridX = Mathf.RoundToInt((hitPosition.x - center.x) / gridSize);
            int gridY = Mathf.RoundToInt((hitPosition.y - center.y) / gridSize);
            int gridZ = Mathf.RoundToInt((hitPosition.z - center.z) / gridSize);

            snappedPosition = new Vector3(
                gridX * gridSize + center.x,
                gridY * gridSize + center.y,
                gridZ * gridSize + center.z
            );

            isValidPosition = true;
        }
        else
        {
            // Ray�� �ƹ��͵� �ε����� ���� ���, �⺻ ��� ���� ��Ʈ ��� ��ġ
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // Y=0 ���
            if (groundPlane.Raycast(ray, out float distance))
            {
                Vector3 pointOnPlane = ray.GetPoint(distance);

                // �׸��� ���� ����
                Vector3 center = currentBlock.GetCenter();
                int gridX = Mathf.RoundToInt((pointOnPlane.x - center.x) / gridSize);
                int gridZ = Mathf.RoundToInt((pointOnPlane.z - center.z) / gridSize);

                snappedPosition = new Vector3(
                    gridX * gridSize + center.x,
                    0, // �⺻ ����� Y ��ǥ
                    gridZ * gridSize + center.z
                );

                isValidPosition = true;
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
                    : new Color(currentBlock.blockColor.r, currentBlock.blockColor.g, currentBlock.blockColor.b, 0.5f); // ���� ���� (������)
            }

            // ��� ��ġ ó��
            if (isPlacing && !isOverlapping)
            {
                // ���� ��� ��ġ
                foreach (Vector3 offset in currentBlock.cubePositions)
                {
                    // �߽����� �������� ť���� ���� ��ġ ���
                    Vector3 cubePosition = snappedPosition + offset - currentBlock.GetCenter();
                    GameObject placedCube = Instantiate(cubePrefab, cubePosition, Quaternion.identity);

                    // ��ġ�� ť���� ��� �ݶ��̴� Ȱ��ȭ
                    Collider[] colliders = placedCube.GetComponents<Collider>();
                    foreach (Collider collider in colliders)
                    {
                        collider.enabled = true;
                    }
                }
            }
        }
    }

    private Vector3 CalculateSnappedPosition(Vector3 hitPosition)
    {
        // �߽����� �������� �׸��� ���� ����
        Vector3 center = currentBlock.GetCenter();
        int gridX = Mathf.RoundToInt((hitPosition.x - center.x) / gridSize);
        int gridY = Mathf.RoundToInt((hitPosition.y - center.y) / gridSize);
        int gridZ = Mathf.RoundToInt((hitPosition.z - center.z) / gridSize);

        return new Vector3(
            gridX * gridSize + center.x,
            gridY * gridSize + center.y,
            gridZ * gridSize + center.z
        );
    }

    private void PlaceBlockAtPosition(Vector3 position)
    {
        // ���� ��� ��ġ
        foreach (Vector3 offset in currentBlock.cubePositions)
        {
            // �߽����� �������� ť���� ���� ��ġ ���
            Vector3 cubePosition = position + offset;
            GameObject placedCube = Instantiate(cubePrefab, cubePosition, Quaternion.identity);

            // ��ġ�� ť���� ��� �ݶ��̴� Ȱ��ȭ
            Collider[] colliders = placedCube.GetComponents<Collider>();
            foreach (Collider collider in colliders)
            {
                collider.enabled = true;
            }
        }
    }

    private void UpdateGhostBlock()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Vector3 snappedPosition = Vector3.zero;
        bool isValidPosition = false;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Ray�� �ε��� ��ġ�� �������� ������ ��ġ ���
            snappedPosition = CalculateSnappedPosition(hit.point);
            isValidPosition = true;
        }
        else
        {
            // Ray�� �ƹ��͵� �ε����� ���� ���, �⺻ ��� ���� ��Ʈ ��� ��ġ
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // Y=0 ���
            if (groundPlane.Raycast(ray, out float distance))
            {
                Vector3 pointOnPlane = ray.GetPoint(distance);
                snappedPosition = CalculateSnappedPosition(pointOnPlane);
                isValidPosition = true;
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
                    : new Color(currentBlock.blockColor.r, currentBlock.blockColor.g, currentBlock.blockColor.b, 0.5f); // ���� ���� (������)
            }
        }
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
}
