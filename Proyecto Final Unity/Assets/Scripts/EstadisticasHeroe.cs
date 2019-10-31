using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EstadisticasHeroe : MonoBehaviour
{
    public int Salud { get; set; }
    public int ManaMaximo { get; set; }
    public int ManaDisponible { get; set; }
    public int DañoFatiga { get; set; }
    //TO DO public int Ataque {get;set;}
    void Start()
    {
        Salud = 30;
        ManaMaximo = 0;
        ManaDisponible = 0;
        DañoFatiga = 1;
    }

    // Update is called once per frame
    void Update()
    {
        transform.GetChild(0).GetComponent<Text>()
            .text = "Mana: " + ManaDisponible.ToString();
        transform.GetChild(1).GetComponent<Text>()
            .text = "Salud: " + Salud.ToString();
    }
}
