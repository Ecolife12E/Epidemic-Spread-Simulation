using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.IO;


public class GuiManager : MonoBehaviour
{

    [SerializeField] private DatabaseManager database_manager;
    // Data Object
    public DataObject data_object;
    [SerializeField] private ComputeManager compute_manager;

    [Header("Settings Menu Objects")]
    // Settings Menu Canvas
    [SerializeField] private CanvasGroup settings_menu;
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

    // Immunity chance Slider
    [SerializeField] private Slider _immunity_slider;
    [SerializeField] private TextMeshProUGUI _immunity_slider_text;

    // Immunity Toggle
    [SerializeField] private Toggle _immunity_toggle;


    // Disease Name Input
    [SerializeField] private TMP_InputField _disease_name_input;

    // Disease Preset Input
    [SerializeField] private TMP_InputField _disease_preset_input;

    // Disease Preset Dropdown
    [SerializeField] private TMP_Dropdown _disease_preset_dropdown;


    // Error text box
    [SerializeField] private TextMeshProUGUI _error_text_box;

    [Header("Graph GUI Objects")]

    [SerializeField] private CanvasGroup Graph_Menu;

    [SerializeField] private TMP_Dropdown _graph_1_options_dropdown;
    [SerializeField] private TMP_Dropdown _graph_2_options_dropdown;

    [SerializeField] private CanvasGroup _close_graph_button;

    [Header("Graph Data Canvas")]
    [SerializeField] private CanvasGroup Graph_Data_Menu;
    [SerializeField] private TextMeshProUGUI _graph_1_label;
    [SerializeField] private TextMeshProUGUI _graph_2_label;

    [SerializeField] private TextMeshProUGUI _healthy_graph_data;
    [SerializeField] private TextMeshProUGUI _infected_graph_data;
    [SerializeField] private TextMeshProUGUI _recovered_graph_data;
    [SerializeField] private TextMeshProUGUI _immmune_graph_data;

    [SerializeField] private TextMeshProUGUI _healthy_graph_2_data;
    [SerializeField] private TextMeshProUGUI _infected_graph_2_data;
    [SerializeField] private TextMeshProUGUI _recovered_graph_2_data;
    [SerializeField] private TextMeshProUGUI _immmune_graph_2_data;

    [SerializeField] private TextMeshProUGUI _graph_1_R_value;
    [SerializeField] private TextMeshProUGUI _graph_2_R_value;

    void Start()
    {
        _show_settings_button_text.text = "Hide Settings";
        _error_text_box.text = "";

        data_object.graph_settings_is_showing = false;
        data_object.graph_active = false;
        data_object.chosen_simulation_id_1 = 0;
        data_object.chosen_simulation_id_2 = 0;
        _close_graph_button.alpha = 0;
        Graph_Data_Menu.alpha = 0;

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

        _immunity_slider.onValueChanged.AddListener((v) =>
        {
            _immunity_slider_text.text = (v*5).ToString();
            data_object.immunity_chance = (float)(v * 0.05);
        });

        // Reset Options for Dropdown menues
        Update_Disease_Dropdown_Options();
        Update_Population_Dropdown_Options();
        Update_Graph_Dropdown_Options();


    }

    public void Update()
    {
        if (data_object.settings_is_showing)
        {
            settings_menu.alpha = 1;
            settings_menu.interactable = true;
        }
        else
        {
            settings_menu.alpha = 0;
            settings_menu.interactable = false;
        }

        if (data_object.graph_settings_is_showing)
        {
            Graph_Menu.alpha = 1;
            Graph_Menu.interactable = true;
        }
        else
        {
            Graph_Menu.alpha = 0;
            Graph_Menu.interactable = false;
        }
    }




