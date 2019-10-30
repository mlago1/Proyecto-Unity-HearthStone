using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static int MaxCartasTablero = 4;
    public static int MaxCartasMano = 4;

    public Transform CartaSeleccionada { get; set; }
    public Transform CartaObjetivo { get; set; }
    public bool HayCartaSeleccionada { get; set; }

    int cartasRepetidasPorMazo;
    public bool esTurnoJugador { get; set; }

    float tiempoPorTurno;

    void Start()
    {
        HayCartaSeleccionada = false;
        tiempoPorTurno = 30;
        cartasRepetidasPorMazo = 2;
        esTurnoJugador = true;
        CargarMazos();
        IniciarSiguienteTurno();
    }

    void Update()
    {
        ComprobarSaludEsbirros();
        ActualizarTiempo();
    }

    void CargarMazos()
    {
        DirectoryInfo dir = new DirectoryInfo("Assets/Resources");
        FileInfo[] info = dir.GetFiles("*.prefab");
        for (int i = 0; i < cartasRepetidasPorMazo; i++)
            foreach (FileInfo f in info)
            {
                string fullPath = f.FullName.Replace(@"\", "/");
                string assetPath = fullPath.Replace(Application.dataPath, "");
                assetPath = assetPath.Replace("/Resources/", "");
                assetPath = assetPath.Replace(".prefab", "");

                GameObject carta = Instantiate(
                    Resources.Load(assetPath, typeof(GameObject))) as GameObject;
                carta.transform.SetParent(GameObject.Find("MazoJugador").transform);

                carta = Instantiate(
                    Resources.Load(assetPath, typeof(GameObject))) as GameObject;
                carta.transform.SetParent(GameObject.Find("MazoEnemigo").transform);
            }
    }

    void ActualizarTiempo()
    {
        GameObject.Find("Tiempo").GetComponent<Text>().text =
            "Tiempo: " + tiempoPorTurno.ToString("0"); ;
        tiempoPorTurno -= Time.deltaTime;
        if (tiempoPorTurno <= 0)
        {
            esTurnoJugador = !esTurnoJugador;
            IniciarSiguienteTurno();
        }
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
            CartaSeleccionada.GetComponent<EstadisticasEsbirro>().EscudoDivino ?
            0: CartaObjetivo.GetComponent<EstadisticasEsbirro>().Veneno ?
            CartaSeleccionada.GetComponent<EstadisticasEsbirro>().Salud :
            CartaObjetivo.GetComponent<EstadisticasEsbirro>().Ataque;

        if (CartaSeleccionada.GetComponent<EstadisticasEsbirro>().EscudoDivino)
            CartaSeleccionada.GetComponent<EstadisticasEsbirro>().EscudoDivino = false;

        CartaObjetivo.GetComponent<EstadisticasEsbirro>().Salud -=
            CartaObjetivo.GetComponent<EstadisticasEsbirro>().EscudoDivino ?
            0 : CartaSeleccionada.GetComponent<EstadisticasEsbirro>().Veneno ?
            CartaObjetivo.GetComponent<EstadisticasEsbirro>().Salud :
            CartaSeleccionada.GetComponent<EstadisticasEsbirro>().Ataque;

        if (CartaObjetivo.GetComponent<EstadisticasEsbirro>().EscudoDivino)
            CartaObjetivo.GetComponent<EstadisticasEsbirro>().EscudoDivino = false;

    }

    public void IniciarSiguienteTurno()
    {
        tiempoPorTurno = 40;
        if (esTurnoJugador)
        {
            ActivarCartasDormidas("TableroJugador");
            RobarCarta("MazoJugador","ManoJugador",false);
            esTurnoJugador = false;
        }
        else
        {
            ActivarCartasDormidas("TableroEnemigo");
            RobarCarta("MazoEnemigo", "ManoEnemigo",true);
            esTurnoJugador = true;
            IniciarSiguienteTurno();
        }
    }
    
    void ActivarCartasDormidas(string tablero)
    {
        foreach (Transform t in GameObject.Find(tablero).transform)
        {
            t.GetComponent<EstadisticasEsbirro>().CartaDormida = false;
            t.GetComponent<EstadisticasEsbirro>().YaHaAtacado = false;
        }
    }

    void RobarCarta(string mazo, string mano, bool ocultarCarta)
    {
        System.Random r = new System.Random();
        int posicion = r.Next(
            0, GameObject.Find(mazo).transform.childCount);
        GameObject cartaRobar = GameObject.Find(mazo).
            transform.GetChild(posicion).gameObject; //TO DO Crear heroe y recibir daño por fatiga
        if (GameObject.Find(mano).transform.childCount < MaxCartasMano)
        {
            cartaRobar.transform.SetParent(
                GameObject.Find(mano).transform);
            if (ocultarCarta)
                cartaRobar.GetComponent<EstadisticasEsbirro>()
                    .CartaOculta = true;
        }
        else
            cartaRobar.transform.SetParent(
                GameObject.Find("Cementerio").transform);
    }

    public void MostrarMensaje(string mensaje)
    {
        StartCoroutine(auxMens(mensaje));
    }

    IEnumerator auxMens(string mensaje)
    {
        GameObject.Find("Avisos").GetComponent<TextMeshProUGUI>()
            .text = mensaje;
        yield return new WaitForSeconds(2);
        GameObject.Find("Avisos").GetComponent<TextMeshProUGUI>()
            .text = "";
    }
}
