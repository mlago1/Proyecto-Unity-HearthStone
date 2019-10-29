using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EsbirroAtacante : MonoBehaviour, IPointerClickHandler
{
    void Start()
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!FindObjectOfType<GameController>().HayCartaSeleccionada && transform.parent.name == "TableroJugador")
        {
            FindObjectOfType<GameController>().CartaSeleccionada = transform;
            FindObjectOfType<GameController>().HayCartaSeleccionada = true;
            FindObjectOfType<GameController>().CartaSeleccionada.GetComponent<CanvasGroup>().alpha = 0.8f;
        }
        else if (FindObjectOfType<GameController>().HayCartaSeleccionada && transform.parent.name == "TableroJugador")
        {
            FindObjectOfType<GameController>().CartaSeleccionada.GetComponent<CanvasGroup>().alpha = 1f;
            FindObjectOfType<GameController>().CartaSeleccionada = transform;
            FindObjectOfType<GameController>().HayCartaSeleccionada = true;
            FindObjectOfType<GameController>().CartaSeleccionada.GetComponent<CanvasGroup>().alpha = 0.8f;
        }
        else if (FindObjectOfType<GameController>().HayCartaSeleccionada)
        {
            if (transform.parent.name == "TableroEnemigo")
            {
                FindObjectOfType<GameController>().CartaObjetivo = transform;
                FindObjectOfType<GameController>().SendMessage("IntercambiarDañoCartas");
                FindObjectOfType<GameController>().HayCartaSeleccionada = false;
                FindObjectOfType<GameController>().CartaSeleccionada.GetComponent<CanvasGroup>().alpha = 1f;
            }
        }
    }
}
