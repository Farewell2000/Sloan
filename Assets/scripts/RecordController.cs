using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//modified from https://github.com/ctrble/UnityScreenRecorder.git
// remeber to go to the Inspector Panel, attach the camera to this script


public class recordController : MonoBehaviour {

	// Capture frames as a screenshot sequence stored as PNG files in a folder
	// Can them combine them into a video using software like ffmpeg

	public bool recordScreen = true;
	public string recordButton = "9"; // Or set whatever key you want in the Inspector
	public int frameRate = 24; // Remember to also set this in the frames to video script outside of Unity
	public int magnificaton = 0; // Default is 0, Unity will multiply screenshot resolution with magnification x magnification (like 4x4), be wary of performance
	private int elapsedTime;
	private int countOffset;
	private int fileName;
	private string folder;
	private string date;

	public Camera captureCamera;  
	public int captureWidth = 1920;
	public int captureHeight = 1080;


	void Start () {

		// Set the playback framerate (real time will not relate to game time after this).
		Time.captureFramerate = frameRate;

		if (recordScreen) {

			// For consistent file naming if recording immediately
			countOffset = 2;

			// Prep the folder
			CalculateFolderName ();
		}
	}

	void Update () {

		elapsedTime = Time.frameCount;

		if (Input.GetKeyDown (recordButton)) {

			// Update frame count
			countOffset = elapsedTime;

			// Prep the folder
			CalculateFolderName ();

			// Toggle screen recording on or off
			recordScreen = !recordScreen;
		}
			
		if (Application.isEditor && recordScreen) {

			// Go screenshots, go!
			RecordScreenShots ();
		}
	}

	void RecordScreenShots()
	{
		System.IO.Directory.CreateDirectory(folder);
		fileName = Time.frameCount - countOffset + 2;
		string name = string.Format("{0}/{1:D04} shot.png", folder, fileName);

		RenderTexture rt = new RenderTexture(captureWidth, captureHeight, 24);
		captureCamera.targetTexture = rt;

		Texture2D screenShot = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);
		captureCamera.Render();
		RenderTexture.active = rt;
		screenShot.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
		screenShot.Apply();

		captureCamera.targetTexture = null;
		RenderTexture.active = null;
		Destroy(rt);

		byte[] bytes = screenShot.EncodeToPNG();
		System.IO.File.WriteAllBytes(name, bytes);
	}


	void CalculateFolderName() {

		// Name the folders by date and time
		date = System.DateTime.Now.ToString("MMddHHmmss");
		folder = "Screenshots/" + date;
	}
}