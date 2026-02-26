using UnityEngine;

public class FlashLight : MonoBehaviour
{
    [SerializeField] GameObject flashlight;
    private bool isOn = false;

    void Start()
    {
        if (flashlight != null)
            flashlight.SetActive(false);
    }

    void Update()
    {
        // Check if E key is pressed
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleFlashLight();
        }
    }

    void ToggleFlashLight()
    {
        isOn = !isOn;
        if (flashlight != null)
            flashlight.SetActive(isOn);
    }
}
