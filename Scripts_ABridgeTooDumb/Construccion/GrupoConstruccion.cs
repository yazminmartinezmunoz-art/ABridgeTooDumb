using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

/// Representa una construcción compuesta
/// por varios objetos unidos. 
public class GrupoConstruccion : MonoBehaviour
{
    [Header("Objetos del grupo")]

    public List<ObjetoConstruible> objetos =
        new List<ObjetoConstruible>();

    [Header("Separación")]

    public float distanciaSeparacion = 0.18f;

    public event Action AlCambiarGrupo;


    public event Action AlCambiarJugadores;

    private float tiempoRevision = 0.5f;

    private float contadorRevision;

    private void Awake()
    {
        objetos =
            new List<ObjetoConstruible>();
    }
    private void Update()
    {

        // Si alguna pieza del grupo está siendo agarrada
        // no revisamos conexiones
        foreach (ObjetoConstruible objeto
                 in objetos)
        {
            if (objeto == null)
                continue;

            if (objeto.estaSiendoAgarrado)
            {
                return;
            }
        }
        contadorRevision += Time.deltaTime;

        if (contadorRevision >=
            tiempoRevision)
        {
            contadorRevision = 0f;

            RevisarConexiones();
        }
    }

    /// Agrega un objeto al grupo.
    public void AgregarObjeto(
        ObjetoConstruible objeto)
    {
        if (objeto == null)
            return;

        if (!objetos.Contains(objeto))
        {
            objetos.Add(objeto);

            objeto.grupoActual =
                this;
        }

        RecalcularFuerza();

        ActualizarReferencias();
    }
    private void ActualizarReferencias()
    {
        foreach (ObjetoConstruible objeto
                 in objetos)
        {
            if (objeto == null)
                continue;

            objeto.grupoActual =
                this;
        }
    }

    /// Elimina un objeto.
    public void RemoverObjeto(
        ObjetoConstruible objeto)
    {
        if (objetos.Contains(objeto))
        {

            SonidosConstruccion audioObjeto = objeto.GetComponent<SonidosConstruccion>();

            if (audioObjeto != null)
            {
                audioObjeto.Reproducir(
                    SonidosConstruccion.TipoSonido.Separar);
            }

            objetos.Remove(objeto);

            // Limpiamos jugadores agarrando
            objeto.jugadoresAgarrando.Clear();

            // Quitamos referencia al grupo
            objeto.grupoActual = null;
        }

        RecalcularFuerza();

        // Si queda un único objeto,
        // deja de ser grupo
        if (objetos.Count <= 1)
        {
            foreach (ObjetoConstruible restante
                     in objetos)
            {
                restante.grupoActual = null;
            }

            Destroy(gameObject);
        }
    }
    public int ObtenerFuerzaNecesaria()
    {
        int total = 0;

        foreach (ObjetoConstruible objeto in objetos)
        {
            total += objeto.jugadoresNecesarios;
        }

        int maximo =
            ObtenerMaximoPermitido();

        return Mathf.Clamp(
            total,
            1,
            maximo);
    }

    private void RevisarConexiones()
    {
        if (objetos.Count <= 1)
            return;

        List<ObjetoConstruible> desconectados =
            new List<ObjetoConstruible>();

        foreach (ObjetoConstruible objeto in objetos)
        {
            if (!SigueConectado(objeto))
            {
                desconectados.Add(objeto);
            }
        }

        foreach (ObjetoConstruible objeto in desconectados)
        {
            RemoverObjeto(objeto);
        }
    }
    private bool SigueConectado(
    ObjetoConstruible objeto)
    {
        if (objeto == null)
            return false;

        foreach (PuntoUnion miPunto
                 in objeto.puntosUnion)
        {
            // Punto destruido
            if (miPunto == null)
                continue;

            foreach (ObjetoConstruible otro
                     in objetos)
            {
                if (otro == null)
                    continue;

                if (otro == objeto)
                    continue;

                foreach (PuntoUnion otroPunto
                         in otro.puntosUnion)
                {
                    if (otroPunto == null)
                        continue;

                    float distancia =
                        Vector3.Distance(
                            miPunto.transform.position,
                            otroPunto.transform.position);

                    if (distancia <
                        distanciaSeparacion)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    /// Fusiona otro grupo dentro de este.
    public void FusionarGrupo(
        GrupoConstruccion otroGrupo)
    {
        if (otroGrupo == null)
            return;

        if (otroGrupo == this)
            return;

        foreach (ObjetoConstruible objeto
                 in otroGrupo.objetos)
        {
            // Evita duplicados
            if (objetos.Contains(objeto))
                continue;

            // Limpiamos jugadores anteriores
            objeto.jugadoresAgarrando.Clear();

            objetos.Add(objeto);

            // El objeto ahora pertenece
            // a este grupo
            objeto.grupoActual = this;
        }

        RecalcularFuerza();

        Destroy(otroGrupo.gameObject);
    }



    /// Revisa si cualquier objeto del grupo está tocando una Base.
    /// Si una sola pieza está conectada a una Base, todo el grupo se considera anclado
    public bool EstaAnclado()
    {
        foreach (ObjetoConstruible objeto in objetos)
        {
            foreach (PuntoUnion punto in objeto.puntosUnion)
            {
                // Busca colliders cerca del punto
                Collider[] cercanos =
                    Physics.OverlapSphere(
                        punto.transform.position,
                        0.15f);

                foreach (Collider col in cercanos)
                {
                    // Si encuentra una Base
                    if (col.CompareTag("Base"))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }



    public void RecalcularFuerza()
    {
        AlCambiarGrupo?.Invoke();
    }


    public int ObtenerJugadoresAgarrando()
    {
        HashSet<int> ids =
            new HashSet<int>();

        // Eliminar referencias destruidas
        objetos.RemoveAll(
            objeto => objeto == null);

        foreach (ObjetoConstruible objeto
                 in objetos)
        {
            if (objeto == null)
                continue;

            foreach (int id
                     in objeto.jugadoresAgarrando)
            {
                ids.Add(id);
            }
        }

        return ids.Count;
    }

    public bool TieneFuerzaSuficiente()
    {
        return ObtenerJugadoresAgarrando()
               >= ObtenerFuerzaNecesaria();
    }
    private int ObtenerMaximoPermitido()
    {
        if (!PhotonNetwork.InRoom)
            return 1;

        return PhotonNetwork
            .CurrentRoom
            .PlayerCount;
    }
    public void ActualizarJugadores()
    {
        AlCambiarJugadores?.Invoke();

        AlCambiarGrupo?.Invoke();
    }
}

