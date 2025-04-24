using UnityEngine;
using System.Collections.Generic;

public class BlockPlacer : MonoBehaviour
{
    [SerializeField] private Camera placeCam;

    public GameObject tetrisBlockPrefab; // TetrisBlock ������
    [SerializeField] private Material ghostCubeMaterial; // ��Ʈ ť�� ��Ƽ����
    [SerializeField] private Material placedCubeMaterial; // ��ġ�� ť�� ��Ƽ����

    private TetrisBlock currentGhostBlock; // ���� ��Ʈ ��
    private Player player;

    [Header("Grid Settings")]
    public float gridSize = 1f; // �׸��� ũ��
    public int gridWidth = 10; // �׸����� ���� ũ��
    public int gridHeight = 10; // �׸����� ���� ũ��

    public void Initialize(Player player)
    {
        this.player = player;
    }

    public void CreateGhostBlock(TetrisBlockData blockData)
    {
        if (blockData == null)
        {
            Debug.LogError("TetrisBlockData�� null�Դϴ�. ��Ʈ ���� ������ �� �����ϴ�.");
            return;
        }

        // ���� ��Ʈ �� ����
        if (currentGhostBlock != null)
        {
            Destroy(currentGhostBlock.gameObject);
        }

        // ���ο� ��Ʈ �� ����
        GameObject ghostObject = Instantiate(tetrisBlockPrefab);
        currentGhostBlock = ghostObject.GetComponent<TetrisBlock>();
        currentGhostBlock.Initialize(blockData, ghostCubeMaterial);

        // ��Ʈ ���� �ݶ��̴� ��Ȱ��ȭ
        currentGhostBlock.EnableColliders(false);

        Debug.Log($"��Ʈ �� ����: {blockData.blockName}");
    }

    private void Update()
    {
        if (currentGhostBlock != null)
        {
            UpdateGhostBlock();
            HandleRotationInput();

            if (InputManager.Instance.PlaceBlock)
            {
                PlaceBlock();
            }
        }
    }

    private void HandleRotationInput()
    {
        switch (true)
        {
            case var _ when InputManager.Instance.RotateXPositive:
                currentGhostBlock.Rotate(Vector3.right);
                break;
            case var _ when InputManager.Instance.RotateXNegative:
                currentGhostBlock.Rotate(-Vector3.right);
                break;
            case var _ when InputManager.Instance.RotateYPositive:
                currentGhostBlock.Rotate(Vector3.up);
                break;
            case var _ when InputManager.Instance.RotateYNegative:
                currentGhostBlock.Rotate(-Vector3.up);
                break;
            case var _ when InputManager.Instance.RotateZPositive:
                currentGhostBlock.Rotate(Vector3.forward);
                break;
            case var _ when InputManager.Instance.RotateZNegative:
                currentGhostBlock.Rotate(-Vector3.forward);
                break;
        }
    }

    private void UpdateGhostBlock()
    {
        Ray ray = placeCam.ScreenPointToRay(Input.mousePosition);
        Vector3 snappedPosition = Vector3.zero;
        bool isValidPosition = false;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // �ε��� ������Ʈ�� �߽� ��ǥ�� �������� �׸��� ��ǥ ���
            Vector3 hitObjectCenter = hit.collider.transform.position;
            Vector3 gridPosition = CalculateGridPosition(hitObjectCenter);

            // Normal �������� �̵�
            Vector3 offsetPosition = gridPosition + hit.normal * gridSize;
            snappedPosition = CalculateGridPosition(offsetPosition);
            isValidPosition = true;
        }
        else
        {
            // Ray�� �ƹ��͵� �ε����� ���� ���, Grid ǥ�� ���� ��Ʈ ��� ��ġ
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

            // ��Ʈ ��� ���� ������Ʈ
            UpdateGhostBlockColor(snappedPosition);
        }
    }

    private void UpdateGhostBlockColor(Vector3 position)
    {
        // ��ħ ���� Ȯ��
        bool isOverlapping = IsBlockOverlapping(position);

        // ���� ����
        Color ghostColor = isOverlapping ? new Color(1, 0, 0, 0.5f) : new Color(0, 1, 0, 0.5f);
        currentGhostBlock.SetColor(ghostColor);
    }

    private Vector3 CalculateGridPosition(Vector3 position)
    {
        // Grid ��ǥ ���
        int gridX = Mathf.RoundToInt(position.x / gridSize);
        int gridY = Mathf.RoundToInt(position.y / gridSize);
        int gridZ = Mathf.RoundToInt(position.z / gridSize);

        // Grid ��� ����
        gridX = Mathf.Clamp(gridX, 0, gridWidth - 1);
        gridY = Mathf.Clamp(gridY, 0, gridHeight - 1);
        gridZ = Mathf.Clamp(gridZ, 0, gridWidth - 1); // ���ο� ���̰� �����ϴٰ� ����

        return new Vector3(
            gridX * gridSize,
            gridY * gridSize,
            gridZ * gridSize
        );
    }

    public void PlaceBlock()
    {
        if (currentGhostBlock == null)
        {
            Debug.LogError("��Ʈ ���� �����ϴ�. ���� ��ġ�� �� �����ϴ�.");
            return;
        }

        Vector3 position = currentGhostBlock.transform.position;

        // �� ��ġ ���� ���� Ȯ��
        if (IsBlockOverlapping(position))
        {
            Debug.Log("Cannot place block: position is already occupied.");
            return;
        }

        // �� ��ġ
        GameObject placedObject = Instantiate(tetrisBlockPrefab, position, Quaternion.identity);
        TetrisBlock placedBlock = placedObject.GetComponent<TetrisBlock>();

        // ��ġ�� �� �ʱ�ȭ
        placedBlock.Initialize(currentGhostBlock.GetBlockData(), placedCubeMaterial);
        placedBlock.SetCubePositions(currentGhostBlock.GetCubePositions());
        placedBlock.EnableColliders(true);

        Debug.Log($"�� ��ġ �Ϸ�: {currentGhostBlock.GetBlockData().blockName}");

        // ��Ʈ �� ����
        Destroy(currentGhostBlock.gameObject);
        currentGhostBlock = null;
    }

    private bool IsBlockOverlapping(Vector3 position)
    {
        foreach (Vector3 offset in currentGhostBlock.GetCubePositions())
        {
            // ���� ����� �� ť���� ���� ��ġ ���
            Vector3 cubePosition = position + offset;

            // �ش� ��ġ�� ��Ȯ�� ������ ��ġ�� �ִ� ��ġ�� ����� �ִ��� Ȯ��
            Collider[] colliders = Physics.OverlapBox(
                cubePosition,
                Vector3.one * (gridSize / 2 * 0.9f), // �ڽ� ũ�⸦ gridSize���� �ణ �۰� ����
                Quaternion.identity
            );

            foreach (Collider collider in colliders)
            {
                // ��Ʈ ����� �����ϰ�, ��Ȯ�� ������ ��ġ�� �ִ��� Ȯ��
                if (collider.gameObject != currentGhostBlock.gameObject)
                {
                    // �� ��ġ�� ��Ȯ�� ��ġ�� ��츸 true ��ȯ
                    if (Vector3.Distance(collider.transform.position, cubePosition) < gridSize * 0.1f)
                    {
                        return true; // ������ ��ģ ���
                    }
                }
            }
        }

        return false; // ������ ��ġ�� ����� ���� ���
    }
}
