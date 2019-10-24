using UnityEngine;
using UnityEngine.EventSystems;

public class ZonaCaidaCartas : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public void OnDrop(PointerEventData eventData) 
    {
        if (transform.name == "TableroJugador")
        {
            EsbirroArrastrable a = eventData.pointerDrag.
                GetComponent<EsbirroArrastrable>();

            a.ManoJugador = this.transform;
            a.GetComponent<EstadisticasEsbirro>().CartaJugada = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData) 
    {
        if (transform.name == "TableroJugador")
        {
            if (eventData.pointerDrag == null)
                return;

            EsbirroArrastrable a = eventData.pointerDrag.
                GetComponent<EsbirroArrastrable>();
            a.TableroJugadorTemporal = this.transform;
        }
    }

    public void OnPointerExit(PointerEventData eventData) 
    {
        if (transform.name == "TableroJugador")
        {
            if (eventData.pointerDrag == null)
                return;

            EsbirroArrastrable a = eventData.pointerDrag.
                GetComponent<EsbirroArrastrable>();
            if (a.TableroJugadorTemporal == transform)
                a.TableroJugadorTemporal = a.ManoJugador;
        }
    }
}
