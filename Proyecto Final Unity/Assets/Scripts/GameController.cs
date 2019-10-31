using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public static int MaxCartasTablero = 4;
    public static int MaxCartasMano = 4;
    static int cartasRepetidasPorMazo = 2;

    public Transform CartaSeleccionada { get; set; }
    public Transform CartaObjetivo { get; set; }
    public bool HayCartaSeleccionada { get; set; }

    public Transform HeroeJugador { get; set; }
    public Transform HeroeEnemigo { get; set; }

    public bool esTurnoJugador { get; set; }

    float tiempoPorTurno;

    void Start()
    {
        HayCartaSeleccionada = false;
        tiempoPorTurno = 30;
        esTurnoJugador = true;
        HeroeJugador = GameObject.Find("HeroeJugador").transform;
        HeroeEnemigo = GameObject.Find("HeroeEnemigo").transform;
        CargarMazos();
        RobarCarta(HeroeJugador,"MazoJugador","ManoJugador",false,3);
        RobarCarta(HeroeEnemigo, "MazoEnemigo", "ManoEnemigo", true, 3);
        IniciarSiguienteTurno();
    }

    void Update()
    {
        ComprobarSaludEsbirros();
        ActualizarTiempo();
        ComprobarSaludHeroes();
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

    private void ComprobarSaludHeroes() 
    {
        //TEMPORAL
        if(HeroeJugador.GetComponent<EstadisticasHeroe>()
            .Salud <= 0 || HeroeEnemigo.GetComponent<EstadisticasHeroe>()
            .Salud <= 0)
        {
            SceneManager.LoadScene("Menu");
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
        tiempoPorTurno = 30;
        if (esTurnoJugador)
        {
            ActualizarMana(HeroeJugador);
            ActivarCartasDormidas("TableroJugador");
            RobarCarta(HeroeJugador,"MazoJugador","ManoJugador",false,1);
            esTurnoJugador = false;
        }
        else
        {
            ActualizarMana(HeroeEnemigo);
            ActivarCartasDormidas("TableroEnemigo");
            RobarCarta(HeroeEnemigo,"MazoEnemigo", "ManoEnemigo",true,1);
            HacerJugarEnemigo();
            esTurnoJugador = true;
            IniciarSiguienteTurno();
        }
    }

    void ActualizarMana(Transform heroe)
    {
        heroe.GetComponent<EstadisticasHeroe>()
                   .ManaMaximo += heroe.GetComponent<EstadisticasHeroe>()
                   .ManaMaximo < 10 ? 1 : 0;
        heroe.GetComponent<EstadisticasHeroe>()
            .ManaDisponible = heroe.GetComponent<EstadisticasHeroe>()
            .ManaMaximo;
    }
    
    void ActivarCartasDormidas(string tablero)
    {
        foreach (Transform t in GameObject.Find(tablero).transform)
        {
            t.GetComponent<EstadisticasEsbirro>().CartaDormida = false;
            t.GetComponent<EstadisticasEsbirro>().YaHaAtacado = false;
        }
    }

    void RobarCarta(Transform heroe, string mazo, string mano, bool ocultarCarta, int numeroCartasRobar)
    {
        for (int i = 0; i < numeroCartasRobar; i++)
        {
            System.Random r = new System.Random();
            int posicion = r.Next(
                0, GameObject.Find(mazo).transform.childCount);

            GameObject cartaRobar = null;

            if (GameObject.Find(mazo).
                transform.childCount == 0)
            {
                MostrarMensaje("No quedan cartas en el mazo. Pierdes " +
                    heroe.GetComponent<EstadisticasHeroe>()
                    .DañoFatiga + " puntos de salud");
                heroe.GetComponent<EstadisticasHeroe>()
                    .Salud -= heroe.GetComponent<EstadisticasHeroe>()
                    .DañoFatiga;
                heroe.GetComponent<EstadisticasHeroe>()
                    .DañoFatiga++;
                return;
            }

            cartaRobar = GameObject.Find(mazo).
                transform.GetChild(posicion).gameObject; 
            if (GameObject.Find(mano).transform.childCount < MaxCartasMano)
            {
                cartaRobar.transform.SetParent(
                    GameObject.Find(mano).transform);
                if (ocultarCarta)
                    cartaRobar.GetComponent<EstadisticasEsbirro>()
                        .CartaOculta = true;
            }
            else
            {
                cartaRobar.transform.SetParent(
                    GameObject.Find("Cementerio").transform);
                if (mano == "ManoJugador")
                    MostrarMensaje("No caben más cartas en la mano");
            }
        }
    }

    void HacerJugarEnemigo()
    {
        EnemigoJugarCartas();
        EnemigoAtacarCartas();
    }

    void EnemigoJugarCartas()
    {
        if (GameObject.Find("TableroEnemigo").transform.childCount
            >= MaxCartasTablero)
            return;
      

        List<Transform> CartasJugables = new List<Transform>();
        foreach (Transform carta in GameObject.Find("ManoEnemigo").transform)
        {
            if (carta.GetComponent<EstadisticasEsbirro>()
                .Coste <= HeroeEnemigo.GetComponent<EstadisticasHeroe>()
                .ManaDisponible)
            {
                CartasJugables.Add(carta);
            }
        }

        for (int i = 0; i < CartasJugables.Count; i++)
        {
            Transform temp = CartasJugables[i];
            int randomIndex = UnityEngine.Random.Range(i, CartasJugables.Count);
            CartasJugables[i] = CartasJugables[randomIndex];
            CartasJugables[randomIndex] = temp;
        }

        List<Transform> CartasDefinitivas = new List<Transform>();
        foreach (Transform carta in CartasJugables)
        {
            if(carta.GetComponent<EstadisticasEsbirro>()
                .Coste > HeroeEnemigo.GetComponent<EstadisticasHeroe>()
                .ManaDisponible)
                continue;
            else
            {
                CartasDefinitivas.Add(carta);
                HeroeEnemigo.GetComponent<EstadisticasHeroe>()
                    .ManaDisponible -= carta.GetComponent<EstadisticasEsbirro>()
                    .Coste;
            }
        }
        foreach(Transform carta in CartasDefinitivas)
        {
            carta.SetParent(GameObject.Find("TableroEnemigo").transform);
            carta.GetComponent<EstadisticasEsbirro>()
                .CartaJugada = true;
            carta.GetComponent<EstadisticasEsbirro>()
                .CartaOculta = false;

            if (GameObject.Find("TableroEnemigo").transform.childCount
            >= MaxCartasTablero)
                return;
        }
    }

    void EnemigoAtacarCartas()
    {
        if (GameObject.Find("TableroEnemigo").transform.childCount == 0)
            return;
        foreach(Transform carta in GameObject.Find("TableroEnemigo").transform)
        {
            if(!carta.GetComponent<EstadisticasEsbirro>()
                    .CartaDormida && carta.GetComponent<EstadisticasEsbirro>()
                    .Ataque > 0)
            {
                CartaSeleccionada = carta;
                if(GameObject.Find("TableroJugador").transform.childCount == 0)
                {
                    HeroeJugador.GetComponent<EstadisticasHeroe>()
                        .Salud -= CartaSeleccionada.GetComponent<EstadisticasEsbirro>()
                        .Ataque;
                    if (CartaSeleccionada.GetComponent<EstadisticasEsbirro>()
                        .Sigilo)
                        CartaSeleccionada.GetComponent<EstadisticasEsbirro>()
                        .Sigilo = false;

                    CartaSeleccionada = null;
                    CartaObjetivo = null;
                    return;
                }

                foreach(Transform cartaJugador in GameObject.Find("TableroJugador").transform)
                {
                    if(cartaJugador.GetComponent<EstadisticasEsbirro>().
                        Provocar)
                    {
                        CartaObjetivo = cartaJugador;
                        break;
                    }
                }
                if(CartaObjetivo == null)
                {
                    CartaObjetivo = GameObject.Find("TableroJugador").transform
                        .GetChild(UnityEngine.Random.Range(0, 
                        GameObject.Find("TableroJugador").transform.childCount));
                }

                IntercambiarDañoCartas();
                CartaSeleccionada = null;
                CartaObjetivo = null;
            }
        }
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
