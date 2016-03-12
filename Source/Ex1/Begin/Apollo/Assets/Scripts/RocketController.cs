using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RocketController : MonoBehaviour {
    const float MAXTHRUST = 20.0f;
    const float MECO = 25.0f;

    public SmoothFollow cameraTargetSettings;
    public AudioSource rocketBoosterSFX;
    public Light theSun;
    public Transform planetEarth;
    public ParticleSystem e1, e2, e3, e4, e5, fire, smoke, pad1, pad2, wind;
    public Text windDirectionText;
    public Button launchButton;

    public bool isGameOver = false;

    bool ignition = false;
    bool isBurning = false;
    bool initialCountdownDone = false;
    float thrustForce = 10.0f;
    float internalCountdown = 0.0f;
    float ignitionTime = 4.5f;
    Vector3 windDirection;
    float windModifier;

    public void Ignite()
    {
        ignition = true;
        Destroy(pad1.gameObject, 20);
        Destroy(pad1.gameObject, 20);
        Destroy(smoke.gameObject, 60);
        rocketBoosterSFX.PlayOneShot(rocketBoosterSFX.clip);
    }

    // Use this for initialization
    void Start() {
        windDirection = Vector3.left * Random.Range(200.0f, 1000.0f);
    }

    float WindRandomnessGenerator()
    {
        float windRandomness = 0.0f;
        int octaves = 0;
        int lastOctaveDistance = 100000;
        int increaseDistancePerOctave = 1;
        for (int i = 0; i < lastOctaveDistance; i += increaseDistancePerOctave)
        {
            octaves++;
            windRandomness += Mathf.PerlinNoise(
                transform.position.x / (lastOctaveDistance - i) + 1.0f,
                transform.position.y / (lastOctaveDistance - i) + 1.0f);
        }

        return windRandomness / octaves;
    }

    // Update is called once per frame
    void Update() {
        windModifier = WindRandomnessGenerator();

        EmitParticles();
        RepositionEarth();
        IsMECONow();
        HandleInput();

        //if (!isGameOver)
        //    windDirectionText.text = "<== " + (Mathf.Abs(windDirection.x / 10.0f)).ToString("0.00") + " m/s ===";
        //else windDirectionText.text = "";
    }

    void FixedUpdate()
    {
        Thrust();
        RotationThrust();
        ApplyWindForce();
    }

    void ApplyWindForce()
    {
        this.GetComponent<Rigidbody>().AddForce(windDirection * windModifier, ForceMode.Force);
    }
    void HandleInput()
    {
        if (Input.GetButtonDown("ButtonA") || Input.GetKeyDown(KeyCode.Space))
        {
            launchButton.gameObject.SetActive(false);
            Ignite();
        }


        if (IsBurning()) { 
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                this.transform.Rotate(-transform.forward * 1.0f * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                this.transform.Rotate(transform.forward * 1.0f * Time.deltaTime);
            }


            this.transform.Rotate(transform.forward * Input.GetAxis("XBoxHorizontal") * Time.deltaTime * 2.0f);
        }
    }
    void Thrust()
    {
        if (ignition)
        {
            internalCountdown += Time.deltaTime;
            if (internalCountdown >= ignitionTime && !initialCountdownDone)
            {
                initialCountdownDone = true;
                isBurning = true;
                GetComponent<Rigidbody>().isKinematic = false;
            }
        }

        if (IsBurning())
        {
            if (thrustForce < MAXTHRUST)
            {
                thrustForce += Time.deltaTime / 2.5f;
            }

            GetComponent<Rigidbody>().AddForce(transform.forward * thrustForce, ForceMode.Acceleration);
        }
    }
    void IsMECONow()
    {
        if (this.transform.position.y > 800.0f)
        {
            ignition = false;
            //isBurning = false;
            cameraTargetSettings.distance = 25.0f;
            cameraTargetSettings.height = 10.0f;
            cameraTargetSettings.heightDamping = 1000.0f;
            GetComponent<Rigidbody>().useGravity = false;
            theSun.intensity = 0.0f;

            isGameOver = true;

            rocketBoosterSFX.volume -= Time.deltaTime;
        }
    }
    void EmitParticles()
    {
        if (ignition)
        {
            if (fire != null)
                fire.Emit(5);


            //if(smoke != null)
            //    smoke.Emit(1);
        }

        if(IsBurning())
        {
            e1.gameObject.SetActive(true);
            e2.gameObject.SetActive(true);
            e3.gameObject.SetActive(true);
            e4.gameObject.SetActive(true);
            e5.gameObject.SetActive(true);


            if (pad1 != null)
                pad1.Emit(5);

            if (pad2 != null)
                pad2.Emit(2);
        }
        else
        {
            e1.gameObject.SetActive(false);
            e2.gameObject.SetActive(false);
            e3.gameObject.SetActive(false);
            e4.gameObject.SetActive(false);
            e5.gameObject.SetActive(false);
        }
    }
    void RotationThrust()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * Input.GetAxis("XBLeftThumbVertical") * 5.0f, ForceMode.Acceleration);
    }
    void RepositionEarth()
    {
        planetEarth.position = this.transform.position + ( Vector3.down * 110.0f * 2.0f);
    }

    bool IsBurning()
    {
        return isBurning;
    }
}
