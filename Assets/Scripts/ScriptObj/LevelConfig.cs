using UnityEngine;

[CreateAssetMenu(menuName = "Cube Invaders/Level/Level Config", fileName = "Level Config")]
public class LevelConfig : ScriptableObject
{
    [Tooltip("The player can recreate a destroyed cell")] public bool CanRecreateCell = false;
    [Tooltip("Randomize world at start")] public bool RandomizeWorldAtStart = true;

    //ci sarà la lista dei nemici, e a seconda del livello dovrà essere passato un level config differente
    //per ora lavorano tutti in start, l'unico con awake è World per settarsi le reference proprie
}
