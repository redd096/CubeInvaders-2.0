using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Cube Invaders/Turret Graphics/Radar Graphics")]
public class RadarGraphics : BuildableGraphics
{
    [Header("Radar")]
    [SerializeField] Transform objectToFlick = default;

    [Header("Flick")]
    [SerializeField] Gradient flickColor = default;
    [SerializeField] float minFlick = 1;
    [SerializeField] float maxFlick = 10;

    Radar radar;
    Dictionary<Renderer, Color> normalColors = new Dictionary<Renderer, Color>();

    protected override void Awake()
    {
        base.Awake();

        //get logic component as radar
        radar = buildableObject as Radar;

        //set normal colors
        foreach(Renderer r in objectToFlick.GetComponentsInChildren<Renderer>())
        {
            normalColors.Add(r, r.material.color);
        }
    }

    protected override void Update()
    {
        base.Update();

        if (buildableObject.IsActive)
        {
            ColorRadar();
        }
    }

    protected override Enemy GetEnemy()
    {
        //get enemy from logic component
        return radar.EnemyToAttack;
    }

    void ColorRadar()
    {
        //need model for flick
        if (objectToFlick == null) 
            return;

        //if enemy is attacking, flick color
        if(GetEnemy())
        {
            SetColorFlick();
        }
        //else show normal color
        else 
        {
            SetColor(Color.white, 0, true);
        }
    }

    void SetColorFlick()
    {
        int currentWave = GameManager.instance.waveManager.CurrentWave;

        //get flick speed based on enemy distance to its coordinates to attack
        float enemyDistance = Vector3.Distance(GetEnemy().coordinatesToAttack.position, GetEnemy().transform.position);
        float distanceFrom0To1 = 1 - (enemyDistance / GameManager.instance.waveManager.waveConfig.Waves[currentWave].DistanceFromWorld);        //distance from 0 to 1
        float flickSpeed = Mathf.Lerp(minFlick, maxFlick, distanceFrom0To1);                                                                    //speed from minFlick to maxFlick

        //sin from 0 to 1
        float flick = Mathf.Abs(Mathf.Sin(Time.time * flickSpeed));

        //set color based on enemy distance
        Color color = flickColor.Evaluate(distanceFrom0To1);

        SetColor(color, flick, false);
    }

    void SetColor(Color colorFlick, float delta, bool setNormalColor)
    {
        //foreach renderer set color and emission
        foreach (Renderer renderer in normalColors.Keys)
        {
            //set color flick or stay normal color
            Color color = setNormalColor ? normalColors[renderer] : colorFlick;

            renderer.material.color = Color.Lerp(normalColors[renderer], color, delta);
            renderer.material.SetColor("_EmissionColor", Color.Lerp(normalColors[renderer], color, delta));
        }
    }
}
