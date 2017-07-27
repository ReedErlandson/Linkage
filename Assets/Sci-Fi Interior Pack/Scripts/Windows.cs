
/* 

This script is designed to control the animation. Open and close the windows.
This script is an example of how you can control the animation.
Each window has its own animator and animation. You can control the animation of each window.

This script should be attached to the Characters. In this case the FPSController.

*/



using UnityEngine;
using System.Collections;

public class Windows : MonoBehaviour {



	// Variables Animator.
	// With these variables, you can control the animation of each window.

	public Animator Window_Room_1;

	public Animator Window_Room_3;

	public Animator Window_Room_4;

	public Animator Window_Room_5;

	public Animator Window_Room_6;


	// Use this for initialization
	void Start () 
	{
	
	}


	// Update is called once per frame
	// Pressing the key "C" activates the animation (Close the windows).
	// Pressing the key "O" activates the animation (Open the windows).

	void Update () 
	{
	
		if (Input.GetKey (KeyCode.C)) 
		{

			Window_Room_1.Play ("Window_Room_1_Close", 0);

			Window_Room_3.Play ("Window_Room_3_Close", 0);

			Window_Room_4.Play ("Window_Room_4_Close", 0);

			Window_Room_5.Play ("Window_Room_5_Close", 0);

			Window_Room_6.Play ("Window_Room_6_Close", 0);

		}


		if (Input.GetKey (KeyCode.O)) 
		{

			Window_Room_1.Play ("Window_Room_1_Open", 0);

			Window_Room_3.Play ("Window_Room_3_Open", 0);

			Window_Room_4.Play ("Window_Room_4_Open", 0);

			Window_Room_5.Play ("Window_Room_5_Open", 0);

			Window_Room_6.Play ("Window_Room_6_Open", 0);

		}


	}


}
