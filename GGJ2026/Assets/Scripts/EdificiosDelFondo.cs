using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EdificiosDelFondo : MonoBehaviour
{
	public Transform AceraSprite;
	//Array de empties. Cada empty tiene dos sprites, uno para la parte trasera del edificio y otro para la parte delantera.
	public GameObject[] Edificios;

	//Las tres lineas de edificios se mueven hacia la izquierda desde el punto de vista del protagonista, estos numeros las desplazan hacia la derecha para que se queden atras mas lentamente.
	//Ejemplo: Si se van a la izquierda a una velocidad de 5 y este numero las mueve a la derecha a una velocidad de 3, entonces se sigue moviendo hacia la izquierda pero a una velocidad de 2.
	public float[] DesplazamientoParallax;

	//Un array con las partes traseras de los edificios y otro array con las partes delanteras de los edificios.
	public Sprite[] Edificios_ParteTrasera, Edificios_ParteDelantera;

	//Referencia al protagonista para saber si esta muy lejos de un edificio y por tanto teletransportar ese edificio mas adelante.
	public GameObject Protagonista;

	//Al empezar llama a la funcion de aleatorizar los aspectos de los edificios e inicia el IEnumerator que cada poco tiempo comprueba si hay edificios que teletransportar hacia delante porque se han quedado atras.
	void Start()
	{
		for (int i = 0; i < Edificios.Length; i++)
		{
			for (int j = 0; j < Edificios[i].GetComponent<Transform>().childCount; j++)
			{
				CambiarCasa(Edificios[i].GetComponent<Transform>().GetChild(j).GetComponent<Transform>());
			}
		}
		StartCoroutine(ComprobarDistanciaDeEdificios());
	}

	//Efecto parallax de las lineas de edificios.
	private void Update()
	{
		if (Managers.Ins.GameScript.IsPaused)
			return;
		//La linea del fondo.
		for (int i = 0; i < Edificios[0].GetComponent<Transform>().childCount; i++)
		{
			Edificios[0].GetComponent<Transform>().GetChild(i).transform.localPosition = new Vector3(Edificios[0].GetComponent<Transform>().GetChild(i).transform.localPosition.x + DesplazamientoParallax[0] * Time.deltaTime, Edificios[0].GetComponent<Transform>().GetChild(i).transform.localPosition.y, 0);
		}
		//La linea de en medio.
		for (int i = 0; i < Edificios[1].GetComponent<Transform>().childCount; i++)
		{
			Edificios[1].GetComponent<Transform>().GetChild(i).transform.localPosition = new Vector3(Edificios[1].GetComponent<Transform>().GetChild(i).transform.localPosition.x + DesplazamientoParallax[1] * Time.deltaTime, Edificios[1].GetComponent<Transform>().GetChild(i).transform.localPosition.y, 0);
		}
		//La linea delantera.
		for (int i = 0; i < Edificios[1].GetComponent<Transform>().childCount; i++)
		{
			Edificios[2].GetComponent<Transform>().GetChild(i).transform.localPosition = new Vector3(Edificios[2].GetComponent<Transform>().GetChild(i).transform.localPosition.x + DesplazamientoParallax[2] * Time.deltaTime, Edificios[2].GetComponent<Transform>().GetChild(i).transform.localPosition.y, 0);
		}
	}

	//Un IEnumerator que comprueba si algun edificio se ha quedado atras o si se ha adelantado para teletransportarlo de vuelta a un rango aceptable.
	//Es IEnumerator para no llamar a esta comprobacion cada fotograma.
	private IEnumerator ComprobarDistanciaDeEdificios()
	{
		while (Managers.Ins.GameScript.IsPaused)
			yield return null;

		//Este bloque de la funcion adelanta las casas si se han quedado atras.
		for (int i = 0; i < Edificios.Length; i++)
		{
			for (int j = 0; j < Edificios[i].GetComponent<Transform>().childCount; j++)
			{
				if (Protagonista.transform.position.x - Edificios[i].GetComponent<Transform>().GetChild(j).transform.position.x > 29 * transform.localScale.x)
				{
					TeletransportarCasa(Edificios[i].GetComponent<Transform>().GetChild(j).GetComponent<Transform>(), 1);
					CambiarCasa(Edificios[i].GetComponent<Transform>().GetChild(j).GetComponent<Transform>());
				}
			}
		}
		yield return new WaitForSeconds(0.01f);
		//Este bloque de la funcion atrasa las casas si se han adelantado.
		for (int i = 0; i < Edificios.Length; i++)
		{
			for (int j = 0; j < Edificios[i].GetComponent<Transform>().childCount; j++)
			{
				if (Edificios[i].GetComponent<Transform>().GetChild(j).transform.position.x - Protagonista.transform.position.x > 29 * transform.localScale.x)
				{
					TeletransportarCasa(Edificios[i].GetComponent<Transform>().GetChild(j).GetComponent<Transform>(), -1);
				}
			}
		}
		yield return new WaitForSeconds(0.01f);
		StartCoroutine(ComprobarDistanciaDeEdificios());

		// También comprueba la acera.
		if (Protagonista.transform.position.x - AceraSprite.position.x > 5 * transform.localScale.x)
		{
			AceraSprite.Translate(Vector2.right * 3);
		}
	}

	//Funcion que cambia el aspecto del edificio que se le indica al llamarla.
	private void CambiarCasa(Transform Edificio)
	{
		int NumeroAlAzar = Random.Range(0, Edificios_ParteTrasera.Length);
		Edificio.GetChild(0).GetComponent<SpriteRenderer>().sprite = Edificios_ParteTrasera[NumeroAlAzar];
		Edificio.GetChild(1).GetComponent<SpriteRenderer>().sprite = Edificios_ParteDelantera[NumeroAlAzar];
	}

	//Funcion que desplaza el edificio que se le indica al llamarla hasta el principio de la fila de edificios.
	private void TeletransportarCasa(Transform Edificio, int Positivo)
	{
		Edificio.transform.localPosition = new Vector3(Edificio.transform.localPosition.x + 55.25f * Positivo, Edificio.transform.localPosition.y, 0);
	}

	public void CambiarColoresDeEdificios(Color ColorTraseros, Color ColorDelanteros)
	{
		for (int i = 0; i < Edificios.Length; i++)
		{
			for (int j = 0; j < Edificios[i].GetComponent<Transform>().childCount; j++)
			{
				Edificios[i].GetComponent<Transform>().GetChild(j).GetChild(0).GetComponent<SpriteRenderer>().color = ColorTraseros;
				Edificios[i].GetComponent<Transform>().GetChild(j).GetChild(1).GetComponent<SpriteRenderer>().color = ColorDelanteros;
			}
		}
	}
}