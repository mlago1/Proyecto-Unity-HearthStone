using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EsbirroArrastrable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform ManoJugador { get; set; }
    public Transform TableroJugadorTemporal { get; set; }
    public GameObject tempCard { get; set; }

    int TamañoTablero;
    int TamañoMaximo;

    public void Update()
    {
        TamañoTablero = GameObject.Find("TableroJugador")
            .transform.childCount;
        TamañoMaximo = GameController.MaxCartasTablero;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!GetComponent<EstadisticasEsbirro>().CartaJugada &&
            transform.parent.name != "TableroEnemigo")
        {
            CrearCartaTemporal();
            ManoJugador = transform.parent;
            TableroJugadorTemporal = ManoJugador;
            transform.SetParent(transform.root);
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void CrearCartaTemporal()
    {
        tempCard = new GameObject();
        tempCard.transform.SetParent(transform.root);
        tempCard.transform.position = transform.position;
        tempCard.transform.SetParent(transform.parent);
        LayoutElement le = tempCard.AddComponent<LayoutElement>();
        le.preferredWidth = GetComponent<LayoutElement>().preferredWidth;
        le.preferredHeight = GetComponent<LayoutElement>().preferredHeight;
        le.flexibleHeight = 0;
        le.flexibleWidth = 0;

        tempCard.transform.SetSiblingIndex(transform.GetSiblingIndex());
    }

    public void MoverCartaEnMano()
    {
        int nuevaPosicion = TableroJugadorTemporal.childCount;

        for (int i = 0; i < TableroJugadorTemporal.childCount; i++)
        {
            if (transform.position.x < TableroJugadorTemporal.GetChild(i).position.x)
            {
                nuevaPosicion = i;
                if (tempCard.transform.GetSiblingIndex() < nuevaPosicion)
                    nuevaPosicion--;
                break;
            }
        }
        tempCard.transform.SetSiblingIndex(nuevaPosicion);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!GetComponent<EstadisticasEsbirro>().CartaJugada &&
            transform.parent.name != "TableroEnemigo")
        {
            transform.position = eventData.position;

            if (tempCard.transform.parent != TableroJugadorTemporal)
                tempCard.transform.SetParent(TableroJugadorTemporal);

            MoverCartaEnMano();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (transform.parent.name != "TableroEnemigo")
        {
            if (TamañoTablero <= TamañoMaximo)
            {
                transform.SetParent(ManoJugador);

                if (transform != null && tempCard != null)
                    transform.SetSiblingIndex(tempCard.transform.GetSiblingIndex());
            }
            else
            {
                transform.SetParent(GameObject.Find("ManoJugador").transform);
                GetComponent<EstadisticasEsbirro>().CartaJugada = false;
            }

            Destroy(tempCard);
            GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
    }
}
