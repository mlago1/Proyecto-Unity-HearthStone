using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static int MaxCartasTablero = 4;

    public Transform CartaSeleccionada { get; set; }
    public Transform CartaObjetivo { get; set; }
    public bool HayCartaSeleccionada { get; set; }

    void Start()
    {
        HayCartaSeleccionada = false;
    }

    void Update()
    {
        ComprobarSaludEsbirros();
    }

    private void ComprobarSaludEsbirros()
    {
        foreach (Transform t in GameObject.Find("TableroJugador").transform)
        {
            if (t.name == "New Game Object")
                continue;
            if (t.GetComponent<EstadisticasEsbirro>().Salud <= 0)
                t.SetParent(GameObject.Find("Cementerio").transform);
        }
        foreach (Transform t in GameObject.Find("TableroEnemigo").transform)
        {
            if (t.name == "New Game Object")
                continue;
            if (t.GetComponent<EstadisticasEsbirro>().Salud <= 0)
                t.SetParent(GameObject.Find("Cementerio").transform);
        }
    }

    private void IntercambiarDañoCartas()
    {
        CartaSeleccionada.GetComponent<EstadisticasEsbirro>().Salud -=
            CartaObjetivo.GetComponent<EstadisticasEsbirro>().Ataque;

        CartaObjetivo.GetComponent<EstadisticasEsbirro>().Salud -=
            CartaSeleccionada.GetComponent<EstadisticasEsbirro>().Ataque;
    }
}
