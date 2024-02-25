
#region Includes 
using UnityEngine;
#endregion 

public class CarController : MonoBehaviour 
{
    #region Members 
    #region IDGenerator 

    // Unique ID generation 
    private static int iDGenerator = 0;

    // Return next unique id in the sequence 
    private static int NextID
    {
        get { return iDGenerator++; }
    }
    #endregion 

    // Time between checkpoints before car dies 
    private const float MAX_CHECKPOINT_DELAY = 7;

    // AI agent for given car
    public Agent Agent
    {
        get;
        set;
    }

    public float CurrentCompletionReward
    {
        get { return Agent.Genotype.Evaluation; }
        set { Agent.Genotype.Evaluation = value; }
    }
    
    // Not controllable by user input (keyboard)
    public bool UseUserInput = false;

    public CarMovement Movement
    {
        get;
        private set;
    }

    // Cached SpiritRenderer of this car
    public SpriteRenderer SpriteRenderer
    {
        get;
        private set;
    }

    private Sensor[] sensors;
    private float timeSinceLastCheckpoint;
    #endregion 

    #region Constructors 
    void Awake()
    {
        // Cache components
        Movement = GetComponent<CarMovement>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        sensors = GetComponentsInChildren<Sensor>();
    }

    void Start()
    {
        Movement.HitWall += Die;

        // Give unique name
        this.name = "Car (" + NextID + ")";
    }
    #endregion 

    #region Methods 

    // Restart car and make movable 
    public void Restart()
    {
        Movement.enabled = true;
        timeSinceLastCheckpoint = 0;

        foreach (Sensor s in sensors)
            s.Show();
        
        Agent.Reset();
        this.enabled = true;

    }

    // Unity method for normal update 
    void Update()
    {
        timeSinceLastCheckpoint += Time.deltaTime;
    }

    // Unity method for physics update 
    void FixedUpdate()
    {

        // Get control inputs from Agent 
        if(!UseUserInput)
        {

            // Get readings from sensors 
            double[] sensorOutput = new double[sensors.Length];
            for (int i = 0; i < sensors.Length; i++)
                sensorOutput[i] = sensors[i].Output;

            double[] controlInputs = Agent.FNN.ProcessInputs(sensorOutput);
            Movement.SetInputs(controlInputs);
        }
    }

    // Car dies (stop movement and controls)
    private void Die()
    {
        this.enabled = false;
        Movement.Stop();
        Movement.enabled = false;

        foreach (Sensor s in sensors)
            s.Hide();
        
        Agent.Kill();
    }

    public void CheckpointCaptured()
    {
        timeSinceLastCheckpoint = 0;
    }

    #endregion 

}