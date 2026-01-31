using System;
using UnityEngine;

public class Managers : MonoBehaviour
{
    public Managers Ins { get; private set; }

    void Awake()
    {
        Ins = Util.MakeSingleton(this);
	}

	private void Start()
	{
		Console.WriteLine("Managers starting.");
	}
}
