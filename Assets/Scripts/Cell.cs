using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Cube Invaders/Cell")]
public class Cell : MonoBehaviour
{
    [Header("Important")]
    [SerializeField] Turret turretToCreate;

    [Header("Debug")]
    public Coordinates coordinates;

    //used from turret to know when is rotating
    public System.Action onWorldRotate;

    #region public API

    public void ActivateCell()
    {
        //deve controllare se su questa cella si può costruire (o magari è radar - turretToCreate == null)
        //se questa cella effettivamente costruisce una torretta (magari è il vulcano - nuova classe che overrida questa funzione?)
        //se questa cella è vuota (magari è già attivata / ha già su una torretta)
        //nel caso si costruisse una torretta, quale costruire
    }

    #endregion
}
