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

    public float HalfCell => CellsSize / 2;

    public Vector2Int CenterCell => new Vector2Int(NumberCells / 2, NumberCells / 2);

    #endregion

    public Vector3 PivotBasedOnFace(EFace face)
    {
        switch (face)
        {
            case EFace.front:
                return new Vector3(HalfCell, HalfCell, 0);
            case EFace.right:
                return new Vector3(0, HalfCell, HalfCell);
            case EFace.back:
                return new Vector3(HalfCell, HalfCell, 0);
            case EFace.left:
                return new Vector3(0, HalfCell, HalfCell);
            case EFace.up:
                return new Vector3(HalfCell, 0, HalfCell);
            case EFace.down:
                return new Vector3(HalfCell, 0, HalfCell);
        }

        return Vector3.zero;
    }
}
