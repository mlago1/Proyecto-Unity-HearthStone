using UnityEngine;
using UnityEngine.UI;

public class EstadisticasEsbirro : MonoBehaviour
{
    //Campos Visibles
    [SerializeField] public string Nombre;
    [SerializeField] public string Descripcion;

    //Estadisticas
    [SerializeField] public int Coste;
    [SerializeField] public int Ataque;
    [SerializeField] public int Salud;

    //Propiedades
    public bool CartaJugada;
    public bool CartaDormida;
    public int NumeroAtaques;
    [SerializeField] public bool Cargar;
    [SerializeField] public bool Provocar;
    [SerializeField] public bool Veneno;
    [SerializeField] public bool EscudoDivino;
    [SerializeField] public bool Sigilo;

    private void Start()
    {
        CartaJugada = false;
        CartaDormida = Cargar ? false : true;
        NumeroAtaques = 1;
    }
    void Update()
    {
        transform.GetChild(0).GetComponent<Text>().text = Coste.ToString();
        transform.GetChild(1).GetComponent<Text>().text = Ataque.ToString();
        transform.GetChild(2).GetComponent<Text>().text = Salud.ToString();
        transform.GetChild(3).GetComponent<Text>().text = Nombre.ToString();
        transform.GetChild(4).GetComponent<Text>().text = Descripcion.ToString();
    }
}
