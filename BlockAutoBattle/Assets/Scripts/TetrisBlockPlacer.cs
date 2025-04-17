using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TetrisBlockPlacer : MonoBehaviour
{
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
            UpdateGhostBlockPosition();

            if (Input.GetMouseButtonDown(0))
            {
                if (PlaceBlock())
                {
                    SelectRandomBlock(); // ��� ��ġ �� ���ο� ��� ����
                }
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

    void UpdateGhostBlockPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Ray�� �ε��� ��ġ�� �������� ��Ʈ ��� ��ġ
            Vector3 hitPosition = hit.point;

            // �׸��� ���� ����
            int gridX = Mathf.RoundToInt(hitPosition.x / gridSize);
            int gridY = Mathf.RoundToInt(hitPosition.y / gridSize);
            int gridZ = Mathf.RoundToInt(hitPosition.z / gridSize);

            Vector3 snappedPosition = new Vector3(
                gridX * gridSize,
                gridY * gridSize,
                gridZ * gridSize
            );

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
        else
        {
            // Ray�� �ƹ��͵� �ε����� ���� ���, �⺻ ��� ���� ��Ʈ ��� ��ġ
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // Y=0 ���
            if (groundPlane.Raycast(ray, out float distance))
            {
                Vector3 pointOnPlane = ray.GetPoint(distance);

                // �׸��� ���� ����
                int gridX = Mathf.RoundToInt(pointOnPlane.x / gridSize);
                int gridZ = Mathf.RoundToInt(pointOnPlane.z / gridSize);

                Vector3 snappedPosition = new Vector3(
                    gridX * gridSize,
                    0, // �⺻ ����� Y ��ǥ
                    gridZ * gridSize
                );

                ghostBlock.transform.position = snappedPosition;

                // ��Ʈ ��� ������ �⺻������ �ʷϻ� (������)
                foreach (Renderer renderer in ghostRenderers)
                {
                    renderer.material.color = new Color(currentBlock.blockColor.r, currentBlock.blockColor.g, currentBlock.blockColor.b, 0.5f);
                }
            }
        }
    }

    bool IsBlockOverlapping(Vector3 position)
    {
        foreach (Vector3 offset in currentBlock.cubePositions)
        {
            Vector3 cubePosition = position + offset;
            Collider[] colliders = Physics.OverlapBox(
                cubePosition,
                Vector3.one * (gridSize / 2),
                Quaternion.identity
            );

            foreach (Collider collider in colliders)
            {
                // ��ħ Ȯ��
                if (collider.gameObject != ghostBlock)
                {
                    return true;
                }
            }
        }

        return false;
    }

    bool PlaceBlock()
    {
        Vector3 position = ghostBlock.transform.position;

        if (IsBlockOverlapping(position))
        {
            Debug.Log("Cannot place block: position is already occupied.");
            return false;
        }

        // ��� ��ġ
        foreach (Vector3 offset in currentBlock.cubePositions)
        {
            Vector3 cubePosition = position + offset;
            GameObject placedCube = Instantiate(cubePrefab, cubePosition, Quaternion.identity);

            // ��ġ�� ť���� ��� �ݶ��̴� Ȱ��ȭ
            Collider[] colliders = placedCube.GetComponents<Collider>();
            foreach (Collider collider in colliders)
            {
                collider.enabled = true;
            }
        }

        return true;
    }
}
