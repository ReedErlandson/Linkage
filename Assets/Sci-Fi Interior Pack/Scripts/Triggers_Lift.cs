
/* 

This script is designed to control the animation. Open and close the lift doors. Also, control the horizontal position of the lift.
This script is an example of how you can control the animation.

Lift operates automatically and does not require pressing buttons.
However, you can easily add buttons.
It is important that the door of the the lift and the door of another room contain different animation.

Next to the elevator located triggers.
Two trigger (Trigger_Lift_Door_Room) designed for playing animations (Open the lift doors).
Two trigger (Trigger_Lift) designed for playing animations (Change the horizontal position of the lift).

In the game there is a special object of the lift platform. The platform allows you to move your character, when the lift is moving.

This script should be attached to the Characters. In this case the FPSController.

*/


using UnityEngine;
using System.Collections;

public class Triggers_Lift : MonoBehaviour {


	// Variables Animator.
	// With these variables, you can control the animation.

	public Animator Lift_Door_Room_3;

	public Animator Lift_Door_Room_7;

	public Animator Lift;


	// Variables GameObject (Triggers).
	// These variables need to activate or deactivate triggers.

	public GameObject Trigger_Lift_1;

	public GameObject Trigger_Lift_2;

	public GameObject Trigger_Lift_Door_Room_3;

	public GameObject Trigger_Lift_Door_Room_7;


	// Variables GameObject (Lift_Platform).

	public GameObject Lift_Platform;

	// Variables CharacterController (FPSController).
	// This variable is necessary to move the character in the root of the hierarchy (Lift_Platform).

	public CharacterController FPSController;


	// Variables AudioSource.
	// With these variables, you can play the sounds (Movement the lift, Open and close the door).

	public AudioSource Sound_Lift_Moves;

	public AudioSource Sound_Lift_Door_Room_3;

	public AudioSource Sound_Lift_Door_Room_7;



	// Use this for initialization
	void Start () 

	{
	
	}
	
	// Update is called once per frame
	void Update () 

	{

	}



	// OnTriggerEnter
	// When an FPSController enters the trigger, then there is playing the animation.
	// There is a condition to check each trigger. There is a triggers for opening the door, and a trigger for the movement the lift.
	// At some points you need to disable triggers. For example, to not play the animation continuously. Otherwise, the elevator would move endlessly.

	void OnTriggerEnter (Collider Col) 

	{

		if (Col.gameObject.name == "Trigger_Lift_Door_Room_3" && Trigger_Lift_1.activeSelf == true) 
			
		{

			Lift_Door_Room_3.Play ("Lift_Door_Room_3_Open", 0);

			Lift.Play ("Lift_Door_Level_1_Open", 0);

			Sound_Lift_Door_Room_3.Play ();

		}


		if (Col.gameObject.name == "Trigger_Lift_Door_Room_7" && Trigger_Lift_2.activeSelf == true) 
			
		{

			Lift_Door_Room_7.Play ("Lift_Door_Room_7_Open", 0);

			Lift.Play ("Lift_Door_Level_2_Open", 0);

			Sound_Lift_Door_Room_7.Play ();

		}


		if (Col.gameObject.name == "Trigger_Lift_1") 
			
		{

			FPSController.transform.parent = Lift_Platform.transform;

			Lift_Door_Room_3.Play ("Lift_Door_Room_3_Close", 0);

			Lift.Play ("Lift_Moves_1", 0);

			Lift_Door_Room_7.Play ("Lift_Door_Room_7_Open", 0);

			Sound_Lift_Moves.Play ();

			Sound_Lift_Door_Room_3.Play ();

			Trigger_Lift_1.SetActive (false);

			Trigger_Lift_2.SetActive (false);

			Trigger_Lift_Door_Room_3.SetActive (false);

		}
	
	
		if (Col.gameObject.name == "Trigger_Lift_2") 
			
		{

			FPSController.transform.parent = Lift_Platform.transform;

			Lift_Door_Room_7.Play ("Lift_Door_Room_7_Close", 0);

			Lift.Play ("Lift_Moves_2", 0);

			Lift_Door_Room_3.Play ("Lift_Door_Room_3_Open", 0);

			Sound_Lift_Moves.Play ();

			Sound_Lift_Door_Room_7.Play ();

			Trigger_Lift_1.SetActive (false);

			Trigger_Lift_2.SetActive (false);

			Trigger_Lift_Door_Room_7.SetActive (false);

		}
	
	
	
	
	
	}




	// OnTriggerExit
	// When the FPSController leaves the trigger, then there is playing the animation (Close the lift doors).
	// There is a condition to check the animation. Animation that was active when the FPSController was in the trigger area.

	void OnTriggerExit (Collider Col) 

	{


		if (Lift_Door_Room_3.GetCurrentAnimatorStateInfo (0).IsName ("Lift_Door_Room_3_Open") && Col.gameObject.name == "Trigger_Lift_Door_Room_3") 
			
		{

			Lift_Door_Room_3.Play ("Lift_Door_Room_3_Close", 0);

			Lift.Play ("Lift_Door_Level_1_Close", 0);

			Sound_Lift_Door_Room_3.Play ();

			Trigger_Lift_1.SetActive (true);

			Trigger_Lift_Door_Room_7.SetActive (true);

			FPSController.transform.parent.DetachChildren ();

		}


		if (Lift_Door_Room_7.GetCurrentAnimatorStateInfo (0).IsName ("Lift_Door_Room_7_Open") && Col.gameObject.name == "Trigger_Lift_Door_Room_7") 
			
		{

			Lift_Door_Room_7.Play ("Lift_Door_Room_7_Close", 0);

			Lift.Play ("Lift_Door_Level_2_Close", 0);

			Sound_Lift_Door_Room_7.Play ();

			Trigger_Lift_2.SetActive (true);

			Trigger_Lift_Door_Room_3.SetActive (true);
		
			FPSController.transform.parent.DetachChildren ();
		
		}


	}






}

