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


    [Header ("Random Variables")]
    // int used to store the maximum frame count.
    public int max_frame;
    // Flags to tell what function the user is using
    public bool simulation_active;
    public bool graph_active;
    // Flags used to determine when to show the UI
    public bool settings_is_showing;
    public bool graph_settings_is_showing;
    // Variable used to store the sim ID's for solo graphing
    public int chosen_simulation_id;
    // Variables used to store the sim ID's when comparing
    public int chosen_simulation_id_1;
    public int chosen_simulation_id_2;
    // Flag so the database isnt constantly queried
    public bool chosen_graph_drawn;


    // Variables used to store frame ID's for zoom feature
    public int chosen_frame_1;
    public int chosen_frame_2;

    public bool zoomed;
    

    public void Start()
    {
        // Resetting the variables
        chosen_frame_1 = 0;
        chosen_frame_2 = 0;

        chosen_simulation_id = 0;
        chosen_simulation_id_1 = 0;
        chosen_simulation_id_2 = 0;
    }

}