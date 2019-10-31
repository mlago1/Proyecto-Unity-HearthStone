using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HeroeAtacable : MonoBehaviour, IPointerClickHandler
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!ComprobarExisteProvocar())
            return;
        if(GameObject.Find("GameController").GetComponent<GameController>()
            .HayCartaSeleccionada)
        {
            GetComponent<EstadisticasHeroe>().Salud -=
                GameObject.Find("GameController").GetComponent<GameController>()
                .CartaSeleccionada.GetComponent<EstadisticasEsbirro>()
                .Ataque;

            GameObject.Find("GameController").GetComponent<GameController>()
                .CartaSeleccionada.GetComponent<EstadisticasEsbirro>()
                .YaHaAtacado = true;

            GameObject.Find("GameController").GetComponent<GameController>()
                .CartaSeleccionada.GetComponent<CanvasGroup>()
                .alpha = 1f;

            GameObject.Find("GameController").GetComponent<GameController>()
            .CartaSeleccionada = null;


            GameObject.Find("GameController").GetComponent<GameController>()
            .HayCartaSeleccionada = false;
        }
    }

    bool ComprobarExisteProvocar()
    {
        bool check = false;
        foreach (Transform t in GameObject.Find("TableroEnemigo").transform)
        {
            if (t.GetComponent<EstadisticasEsbirro>().Provocar)
            {
                check = true;
                break;
            }
        }
        if (check)
        {
            GameObject.Find("GameController")
            .SendMessage("MostrarMensaje",
            "Un esbirro con Provocar bloquea el paso");
            return false;
        }
        return true;
    }
}
