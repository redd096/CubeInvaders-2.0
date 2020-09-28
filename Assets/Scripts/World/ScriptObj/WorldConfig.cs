using UnityEngine;

[CreateAssetMenu(menuName = "Cube Invaders/World Config", fileName = "World Config")]
public class WorldConfig : ScriptableObject
{
    [Tooltip("Number of cell for every face (for example 3x3x3 cube)")] public int NumberCells = 3;
    [Tooltip("Size of every cell in meters")] public float CellsSize = 1;
    [Tooltip("Sun prefab (center of the world)")] public GameObject SunPrefab;
    [Tooltip("Cell selector")] public GameObject Selector;

    #region readonly vars

    public float FaceSize => NumberCells * CellsSize;

    public Vector3 HalfCube
    {
        get
        {
            float halfDist = FaceSize / 2;

            return new Vector3(halfDist, halfDist, halfDist);
        }
    }

    public Vector3 HalfCubePivot2
    {
        get
        {
            float halfDist = FaceSize / 2;
            bool odd = NumberCells % 2 != 0;

            Vector3 half = new Vector3(halfDist - HalfCell, halfDist - HalfCell, halfDist - HalfCell);

            //if odd number, add half cell to every axis
            return odd ? half : half + new Vector3(HalfCell, HalfCell, HalfCell);

        }
    }

    public Vector3 HalfCubePivot
    {
        get
        {
            float center = (NumberCells - 1) / 2 * CellsSize;
            bool odd = NumberCells % 2 != 0;

            Vector3 half = new Vector3(center, center, center + HalfCell);

            //if odd number, add half cell to every axis
            return odd ? half : half + new Vector3(HalfCell, HalfCell, HalfCell);
        }
    }

    public float HalfCell => CellsSize / 2;

    #endregion
}
