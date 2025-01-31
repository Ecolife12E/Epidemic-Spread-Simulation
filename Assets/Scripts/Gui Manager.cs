using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class GuiManager : MonoBehaviour
{

    [SerializeField] private DatabaseManager database_manager;
    // Data Object
    public DataObject data_object;

    // Settings Menu Canvas
    [SerializeField] private GameObject settings_menu;
    [SerializeField] private GameObject show_settings_button;
    [SerializeField] private TextMeshProUGUI _show_settings_button_text;

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

    // Population Preset Input
    [SerializeField] private TMP_InputField _population_preset_input;

    //Population Preset Dropdown
    [SerializeField] private TMP_Dropdown _population_preset_dropdown;

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
    [SerializeField] private Toggle _reccuring_disease_toggle;

    // Disease Name Input
    [SerializeField] private TMP_InputField _disease_name_input;

    // Disease Preset Input
    [SerializeField] private TMP_InputField _disease_preset_input;

    // Disease Preset Dropdown
    [SerializeField] private TMP_Dropdown _disease_preset_dropdown;


    // Error text box
    [SerializeField] private TextMeshProUGUI _error_text_box;

    public bool settings_is_showing;
    
    

    void Start()
    {
        settings_is_showing = true;
        _show_settings_button_text.text = "Hide Settings";
        _error_text_box.text = "";


        // Assigning the inital values to the sliders and text boxes
        _population_count_slider.value = data_object.population_count;
        _population_count_text.text = data_object.population_count.ToString();

        _global_speed_slider.value = data_object.global_speed;
        _global_speed_text.text = data_object.global_speed.ToString("0.0");

        _sensor_count_slider.value = data_object.number_of_sensors;
        _sensor_count_text.text = data_object.number_of_sensors.ToString();

        _transmission_radius_slider.value = data_object.radius;
        _transmission_radius_text.text = data_object.radius.ToString("0.0");

        _min_infection_time_slider.value = data_object.infection_time.x;
        _min_infection_time_text.text = data_object.infection_time.x.ToString("0.0");

        _max_infection_time_slider.value = data_object.infection_time.y;
        _max_infection_time_text.text = data_object.infection_time.y.ToString("0.0");

        _min_recovering_time_slider.value = data_object.recovering_time.x;
        _min_recovering_time_text.text = data_object.recovering_time.x.ToString("0.0");

        _max_recovering_time_slider.value = data_object.recovering_time.y;
        _max_recovering_time_text.text = data_object.recovering_time.y.ToString("0.0");



        // Adding the Listeners for the Sliders
        _population_count_slider.onValueChanged.AddListener((v) =>
        {
            _population_count_text.text = (v * 1000).ToString();
            data_object.population_count = (int)(v * 1000);
        });

        _global_speed_slider.onValueChanged.AddListener((v) =>
        {
            _global_speed_text.text = v.ToString("0.0");
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

        // Reset Options for Dropdown menues
        Update_Disease_Dropdown_Options();
        Update_Population_Dropdown_Options();



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

    public void Is_reccuring_Change()
    {
        data_object.is_reccuring = _reccuring_disease_toggle.isOn;
    }

    public void Reset_Population_Sliders()
    {
        _population_count_slider.value = (data_object.population_count / 1000);
        _population_count_text.text = data_object.population_count.ToString();
        _global_speed_slider.value = data_object.global_speed;
        _global_speed_text.text = data_object.global_speed.ToString("0.0");
        _sensor_count_slider.value = data_object.number_of_sensors;
        _sensor_count_text.text = data_object.number_of_sensors.ToString();
        _population_name_input.text = data_object.population_name;

        _population_preset_input.text = "";

    }

    public void Reset_Disease_Sliders()
    {
        _transmission_radius_slider.value = data_object.radius;
        _transmission_radius_text.text = data_object.radius.ToString();
        _min_infection_time_slider.value = data_object.infection_time.x;
        _min_infection_time_text.text = data_object.infection_time.x.ToString("0.0");
        _max_infection_time_slider.value = data_object.infection_time.y;
        _max_infection_time_text.text = data_object.infection_time.y.ToString("0.0");
        _min_recovering_time_slider.value = data_object.recovering_time.x;
        _min_recovering_time_text.text = data_object.recovering_time.x.ToString("0.0");
        _max_recovering_time_slider.value = data_object.recovering_time.y;
        _max_recovering_time_text.text = data_object.recovering_time.y.ToString("0.0");
        _disease_name_input.text = data_object.disease_name;

        _disease_preset_input.text = "";
        _error_text_box.text = "";
    }

    public void Invalid_Disease_Query()
    {
        _error_text_box.text = "Disease preset with that name does not exist";
    }

    public void Invalid_Population_Query()
    {
        _error_text_box.text = "Population preset with that name does not exist";
    }

    public void Change_Settings_Visibility()
    {
        settings_menu.SetActive(!settings_is_showing);
        if (settings_is_showing)
        {
            _show_settings_button_text.text = "Show Settings";
        }
        else
        {
            _show_settings_button_text.text = "Hide Settings";
        }
        settings_is_showing = !settings_is_showing;

        Debug.Log("Pressed");
    }

    public void Update_Population_Dropdown_Options()
    {
        _population_preset_dropdown.ClearOptions();

        // initialises local variables and gets the list of strings
        List<TMP_Dropdown.OptionData> data_text = new List<TMP_Dropdown.OptionData>();
        List<string> names = database_manager.Get_Population_Names();
        TMP_Dropdown.OptionData new_data = new TMP_Dropdown.OptionData();


        // Iterates through the returned list of strings
        for (int i = 0; i < names.Count; i++)
        {
            Debug.Log(i.ToString());
            new_data = new TMP_Dropdown.OptionData();
            new_data.text = names[i];
            data_text.Add(new_data); // adds that string as an option for the dropdown menu
        }
        // sends the list of options to the menu
        _population_preset_dropdown.AddOptions(data_text);
    }

    public void Update_Disease_Dropdown_Options()
    {
        _disease_preset_dropdown.ClearOptions();

        List<TMP_Dropdown.OptionData> data_text = new List<TMP_Dropdown.OptionData>();
        List<string> names = database_manager.Get_Disease_Names();
        TMP_Dropdown.OptionData new_data = new TMP_Dropdown.OptionData();

        for (int i = 0;i < names.Count;i++)
        {
            new_data = new TMP_Dropdown.OptionData();
            new_data.text = names[i];
            data_text.Add(new_data);
        }

        _disease_preset_dropdown.AddOptions(data_text);

    }

    public void Population_Dropdown_Value_Changed()
    {
        data_object.population_name = _population_preset_dropdown.captionText.text.ToString();
        database_manager.Load_Population_Preset();
    }

    public void Disease_Dropdown_Value_Changed()
    {
        data_object.disease_name = _disease_preset_dropdown.captionText.text.ToString();
        database_manager.Load_Disease_Preset();
    }


}
