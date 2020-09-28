using UnityEngine;

[System.Serializable]
public struct BiomesMenu
{
    public Cell front, right, back, left, up, down;
}

[CreateAssetMenu(menuName = "Cube Invaders/Level Config", fileName = "Level Config")]
public class LevelConfig : ScriptableObject
{
    public BiomesMenu Biomes;
}
