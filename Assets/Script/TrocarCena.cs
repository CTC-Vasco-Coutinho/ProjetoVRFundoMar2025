using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class TrocarCena : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        Cena();

    }


    void Cena()
    {
        if (Keyboard.current.f2Key.wasPressedThisFrame)
        {

            // Load "Cena1"
            SceneManager.LoadScene(0);

        }

        if (Keyboard.current.f3Key.wasPressedThisFrame)
        {

            // Load "Cena1"
            SceneManager.LoadScene(1);

        }
    }

}
