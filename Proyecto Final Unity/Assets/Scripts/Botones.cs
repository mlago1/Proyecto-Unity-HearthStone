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
        }
    }
}
