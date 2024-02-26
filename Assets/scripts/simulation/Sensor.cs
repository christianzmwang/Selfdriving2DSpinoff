
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    #region members
    [SerializeField]
    // Layer sensor will react to 
    private LayerMask LayerToSense;
    // Point of the sensor, set in Unity editor 
    [SerializeField]
    private SpriteRenderer Dot;

    // Max and min readings
    private const float MAX_DIST = 10f;
    private const float MIN_DIST = 0.01f;

    // Current sensor readings in percent of maximum distance
    public float Output
    {
        get;
        private set;
    }
    #endregion 
    
    void Start ()
    {
        Dot.gameObject.SetActive(true);
    }

    #region Methods
    void FixedUpdate ()
    {
        // Calculate direction of sensor
        Vector2 direction = Dot.transform.position - this.transform.position;
        direction.Normalize();

        //Send raycast into direction of sensor 
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, direction, MAX_DIST, LayerToSense);

        // Check distance
        if (hit.collider == null)
            hit.distance = MAX_DIST;
        else if (hit.distance < MIN_DIST)
            hit.distance = MIN_DIST;

        this.Output = hit.distance; // transform to percent of max distance
        Dot.transform.position = (Vector2) this.transform.position + direction * hit.distance; // set position of visual dot to current reading

    }

    // Hides dot of sensor
    public void Hide()
    {
        Dot.gameObject.SetActive(false);
    }

    // Show dot of this sensor
    public void Show()
    {
        Dot.gameObject.SetActive(true);
    }
    
    #endregion

}