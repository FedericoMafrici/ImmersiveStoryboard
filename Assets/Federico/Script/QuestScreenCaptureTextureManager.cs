using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Trev3d.Quest.ScreenCapture
{
	[DefaultExecutionOrder(-1000)]
	public class QuestScreenCaptureTextureManager : MonoBehaviour
	{
		
		public ConsoleDebugger _debuggingWindow;
		[SerializeField] public RawImage _screenshot ; // Database con la lista delle azioni per ogni oggetto
		[SerializeField] public RawImage _duplicateScreen ;
		private AndroidJavaObject byteBuffer;
		private unsafe sbyte* imageData;
		private int bufferSize;
		
		public static QuestScreenCaptureTextureManager Instance { get; private set; }

		private AndroidJavaClass UnityPlayer;
		private AndroidJavaObject UnityPlayerActivityWithMediaProjector;
		
		private Texture2D screenTexture;
		private RenderTexture flipTexture;
		public Texture2D ScreenCaptureTexture => screenTexture;

		public bool startScreenCaptureOnStart = true;
		public bool flipTextureOnGPU = false;
		
		public UnityEvent<Texture2D> OnTextureInitialized = new();
		public UnityEvent OnScreenCaptureStarted = new();
		public UnityEvent OnScreenCapturePermissionDeclined = new();
		public UnityEvent OnScreenCaptureStopped = new();
		public UnityEvent OnNewFrameIncoming = new();
		public UnityEvent OnNewFrame = new();

		public static readonly Vector2Int Size = new(1024, 1024);

		private void Awake()
		{
			Instance = this;
			screenTexture = new Texture2D(Size.x, Size.y, TextureFormat.RGBA32, 1, false);
		}

		private void Start()
		{
			
				UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				UnityPlayerActivityWithMediaProjector = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
				if (UnityPlayerActivityWithMediaProjector != null)
				{
					Debug.Log("currentActivity ottenuto con successo.");
				}
				else
				{
					Debug.LogError("currentActivity è nullo.");
				}
				// Passa il nome del gameObject, larghezza e altezza
				flipTexture = new RenderTexture(Size.x, Size.y, 1, RenderTextureFormat.ARGB32, 1);
				flipTexture.Create();

				OnTextureInitialized.Invoke(screenTexture);

				if (startScreenCaptureOnStart)
				{
					Debug.Log("Finestra di PoPup avviata dovrebbe essere visibile");
					StartScreenCapture();
				}
				bufferSize = Size.x * Size.y * 4; // RGBA_8888 format: 4 bytes per pixel
				OnNewFrameIncoming.AddListener(FirstStopScreenCapture);
				StartScreenCapture();
				
			
				
			
			
		}

		private unsafe void InitializeByteBufferRetrieved()
		{
			// Retrieve the ByteBuffer from Java and cache it
			byteBuffer = UnityPlayerActivityWithMediaProjector.Call<AndroidJavaObject>("getLastFrameBytesBuffer");

			// Get the memory address of the direct ByteBuffer
			imageData = AndroidJNI.GetDirectBufferAddress(byteBuffer.GetRawObject());
		}

		public void StartScreenCapture()
		{
			try
			{
				UnityPlayerActivityWithMediaProjector.Call("startScreenCaptureWithPermission", gameObject.name, Size.x,
					Size.y);
			}
			catch( AndroidJavaException ex)
			{
				Debug.LogError("Errore nella chiamata a startScreenCaptureWithPermission: " + ex.Message);
			}

			_duplicateScreen.texture = screenTexture;
		}

		public void StopScreenCapture()
		{
			UnityPlayerActivityWithMediaProjector.Call("stopScreenCapture");
		}

		// Messages sent from android activity

		private void ScreenCaptureStarted()
		{
			OnScreenCaptureStarted.Invoke();
			InitializeByteBufferRetrieved();
		}

		private void FirstStopScreenCapture()
		{
			OnNewFrameIncoming.RemoveListener(StopScreenCapture);
			StopScreenCapture();
		}
		private void ScreenCapturePermissionDeclined()
		{
			OnScreenCapturePermissionDeclined.Invoke();
		}

		private void NewFrameIncoming()
		{
			OnNewFrameIncoming.Invoke();
		}

		private unsafe void NewFrameAvailable()
		{
			if (imageData == default) return;
			screenTexture.LoadRawTextureData((IntPtr)imageData, bufferSize);
			screenTexture.Apply();

			if (flipTextureOnGPU)
			{
				Graphics.Blit(screenTexture, flipTexture, new Vector2(1, -1), Vector2.zero);
				Graphics.CopyTexture(flipTexture, screenTexture);
			}

			OnNewFrame.Invoke();
		}

		public void TakeScreenShot(RawImage screenshot,Texture2D screenshottexture2D)
		{
			
			StartScreenCapture();
			Texture2D screenshotCopy = new Texture2D(screenTexture.width, screenTexture.height, TextureFormat.RGBA32, false);
			screenshotCopy.SetPixels(screenTexture.GetPixels());
			screenshotCopy.Apply();
			if (flipTextureOnGPU)
			{
				Graphics.Blit(screenshotCopy,flipTexture,new Vector2(1,-1),Vector2.zero);
				Graphics.CopyTexture(flipTexture, screenshotCopy);
			}
			// Assegna la copia alla RawImage
			screenshot.texture = screenshotCopy;
			screenshottexture2D = screenshotCopy;
			Debug.Log("Screenshot taken and assigned to RawImage");
			StopScreenCapture();
		}
		private void ScreenCaptureStopped()
		{
			OnScreenCaptureStopped.Invoke();
		}
	}
	
}