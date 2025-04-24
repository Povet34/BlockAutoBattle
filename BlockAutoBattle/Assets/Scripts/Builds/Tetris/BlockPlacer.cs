using UnityEngine;
using System.Collections.Generic;

public class BlockPlacer : MonoBehaviour
{
    [SerializeField] private Camera placeCam;

    public GameObject tetrisBlockPrefab; // TetrisBlock 프리팹
    [SerializeField] private Material ghostCubeMaterial; // 고스트 큐브 머티리얼
    [SerializeField] private Material placedCubeMaterial; // 배치된 큐브 머티리얼

    private TetrisBlock currentGhostBlock; // 현재 고스트 블럭
    private Player player;

    [Header("Grid Settings")]
    public float gridSize = 1f; // 그리드 크기
    public int gridWidth = 10; // 그리드의 가로 크기
    public int gridHeight = 10; // 그리드의 세로 크기

    public void Initialize(Player player)
    {
        this.player = player;
    }

    public void CreateGhostBlock(TetrisBlockData blockData)
    {
        if (blockData == null)
        {
            Debug.LogError("TetrisBlockData가 null입니다. 고스트 블럭을 생성할 수 없습니다.");
            return;
        }

        // 기존 고스트 블럭 제거
        if (currentGhostBlock != null)
        {
            Destroy(currentGhostBlock.gameObject);
        }

        // 새로운 고스트 블럭 생성
        GameObject ghostObject = Instantiate(tetrisBlockPrefab);
        currentGhostBlock = ghostObject.GetComponent<TetrisBlock>();
        currentGhostBlock.Initialize(blockData, ghostCubeMaterial);

        // 고스트 블럭의 콜라이더 비활성화
        currentGhostBlock.EnableColliders(false);

        Debug.Log($"고스트 블럭 생성: {blockData.blockName}");
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
            // 부딪힌 오브젝트의 중심 좌표를 기준으로 그리드 좌표 계산
            Vector3 hitObjectCenter = hit.collider.transform.position;
            Vector3 gridPosition = CalculateGridPosition(hitObjectCenter);

            // Normal 방향으로 이동
            Vector3 offsetPosition = gridPosition + hit.normal * gridSize;
            snappedPosition = CalculateGridPosition(offsetPosition);
            isValidPosition = true;
        }
        else
        {
            // Ray가 아무것도 부딪히지 않은 경우, Grid 표면 위에 고스트 블록 배치
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // Y=0 평면
            if (groundPlane.Raycast(ray, out float distance))
            {
                Vector3 pointOnPlane = ray.GetPoint(distance);
                snappedPosition = CalculateGridPosition(pointOnPlane);
                isValidPosition = true;
            }
        }

        if (isValidPosition)
        {
            // 고스트 블록 위치 업데이트
            currentGhostBlock.SetPosition(snappedPosition);

            // 고스트 블록 색상 업데이트
            UpdateGhostBlockColor(snappedPosition);
        }
    }

    private void UpdateGhostBlockColor(Vector3 position)
    {
        // 겹침 여부 확인
        bool isOverlapping = IsBlockOverlapping(position);

        // 색상 설정
        Color ghostColor = isOverlapping ? new Color(1, 0, 0, 0.5f) : new Color(0, 1, 0, 0.5f);
        currentGhostBlock.SetColor(ghostColor);
    }

    private Vector3 CalculateGridPosition(Vector3 position)
    {
        // Grid 좌표 계산
        int gridX = Mathf.RoundToInt(position.x / gridSize);
        int gridY = Mathf.RoundToInt(position.y / gridSize);
        int gridZ = Mathf.RoundToInt(position.z / gridSize);

        // Grid 경계 제한
        gridX = Mathf.Clamp(gridX, 0, gridWidth - 1);
        gridY = Mathf.Clamp(gridY, 0, gridHeight - 1);
        gridZ = Mathf.Clamp(gridZ, 0, gridWidth - 1); // 가로와 깊이가 동일하다고 가정

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
            Debug.LogError("고스트 블럭이 없습니다. 블럭을 배치할 수 없습니다.");
            return;
        }

        Vector3 position = currentGhostBlock.transform.position;

        // 블럭 배치 가능 여부 확인
        if (IsBlockOverlapping(position))
        {
            Debug.Log("Cannot place block: position is already occupied.");
            return;
        }

        // 블럭 배치
        GameObject placedObject = Instantiate(tetrisBlockPrefab, position, Quaternion.identity);
        TetrisBlock placedBlock = placedObject.GetComponent<TetrisBlock>();

        // 배치된 블럭 초기화
        placedBlock.Initialize(currentGhostBlock.GetBlockData(), placedCubeMaterial);
        placedBlock.SetCubePositions(currentGhostBlock.GetCubePositions());
        placedBlock.EnableColliders(true);

        Debug.Log($"블럭 배치 완료: {currentGhostBlock.GetBlockData().blockName}");

        // 고스트 블럭 제거
        Destroy(currentGhostBlock.gameObject);
        currentGhostBlock = null;
    }

    private bool IsBlockOverlapping(Vector3 position)
    {
        foreach (Vector3 offset in currentGhostBlock.GetCubePositions())
        {
            // 현재 블록의 각 큐브의 실제 위치 계산
            Vector3 cubePosition = position + offset;

            // 해당 위치에 정확히 동일한 위치에 있는 배치된 블록이 있는지 확인
            Collider[] colliders = Physics.OverlapBox(
                cubePosition,
                Vector3.one * (gridSize / 2 * 0.9f), // 박스 크기를 gridSize보다 약간 작게 설정
                Quaternion.identity
            );

            foreach (Collider collider in colliders)
            {
                // 고스트 블록은 제외하고, 정확히 동일한 위치에 있는지 확인
                if (collider.gameObject != currentGhostBlock.gameObject)
                {
                    // 두 위치가 정확히 겹치는 경우만 true 반환
                    if (Vector3.Distance(collider.transform.position, cubePosition) < gridSize * 0.1f)
                    {
                        return true; // 완전히 겹친 경우
                    }
                }
            }
        }

        return false; // 완전히 겹치는 블록이 없는 경우
    }
}
