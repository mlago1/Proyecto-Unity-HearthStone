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
    public static int TiempoTurnos = 40;
    public static int TiempoPausa = 2;
    static int cartasRepetidasPorMazo = 4;

    public Transform CartaSeleccionada { get; set; }
    public Transform CartaObjetivo { get; set; }
    public bool HayCartaSeleccionada { get; set; }

    public Transform HeroeJugador { get; set; }
    public Transform HeroeEnemigo { get; set; }

    public bool esTurnoJugador { get; set; }
    public bool botonPulsado { get; set; }

    float tiempoPorTurno;

    void Start()
    {
        HayCartaSeleccionada = false;
        tiempoPorTurno = TiempoTurnos;
        esTurnoJugador = true;
        botonPulsado = false;
        HeroeJugador = GameObject.Find("HeroeJugador").transform;
        HeroeEnemigo = GameObject.Find("HeroeEnemigo").transform;
        CargarMazos();
        StartCoroutine(RobarCarta(HeroeJugador,"MazoJugador","ManoJugador",false,3));
        StartCoroutine(RobarCarta(HeroeEnemigo, "MazoEnemigo", "ManoEnemigo", true, 3));
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
            esTurnoJugador = false;
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
        if (botonPulsado)
        {
            MostrarMensaje("No es tu turno");
            return;
        }
        tiempoPorTurno = TiempoTurnos;
        if (esTurnoJugador)
            StartCoroutine(TurnoJugador());
        else
        {
            MostrarMensaje("Turno Enemigo");
            StartCoroutine(TurnoEnemigo());
        }
    }

    IEnumerator TurnoJugador()
    {
        ActualizarMana(HeroeJugador);
        ActivarCartasDormidas("TableroJugador");
        StartCoroutine(RobarCarta(HeroeJugador, "MazoJugador", "ManoJugador", false, 1));
        esTurnoJugador = false;
        yield return new WaitForSeconds(TiempoPausa);
        botonPulsado = false;
        CartaObjetivo = null;
        if(CartaSeleccionada != null)
        {
            CartaSeleccionada.GetComponent<CanvasGroup>()
                .alpha = 1;
            CartaSeleccionada = null;
        }
        MostrarMensaje("Tu Turno");

        HayCartaSeleccionada = false;
    }

    IEnumerator TurnoEnemigo()
    {
        botonPulsado = true;
        ActualizarMana(HeroeEnemigo);
        ActivarCartasDormidas("TableroEnemigo");
        StartCoroutine(RobarCarta(HeroeEnemigo, "MazoEnemigo", "ManoEnemigo", true, 1));
        HacerJugarEnemigo();
        esTurnoJugador = true;
        yield return new WaitForSeconds(TiempoPausa*2);
        botonPulsado = false;
        CartaObjetivo = null;
        if (CartaSeleccionada != null)
        {
            CartaSeleccionada.GetComponent<CanvasGroup>()
                .alpha = 1;
            CartaSeleccionada = null;
        }

        HayCartaSeleccionada = false;
        IniciarSiguienteTurno();
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

    IEnumerator RobarCarta(Transform heroe, string mazo, string mano, bool ocultarCarta, int numeroCartasRobar)
    {
        for (int i = 0; i < numeroCartasRobar; i++)
        {
            yield return new WaitForSeconds(TiempoPausa);
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
                break;
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
            }
        }
    }

    void HacerJugarEnemigo()
    {
        StartCoroutine(EnemigoJugarCartas());
        StartCoroutine(EnemigoAtacarCartas());
    }

    IEnumerator EnemigoJugarCartas()
    {
        if (GameObject.Find("TableroEnemigo").transform.childCount
            < MaxCartasTablero)
        {
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
                if (carta.GetComponent<EstadisticasEsbirro>()
                    .Coste <= HeroeEnemigo.GetComponent<EstadisticasHeroe>()
                    .ManaDisponible)
                {
                    CartasDefinitivas.Add(carta);
                    HeroeEnemigo.GetComponent<EstadisticasHeroe>()
                        .ManaDisponible -= carta.GetComponent<EstadisticasEsbirro>()
                        .Coste;
                }
            }
            foreach (Transform carta in CartasDefinitivas)
            {
                yield return new WaitForSeconds(TiempoPausa);
                carta.SetParent(GameObject.Find("TableroEnemigo").transform);
                carta.GetComponent<EstadisticasEsbirro>()
                    .CartaJugada = true;
                carta.GetComponent<EstadisticasEsbirro>()
                    .CartaOculta = false;
                if (GameObject.Find("TableroEnemigo").transform.childCount
                >= MaxCartasTablero)
                    break;
            }
        }
    }

    IEnumerator EnemigoAtacarCartas()
    {
        if (GameObject.Find("TableroEnemigo").transform.childCount > 0)
        {
            foreach (Transform carta in GameObject.Find("TableroEnemigo").transform)
            {
                yield return new WaitForSeconds(TiempoPausa);
                if (!carta.GetComponent<EstadisticasEsbirro>()
                        .CartaDormida && carta.GetComponent<EstadisticasEsbirro>()
                        .Ataque > 0)
                {
                    CartaSeleccionada = carta;
                    if (GameObject.Find("TableroJugador").transform.childCount == 0)
                    {
                        AtacarHeroeJugador();
                        break;
                    }
                    List<Transform> CartasAtacables = new List<Transform>();
                    foreach (Transform cartaJugador in GameObject.Find("TableroJugador").transform)
                    {
                        if (cartaJugador.GetComponent<EstadisticasEsbirro>().
                            Provocar)
                        {
                            CartaObjetivo = cartaJugador;
                            break;
                        }
                        if (!cartaJugador.GetComponent<EstadisticasEsbirro>().
                            Sigilo)
                            CartasAtacables.Add(cartaJugador);
                    }

                    if (CartasAtacables.Count == 0 &&
                        CartaObjetivo == null)
                    {
                        AtacarHeroeJugador();
                        break;
                    }
                    else if (CartaObjetivo == null)
                    {
                        CartaObjetivo = CartasAtacables[UnityEngine.Random.Range(0,
                            CartasAtacables.Count)];
                    }

                    IntercambiarDañoCartas();
                    CartaSeleccionada = null;
                    CartaObjetivo = null;
                }
            }
        }
    }

    void AtacarHeroeJugador()
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
    }

    public void MostrarMensaje(string mensaje)
    {
        StartCoroutine(auxMens(mensaje));
    }

    IEnumerator auxMens(string mensaje)
    {
        GameObject.Find("Avisos").GetComponent<TextMeshProUGUI>()
            .text = mensaje;
        yield return new WaitForSeconds(TiempoPausa);
        GameObject.Find("Avisos").GetComponent<TextMeshProUGUI>()
            .text = "";
    }
}
