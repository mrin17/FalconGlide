using UnityEngine;
using System.Collections;

public class UserInputFalconRPhysics : MonoBehaviour {

    public float diveToBoostTransitionTime; //How quickly we want the transition from DIVE - BOOST
    public float boostEnergyMultiplier;     //Affects amount of energy gained from diving
    public float diveYSteepness; //How sharp the falcon dives down
    public float diveXSteepness; //How slow the falcon gets while diving
    public float glideSteepness; //How sharp the falcon glides
    public float boostYSteepness; //How much height collected during boost
    public float boostXSteepness; //Rate at which horizantal speed is gained from boost
    public float minXSpeed; //Minimum horizantal speed that the falcon will travel
    public float glideSpeed; //How fast falcon will fly horizantal while gliding
    public float initialBoostX; //Initial starting speed of boost

    falconStates currentState; //Current state falcon is in
    float timeDive;  //Running Counter for time in DIVE
    float timeGlide; //Running Counter for time in GLIDE
    float timeBoost; //Running Counter for time in BOOST
    float boostEnergy; //Energy consumed during boost, gained on dive
    float boostEnergyTotal; //Total energy amassed during a dive
    bool diveToBoostTransitionDone = false; //Whether DIVE-BOOST transition is complete
    float currentXVelocity; //Current X movememt to translate
    float currentYVelocity; //Current Y movement to translate
    float lastGlideYVelocity; //Storage of last Y-velocity in GLIDE
    float lastGlideXVelocity; //Storage of last X-velocity in GLIDE
    float lastDiveYVelocity;  //Storage of last Y-velocity in DIVE
    float lastDiveXVelocity; //Storage of last X-Velocity in DIVE
    float lastBoostYVelocity; //Storage of last Y-velocity in BOOST
    float lastBoostXVelocity; //Storage of last X-velocity in BOOST
    float lastTransitionXVelocity; //Storage of last X-velocity in DIVE-BOOST
    float lastTransitionYVelocity; //Storage of last Y-velocity in DIVE-BOOST
    float boostYEnergyAfterTransition; //Storage of last Y-velocity in DIVE-BOOST transition
    float boostXEnergyAfterTransition; //Storage of last X-velocity in DIVE-BOOST transition
    public Vector2 movementVect; //Vector for translating falcon
    
    public enum falconStates
    {
        gliding,
        boosting,
        diving,
    }

    /******************************
    /*****EQUATIONS FOR STATES*****
    ******************************/

    float sinRiseYEquation(float timeBoosting)
    {
        return boostYSteepness * Mathf.Sin(Mathf.PI / boostYEnergyAfterTransition * timeBoosting);
    }

    float sinRiseXEquation(float timeBoosting)
    {
        float h = Mathf.Log((-Mathf.Exp(-boostXEnergyAfterTransition) + 1) / lastTransitionXVelocity - .00001f);
        float k = lastTransitionXVelocity - Mathf.Exp(-h);
        return Mathf.Exp((1/boostXSteepness) * -timeBoosting - h) + k + minXSpeed;
    }

    float glidingYEquation(float timeGliding)
    {
        return (glideSteepness * timeGliding) + lastBoostYVelocity;
    }

    float glidingXEquation(float timeGliding)
    {
        return glideSpeed + minXSpeed;
    }

    float diveTransitionYEquation(float timeBoosting)
    {
        return (lastDiveYVelocity / (-diveToBoostTransitionTime)) * timeBoosting + lastDiveYVelocity;
    }

    float diveTransitionXEquation(float timeBoosting)
    {
        return ((lastDiveXVelocity - initialBoostX)/ (-diveToBoostTransitionTime)) * timeBoosting + lastDiveXVelocity;
    }

    float divingYEquation(float timeDiving)
    {
        return (diveYSteepness * timeDiving) + lastGlideYVelocity;
    }

    float divingXEquation(float timeDiving)
    {
        float result = (diveXSteepness * -Mathf.Pow(timeDiving, 2)) + lastGlideXVelocity;
        if (result < minXSpeed)
        {
            result = minXSpeed;
        }
        return result;
    }

    /********************************************
    *****************END EQUATIONS***************
    ********************************************/

    void Start ()
    {
        currentState = falconStates.gliding;
        currentYVelocity = 0;
        currentXVelocity = minXSpeed;
        lastBoostXVelocity = minXSpeed;
        movementVect = new Vector2(currentXVelocity, currentYVelocity);
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
            currentYVelocity = glidingYEquation(timeGlide);
            currentXVelocity = glidingXEquation(timeGlide);
            lastGlideYVelocity = currentYVelocity;
            lastGlideXVelocity = currentXVelocity;

            timeDive = 0.0f; //Reset Dive Time
            timeBoost = 0.0f; //Reset Boost Time

            movementVect = new Vector2(currentXVelocity, -currentYVelocity);
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
                currentYVelocity = diveTransitionYEquation(timeBoost);
                currentXVelocity = diveTransitionXEquation(timeBoost);
                lastTransitionXVelocity = currentXVelocity;
                lastTransitionYVelocity = currentYVelocity;
            }
            //Trigger for if the transition is complete
            else if (!diveToBoostTransitionDone)
            {
                diveToBoostTransitionDone = true;
                timeBoost = 0.0f;
                boostYEnergyAfterTransition = boostEnergy;
                boostXEnergyAfterTransition = boostEnergy;
            }
            //Boosting
            else
            {
                currentYVelocity = -sinRiseYEquation(timeBoost);
                currentXVelocity = sinRiseXEquation(timeBoost);
                lastBoostYVelocity = currentYVelocity;
                lastBoostXVelocity = currentXVelocity;
            }

            boostEnergy -= Time.deltaTime; //Subtract from boost energy every second
            timeDive = 0.0f;  //Reset dive time
            timeGlide = 0.0f; //Reset glide time

            movementVect = new Vector2(currentXVelocity, -currentYVelocity);
        }

        /**************DIVE STATE**************/
        else if (currentState == falconStates.diving)
        {
            timeDive += Time.deltaTime;
            boostEnergy = boostEnergyMultiplier * timeDive; //Convert time diving into boost power
            boostEnergyTotal = boostEnergy; //Stores the maximum boost energy gained from dive
            timeGlide = 0.0f; //Reset glide counter to 0
            timeBoost = 0.0f; //Reset boost counter to 0
            diveToBoostTransitionDone = false; //Reset transition toggle

            currentYVelocity = divingYEquation(timeDive);
            currentXVelocity = divingXEquation(timeDive);
            lastDiveYVelocity = currentYVelocity;
            lastDiveXVelocity = currentXVelocity;

            movementVect = new Vector2(currentXVelocity, -currentYVelocity);
        }

        transform.Translate(movementVect * Time.deltaTime, Space.World);
	}
}
