using UnityEngine;
using System.IO;
using System.Collections;
using AudioSynthesis;
using AudioSynthesis.Bank;
using AudioSynthesis.Synthesis;
using AudioSynthesis.Sequencer;
using AudioSynthesis.Midi;
using System;
using System.Collections.Generic;
#if UNITY_WEBGL
using System.Runtime.InteropServices;
#endif

namespace UnityMidi
{
	[RequireComponent(typeof(AudioSource))]
	public class MidiPlayer : MonoBehaviour
	{
#if UNITY_WEBGL && !UNITY_EDITOR
		[DllImport("__Internal")]
		private static extern void InitMidi(int sampleRate, int BufferSize);

		[DllImport("__Internal")]
		private static extern void FillFrontBuffer(int frameCount, int sampleRate, float[] data, bool play);

		[DllImport("__Internal")]
		private static extern void FillRearBuffer(int frameCount, int sampleRate, float[] data, bool play);
#endif

#if UNITY_WEBGL
		[HideInInspector]
		int bufferSamples = 16;
#endif
		[SerializeField]
		public StreamingAssetResouce bankExternalSourceFile;
		public string bankResourceSourceFile = "soundfonts/Scc1t2.sf2";
		[SerializeField] StreamingAssetResouce midiSource;
		[SerializeField] bool loadOnAwake = true;
		[SerializeField] bool loop = true;
		[SerializeField] bool playOnStart = true;
		[SerializeField] int channel = 2;
		[SerializeField] int sampleRate = 44100;
		[SerializeField] int bufferSize = 1024;
		PatchBank bank;
		MidiFile midi;
		Synthesizer synthesizer;
		AudioSource audioSource;
		MidiFileSequencer sequencer;
		int bufferHead = 0;
		float[] currentBuffer;

		public List<float> MidiBuffer;
		public AudioSource AudioSource { get { return audioSource; } }

		public MidiFileSequencer Sequencer { get { return sequencer; } }

		public PatchBank Bank { get { return bank; } }

		public MidiFile MidiFile { get { return midi; } }

		public void Awake()
		{
			synthesizer = new Synthesizer(sampleRate, channel, bufferSize, 1);
			sequencer = new MidiFileSequencer(synthesizer);
			audioSource = GetComponent<AudioSource>();
#if UNITY_WEBGL
			InitMidi(sampleRate, bufferSamples * bufferSize);
#endif
			if (loadOnAwake)
			{
				LoadBank();
			}
		}

		public void Start()
		{
			if (playOnStart)
			{
				LoadMidi(new MidiFile(midiSource));
				Play();
			}
		}

		public void LoadBank()
		{
			if (bankExternalSourceFile.FileExist())
				LoadBank(new PatchBank(bankExternalSourceFile));
			else
				LoadBank(new PatchBank(bankResourceSourceFile));
		}
		public void LoadBank(PatchBank bank)
		{
			this.bank = bank;
			synthesizer.UnloadBank();
			synthesizer.LoadBank(bank);
		}
		public void StreamMidi(string Midisong)
		{
			TextAsset midisong = Resources.Load(Midisong) as TextAsset;

			if (midisong == null)
				throw new Exception("The bank file provided does not exist.");
			LoadMidi(new MidiFile(midisong.bytes));
			Play();
		}
		public void StreamMidi(byte[] Midisong)
		{
			LoadMidi(new MidiFile(Midisong));
			Play();
		}
		public void LoadMidi(MidiFile midi)
		{
			this.midi = midi;
			sequencer.Stop();
			sequencer.UnloadMidi();
			sequencer.LoadMidi(midi);
		}

		public void Play()
		{
#if UNITY_WEBGL
			sequencer.Play(loop);
			GetBufferData();
#else
			audioSource.clip = AudioClip.Create("Midi", bufferSize, channel, sampleRate, true, OnAudioRead);
			audioSource.spatialBlend = 0f;
			sequencer.Play(loop);
#endif
			audioSource.Play();
		}

#if UNITY_WEBGL
		public void Stop()
		{
			sequencer.Stop();
			sequencer.ResetMidi();
			sequencer.UnloadMidi();
		}
#else
		public void Stop()
		{
			audioSource.Stop();
			sequencer.Stop();
			sequencer.ResetMidi();
			sequencer.UnloadMidi();
		}
#endif
		void FillBuffer()
		{
			sequencer.FillMidiEventQueue();
			synthesizer.GetNext();
			currentBuffer = synthesizer.WorkingBuffer;
			bufferHead = 0;
		}
#if UNITY_WEBGL && UNITY_EDITOR
		void InitMidi(int sampleRate, int BufferSize)
		{
		}
		void FillFrontBuffer(int frameCount, int sampleRate, float[] data, bool play)
		{
		}
		void FillRearBuffer(int frameCount, int sampleRate, float[] data, bool play)
		{
		}
#endif

#if UNITY_WEBGL
		void GetBufferData()
		{
			MidiBuffer = new List<float>(bufferSamples * synthesizer.WorkingBufferSize);
			for (int i = 0; i < bufferSamples; i++)
			{
				FillBuffer();
				MidiBuffer.AddRange(currentBuffer);
			}
			FillFrontBuffer((MidiBuffer.Count) /2, sampleRate, MidiBuffer.ToArray(), false);
			MidiBuffer.Clear();
			for (int i = 0; i < bufferSamples; i++)
			{
				FillBuffer();
				MidiBuffer.AddRange(currentBuffer);
			}
			FillRearBuffer((MidiBuffer.Count) / 2, sampleRate, MidiBuffer.ToArray(), true);
		}

		public void FillBuffer(bool front)
		{
			MidiBuffer.Clear();
			if (sequencer.IsPlaying)
			{
				for (int i = 0; i < bufferSamples; i++)
				{
					FillBuffer();
					MidiBuffer.AddRange(currentBuffer);
				}
			}
			if (front)
				FillFrontBuffer((MidiBuffer.Count) / 2, sampleRate, MidiBuffer.ToArray(), false);
			else
				FillRearBuffer((MidiBuffer.Count) / 2, sampleRate, MidiBuffer.ToArray(), false);
		}
		public void FillFrontBuffer()
		{
			FillBuffer(true);
		}
		public void FillRearBuffer()
		{
			FillBuffer(false);
		}
#else
		void OnAudioRead(float[] data)
		{
			int count = 0;

			while (count < data.Length)
			{
				if (currentBuffer == null)
					FillBuffer();
				else if (bufferHead >= currentBuffer.Length)
					FillBuffer();
				var length = Mathf.Min(currentBuffer.Length - bufferHead, data.Length - count);
				Array.Copy(currentBuffer, bufferHead, data, count, length);
				bufferHead += length;
				count += length;
			}
		}
#endif
	}
}