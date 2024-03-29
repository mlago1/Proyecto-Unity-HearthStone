﻿using UnityEngine;
using UnityEngine.EventSystems;

public class EsbirroAtacante : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameObject.Find("GameController").GetComponent<GameController>()
            .botonPulsado)
        {
            GameObject.Find("GameController").GetComponent<GameController>()
            .MostrarMensaje("No es tu turno");
            return;
        }

        if (GetComponent<EstadisticasEsbirro>().Ataque == 0
            && transform.parent.name == "TableroJugador")
        {
            GameObject.Find("GameController")
                .SendMessage("MostrarMensaje",
                "Este esbirro no tiene ataque");
            return;
        }

        if (GetComponent<EstadisticasEsbirro>().CartaDormida
            && transform.parent.name == "TableroJugador")
        {
            GameObject.Find("GameController")
                .SendMessage("MostrarMensaje",
                "Este esbirro necesita un turno para poder atacar");
            return;
        }

        if (GetComponent<EstadisticasEsbirro>().YaHaAtacado
            && transform.parent.name == "TableroJugador")
        {
            GameObject.Find("GameController")
                .SendMessage("MostrarMensaje",
                "Este esbirro ya ha atacado");
            return;
        }

        if (!FindObjectOfType<GameController>().HayCartaSeleccionada 
            && transform.parent.name == "TableroJugador")
        {
            FindObjectOfType<GameController>().
                CartaSeleccionada = transform;
            FindObjectOfType<GameController>().
                HayCartaSeleccionada = true;
            FindObjectOfType<GameController>().
                CartaSeleccionada.GetComponent<CanvasGroup>().alpha = 0.8f;
        }
        else if (FindObjectOfType<GameController>().HayCartaSeleccionada
            && transform.parent.name == "TableroJugador")
        {
            FindObjectOfType<GameController>()
                .CartaSeleccionada.GetComponent<CanvasGroup>().alpha = 1f;
            FindObjectOfType<GameController>()
                .CartaSeleccionada = transform;
            FindObjectOfType<GameController>()
                .HayCartaSeleccionada = true;
            FindObjectOfType<GameController>()
                .CartaSeleccionada.GetComponent<CanvasGroup>().alpha = 0.8f;
        }
        else if (FindObjectOfType<GameController>().HayCartaSeleccionada)
        {
            if (transform.parent.name == "TableroEnemigo")
            {
                if (!ComprobarExisteProvocar())
                    return;

                if (!ComprobarTieneSigilo())
                    return;
                FindObjectOfType<GameController>()
                    .CartaObjetivo = transform;
                FindObjectOfType<GameController>()
                    .SendMessage("IntercambiarDañoCartas");
                FindObjectOfType<GameController>()
                    .HayCartaSeleccionada = false;
                FindObjectOfType<GameController>()
                    .CartaSeleccionada
                    .GetComponent<EstadisticasEsbirro>().YaHaAtacado = true;
                FindObjectOfType<GameController>()
                    .CartaSeleccionada.GetComponent<CanvasGroup>().alpha = 1f;
                FindObjectOfType<GameController>()
                    .CartaSeleccionada = null;
                FindObjectOfType<GameController>()
                    .CartaObjetivo = null;
            }
        }
    }

    bool ComprobarTieneSigilo()
    {
        if(GetComponent<EstadisticasEsbirro>().Sigilo)
        {
            GameObject.Find("GameController")
                .SendMessage("MostrarMensaje",
                "No puedes atacar a objetivos en sigilo");
            return false;
        }

        return true;
    }

    bool ComprobarExisteProvocar()
    {
        if (!GetComponent<EstadisticasEsbirro>().Provocar)
        {
            bool check = false;
            foreach (Transform t in transform.parent.transform)
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
        }

        return true;
    }
}
