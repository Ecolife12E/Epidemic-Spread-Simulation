using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeManager : MonoBehaviour
{
    public ComputeShader compute_shader;
    [SerializeField]
    private RenderTexture render_texture;

    // DataObject that holds all the needed data
    public DataObject data_object;


    public struct Person
    {
        public Vector2 position;
        public Vector2 target_position;

        public float speed_percentage;
        public int health_state;

        public float remaining_time;
    }

   
    private int buffer_size; // size of each person in bits
    private Person[] buffer_data; // array to save population data to

    private Vector2[] debug_buffer_data;








    // Start is called before the first frame update
    void Start()
    {
        render_texture = new RenderTexture(data_object.texture_width, data_object.texture_height, 24);
        render_texture.enableRandomWrite = true;
        render_texture.Create();

        // Passes render_texture to the compute shader under the variable name Result
        compute_shader.SetTexture(0, "Result", render_texture);

        // Compute Buffer to hold data being sent to GPU
        buffer_size = sizeof(float) * 6 + sizeof(int);
        buffer_data = new Person[data_object.population_count];

        debug_buffer_data = new Vector2[data_object.population_count];

        // create the population with some random values
        for (int i = 0; i < data_object.population_count; i++)
        {
            buffer_data[i].position = new Vector2(Random.Range(0, data_object.texture_width), Random.Range(0, data_object.texture_height));
            buffer_data[i].target_position = new Vector2(Random.Range(0, data_object.texture_width), Random.Range(0, data_object.texture_height));
            buffer_data[i].speed_percentage = Random.Range(50, 100);
            buffer_data[i].remaining_time = data_object.infectious_time.y;

            if( i == 0)
            {
                buffer_data[i].health_state = 1;
            }
            else
            {
                buffer_data[i].health_state = 0;
            }
        }

        ComputeBuffer buffer = new ComputeBuffer(buffer_data.Length, buffer_size);
        buffer.SetData(buffer_data);

        ComputeBuffer debug_buffer = new ComputeBuffer(debug_buffer_data.Length, sizeof(float) * 2);
        debug_buffer.SetData(debug_buffer_data);


        compute_shader.SetBuffer(0, "buffer", buffer);
        compute_shader.SetFloat("global_speed", data_object.global_speed);
        compute_shader.SetFloat("min_distance", data_object.min_distance);
        compute_shader.SetFloat("PI", Mathf.PI);
        compute_shader.SetInt("texture_width", data_object.texture_width);
        compute_shader.SetInt("texture_height", data_object.texture_height);
        compute_shader.SetFloat("radius", data_object.radius);
        compute_shader.SetInt("number_of_sensors", data_object.number_of_sensors);
        compute_shader.SetBool("show_sensors", data_object.show_sensors);
        compute_shader.SetBuffer(0, "debug_buffer", debug_buffer);
        compute_shader.SetFloat("deltatime", Time.deltaTime);
        compute_shader.SetFloat("min_infectious_time", data_object.infectious_time.x);
        compute_shader.SetFloat("max_infectious_time", data_object.infectious_time.y);
        compute_shader.SetFloat("min_recovering_time", data_object.recovering_time.x);
        compute_shader.SetFloat("max_recovering_time", data_object.recovering_time.y);

        // Allows the Compute Shader to run, with the needed threads
        compute_shader.Dispatch(0, 512, 1, 1);


        buffer.GetData(buffer_data);
        buffer.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        render_texture.Release();
        compute_shader.SetTexture(0, "Result", render_texture);

        ComputeBuffer buffer = new ComputeBuffer(buffer_data.Length, buffer_size);
        buffer.SetData(buffer_data);
        compute_shader.SetBuffer(0, "buffer", buffer);

        ComputeBuffer debug_buffer = new ComputeBuffer(debug_buffer_data.Length, sizeof(float)*2);
        debug_buffer.SetData(debug_buffer_data);
        compute_shader.SetBuffer(0, "debug_buffer", debug_buffer);

        compute_shader.SetFloat("global_speed", data_object.global_speed);
        compute_shader.SetFloat("min_distance", data_object.min_distance);
        compute_shader.SetFloat("PI", Mathf.PI);
        compute_shader.SetInt("texture_width", data_object.texture_width);
        compute_shader.SetInt("texture_height", data_object.texture_height);
        compute_shader.SetFloat("radius", data_object.radius);
        compute_shader.SetInt("number_of_sensors", data_object.number_of_sensors);
        compute_shader.SetBool("show_sensors", data_object.show_sensors);
        compute_shader.SetFloat("deltatime", Time.deltaTime);
        compute_shader.SetFloat("min_infectious_time", data_object.infectious_time.x);
        compute_shader.SetFloat("max_infectious_time", data_object.infectious_time.y);
        compute_shader.SetFloat("min_recovering_time", data_object.recovering_time.x);
        compute_shader.SetFloat("max_recovering_time", data_object.recovering_time.y);

        compute_shader.Dispatch(0, 512, 1, 1);

        buffer.GetData(buffer_data);
        debug_buffer.GetData(debug_buffer_data);

        //Debug.Log(buffer_data[0].position + buffer_data[0].target_position);
        //Debug.Log(debug_buffer_data[0]);

        buffer.Dispose();
        debug_buffer.Dispose();


    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // places the result of the graphics onto the screen
        Graphics.Blit(render_texture, destination);
    }
}
