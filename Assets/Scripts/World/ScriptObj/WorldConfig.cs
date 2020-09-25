using UnityEngine;


[System.Serializable]
public struct SRandomize
{
    public bool randomAtStart;
    public bool loop;
    public float timeBeforeRandomize;
    public int randomizeTimes;
    public float animationRandomizeTime;
    public float timeBetweenRandomize;

    public SRandomize(bool randomAtStart, bool loop, int timeBeforeRandomize, int randomizeTimes, float animationRandomizeTime, float timeBetweenRandomize)
    {
        this.randomAtStart = randomAtStart;
        this.loop = loop;
        this.timeBeforeRandomize = timeBeforeRandomize;
        this.randomizeTimes = randomizeTimes;
        this.animationRandomizeTime = animationRandomizeTime;
        this.timeBetweenRandomize = timeBetweenRandomize;
    }
}

[CreateAssetMenu(menuName = "Cube Invaders/World Config", fileName = "World Config")]
public class WorldConfig : ScriptableObject
{
    public int numberCells = 3;
    public float cellsSize = 1;
    public GameObject sunPrefab;
    public SRandomize randomize = new SRandomize(true, false, 1, 30, 0.1f, 0.1f);
    [SerializeField] EBiomes[] biomesNotBuildable = default;


    #region readonly vars

    public float faceSize => numberCells * cellsSize;
    public Vector3 halfCube
    {
        get
        {
            float center = (numberCells - 1) / 2 * cellsSize;
            bool odd = numberCells % 2 != 0;

            Vector3 half = new Vector3(center, center, center + halfCell);

            //if odd number, add half cell to every axis
            return odd ? half : half + new Vector3(halfCell, halfCell, halfCell);
        }
    }

    public float halfCell => cellsSize / 2;

    #endregion
}
