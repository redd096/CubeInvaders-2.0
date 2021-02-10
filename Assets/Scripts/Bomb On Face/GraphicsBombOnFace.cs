using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Cube Invaders/Bomb On Face/Graphics Bomb On Face")]
public class GraphicsBombOnFace : MonoBehaviour
{
    [Header("Important")]
    [SerializeField] Color explosionColor = Color.red;

    BombOnFace bombOnFace;
    Coroutine changeColorOverTime;

    Renderer rend;
    Color normalColor;

    void Start()
    {
        //get reference on self or parent
        bombOnFace = GetComponentInParent<BombOnFace>();

        //get base color
        rend = GetComponentInChildren<Renderer>();
        normalColor = rend.material.color;

        AddEvents();
    }

    void OnDestroy()
    {
        RemoveEvents();
    }

    #region events

    void AddEvents()
    {
        bombOnFace.onStartTimer += OnStartTimer;
    }

    void RemoveEvents()
    {
        bombOnFace.onStartTimer -= OnStartTimer;
    }

    void OnStartTimer()
    {
        //on start timer, start coroutine
        if (changeColorOverTime != null)
            StopCoroutine(changeColorOverTime);

        if(gameObject.activeInHierarchy)
            changeColorOverTime = StartCoroutine(ChangeColorOverTime());
    }

    #endregion

    IEnumerator ChangeColorOverTime()
    {
        //set color over time
        float delta = 0;
        while(delta < 1)
        {
            delta += Time.deltaTime / bombOnFace.TimeBeforeCanRotate;
            SetColor(delta);

            yield return null;
        }
    }

    void SetColor(float delta)
    {
        //set color and emission
        rend.material.color = Color.Lerp(explosionColor, normalColor, delta);
        rend.material.SetColor("_EmissionColor", Color.Lerp(explosionColor, normalColor, delta));
    }
}
