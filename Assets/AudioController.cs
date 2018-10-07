using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour {
	
	
	public AudioMixerSnapshot outOfCombat;
	public AudioMixerSnapshot bothMusic;
	public AudioMixerSnapshot inCombat;
	public float bpm = 128;

	private float m_TransitionIn;
	private float m_TransitionOut;
	private float m_QuarterNote;

	public bool CombatMusic = false, change = false;

	// Use this for initialization
	void Start () {
		outOfCombat.TransitionTo(0);
		CombatMusic = false;
	}
	
	
	// Update is called once per frame
	void Update () {
		if (!CombatMusic&& change)
		{
			bothMusic.TransitionTo(bpm);
			inCombat.TransitionTo(bpm);
			CombatMusic = true;
		}else if (CombatMusic && change)
		{
			bothMusic.TransitionTo(bpm);
			outOfCombat.TransitionTo(bpm);
			CombatMusic = false;
		}
		change = false;
	}
}
