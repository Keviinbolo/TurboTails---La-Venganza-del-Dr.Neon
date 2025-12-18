using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;

public class BarraVidaPersonaje : MonoBehaviour
{
   
    public Image rellenoBarraVida;
    public  ControladorJugador playerController;
    private float vidaMaxima;

    void Start()
    {
        playerController = GameObject.Find("pj").GetComponent<ControladorJugador>();
        vidaMaxima = playerController.vidas;        
    }

    // Update is called once per frame
    void Update()
    {
        rellenoBarraVida.fillAmount = playerController.vidas / vidaMaxima;
    }
}
