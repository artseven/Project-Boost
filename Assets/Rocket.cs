using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {

    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip death;


    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;



    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State { Alive, Dying, Transcending }
    State state = State.Alive;

    // Start is called before the first frame update
    void Start() {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        //todo somewhere stop sound on death
        if (state == State.Alive) { 
            RespondToThrustInput();
            Rotate();            
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (state != State.Alive) { //ignore collisions
            return;
        }

        switch (collision.gameObject.tag) {
            case "Friendly":
                //do nothing 
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            case "Death":
                StartDeathSequence();
                break;
        }
    }

    private void StartSuccessSequence() {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        successParticles.Play();
        Invoke("LoadNextLevel", 1f);
    }

    private void StartDeathSequence() {
        state = State.Dying;
        deathParticles.Play();
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        Invoke("LoadFirstLevel", 1f);
    }



    private void LoadNextLevel() {
        SceneManager.LoadScene(1); //todo allow for more than 2 levels;
    }

    private void LoadFirstLevel() {
        SceneManager.LoadScene(0); 
    }

    private void RespondToThrustInput() {
        if (Input.GetKey(KeyCode.Space))
        { //can thrust while rotating
            ApplyThrust();
        }
        else {
            audioSource.Stop();
            //mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust()
    {
        mainEngineParticles.Play();
        rigidBody.AddRelativeForce(Vector3.up * mainThrust);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
    }

    private void Rotate() {
        rigidBody.freezeRotation = true; //take manual control of rotation

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A)) {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        } else if (Input.GetKey(KeyCode.D)) {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
        rigidBody.freezeRotation = false; //resume physics control of rotation
    }
}