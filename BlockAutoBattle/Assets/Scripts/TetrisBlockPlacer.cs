using UnityEngine;
using System.Collections.Generic;

public class TetrisBlockPlacer : MonoBehaviour
{
    public List<TetrisBlockData> tetrisBlocks; // ����� ��Ʈ���� ��� ����Ʈ
    public GameObject cubePrefab; // ť�� ������
    public GameObject tetrisBlockPrefab; // TetrisBlock ������
    public float gridSize = 1f; // �׸��� ũ��
    public TileGridGenerator gridDrawer; // GridLinesDrawer ����

    [SerializeField] private Material ghostCubeMaterial; // ��Ʈ ť�� ��Ƽ����
    [SerializeField] private Material placeCubeMaterial; // ��ġ�� ť�� ��Ƽ����

    private TetrisBlock currentGhostBlock; // ���� ��Ʈ ��
    private TetrisBlockData currentBlockData; // ���� ���õ� ��Ʈ���� �� ������

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
        if (currentGhostBlock != null)
        {
            // ��Ʈ ��� ��ġ ������Ʈ
            UpdateGhostBlock();

            // ȸ�� �Է� ó��
            HandleRotationInput();

            // ���콺 Ŭ�� �� ��� ��ġ
            if (Input.GetMouseButtonDown(0))
            {
                if(PlaceBlock())
                {
                    SelectRandomBlock(); // ��� ��ġ �� ���ο� ��� ����
                }
            }
        }
    }

    private void HandleRotationInput()
    {
        // X�� ȸ�� (A, D)
        if (Input.GetKeyDown(KeyCode.A))
        {
            currentGhostBlock.Rotate(Vector3.right);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            currentGhostBlock.Rotate(-Vector3.right);
        }

        // Y�� ȸ�� (W, S)
        if (Input.GetKeyDown(KeyCode.W))
        {
            currentGhostBlock.Rotate(Vector3.up);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            currentGhostBlock.Rotate(-Vector3.up);
        }

        // Z�� ȸ�� (Q, E)
        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentGhostBlock.Rotate(Vector3.forward);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            currentGhostBlock.Rotate(-Vector3.forward);
        }
    }

    private void SelectRandomBlock()
    {
        if (tetrisBlocks.Count == 0)
        {
            Debug.Log("No more blocks available.");
            return;
        }

        // �������� ��� ����
        int randomIndex = Random.Range(0, tetrisBlocks.Count);
        currentBlockData = tetrisBlocks[randomIndex];

        // ���� ��Ʈ ��� ����
        if (currentGhostBlock != null)
        {
            Destroy(currentGhostBlock.gameObject);
        }

        // ���ο� ��Ʈ ��� ����
        GameObject ghostObject = Instantiate(tetrisBlockPrefab);
        currentGhostBlock = ghostObject.GetComponent<TetrisBlock>();
        currentGhostBlock.Initialize(currentBlockData, ghostCubeMaterial);

        // ��Ʈ ���� �ݶ��̴� ��Ȱ��ȭ
        currentGhostBlock.EnableColliders(false);
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
            }
        }

        if (isValidPosition)
        {
            // ��Ʈ ��� ��ġ ������Ʈ
            currentGhostBlock.SetPosition(snappedPosition);

            // ��ħ ���� Ȯ��
            bool isOverlapping = IsBlockOverlapping(snappedPosition);

            // ��Ʈ ��� ���� ����
            currentGhostBlock.SetColor(isOverlapping ? new Color(1, 0, 0, 0.5f) : new Color(0, 1, 0, 0.5f));
        }
    }

    private bool IsBlockOverlapping(Vector3 position)
    {
        foreach (Vector3 offset in currentBlockData.cubePositions)
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
                if (collider.gameObject != currentGhostBlock.gameObject && collider.transform.position == cubePosition)
                {
                    return true; // ������ ��ģ ���
                }
            }
        }

        return false; // ������ ��ġ�� ����� ���� ���
    }

    private bool PlaceBlock()
    {
        Vector3 position = currentGhostBlock.transform.position;

        if (IsBlockOverlapping(position))
        {
            Debug.Log("Cannot place block: position is already occupied.");
            return false;
        }

        // ��� ��ġ
        GameObject placedObject = Instantiate(tetrisBlockPrefab);
        TetrisBlock placedBlock = placedObject.GetComponent<TetrisBlock>();
        placedBlock.Initialize(currentBlockData, placeCubeMaterial);

        // ��Ʈ ���� ��ġ�� ȸ�� ���¸� ��ġ�� ���� ����
        placedBlock.SetPosition(position);
        placedBlock.transform.rotation = currentGhostBlock.transform.rotation;

        // ��ġ�� ���� �ݶ��̴� Ȱ��ȭ
        placedBlock.EnableColliders(true);

        return true;
    }
}
