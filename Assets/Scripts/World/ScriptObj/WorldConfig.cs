using UnityEngine;


[System.Serializable]
public struct SRandomize
{
    public bool randomAtStart;
    public bool loop;
    public float timeBeforeRandomize;
    public int randomizeTimes;
    public float rotationTime;
    public float timeBetweenRandomize;

    public SRandomize(bool randomAtStart, bool loop, int timeBeforeRandomize, int randomizeTimes, float rotationTime, float timeBetweenRandomize)
    {
        this.randomAtStart = randomAtStart;
        this.loop = loop;
        this.timeBeforeRandomize = timeBeforeRandomize;
        this.randomizeTimes = randomizeTimes;
        this.rotationTime = rotationTime;
        this.timeBetweenRandomize = timeBetweenRandomize;
    }
}

[CreateAssetMenu(menuName = "Cube Invaders/World Config", fileName = "World Config")]
public class WorldConfig : ScriptableObject
{
    public int NumberCells = 3;
    public float CellsSize = 1;
    public GameObject SunPrefab;
    public SRandomize Randomize = new SRandomize(true, false, 1, 30, 0.1f, 0.1f);
    public EBiomes[] BiomesNotBuildable = default;


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
