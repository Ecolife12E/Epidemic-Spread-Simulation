using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class GuiManager : MonoBehaviour
{
    // Data Object
    public DataObject data_object;


    // Population Count Slider
    [SerializeField] private Slider _population_count_slider;
    [SerializeField] private TextMeshProUGUI _population_count_text;

    // Global Speed Slider
    [SerializeField] private Slider _global_speed_slider;
    [SerializeField] private TextMeshProUGUI _global_speed_text;

    // Sensor Count Slider
    [SerializeField] private Slider _sensor_count_slider;
    [SerializeField] private TextMeshProUGUI _sensor_count_text;

    // Population Name Input
    [SerializeField] private TMP_InputField _population_name_input;

    // Transmission Radius Slider
    [SerializeField] private Slider _transmission_radius_slider;
    [SerializeField] private TextMeshProUGUI _transmission_radius_text;

    // Min infection time Slider
    [SerializeField] private Slider _min_infection_time_slider;
    [SerializeField] private TextMeshProUGUI _min_infection_time_text;

    // Max infection time Slider
    [SerializeField] private Slider _max_infection_time_slider;
    [SerializeField] private TextMeshProUGUI _max_infection_time_text;

    // Min recovering time Slider
    [SerializeField] private Slider _min_recovering_time_slider;
    [SerializeField] private TextMeshProUGUI _min_recovering_time_text;

    // Max recovering time Slider
    [SerializeField] private Slider _max_recovering_time_slider;
    [SerializeField] private TextMeshProUGUI _max_recovering_time_text;

    // Reccuring Disease Toggle

    // Disease Name Input
    [SerializeField] private TMP_InputField _disease_name_input;


    void Start()
    {
        _population_count_slider.onValueChanged.AddListener((v) =>
        {
            _population_count_text.text = (v * 1000).ToString();
            data_object.population_count = (int)(v * 1000);
        });

        _global_speed_slider.onValueChanged.AddListener((v) =>
        {
            _global_speed_text.text = v.ToString();
            data_object.global_speed = (v);
        });

        _sensor_count_slider.onValueChanged.AddListener((v) =>
        {
            _sensor_count_text.text = v.ToString();
            data_object.number_of_sensors = (int)(v);
        });

        _transmission_radius_slider.onValueChanged.AddListener((v) =>
        {
            _transmission_radius_text.text = v.ToString("0.0");
            data_object.radius = (v);
        });

        _min_infection_time_slider.onValueChanged.AddListener((v) =>
        {
            _min_infection_time_text.text = v.ToString("0.0");
            data_object.infection_time.x = v;
        });

        _max_infection_time_slider.onValueChanged.AddListener((v) =>
        {
            _max_infection_time_text.text = v.ToString("0.0");
            data_object.infection_time.y = v;
        });

        _min_recovering_time_slider.onValueChanged.AddListener((v) =>
        {
            _min_recovering_time_text.text = v.ToString("0.0");
            data_object.recovering_time.x = v;
        });

        _max_recovering_time_slider.onValueChanged.AddListener((v) =>
        {
            _max_recovering_time_text.text = v.ToString("0.0");
            data_object.recovering_time.y = v;
        });
    }

    public void Restart_The_Simulation()
    {
        // Loads the current scene which will restart the simulation.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Population_Name_Save(string s)
    {
        data_object.population_name = s;
    }

    public void Disease_Name_Save(string s)
    {
        data_object.disease_name = s;
    }

    public void Reset_Population_Sliders()
    {
        _population_count_slider.value = data_object.population_count;
        _global_speed_slider.value = data_object.global_speed;
        _sensor_count_slider.value = data_object.number_of_sensors;
    }

    public static void Reset_Population_static()
    {
        Reset_Population_Sliders();
    }
}
