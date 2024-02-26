using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : IComparable<Agent>
{
    
    #region Members
    // The underlying genotype of this agent
    
    public Genotype Genotype
    {
        get;
        private set;
    }

    // FNN constructed from genotype of this agent
    public NeuralNetwork FNN
    {
        get;
        private set;
    }

    private bool isAlive = false;

    // Whether this agent is alive
    public bool isAlive
    {
        get { return isAlive; }
        private set
        {
            if (isAlive != value)
            {
                isAlive = value;

                if (!isAlive && AgentDied != null)
                    AgentDied(this);
            }
        }
    }

    // Event for when agent die
    public event Action<Agent> AgentDied;
    #endregion

    #region Constructors 
    // Initializses new agent from given genotype, constructing new fnn from parameters of the genotype
    public Agent(Genotype genotype, NeuralLayer.ActivationFunction defaultActivation, params uint[] topology)
    {
        IsAlive = false;
        this.Genotype = genotype;
        FNN = new NeuralNetwork(topology);
        foreach (NeuralLayer layer in FNN.Layers)
            layer.NeuronActivationFunction = defaultActivation;

        // Check if topology is valid 
        if (FNN.WeightCount != genotype.ParameterCount)
            throw new ArgumentException("The given genotype's parameter count must match the neural network topology's weight count.");
        
        // Contruct FNN from genotype
        IEnumerator<float> parameters = genotype.GetEnumerator();
        foreach (NeuralLayer layer in FNN.Layers) // Loop over all layers
        {
            for (int i = 0; i < layer.Weights.GetLength(0); i++) // Loop over all nodes of current layer
            {
                for (int j = 0; j < layer.Weights.GetLength(1); j++) // Loop over all nodes of next layer
                {
                    layer.Weights[i, j] = parameters.Current;
                    parameters.MoveNext();
                }
            }
        }
    }
    #endregion
    
    #region Methods
    // Resets this agent to be alive again

    public void Reset()
    {
        Genotype.Evaluation = 0;
        Genotype.Fitness = 0;
        IsAlive = true;
    }

    // Kills this agent, sets IsAlive = false
    public void Kill()
    {
        IsAlive = false;
    }

    #region IComparable 
    //compares agents
    public int CompareTo(Agent other)
    {
        return this.Genotype.CompareTo(other.Genotype);
    }
    #endregion
    #endregion

}
