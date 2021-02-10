using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Cube Invaders/Enemy Component/Poison Cell")]
public class PoisonCell : MonoBehaviour
{
    [SerializeField] float timerPoison = 10;
    [SerializeField] int limitSpread = 1;

    Coroutine poison_Coroutine;

    public void Init(float timerPoison, int limitSpread)
    {
        this.timerPoison = timerPoison;
        this.limitSpread = limitSpread;
    }

    void Start()
    {
        //start poison timer
        if (poison_Coroutine == null && gameObject.activeInHierarchy)
            poison_Coroutine = StartCoroutine(Poison_Coroutine());
    }

    IEnumerator Poison_Coroutine()
    {
        //wait
        yield return new WaitForSeconds(timerPoison);

        //poison every cell around and kill this one
        Poison();
    }

    void Poison()
    {
        Cell currentCell = GetComponent<Cell>();

        if(limitSpread > 0)
        {
            //remove limit spread
            limitSpread--;

            //foreach cell around
            foreach (Cell cell in GameManager.instance.world.GetCellsAround(currentCell.coordinates))
            {
                //if is alive, poison it
                if (cell.IsAlive)
                {
                    cell.gameObject.AddComponent<PoisonCell>().Init(timerPoison, limitSpread);
                }
            }
        }

        //and kill this one
        currentCell.KillCell(false);
    }
}
