using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TurretShield : Turret
{
    [Header("Important")]
    [SerializeField] Shield shieldPrefab = default;

    public System.Action<TurretShield> onTurretExitQueue;

    Shield shield;

    List<TurretShield> shieldsQueue = new List<TurretShield>();

    /*
     * nel check se attivare lo scudo o mettersi in coda, manca il controllo se lo scudo è ancora integro
     * quando si passa alla prossima wave, manca un modo per sapere chi si deve attivare e come fare dev'essere la coda
     * 
    V quando viene istanziata si crea lo scudo

    V quando viene attivata check se deve attivare lo scudo
    V altrimenti si mette in coda

    V quando ruota, disattiva lo scudo
    - ed esce dalla coda
    
    V quando finisce di ruotare, nuovo check se attivare lo scudo
    V altrimenti si mette in una nuova coda

    - quando una torretta esce dalla coda, check se attivare lo scudo

    - quando viene distrutto lo scudo, esce dalla coda
    - lo scudo non potrà più essere utilizzato per questa wave

    - quando passa alla prossima wave, resetta la vita dello scudo
    - check se è la prima che si deve attivare
    - altrimenti scoprare in quale punto della queue deve andare

    V quando la torretta viene venduta o distrutta, deve disattivare lo scudo
    - ed uscire dalla queue

    //old
    - se c'è un'altra scudo attiva, ci si aggiunge alla sua lista
    - se non c'è, diventa questa quella attiva e se ce n'era una firstToActivateAtNextWave, viene declassata
    -
    - quando ruota, disattiva lo scudo e si elimina dalla lista della scudo attiva
    - se è questa la scudo attiva, attiva la prossima e pulisce la lista. Se ci sono altre torrette, ma non possono attivarsi, la prima viene promossa a firstToActivateAtNextWave
    - se questa era la firstToActivateAtNextWave, ora non lo è più + controlla se ce n'è un'altra e promuove lei
    -
    - quando finisce di ruotare, se può attivarsi controlla se c'è una torretta attiva
    - se c'è si aggiunge alla sua lista
    - se non c'è, si attiva questa e controlla tutte le torrette che ci sono su sta faccia. Se c'è firstToActivateAtNextWave, viene declassata
    - se invece non può attivarsi
    - se ci sono altre torrette non fa niente, altrimenti diventa firstToActivateAtNextWave
    -
    - quando viene distrutto lo scudo, attiva la prossima della lista e pulisce la lista
    - se questa era l'ultima attiva, diventa firstToActivateAtNextWave
    - infine si disabilita, non potrà attivare lo scudo per questa wave
    -
    - quando passa alla prossima wave, resetta la vita di tutti gli scudi
    - se è firstToActivateAtNextWave, ricrea la lista delle next
    - e a tutte le altre, gli setta la current active + le riabilita che potranno riattivare lo scudo
    - infine si attiva - quindi si abilita anche a sé stessa e si rimuove firstToActivateAtNextWave
    -
    - quando la torretta viene venduta:
    - se era quella attiva, attiva la prossima della lista o se non ce ne sono, controlla se ce n'è una da far diventare firstToActivateAtNextWave
    - se era in lista per attivarsi, si rimuove dalla lista del currentActive
    - se era la firstToActivateAtNextWave, controlla se lo può diventare un'altra, altrimenti nada
    */

    void Start()
    {
        //set reference and parent
        shield = Instantiate(shieldPrefab, transform);
        shield.name = "Shield";

        //reset health and set event
        shield.ResetHealth();
        shield.onShieldDestroyed += OnShieldDestroyed;

        //deactive it
        shield.gameObject.SetActive(false);
    }

    public override void ActivateTurret(Cell cellOwner)
    {
        base.ActivateTurret(cellOwner);

        //activate shield
        if (CheckActivateShield())
            shield.ActivateShield(CellOwner.coordinates);
    }

    public override void DeactivateTurret()
    {
        base.DeactivateTurret();

        //remove shield
        shield.gameObject.SetActive(false);
    }

    protected override void OnWorldRotate()
    {
        base.OnWorldRotate();

        //remove shield
        shield.gameObject.SetActive(false);
    }

    protected override void OnEndRotation()
    {
        base.OnEndRotation();

        //reactivate shield
        if (CheckActivateShield())
            shield.ActivateShield(CellOwner.coordinates);
    }

    #region private API

    #region events

    void OnShieldDestroyed()
    {

    }

    void OnTurretExitQueue(TurretShield turretShield)
    {
        //remove turretShield from queue
        shieldsQueue.Remove(turretShield);
    }

    #endregion

    bool CheckActivateShield()
    {
        //get turrets active on this face
        shieldsQueue = FindObjectsOfType<TurretShield>().Where(
            x => x.IsActive
            && x.CellOwner.coordinates.face == CellOwner.coordinates.face).ToList();

        //set event on every turret
        shieldsQueue.ForEach(x => x.onTurretExitQueue += OnTurretExitQueue);

        //if there is no queue, can activate
        return shieldsQueue.Count <= 0;
    }

    #endregion
}
