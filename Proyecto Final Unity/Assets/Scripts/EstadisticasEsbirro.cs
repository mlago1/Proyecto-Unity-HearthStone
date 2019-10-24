using UnityEngine;
using UnityEngine.UI;

public class EstadisticasEsbirro : MonoBehaviour
{
    [SerializeField] public int Coste;
    [SerializeField] public int Ataque;
    [SerializeField] public int Salud;
    [SerializeField] public string Nombre;
    [SerializeField] public string Descripcion;
    [SerializeField] public Texture Imagen;

    void Update()
    {
        transform.GetChild(0).GetComponent<Text>().text = Coste.ToString();
        transform.GetChild(1).GetComponent<Text>().text = Ataque.ToString();
        transform.GetChild(2).GetComponent<Text>().text = Salud.ToString();
        transform.GetChild(3).GetComponent<Text>().text = Nombre.ToString();
        transform.GetChild(4).GetComponent<Text>().text = Descripcion.ToString();
        transform.GetChild(5).GetComponent<RawImage>().texture = Imagen;
    }
}
