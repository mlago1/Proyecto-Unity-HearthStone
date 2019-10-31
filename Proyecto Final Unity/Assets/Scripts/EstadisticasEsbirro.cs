using UnityEngine;
using UnityEngine.UI;

public class EstadisticasEsbirro : MonoBehaviour
{
    //Campos Visuales
    [SerializeField] public string Nombre;
    [SerializeField] public string Descripcion;
    [SerializeField] public Texture Imagen;
    [SerializeField] public Texture Dorso;

    //Estadisticas
    [SerializeField] public int Coste;
    [SerializeField] public int Ataque;
    [SerializeField] public int Salud;

    //Propiedades
    public bool CartaOculta;
    public bool CartaJugada;
    public bool CartaDormida;
    public bool YaHaAtacado;
    [SerializeField] public bool Cargar;
    [SerializeField] public bool Provocar;
    [SerializeField] public bool Veneno;
    [SerializeField] public bool EscudoDivino;
    [SerializeField] public bool Sigilo;

    private void Start()
    {
        CartaJugada = false;
        CartaDormida = Cargar ? false : true;
        YaHaAtacado = false;
    }
    void Update()
    {
        ActualizarDatos();
        ComprobarPropiedades();
        ActualizarCartasOcultas();
    }

    void ActualizarDatos()
    {
        transform.GetChild(0).GetComponent<Text>()
            .text = Coste.ToString();
        transform.GetChild(1).GetComponent<Text>()
            .text = Ataque.ToString();
        transform.GetChild(2).GetComponent<Text>()
            .text = Salud.ToString();
        transform.GetChild(3).GetComponent<Text>()
            .text = Nombre.ToString();
        transform.GetChild(4).GetComponent<Text>()
            .text = Descripcion.ToString();
    }

    void ComprobarPropiedades()
    {
        transform.GetChild(5).gameObject.SetActive(
            EscudoDivino && !CartaOculta ? true : false);
        transform.GetChild(6).gameObject.SetActive(
            Sigilo && !CartaOculta ? true : false);
    }

    void ActualizarCartasOcultas()
    {
        if (CartaOculta)
        {
            GetComponent<RawImage>().texture = Dorso;
            foreach (Transform child in transform)
                child.gameObject.SetActive(false);
        }
        else
        {
            GetComponent<RawImage>().texture = Imagen;
            for (int i = 0; i < 5; i++)
                transform.GetChild(i).gameObject.SetActive(true);
        }
    }
}
