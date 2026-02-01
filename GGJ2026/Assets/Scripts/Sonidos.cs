using UnityEngine;

public class Sonidos : MonoBehaviour
{
	//Los sonidos.
	public AudioClip AudioBoton; //Clic de boton.
	public AudioClip AudioEnemigoMuere; //Al matar a un enemigo.
	public AudioClip AudioMascara; //Al equiparse una mascara.
	public AudioClip AudioPausa;//Al pausar el vidoejuego.
	public AudioClip AudioReanudar;
	public AudioClip AudioSalir;
	public AudioClip AudioProtaMuere;
	public AudioClip AudioRomperPared;
	public AudioClip AudioSalto;
	public AudioClip AudioAgrandar;
	public AudioClip AudioEncoger;
	public AudioClip AudioVolar;

	bool isPaused = false;

	private void Awake()
	{
		Managers.Ins.Events.OnEnemyKilledEvent += SonidoEnemigo;
		Managers.Ins.Events.OnMaskChangedEvent += SonidoMascara;
		Managers.Ins.Events.OnGameStateChangedEvent += SonidoPausa;
		Managers.Ins.Events.OnGameStateChangedEvent += SonidoReanudar;
		Managers.Ins.Events.OnTileDestroyedEvent += SonidoPared;
	}

	//Funciones que llaman a la funcion de reproducir sonido, cada una le pasa el sonido pertinente.
	public void SonidoBoton()
	{
		ReproducirSonido(AudioBoton);
	}
	public void SonidoEnemigo()
	{
		ReproducirSonido(AudioEnemigoMuere);
	}
	public void SonidoMascara(Enums.MaskType oldMask, Enums.MaskType newMask)
	{
		ReproducirSonido(AudioMascara);
	}
	public void SonidoPausa(Enums.GameState newState)
	{
		if (newState != Enums.GameState.Paused)
			return;
		isPaused = true;
		ReproducirSonido(AudioPausa);
	}
	public void SonidoReanudar(Enums.GameState newState)
	{
		if (newState != Enums.GameState.Playing && !isPaused)
			return;
		isPaused = false;
		ReproducirSonido(AudioReanudar);
	}
	public void SonidoSalir(Enums.GameState newState)
	{
		if (newState != Enums.GameState.MainMenu)
			return;
		ReproducirSonido(AudioSalir);
	}
	public void SonidoProtaM()
	{
		ReproducirSonido(AudioProtaMuere);
	}
	public void SonidoPared(Vector3 position, Enums.TileType tileType)
	{
		ReproducirSonido(AudioRomperPared);
	}
	public void SonidoSalto()
	{
		ReproducirSonido(AudioSalto);
	}
	public void SonidoAgrandar()
	{
		ReproducirSonido(AudioAgrandar);
	}
	public void SonidoEncoger()
	{
		ReproducirSonido(AudioEncoger);
	}
	public void SonidoVuelo()
	{
		ReproducirSonido(AudioVolar);
	}

	//Reproduce el sonido que le pasen al llamarla.
	private void ReproducirSonido(AudioClip Sonido)
	{
		GetComponent<AudioSource>().PlayOneShot(Sonido);
	}
}