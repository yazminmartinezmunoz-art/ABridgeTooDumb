/*using UnityEngine;

/// Detecta posibles uniones al soltar.
public class SistemaUnion : MonoBehaviour
{
    public float distanciaUnion = 0.6f;

    public void IntentarUnir(
        ObjetoConstruible objeto)
    {
        if (objeto == null)
            return;

        PuntoUnion[] misPuntos =
            objeto.GetComponentsInChildren<PuntoUnion>();

        foreach (PuntoUnion miPunto in misPuntos)
        {
            PuntoUnion[] todosLosPuntos =
                FindObjectsOfType<PuntoUnion>();

            foreach (PuntoUnion otroPunto in todosLosPuntos)
            {
                if (otroPunto.transform.root ==
                    objeto.transform)
                    continue;

                float distancia =
                    Vector3.Distance(
                        miPunto.transform.position,
                        otroPunto.transform.position);

                if (distancia <= distanciaUnion)
                {
                    UnirObjetos(
                        objeto,
                        otroPunto.GetComponentInParent<ObjetoConstruible>());

                    return;
                }
            }
        }
    }

    private void UnirObjetos(
    ObjetoConstruible objetoA,
    ObjetoConstruible objetoB)
    {
        GrupoConstruccion grupoA =
            objetoA.grupoActual;

        GrupoConstruccion grupoB =
            objetoB.grupoActual;

        if (grupoA == null &&
            grupoB == null)
        {
            CrearNuevoGrupo(
                objetoA,
                objetoB);

            return;
        }

        if (grupoA != null &&
            grupoB == null)
        {
            grupoA.AgregarObjeto(
                objetoB);

            return;
        }

        if (grupoA == null &&
            grupoB != null)
        {
            grupoB.AgregarObjeto(
                objetoA);

            return;
        }
    }

    private void CrearNuevoGrupo(
    ObjetoConstruible objetoA,
    ObjetoConstruible objetoB)
    {
        GameObject nuevoGrupo =
            new GameObject("GrupoConstruccion");

        GrupoConstruccion grupo =
            nuevoGrupo.AddComponent<GrupoConstruccion>();

        grupo.AgregarObjeto(objetoA);

        grupo.AgregarObjeto(objetoB);
    }
}*/