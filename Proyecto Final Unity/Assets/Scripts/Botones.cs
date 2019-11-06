using UnityEngine;
using UnityEngine.SceneManagement;

public class Botones : MonoBehaviour
{
    public void CambiarScena()
    {
        switch(transform.name)
        {
            case "Jugar":
                SceneManager.LoadScene("Partida"); break;
            case "Salir":
                Application.Quit(); break;
            case "VolverMenu":
            case "Rendirse":
                SceneManager.LoadScene("Menu"); break;
            case "Instrucciones":
                SceneManager.LoadScene("Instrucciones"); break;
        }
    }
}
