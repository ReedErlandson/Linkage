
/* 

This script is designed to control the animation. Open and close the doors.
Next to each door there is one trigger.
Each door has its own animator and animation. You can control the animation of each door.
When an FPSController enters the trigger, then there is playing the animation (Open the door).
When the FPSController leaves the trigger, then there is playing the animation (Close the door).

This script should be attached to the Characters. In this case the FPSController.

*/



using UnityEngine;
using System.Collections;

public class Triggers_Doors : MonoBehaviour {


	// Variables Animator.
	// With these variables, you can control the animation of each door.

	public Animator Door_Room_1_2;

	public Animator Door_Room_2_3;

	public Animator Door_Room_3_4;

	public Animator Door_Room_4_5;

	public Animator Door_Room_5_6;

	public Animator Door_Room_7_8;


	// Variables AudioSource.
	// With these variables, you can play the sound of each door.

	public AudioSource Sound_Door_Room_1_2;

	public AudioSource Sound_Door_Room_2_3;

	public AudioSource Sound_Door_Room_3_4;

	public AudioSource Sound_Door_Room_4_5;

	public AudioSource Sound_Door_Room_5_6;

	public AudioSource Sound_Door_Room_7_8;


	// Use this for initialization

	void Start () 

	{

	}

	// Update is called once per frame

	void Update () 

	{

	}


	// OnTriggerEnter
	// When an FPSController enters the trigger, then there is playing the animation (Open the door).
	// There is a condition to check each trigger.

	void OnTriggerEnter (Collider Col) 
	{


		if (Col.gameObject.name == "Trigger_Door_Room_1_2") 
		
		{

			Door_Room_1_2.Play ("Door_Room_1_2_Open", 0);

			Sound_Door_Room_1_2.Play ();

		}


		if (Col.gameObject.name == "Trigger_Door_Room_2_3") 
		
		{

			Door_Room_2_3.Play ("Door_Room_2_3_Open", 0);

			Sound_Door_Room_2_3.Play ();

		}


		if (Col.gameObject.name == "Trigger_Door_Room_3_4") 
		
		{

			Door_Room_3_4.Play ("Door_Room_3_4_Open", 0);

			Sound_Door_Room_3_4.Play ();

		}


		if (Col.gameObject.name == "Trigger_Door_Room_4_5") 
		
		{

			Door_Room_4_5.Play ("Door_Room_4_5_Open", 0);

			Sound_Door_Room_4_5.Play ();

		}


		if (Col.gameObject.name == "Trigger_Door_Room_5_6") 
		
		{

			Door_Room_5_6.Play ("Door_Room_5_6_Open", 0);

			Sound_Door_Room_5_6.Play ();

		}


		if (Col.gameObject.name == "Trigger_Door_Room_7_8") 
		
		{

			Door_Room_7_8.Play ("Door_Room_7_8_Open", 0);

			Sound_Door_Room_7_8.Play ();

		}

	}


	// OnTriggerExit
	// When the FPSController leaves the trigger, then there is playing the animation (Close the door).
	// There is a condition to check the animation. Animation that was active when the FPSController was in the trigger area.

	void OnTriggerExit (Collider Col) 
	{


		if (Door_Room_1_2.GetCurrentAnimatorStateInfo(0).IsName("Door_Room_1_2_Open") ) 
		
		{

			Door_Room_1_2.Play ("Door_Room_1_2_Close", 0);

			Sound_Door_Room_1_2.Play ();

		}


		if (Door_Room_2_3.GetCurrentAnimatorStateInfo(0).IsName("Door_Room_2_3_Open") ) 
		
		{

			Door_Room_2_3.Play ("Door_Room_2_3_Close", 0);

			Sound_Door_Room_2_3.Play ();

		}


		if (Door_Room_3_4.GetCurrentAnimatorStateInfo(0).IsName("Door_Room_3_4_Open") ) 
		
		{

			Door_Room_3_4.Play ("Door_Room_3_4_Close", 0);

			Sound_Door_Room_3_4.Play ();

		}


		if (Door_Room_4_5.GetCurrentAnimatorStateInfo(0).IsName("Door_Room_4_5_Open") ) 
		
		{

			Door_Room_4_5.Play ("Door_Room_4_5_Close", 0);

			Sound_Door_Room_4_5.Play ();

		}


		if (Door_Room_5_6.GetCurrentAnimatorStateInfo(0).IsName("Door_Room_5_6_Open") ) 
		
		{

			Door_Room_5_6.Play ("Door_Room_5_6_Close", 0);

			Sound_Door_Room_5_6.Play ();

		}

		if (Door_Room_7_8.GetCurrentAnimatorStateInfo(0).IsName("Door_Room_7_8_Open") ) 
		
		{

			Door_Room_7_8.Play ("Door_Room_7_8_Close", 0);

			Sound_Door_Room_7_8.Play ();

		}
	}





}
