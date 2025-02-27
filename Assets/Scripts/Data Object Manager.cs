using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data_Object", order = 1)]
public class DataObject : ScriptableObject
{
    [Header("Simulation Details")]
    public int simulation_id;
    public int num_of_healthy;
    public int num_of_infected;
    public int num_of_recovered;
    public int num_of_immune;

    [Header("Texture Settings")]
    public int texture_width;
    public int texture_height;

    [Header("Immunity Settings")]
    public bool immunity;
    public float immunity_chance;

    [Header("Population Model")]
    public int population_id;
    public int population_count;
    public string population_name;
    public float global_speed;
    public int min_distance;
    [Range(1,360)]
    public int number_of_sensors;
    public bool show_sensors;

    [Header("Disease Model")]
    public string disease_name;
    public int disease_id;
    public float radius;
    public Vector2 infection_time;
    public Vector2 recovering_time;
    public bool is_reccuring;

    public int frame_id;





}