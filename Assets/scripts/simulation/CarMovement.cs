
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    #region Members
    /// <summary>
    /// When the car hits a wall
    /// </summary>
    
    public event System.Action HitWall;

    //Movement constants
    private const float MAX_VEL = 20f;
    private const float ACCELERATION = 8f;
    private const float VEL_FRICT = 2f;
    private const float TURN_SPEED = 100;

    private CarController controller;

    public float Velocity
    {
        get;
        private set;
    }

    public Quaternion Rotation
    {
        get;
        private set;
    }

    private double horizontalInput, verticalInput; // horizontal = engine f, veritcal = turning f 

    public double[] CurrentInputs
    {
        get { return new double[] { horizontalInput, verticalInput }; } 
    }
    #endregion 

    void Start()
    {
        controller = GetComponent<CarController>();
    }

    #region Methods
    // Unity method for physics updates
    void FixedUpdate ()
    {
        if(controller != null && controller.UseUserInput)
            CheckInput();
        
        ApplyInput();

        ApplyVelocity();

        ApplyFriction();

    }

    // Checks for user input

    private void CheckInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    // Applies the curretly set input
    private void ApplyInput()
    {
        // Cap input
        if (verticalInput > 1)
            verticalInput = 1;
        else if (verticalInput < -1)
            verticalInput = -1;
        
        if (horizontalInput > 1)
            horizontalInput = 1;
        else if (horizontalInput < -1)
            horizontalInput = -1;

        // Limit acceleration to engineForce * MAX_VEL
        bool canAccelerate = false;
        if (verticalInput < 0)
            canAccelerate = Velocity > verticalInput * MAX_VEL;
        else if (verticalInput > 0)
            canAccelerate = Velocity < verticalInput * MAX_VEL;

        // Set velocity 
        if (canAccelerate)
        {
            Velocity += (float)verticalInput * ACCELERATION * Time.deltaTime;

            // Cap velocity
            if (Velocity > MAX_VEL)
                Velocity = MAX_VEL;
            else if (Velocity < -MAX_VEL)
                Velocity = -MAX_VEL;

        }

        // Set rotation 
        Rotation = transform.rotation;
        Rotation *= Quaternion.AngleAxis((float)-horizontalInput * TURN_SPEED * Time.deltaTime, new Vector3(0, 0, 1));

    }

    /// Sets the engine and turning input according to given values
    
    public void ApplyVelocity()
    {
        Vector3 direction = new Vector3(0, 1, 0);
        transform.rotation = Rotation;
        direction = Rotation * direction;

        this.transform.position += direction * Velocity * Time.deltaTime;
    }

    // Friction added to vel
    private void ApplyFriction()
    {
        if (verticalInput == 0)
        {
            if (Velocity > 0)
            {
                Velocity -= VEL_FRICT * Time.deltaTime;
                if (Velocity < 0)
                    Velocity = 0;
                
            }
            else if (Velocity < 0)
            {
                Velocity += VEL_FRICT * Time.deltaTime;
                if (Velocity > 0)
                    Velocity = 0;
            }
        }
    }

    // Unity method, triggered when collision was detacted
    void OnCollisionEnter2D()
    {
        if(HitWall != null)
            HitWall();
    }

    // Stops car movement 
    public void Stop()
    {
        Velocity = 0;
        Rotation = Quaternion.AngleAxis(0, new Vector3(0, 0, 1));
    }
    
    #endregion 

}
