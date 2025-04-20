using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TetrisBlockPlacer : MonoBehaviour
{
    [SerializeField] Camera cam;
    public List<TetrisBlock> tetrisBlocks; // 사용할 테트리스 블록 리스트
    public GameObject cubePrefab; // 큐브 프리팹
    public float gridSize = 1f; // 그리드 크기
    public GridLinesDrawer gridDrawer; // GridLinesDrawer 참조

    private TetrisBlock currentBlock; // 현재 선택된 테트리스 블록
    private GameObject ghostBlock; // 고스트 블록
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
            // 고스트 블록 위치 업데이트
            UpdateGhostBlock();

            // 마우스 클릭 시 블록 배치
            if (Input.GetMouseButtonDown(0))
            {
                PlaceBlock();
                SelectRandomBlock(); // 블록 배치 후 새로운 블록 선택
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

        // 랜덤으로 블록 선택
        int randomIndex = Random.Range(0, tetrisBlocks.Count);
        currentBlock = tetrisBlocks[randomIndex];

        // 기존 고스트 블록 제거
        if (ghostBlock != null)
        {
            Destroy(ghostBlock);
        }

        // 새로운 고스트 블록 생성
        CreateGhostBlock();
    }

    void CreateGhostBlock()
    {
        // 기존 고스트 블록 제거
        if (ghostBlock != null)
        {
            if (currentBlock == null)
            {
                Debug.LogError("CurrentBlock is null. Ensure that a valid TetrisBlock is selㅋected.");
                return;
            }
            Destroy(ghostBlock);
            ghostBlock = null; // 참조 제거
        }

        // 고스트 블록 생성
        ghostBlock = new GameObject("GhostBlock");
        ghostRenderers = new Renderer[currentBlock.cubePositions.Length];

        for (int i = 0; i < currentBlock.cubePositions.Length; i++)
        {
            GameObject ghostCube = Instantiate(cubePrefab, ghostBlock.transform);
            ghostCube.transform.localPosition = currentBlock.cubePositions[i];

            // 고스트 블록의 색상을 테트리스 블록과 동일하게 설정 (반투명)
            Renderer renderer = ghostCube.GetComponent<Renderer>();
            renderer.material.color = new Color(
                currentBlock.blockColor.r,
                currentBlock.blockColor.g,
                currentBlock.blockColor.b,
                0.5f // 반투명
            );
            ghostRenderers[i] = renderer;

            // 고스트 블록의 모든 콜라이더 비활성화
            Collider[] colliders = ghostCube.GetComponents<Collider>();
            foreach (Collider collider in colliders)
            {
                collider.enabled = false;
            }
        }

        // 고스트 블록의 위치 초기화
        ghostBlock.transform.position = Vector3.zero;
    }

    bool IsBlockOverlapping(Vector3 position)
    {
        foreach (Vector3 offset in currentBlock.cubePositions)
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
                if (collider.gameObject != ghostBlock && collider.transform.position == cubePosition)
                {
                    return true; // 완전히 겹친 경우
                }
            }
        }

        return false; // 완전히 겹치는 블록이 없는 경우
    }

    void UpdateBlockPosition(bool isPlacing = false)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Vector3 snappedPosition = Vector3.zero;
        bool isValidPosition = false;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Ray가 부딪힌 위치를 기준으로 블록 배치
            Vector3 hitPosition = hit.point;

            // 중심점을 기준으로 그리드 스냅 적용
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
            // Ray가 아무것도 부딪히지 않은 경우, 기본 평면 위에 고스트 블록 배치
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // Y=0 평면
            if (groundPlane.Raycast(ray, out float distance))
            {
                Vector3 pointOnPlane = ray.GetPoint(distance);

                // 그리드 스냅 적용
                Vector3 center = currentBlock.GetCenter();
                int gridX = Mathf.RoundToInt((pointOnPlane.x - center.x) / gridSize);
                int gridZ = Mathf.RoundToInt((pointOnPlane.z - center.z) / gridSize);

                snappedPosition = new Vector3(
                    gridX * gridSize + center.x,
                    0, // 기본 평면의 Y 좌표
                    gridZ * gridSize + center.z
                );

                isValidPosition = true;
            }
        }

        if (isValidPosition)
        {
            // 고스트 블록 위치 업데이트
            ghostBlock.transform.position = snappedPosition;

            // 겹침 여부 확인
            bool isOverlapping = IsBlockOverlapping(snappedPosition);

            // 고스트 블록 색상 변경
            foreach (Renderer renderer in ghostRenderers)
            {
                renderer.material.color = isOverlapping
                    ? new Color(1, 0, 0, 0.5f) // 빨간색 (반투명)
                    : new Color(currentBlock.blockColor.r, currentBlock.blockColor.g, currentBlock.blockColor.b, 0.5f); // 원래 색상 (반투명)
            }

            // 블록 배치 처리
            if (isPlacing && !isOverlapping)
            {
                // 실제 블록 배치
                foreach (Vector3 offset in currentBlock.cubePositions)
                {
                    // 중심점을 기준으로 큐브의 실제 위치 계산
                    Vector3 cubePosition = snappedPosition + offset - currentBlock.GetCenter();
                    GameObject placedCube = Instantiate(cubePrefab, cubePosition, Quaternion.identity);

                    // 배치된 큐브의 모든 콜라이더 활성화
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
        // 중심점을 기준으로 그리드 스냅 적용
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
        // 실제 블록 배치
        foreach (Vector3 offset in currentBlock.cubePositions)
        {
            // 중심점을 기준으로 큐브의 실제 위치 계산
            Vector3 cubePosition = position + offset;
            GameObject placedCube = Instantiate(cubePrefab, cubePosition, Quaternion.identity);

            // 배치된 큐브의 모든 콜라이더 활성화
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
            // Ray가 부딪힌 위치를 기준으로 스냅된 위치 계산
            snappedPosition = CalculateSnappedPosition(hit.point);
            isValidPosition = true;
        }
        else
        {
            // Ray가 아무것도 부딪히지 않은 경우, 기본 평면 위에 고스트 블록 배치
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // Y=0 평면
            if (groundPlane.Raycast(ray, out float distance))
            {
                Vector3 pointOnPlane = ray.GetPoint(distance);
                snappedPosition = CalculateSnappedPosition(pointOnPlane);
                isValidPosition = true;
            }
        }

        if (isValidPosition)
        {
            // 고스트 블록 위치 업데이트
            ghostBlock.transform.position = snappedPosition;

            // 겹침 여부 확인
            bool isOverlapping = IsBlockOverlapping(snappedPosition);

            // 고스트 블록 색상 변경
            foreach (Renderer renderer in ghostRenderers)
            {
                renderer.material.color = isOverlapping
                    ? new Color(1, 0, 0, 0.5f) // 빨간색 (반투명)
                    : new Color(currentBlock.blockColor.r, currentBlock.blockColor.g, currentBlock.blockColor.b, 0.5f); // 원래 색상 (반투명)
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

        // 블록 배치
        PlaceBlockAtPosition(position);
        return true;
    }
}
