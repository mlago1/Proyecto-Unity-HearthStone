using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ControladorInstrucciones : MonoBehaviour
{
    int actual;
    int maximo;

    Text titulo;
    Text contenido;

    string[] texturas =
        {"Piedra", "Caballero", "veladur","Mago","Ninja",
            "sigilo","Esqueleto","Tanque","escudo_divino"};

    string[] titulos =
        {"Nociones Básicas",
        "Esbirro",
        "Heroe",
        "Ataque",
        "Cargar",
        "Sigilo",
        "Provocar",
        "Veneno",
        "Escudo Divino"};
    string[] contenidos =
        {"En este apartado veremos algunas nociones básicas sobre el juego",
        "Los esbirros son las cartas que podemos jugar en el tablero. Para poder" +
            " jugarlos, los arrastramos al tablero. Los esbirros tienen 3 propiedades básicas: \n" +
            " \n Coste: Es el coste de maná necesario para poder jugar ese esbirro." +
            " \n Ataque: Es el daño que hará el esbirro al atacar." +
            " \n Salud: Cuando un esbirro reciba un ataque, su salud disminuirá. Si llega a 0 morirá",
        "Los heroes son los núcleos de la partida. La partida finalizará cuando uno de los 2 héroes muera." +
            "Al principio de cada turno, cada héroe robará 1 carta y obtendrá 1 cristal de maná.",
        "Los esbirros pueden atacar a otros esbirros o al héroe enemigo, pero estos tienen ciertas condiciones: \n" +
            "\n Un esbirro que acaba de ser jugado no podrá atacar inmediatamente, deberá esperar un turno." +
            "\n Un esbirro solo puede atacar una vez por turno.",
        "Cuando un esbirro tiene Cargar, este podrá atacar inmediatamente después de haber sido jugado " +
            ", sin tener que esperar un turno.",
        "Cuando un esbirro tiene Sigilo, no podrá ser objetivo de ataque por parte del rival. " +
            "Esta propiedad se pierde al atacar.",
        "Cuando un esbirro tiene Provocar, tendrá prioridad para ser atacado por parte del enemigo.",
        "Cuando un esbirro tiene Veneno, destruirá a cualquier esbirro al que cause daño.",
        "Cuando un esbirro tiene Escudo Divino, bloqueará el próximo ataque que reciba. " +
            "Esta propiedad se pierde al recibir daño"};

    void Start()
    {
        titulo = GameObject.Find("Titulo").GetComponent<Text>();
        contenido = GameObject.Find("Contenido").GetComponent<Text>();
        actual = 0;
        maximo = titulos.Length;
    }

    // Update is called once per frame
    void Update()
    {
        titulo.text = titulos[actual];
        contenido.text = contenidos[actual];

        GameObject.Find("Anterior").GetComponent<Button>()
            .interactable = actual == 0 ? false : true ;
        GameObject.Find("Siguiente").GetComponent<Button>()
            .interactable = actual == maximo - 1 ? false : true;

        GameObject.Find("Ejemplo").GetComponent<RawImage>()
            .texture = Resources.Load<Texture>(texturas[actual]);
    }

    public void Siguiente()
    {
        if (actual < maximo-1)
            actual++;
    }

    public void Anterior()
    {
        if (actual > 0)
            actual--;
    }
}
