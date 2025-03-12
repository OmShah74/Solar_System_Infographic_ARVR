using UnityEngine;

public class Controller : MonoBehaviour
{
    private Transform[] planets;
    private Transform sun;
    private float[] orbitSpeeds;
    private float[] rotationSpeeds;
    private LineRenderer[] orbitLines;
    private Transform cameraTransform;
    private float[] orbitAngles;
    private float[] semiMajorAxes;
    private float[] semiMinorAxes;
    private float[] orbitEccentricities;
    
    [SerializeField] private float zoomSpeed = 50f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float revolutionSpeed = 5000f;
    [SerializeField] private float minZoom = 20f;
    [SerializeField] private float maxZoom = 200f;

    void Start()
    {
        cameraTransform = Camera.main.transform;

        sun = GameObject.FindGameObjectWithTag("Sun").transform;
        if (sun == null)
        {
            Debug.LogError("Sun not found! Make sure it has the correct tag.");
            return;
        }
        
        // Adjust Sun scale to a more balanced size
        sun.localScale = new Vector3(20f, 20f, 20f);

        string[] planetNames = { "Mercury", "Venus", "Earth", "Mars", "Jupiter", "Saturn", "Uranus", "Neptune", "Pluto" };
        planets = new Transform[planetNames.Length];
        orbitLines = new LineRenderer[planetNames.Length];
        orbitAngles = new float[planetNames.Length];

        orbitSpeeds = new float[] { 478.7f, 350.2f, 297.8f, 240.7f, 130.7f, 96.9f, 68.1f, 54.3f, 46.7f };
        rotationSpeeds = new float[] { 
            3.5f,    // Mercury
            0.85f,   // Venus
            20.9f,   // Earth
            20.3f,   // Mars
            50.4f,   // Jupiter
            46.9f,   // Saturn
            29.0f,   // Uranus
            31.0f,   // Neptune
            3.26f    // Pluto
        };

        semiMajorAxes = new float[] { 15.0f, 23.0f, 30.0f, 45.0f, 125.0f, 200.0f, 350.0f, 500.0f, 650.0f };
        orbitEccentricities = new float[] { 0.206f, 0.007f, 0.017f, 0.093f, 0.049f, 0.057f, 0.046f, 0.010f, 0.244f };
        semiMinorAxes = new float[semiMajorAxes.Length];

        for (int i = 0; i < semiMajorAxes.Length; i++)
        {
            semiMinorAxes[i] = semiMajorAxes[i] * Mathf.Sqrt(1 - orbitEccentricities[i] * orbitEccentricities[i]);
        }

        for (int i = 0; i < planetNames.Length; i++)
        {
            GameObject planetObject = GameObject.FindGameObjectWithTag(planetNames[i]);
            if (planetObject != null)
            {
                planets[i] = planetObject.transform;
                CreateOrbit(planets[i], i);
            }
            else
            {
                Debug.LogError("Planet not found: " + planetNames[i]);
            }
        }
    }

    void Update()
    {
        MovePlanets();
        HandleCamera();
    }

    void MovePlanets()
    {
        for (int i = 0; i < planets.Length; i++)
        {
            if (planets[i] != null)
            {
                orbitAngles[i] += orbitSpeeds[i] * Time.deltaTime / 50;
                float angleRad = orbitAngles[i] * Mathf.Deg2Rad;

                float x = semiMajorAxes[i] * Mathf.Cos(angleRad);
                float z = semiMinorAxes[i] * Mathf.Sin(angleRad);
                planets[i].position = sun.position + new Vector3(x, 0, z);

                planets[i].Rotate(Vector3.up, rotationSpeeds[i] * Time.deltaTime);
            }
        }
    }

    void HandleCamera()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        float newZoom = Mathf.Clamp(cameraTransform.position.magnitude - scroll * zoomSpeed * Time.deltaTime, minZoom, maxZoom);
        cameraTransform.position = cameraTransform.position.normalized * newZoom;

        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            cameraTransform.RotateAround(sun.position, Vector3.up, mouseX);
            cameraTransform.RotateAround(sun.position, cameraTransform.right, -mouseY);
        }
    }

    void CreateOrbit(Transform planet, int index)
    {
        GameObject orbitObject = new GameObject("Orbit_" + planet.name);
        LineRenderer line = orbitObject.AddComponent<LineRenderer>();

        int segments = 180;
        line.positionCount = segments;
        line.startWidth = 0.01f;
        line.endWidth = 0.01f;
        line.useWorldSpace = true;
        line.material = new Material(Shader.Find("Unlit/Color"));
        line.material.color = new Color(1f, 1f, 1f, 0.1f);

        Vector3[] points = new Vector3[segments];
        for (int i = 0; i < segments; i++)
        {
            float angle = (360f / segments) * i * Mathf.Deg2Rad;
            float x = semiMajorAxes[index] * Mathf.Cos(angle);
            float z = semiMinorAxes[index] * Mathf.Sin(angle);
            points[i] = sun.position + new Vector3(x, 0, z);
        }
        line.SetPositions(points);

        orbitLines[index] = line;
    }
}
