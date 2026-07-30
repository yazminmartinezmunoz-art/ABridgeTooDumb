using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using System;
using System.Collections.Generic;

/// Se mueve únicamente cuando todos los jugadores de la sala están arriba.
public class PlataformaCooperativa: MonoBehaviourPunCallbacks
{
    [Header("Movimiento")]
    public Transform puntoDestino;
    public float velocidad = 2f;
    public float tiempoEspera = 2f;

    [Header("Visual")]
    public Renderer meshRenderer;
    public Material materialRojo;
    public Material materialVerde;

    [Header("UI")]
    public TMP_Text textoJugadores;
    private Vector3 posicionInicial;
    private float temporizador;
    private bool moviendose;
    private bool regresar;
    private HashSet<int> jugadoresEncima = new HashSet<int>();
    public event Action AlCambiarJugadores;

    private void Start()
    {
        posicionInicial = transform.position;
        ActualizarColor();
        ActualizarTextoUI();
    }
    private void Update()
    {
        ActualizarEstado();
        MoverPlataforma();
    }
    private void OnTriggerEnter(Collider other)
    {
        PhotonView pv = other.GetComponent<PhotonView>();

        if (pv == null)
            return;

        if (!other.CompareTag("Player"))
            return;

        jugadoresEncima.Add(pv.Owner.ActorNumber);

        AlCambiarJugadores?.Invoke();

        ActualizarTextoUI();
        ActualizarColor();
    }
    private void OnTriggerExit(Collider other)
    {
        PhotonView pv = other.GetComponent<PhotonView>();

        if (pv == null)
            return;

        jugadoresEncima.Remove(pv.Owner.ActorNumber);

        AlCambiarJugadores?.Invoke();

        ActualizarTextoUI();
        ActualizarColor();
    }
    private void ActualizarEstado()
    {
        int jugadoresSala = ObtenerJugadoresSala();

        // Todos arriba
        if (jugadoresEncima.Count >= jugadoresSala)
        {
            temporizador += Time.deltaTime;

            if (temporizador >= tiempoEspera)
            {
                moviendose = true;
                regresar = false;
            }
        }
        else
        {
            temporizador = 0;
        }

        // Todos se bajaron
        if (jugadoresEncima.Count == 0)
        {
            moviendose = false;
            regresar = true;
        }
    }
    private void MoverPlataforma()
    {
        if (moviendose)
        {
            transform.position = Vector3.MoveTowards(transform.position,puntoDestino.position,velocidad * Time.deltaTime);
        }
        if (regresar)
        {
            transform.position = Vector3.MoveTowards(transform.position,posicionInicial,velocidad * Time.deltaTime);
        }
    }
    private void ActualizarColor()
    {
        if (jugadoresEncima.Count >= ObtenerJugadoresSala())
        {
            meshRenderer.material = materialVerde;
        }
        else
        {
            meshRenderer.material = materialRojo;
        }
    }
    private void ActualizarTextoUI()
    {
        if (textoJugadores == null)
            return;

        textoJugadores.text = ": "+ ObtenerJugadoresEncima()+"/"+ObtenerJugadoresSala();
    }
    public int ObtenerJugadoresEncima()
    {
        return jugadoresEncima.Count;
    }
    public int ObtenerJugadoresSala()
    {
        if (!PhotonNetwork.InRoom)
            return 1;

        return PhotonNetwork.CurrentRoom.PlayerCount;
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        ActualizarTextoUI();
        ActualizarColor();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ActualizarTextoUI();
        ActualizarColor();
    }
}