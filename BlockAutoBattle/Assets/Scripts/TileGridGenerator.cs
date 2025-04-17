using UnityEngine;

[ExecuteInEditMode]
public class GridLinesDrawer : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public float cellSize = 1f;
    public Color lineColor = Color.gray;

    private Material lineMaterial;

    // 2���� �迭: �� ��ġ�� �� ���̸� ����
    public int[,] gridHeights;
    public float gridOffsetY = -.5f;

    void Awake()
    {
        InitializeGrid();
    }

    void InitializeGrid()
    {
        gridHeights = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                gridHeights[x, z] = 0; // �ʱ�ȭ: ��� ��ġ�� �� ���̴� 0
            }
        }
    }

    void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader)
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    void OnRenderObject()
    {
        CreateLineMaterial();
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);
        GL.Begin(GL.LINES);
        GL.Color(lineColor);
        
        // Draw grid lines
        for (int x = 0; x <= width; x++)
        {
            GL.Vertex3(x * cellSize, gridOffsetY, 0);
            GL.Vertex3(x * cellSize, gridOffsetY, height * cellSize);
        }

        for (int z = 0; z <= height; z++)
        {
            GL.Vertex3(0, gridOffsetY, z * cellSize);
            GL.Vertex3(width * cellSize, gridOffsetY, z * cellSize);
        }

        GL.End();
        GL.PopMatrix();
    }
}