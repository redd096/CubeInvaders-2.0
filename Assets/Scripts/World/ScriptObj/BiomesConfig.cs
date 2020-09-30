using UnityEngine;

[CreateAssetMenu(menuName = "Cube Invaders/Level/Biomes Config", fileName = "Biomes Config")]
public class BiomesConfig : ScriptableObject
{
    public Cell Front, Right, Back, Left, Up, Down;
}
