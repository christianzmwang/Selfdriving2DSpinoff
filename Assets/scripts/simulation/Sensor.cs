
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    
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
    }
    



}
