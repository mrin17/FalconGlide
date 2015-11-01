using UnityEngine;
using System.Collections;

public class UserInputFalconRPhysics : MonoBehaviour {

    public float diveToBoostTransitionTime; //How quickly we want the transition from DIVE - BOOST
    public float boostEnergyMultiplier;     //Affects amount of energy gained from diving

    falconStates currentState; //Current state falcon is in
    float timeDive;  //Running Counter for time in DIVE
    float timeGlide; //Running Counter for time in GLIDE
    float timeBoost; //Running Counter for time in BOOST
    float boostEnergy; //Energy consumed during boost, gained on dive
    bool diveToBoostTransitionDone = false; //Whether DIVE-BOOST transition is complete
    float currentXVelocity; //Current X movememt to translate
    float currentYVelocity; //Current Y movement to translate
    float lastGlideYVelocity; //Storage of last velocity in GLIDE
    float lastDiveYVelocity;  //Storage of last velocity in DIVE
    float lastBoostYVelocity; //Storage of last velocity in BOOST
    float lastBoostTransitionYVelocity; //Storage of last velocity in DIVE-BOOST transition
    Vector2 movementVect; //Vector for translating falcon
    
    public enum falconStates
    {
        gliding,
        boosting,
        diving,
    }

    /******************************
    /*****EQUATIONS FOR STATES*****
    ******************************/

    float sinRiseEquation(float timeBoosting)
    {
        return Mathf.Sin(Mathf.PI / lastBoostTransitionYVelocity * timeBoosting);
    }

    float glidingEquation(float timeGliding)
    {
        return .5f * timeGlide + lastBoostYVelocity;
    }

    float diveTransitionEquation(float timeBoosting)
    {
        return (lastDiveYVelocity / (-diveToBoostTransitionTime)) * timeBoost + lastDiveYVelocity;
    }

    float divingEquation(float timeDiving)
    {
        return timeDiving + lastGlideYVelocity;
    }

    /********************************************
    *****************END EQUATIONS***************
    ********************************************/

    void Start ()
    {
        currentState = falconStates.gliding;
        currentYVelocity = 0;
        movementVect = new Vector2(1, -1);
    }
	
	void Update ()
    {
        //States based on key-presses
        if (Input.GetKeyDown(KeyCode.D))
        {
            currentState = falconStates.diving;
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            currentState = falconStates.boosting;
        }


        /***********GLIDE STATE****************/
        if (currentState == falconStates.gliding)
        {      
            timeGlide += Time.deltaTime;
            currentYVelocity = glidingEquation(timeGlide);
            lastGlideYVelocity = currentYVelocity;

            timeDive = 0.0f; //Reset Dive Time
            timeBoost = 0.0f; //Reset Boost Time

            movementVect = new Vector2(1, -currentYVelocity);
        }

        /************BOOST STATE**************/
        else if (currentState == falconStates.boosting)
        {
            timeBoost += Time.deltaTime;

            //If no more boost energy, return to gliding
            if (boostEnergy <= 0)
            {
                currentState = falconStates.gliding;
            }
            //Transition from diving to boosting
            else if (timeBoost < diveToBoostTransitionTime && !diveToBoostTransitionDone)
            {
                currentYVelocity = diveTransitionEquation(timeBoost);
            }
            //Trigger for if the transition is complete
            else if (!diveToBoostTransitionDone)
            {
                diveToBoostTransitionDone = true;
                timeBoost = 0.0f;
                lastBoostTransitionYVelocity = boostEnergy;
            }
            //Boosting
            else
            {
                currentYVelocity = -sinRiseEquation(timeBoost);
                lastBoostYVelocity = currentYVelocity;
            }

            boostEnergy -= Time.deltaTime; //Subtract from boost energy every second
            timeDive = 0.0f;  //Reset dive time
            timeGlide = 0.0f; //Reset glide time

            movementVect = new Vector2(1, -currentYVelocity);
        }

        /**************DIVE STATE**************/
        else if (currentState == falconStates.diving)
        {
            timeDive += Time.deltaTime;
            boostEnergy = boostEnergyMultiplier * timeDive; //Convert time diving into boost power
            timeGlide = 0.0f; //Reset glide counter to 0
            timeBoost = 0.0f; //Reset boost counter to 0
            diveToBoostTransitionDone = false; //Reset transition toggle

            currentYVelocity = divingEquation(timeDive);
            lastDiveYVelocity = currentYVelocity;

            movementVect = new Vector2(1, -currentYVelocity);
        }

        transform.Translate(movementVect * Time.deltaTime, Space.World);
	}
}
