using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeManager : MonoBehaviour
{
    public ComputeShader compute_shader;
    [SerializeField]
    private RenderTexture render_texture;


    public int texture_width;
    public int texture_height;


    public struct Person
    {
        public Vector2 position;
        public Vector2 target_position;

        public float speed_percentage;
        public int health_state;
    }

    public int population_count;
    public float global_speed;
    public float min_distance;


    private int buffer_size; // size of each person in bits
    private Person[] buffer_data; // array to save population data to

    private Vector2[] debug_buffer_data;

    // Start is called before the first frame update
    void Start()
    {
        render_texture = new RenderTexture(texture_width, texture_height, 24);
        render_texture.enableRandomWrite = true;
        render_texture.Create();

        // Passes render_texture to the compute shader under the variable name Result
        compute_shader.SetTexture(0, "Result", render_texture);

        // Compute Buffer to hold data being sent to GPU
        buffer_size = sizeof(float) * 5 + sizeof(int);
        buffer_data = new Person[population_count];

        debug_buffer_data = new Vector2[population_count];

        // create the population with some random values
        for (int i = 0; i < population_count; i++)
        {
            buffer_data[i].position = new Vector2(Random.Range(0, texture_width), Random.Range(0, texture_height));
            buffer_data[i].target_position = new Vector2(Random.Range(0, texture_width), Random.Range(0, texture_height));
            buffer_data[i].speed_percentage = (Random.Range(50, 100) / 100);
            buffer_data[i].health_state = 0;
        }

        ComputeBuffer buffer = new ComputeBuffer(buffer_data.Length, buffer_size);
        buffer.SetData(buffer_data);

        ComputeBuffer debug_buffer = new ComputeBuffer(debug_buffer_data.Length, sizeof(float) * 2);
        debug_buffer.SetData(debug_buffer_data);


        compute_shader.SetBuffer(0, "buffer", buffer);
        compute_shader.SetFloat("global_speed", global_speed);
        compute_shader.SetFloat("min_distance", min_distance);
        compute_shader.SetFloat("PI", Mathf.PI);
        compute_shader.SetInt("texture_width", texture_width);
        compute_shader.SetInt("texture_height", texture_height);

        compute_shader.SetBuffer(0, "debug_buffer", debug_buffer);

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

        compute_shader.SetFloat("global_speed", global_speed);
        compute_shader.SetFloat("min_distance", min_distance);
        compute_shader.SetFloat("PI", Mathf.PI);
        compute_shader.SetInt("texture_width", texture_width);
        compute_shader.SetInt("texture_height", texture_height);



        compute_shader.Dispatch(0, 512, 1, 1);

        buffer.GetData(buffer_data);
        debug_buffer.GetData(debug_buffer_data);

        //Debug.Log(buffer_data[0].position + buffer_data[0].target_position);
        Debug.Log(debug_buffer_data[0]);

        buffer.Dispose();
        debug_buffer.Dispose();


    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // places the result of the graphics onto the screen
        Graphics.Blit(render_texture, destination);
    }
}
