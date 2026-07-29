using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// Controla la UI de información del objeto.
public class UIObjeto : MonoBehaviour
{
    [Header("Panel")]

    public GameObject panel;

    [Header("Información")]

    public Image imagenObjeto;

    //public TMP_Text nombreObjeto;

    //public TMP_Text descripcionObjeto;

    public TMP_Text textoJugadores;

    [Header("Corazones")]

    public Transform contenedorCorazones;

    public GameObject prefabCorazon;

    private ObjetoConstruible objetoActual;

    public void Mostrar(ObjetoConstruible objeto)
    {
        if (objetoActual != null)
        {
            objetoActual.AlCambiarJugadores -=
                ActualizarUI;

            objetoActual.AlCambiarVida -=
                ActualizarUI;

            if (objetoActual.grupoActual != null)
            {
                objetoActual.grupoActual.AlCambiarGrupo -=
                    ActualizarUI;
            }
        }

        objetoActual = objeto;

        panel.SetActive(true);

        objetoActual.AlCambiarJugadores +=
            ActualizarUI;

        objetoActual.AlCambiarVida +=
            ActualizarUI;

        if (objetoActual.grupoActual != null)
        {
            objetoActual.grupoActual.AlCambiarGrupo +=
                ActualizarUI;

            objetoActual.grupoActual.AlCambiarJugadores +=
                ActualizarUI;
        }

        ActualizarUI();
    }

    private void ActualizarUI()
    {
        if (objetoActual == null ||
            objetoActual.Equals(null))
        {
            Ocultar();
            return;
        }

        imagenObjeto.sprite =
            objetoActual.imagenObjeto;

        //nombreObjeto.text =
        //objetoActual.nombreObjeto;

        //descripcionObjeto.text =
        //objetoActual.descripcion;

        textoJugadores.text = " : " + ObtenerTextoFuerza();

        MostrarCorazones(objetoActual);
    }

    public void Ocultar()
    {
        if (objetoActual != null)
        {
            objetoActual.AlCambiarJugadores -=
                ActualizarUI;

            objetoActual.AlCambiarVida -=
                ActualizarUI;

            if (objetoActual.grupoActual != null)
            {
                objetoActual.grupoActual.AlCambiarGrupo -=
                    ActualizarUI;

                objetoActual.grupoActual.AlCambiarJugadores -=
                    ActualizarUI;
            }
        }

        objetoActual = null;

        panel.SetActive(false);
    }

    private void MostrarCorazones(ObjetoConstruible objeto)
    {
        foreach (Transform hijo in contenedorCorazones)
        {
            Destroy(hijo.gameObject);
        }

        for (int i = 0; i < objeto.vidasActuales; i++)
        {
            Instantiate(
                prefabCorazon,
                contenedorCorazones);
        }
    }

    private string ObtenerTextoFuerza()
    {
        int necesarios;
        int actuales;

        if (objetoActual.grupoActual == null)
        {
            necesarios =
                objetoActual
                .ObtenerJugadoresNecesarios();

            actuales =
                objetoActual
                .jugadoresAgarrando
                .Count;
        }
        else
        {
            necesarios =
                objetoActual
                .grupoActual
                .ObtenerFuerzaNecesaria();

            actuales =
                objetoActual
                .grupoActual
                .ObtenerJugadoresAgarrando();
        }

        return actuales +
               "/" +
               necesarios;
    }
}