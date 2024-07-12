using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data_Object", order = 1)]
public class DataObject : ScriptableObject
{
    [Header("Simulation Details")]
    public int num_of_healthy;
    public int num_of_infected;
    public int num_of_recovered;

    [Header("Texture Settings")]
    public int texture_width;
    public int texture_height;

    [Header("Population Model")]
    public int population_count;
    public float global_speed;
    public int min_distance;
    [Range(1,360)]
    public int number_of_sensors;
    public bool show_sensors;
    public bool is_reccuring;


    [Header("Disease Model")]
    public float radius;
    public Vector2 infectious_time;
    public Vector2 recovering_time;
}