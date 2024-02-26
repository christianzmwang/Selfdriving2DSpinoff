
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using Unity.VisualScripting;

public class Genotype : IComparable<Genotype>, IEnumerable<float>
{
    #region Members
    private static Random randomizer = new Random();

    // Current evaluation of this genotype
    public float Evaluation
    {
        get;
        set;
    }

    // eval of genotype relative to average eval of whole population of this genotype 

    public float Fitness
    {
        get;
        set;
    }

    // Vector of parameters of this genotype
    private float[] parameters;

    // Amount of parameters stored in the parameter vector of this genotype 
    public int ParameterCount
    {
        get 
        {
            if (parameters == null) return 0;
            return parameters.Length;
        }
    }

    // Overridden indexer for convenient parameter access
    public float this[int index]
    {
        get { return parameters[index]; }
        set { parameters[index] = value; }
    }
    #endregion

    // Instance of new genotype with given parameter vector and initial fitness of 0. 
    public Genotype(float[] parameters)
    {
        this.parameters = parameters;
        Fitness = 0;
    }

    // compares genotypes based on fitness values 
    
    public int CompareTo(Genotype other)
    {
        return other.Fitness.CompareTo(this.Fitness); // in reverse order for larger fitness being first in list
    }

    public IEnumerator<float> GetEnumerator()
    {
        for (int i = 0; i < parameters.Length; i++)
            yield return parameters[i];
    }

    // gets an Enumerator to iterate over all parameters of this genotype 
    IEnumerator IEnumerable.GetEnumerator()
    {
        for (int i = 0; i < parameters.Length; i++)
            yield return parameters[i];
    }

    // Sets the parameters of this genotype to random values in given range 
    public void SetRandomParameters(float minValue, float maxValue)
    {
        // Check arguments 
        if (minValue > maxValue) throw new ArgumentException("Minimum value may not exceed maximum value.");

        // Generate random parameter vector
        float range = maxValue - minValue;
        for (int i = 0; i < parameters.Length; i++)
            parameters[i] = (float)((randomizer.NextDouble() * range) + minValue); // Create a random float beteen minValue and maxValue

    }

    // returns a copy of the parameter vector
    public float[] GetParameterCopy()
    {
        float[] copy = new float[ParameterCount];
        for (int i = 0; i < ParameterCount; i++)
            copy[i] = parameters[i];

        return copy;

    }

    // Saves the parameters of this genotype to a file at given file path
    public void SaveToFile(string filePath)
    {
        StringBuilder builder = new StringBuilder();
        foreach (float param in parameters)
            builder.Append(param.ToString()).Append(";");
        
        builder.Remove(builder.Length - 1, 1);

        File.WriteAllText(filePath, builder.ToString());
    }

    #region Static Methods

    //Generates a random genotype with parameters in given range
    public static Genotype GenerateRandom(uint parameterCount, float minValue, float maxValue)
    {
        // Check arguments
        if (parameterCount == 0) return new Genotype(new float[0]);

        Genotype randomGenotype = new Genotype(new float[parameterCount]);
        randomGenotype.SetRandomParameters(minValue, maxValue);

        return randomGenotype;
    }

    // Loads a genotype from a file with given file path
    public static Genotype LoadFromFile(string filePath)
    {
        string data = File.ReadAllText(filePath);

        List<float> parameters = new List<float>();
        string[] paramStrings = data.Split(';');

        foreach (string parameter in paramStrings)
        {
            float parsed;
            if (!float.TryParse(parameter, out parsed)) throw new ArgumentException("The file at given file path does not contain a valid genotype serialisation.");
            parameters.Add(parsed);

        }

        return new Genotype(parameters.ToArray());

    }

    #endregion

}