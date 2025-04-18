using UnityEngine;
using System.Collections.Generic;

public class TetrisBlockPlacer : MonoBehaviour
{
    public List<TetrisBlockData> tetrisBlocks; // 사용할 테트리스 블록 리스트
    public GameObject cubePrefab; // 큐브 프리팹
    public GameObject tetrisBlockPrefab; // TetrisBlock 프리팹
    public float gridSize = 1f; // 그리드 크기
    public TileGridGenerator gridDrawer; // GridLinesDrawer 참조

    [SerializeField] private Material ghostCubeMaterial; // 고스트 큐브 머티리얼
    [SerializeField] private Material placeCubeMaterial; // 배치된 큐브 머티리얼

    private TetrisBlock currentGhostBlock; // 현재 고스트 블럭
    private TetrisBlockData currentBlockData; // 현재 선택된 테트리스 블럭 데이터

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
            // 고스트 블록 위치 업데이트
            UpdateGhostBlock();

            // 회전 입력 처리
            HandleRotationInput();

            // 마우스 클릭 시 블록 배치
            if (Input.GetMouseButtonDown(0))
            {
                if(PlaceBlock())
                {
                    SelectRandomBlock(); // 블록 배치 후 새로운 블록 선택
                }
            }
        }
    }

    private void HandleRotationInput()
    {
        // X축 회전 (A, D)
        if (Input.GetKeyDown(KeyCode.A))
        {
            currentGhostBlock.Rotate(Vector3.right);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            currentGhostBlock.Rotate(-Vector3.right);
        }

        // Y축 회전 (W, S)
        if (Input.GetKeyDown(KeyCode.W))
        {
            currentGhostBlock.Rotate(Vector3.up);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            currentGhostBlock.Rotate(-Vector3.up);
        }

        // Z축 회전 (Q, E)
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

        // 랜덤으로 블록 선택
        int randomIndex = Random.Range(0, tetrisBlocks.Count);
        currentBlockData = tetrisBlocks[randomIndex];

        // 기존 고스트 블록 제거
        if (currentGhostBlock != null)
        {
            Destroy(currentGhostBlock.gameObject);
        }

        // 새로운 고스트 블록 생성
        GameObject ghostObject = Instantiate(tetrisBlockPrefab);
        currentGhostBlock = ghostObject.GetComponent<TetrisBlock>();
        currentGhostBlock.Initialize(currentBlockData, ghostCubeMaterial);

        // 고스트 블럭의 콜라이더 비활성화
        currentGhostBlock.EnableColliders(false);
    }

    private Vector3 CalculateGridPosition(Vector3 position)
    {
        // Grid 좌표 계산
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
            // 부딪힌 오브젝트의 중심 좌표를 기준으로 그리드 좌표 계산
            Vector3 hitObjectCenter = hit.collider.transform.position;
            Vector3 gridPosition = CalculateGridPosition(hitObjectCenter);

            // Normal 방향으로 이동
            Vector3 offsetPosition = gridPosition + hit.normal * gridSize;

            // 그리드 스냅 적용
            snappedPosition = CalculateGridPosition(offsetPosition);
            isValidPosition = true;
        }
        else
        {
            // Ray가 아무것도 부딪히지 않은 경우, Grid 평면 위에 고스트 블록 배치
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

            // 겹침 여부 확인
            bool isOverlapping = IsBlockOverlapping(snappedPosition);

            // 고스트 블록 색상 변경
            currentGhostBlock.SetColor(isOverlapping ? new Color(1, 0, 0, 0.5f) : new Color(0, 1, 0, 0.5f));
        }
    }

    private bool IsBlockOverlapping(Vector3 position)
    {
        foreach (Vector3 offset in currentBlockData.cubePositions)
        {
            // 현재 블록의 각 큐브의 실제 위치 계산
            Vector3 cubePosition = position + offset;

            // 해당 위치에 정확히 동일한 위치에 있는 배치된 블록이 있는지 확인
            Collider[] colliders = Physics.OverlapBox(
                cubePosition,
                Vector3.one * (gridSize / 2), // 그리드 크기 기준으로 박스 크기 설정
                Quaternion.identity
            );

            foreach (Collider collider in colliders)
            {
                // 고스트 블록은 제외하고, 정확히 동일한 위치에 있는지 확인
                if (collider.gameObject != currentGhostBlock.gameObject && collider.transform.position == cubePosition)
                {
                    return true; // 완전히 겹친 경우
                }
            }
        }

        return false; // 완전히 겹치는 블록이 없는 경우
    }

    private bool PlaceBlock()
    {
        Vector3 position = currentGhostBlock.transform.position;

        if (IsBlockOverlapping(position))
        {
            Debug.Log("Cannot place block: position is already occupied.");
            return false;
        }

        // 블록 배치
        GameObject placedObject = Instantiate(tetrisBlockPrefab);
        TetrisBlock placedBlock = placedObject.GetComponent<TetrisBlock>();
        placedBlock.Initialize(currentBlockData, placeCubeMaterial);

        // 고스트 블럭의 위치와 회전 상태를 배치된 블럭에 복사
        placedBlock.SetPosition(position);
        placedBlock.transform.rotation = currentGhostBlock.transform.rotation;

        // 배치된 블럭의 콜라이더 활성화
        placedBlock.EnableColliders(true);

        return true;
    }
}
