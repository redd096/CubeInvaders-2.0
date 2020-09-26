using UnityEngine;

[CreateAssetMenu(menuName = "Cube Invaders/World Config", fileName = "World Config")]
public class WorldConfig : ScriptableObject
{
    public int NumberCells = 3;
    public float CellsSize = 1;
    public GameObject SunPrefab;

    #region readonly vars

    public float FaceSize => NumberCells * CellsSize;

    public Vector3 HalfCube
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
