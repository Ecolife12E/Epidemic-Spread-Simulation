using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data_Object", order = 1)]
public class DataObject : ScriptableObject
{
    [Header("Texture Settings")]
    public int texture_width;
    public int texture_height;

    [Header("Population Model")]
    public int population_count;
    public float global_speed;
    public int min_distance;
    [Range(1,360)]
    public int number_of_sensors;


    [Header("Disease Model")]
    public float radius;

}