    public void Restart_The_Simulation()
    {
        // Loads the current scene which will restart the simulation.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Pause_Simualtion()
    {
        data_object.simulation_active = !data_object.simulation_active;
        database_manager.Save_Simulation_Preset();
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
    public void Close_Graph()
    {
        data_object.graph_active = false;
        _close_graph_button.alpha = 0;
        Graph_Data_Menu.alpha = 0;
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
        _immunity_slider.value = (float)(data_object.immunity_chance/0.05);
        _immunity_slider_text.text = (data_object.immunity_chance * 100).ToString();


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
    {;
        if (data_object.settings_is_showing)
        {
            _show_settings_button_text.text = "Show Settings";
        }
        else
        {
            _show_settings_button_text.text = "Hide Settings";
        }
        data_object.settings_is_showing = !data_object.settings_is_showing;
    }

    public void Change_Graph_Menu_Visibility()
    {
        data_object.graph_settings_is_showing = !data_object.graph_settings_is_showing;
    }


    public void Update_Population_Dropdown_Options()
    {
        _population_preset_dropdown.ClearOptions();

        // initialises local variables and gets the list of strings
        List<TMP_Dropdown.OptionData> data_text = new List<TMP_Dropdown.OptionData>();
        List<string> names = database_manager.Get_Population_Names();
        TMP_Dropdown.OptionData new_data = new TMP_Dropdown.OptionData();

        new_data.text = "";
        data_text.Add(new_data);

        // Iterates through the returned list of strings
        for (int i = 0; i < names.Count; i++)
        {
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

        new_data.text = "";
        data_text.Add(new_data);

        for (int i = 0; i < names.Count; i++)
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

    public void Update_Graph_Dropdown_Options()
    {
        // Character Lengths for the dropdown options
        const int id_char_length = 6;
        const int pop_char_length = 25;
        const int dis_char_length = 25;
        const int pop_count_char_length = 6;

        _graph_1_options_dropdown.ClearOptions();
        List<TMP_Dropdown.OptionData> data_text = new List<TMP_Dropdown.OptionData>();
        List<List<string>> option_text = database_manager.Get_Graph_Options();
        TMP_Dropdown.OptionData temp = new TMP_Dropdown.OptionData();

        temp.text = "ID    Population Preset        Disease Preset    Population Count";
        data_text.Add(temp);



        for (int i = 0; i < option_text.Count; i++)
        {
            temp = new TMP_Dropdown.OptionData();
            /*
            temp.text = option_text[i][0] + option_text[i][1] + option_text[i][2] + option_text[i][3];
            data_text.Add(temp);
            */

            temp.text += option_text[i][0];
            for(int a = 0; a < id_char_length - option_text[i][0].Length; a++)
            {
                temp.text += " ";
            }

            temp.text += option_text[i][1];
            for(int b = 0; b < pop_char_length - option_text[i][1].Length; b++)
            {
                temp.text += " ";
            }

            temp.text += option_text[i][2];
            for (int c = 0; c < dis_char_length - option_text[i][2].Length; c++)
            {
                temp.text += " ";
            }
            
            temp.text += option_text[i][3];
            for (int d = 0; d < pop_count_char_length - option_text[i][3].Length; d++)
            {
                temp.text += " ";
            }

            data_text.Add(temp);
        }

        _graph_1_options_dropdown.AddOptions(data_text);
        _graph_2_options_dropdown.AddOptions(data_text);
    }

    public void Plot_Graph()
    {
        if(_graph_1_options_dropdown.value != 0) // if 1st graph selected
        {
            if (_graph_2_options_dropdown.value != 0)   // if 2nd graph selected
            {
                Debug.Log("Second Graph Detected");
                // Split string and grab ID from the caption text for first dropdown menu
                string[] value = _graph_1_options_dropdown.captionText.text.ToString().Split(" ");
                data_object.chosen_simulation_id_1 = int.Parse(value[0]);
                // Same as above but for the second dropdown
                value = _graph_2_options_dropdown.captionText.text.ToString().Split(" ");
                data_object.chosen_simulation_id_2 = int.Parse(value[0]);

                // Run Graph Compute Shader with 2 values passed in
                compute_manager.Compare_Graph_Compute_Shader(data_object.chosen_simulation_id_1, data_object.chosen_simulation_id_2);
                
                //Update dropdowns if any needed
                Update_Graph_Dropdown_Options();

                data_object.graph_active = true; // Allow updating of frame
                _close_graph_button.alpha = 1; // Set graph menu visible
                Graph_Data_Menu.alpha = 1;
                data_object.chosen_graph_drawn = false; // allow querying of database
                data_object.settings_is_showing = false; // hide unneeded settings
                data_object.graph_settings_is_showing = false;

                _graph_1_label.text = data_object.chosen_simulation_id_1.ToString();
                _graph_2_label.text = data_object.chosen_simulation_id_2.ToString();
            }
            else
            {
                Debug.Log("One Graph Detected");
                string[] value = _graph_1_options_dropdown.captionText.text.ToString().Split(" ");
                data_object.chosen_simulation_id = int.Parse(value[0]);
                data_object.chosen_graph_drawn = false;
                compute_manager.Graph_Compute_Shader(data_object.chosen_simulation_id);
                data_object.graph_active = true;

                _close_graph_button.alpha = 1;
                Graph_Data_Menu.alpha = 1;
                Update_Graph_Dropdown_Options();

                data_object.settings_is_showing = false;
                data_object.graph_settings_is_showing = false;
                _graph_1_label.text = data_object.chosen_simulation_id.ToString();
                _graph_2_label.text = "";
            }
            
        }
    }


    public void Update_Graph_Data(int healthy, int infected, int recovering, int immune)
    {
        _healthy_graph_data.text = healthy.ToString();
        _infected_graph_data.text = infected.ToString();
        _recovered_graph_data.text = recovering.ToString();
        _immmune_graph_data.text = immune.ToString();
    }

    public void Update_Graph_2_Data(int healthy, int infected, int recovering, int immune)
    {
        _healthy_graph_2_data.text = healthy.ToString();
        _infected_graph_2_data.text = infected.ToString();
        _recovered_graph_2_data.text = recovering.ToString();
        _immmune_graph_2_data.text = immune.ToString();
    }

    public void Update_R_Values(double R_value_1, double R_value_2)
    {
        if (R_value_1 == 100){
            _graph_1_R_value.text = " "; // Hide R Values
        }
        else 
        {
            _graph_1_R_value.text = R_value_1.ToString("0.0000");
        }    
        if(R_value_2 == 100)
        {
            _graph_2_R_value.text = " "; // Hide R Values
        }
        else
        {
            _graph_2_R_value.text = R_value_2.ToString("0.0000");
        }
        
    }

    public void Generate_CSV()
    {
        int simulation_id = data_object.chosen_simulation_id;
        List<List<int>> results_data = database_manager.Get_Results_Data(simulation_id);

        Debug.Log(results_data.Count);
        string folder_path = Path.Combine(Application.dataPath, "Results");

        string file_name = "Simulation_" + data_object.simulation_id.ToString() + ".csv";
        string file_path = Path.Combine(folder_path, file_name);

        // Check that the folder exists
        if (!Directory.Exists(folder_path))
        {
            Directory.CreateDirectory(folder_path);
        }

        // Writing to the CSV
        using(StreamWriter writer = new StreamWriter(file_path))
        {
            // write headers
            writer.WriteLine("Healthy, Infected, Recovered, Immune");

            for(int i = 0; i < results_data.Count; i++)
            {
                // Write Data
                writer.WriteLine(
                    results_data[i][2].ToString() + "," +
                    results_data[i][3].ToString() + "," +
                    results_data[i][4].ToString() + "," +
                    results_data[i][5].ToString());
            }
        }
        Debug.Log("CSV File saved at: " + file_path);
    }
}