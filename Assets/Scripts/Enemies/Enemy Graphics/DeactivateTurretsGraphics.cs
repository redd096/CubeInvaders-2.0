using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Cube Invaders/Enemy Graphics/Deactivate Turrets Graphics")]
public class DeactivateTurretsGraphics : EnemyGraphics
{
    [Header("Deactivate Turrets")]
    [SerializeField] Color effectColor = Color.cyan;

    EnemyDeactivateTurrets logic;
    Dictionary<Renderer, Color> normalColors = new Dictionary<Renderer, Color>();

    void Start()
    {
        //get logic reference
        logic = GetComponent<EnemyDeactivateTurrets>();

        //set normal colors
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            normalColors.Add(r, r.material.color);
        }

        //set event
        logic.onUpdateTimer += OnUpdateTimer;
    }

    void OnDestroy()
    {
        //remove event
        logic.onUpdateTimer -= OnUpdateTimer;
    }

    void OnUpdateTimer(float delta)
    {
        //foreach renderer set color
        foreach (Renderer renderer in normalColors.Keys)
        {
            renderer.material.color = Color.Lerp(normalColors[renderer], effectColor, delta);
        }
    }
}
