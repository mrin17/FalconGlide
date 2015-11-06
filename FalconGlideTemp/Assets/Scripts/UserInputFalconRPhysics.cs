using UnityEngine;
using System.Collections;

public class UserInputFalconRPhysics : MonoBehaviour {

    public float boostEnergyMultiplier;     //Affects amount of energy gained from diving
    public float diveYSteepness; //How sharp the falcon dives down
    public float diveXSteepness; //How slow the falcon gets while diving
    public float glideSteepness; //How sharp the falcon glides
    public float boostYSteepness; //How much height collected during boost
    public float boostXSteepness; //Rate at which horizantal speed is gained from boost
    public float minXSpeed; //Minimum horizantal speed that the falcon will travel
    public float glideSpeed; //How fast falcon will fly horizantal while gliding
    public float boostXScale; //Scales effectiveness of boost x
    public float transitionFactor; //What portion of boost should transition be

    falconStates currentState; //Current state falcon is in
    falconStates previousState; //Previous state falcon was in
    float timeDive;  //Running Counter for time in DIVE
    float timeGlide; //Running Counter for time in GLIDE
    float timeBoost; //Running Counter for time in BOOST
    float timeTransition; //Running Counter for time in TRANSITION
    float boostEnergy; //Energy consumed during boost, gained on dive
    float boostEnergyTotal; //Total energy amassed during a dive
    float initialBoostX; //Initial starting speed of boost
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
    float diveToBoostTransitionTime; //How quickly we want the transition from DIVE - BOOST
    public Vector2 movementVect; //Vector for translating falcon
    
    public enum falconStates
    {
        gliding,
        boosting,
        diving,
        transition
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
        if (lastTransitionXVelocity < .00001f) lastTransitionXVelocity = .00001f;
        float h = Mathf.Log((-Mathf.Exp(-boostXEnergyAfterTransition) + 1) / lastTransitionXVelocity - .00001f);
        float k = lastTransitionXVelocity - Mathf.Exp(-h);
        float result = Mathf.Exp((1 / boostXSteepness) * -timeBoosting - h) + k;
        if (result < minXSpeed)
        {
            result = minXSpeed;
        }
        return result;
    }

    float glidingYEquation(float timeGliding)
    {
        return (glideSteepness * timeGliding) + lastBoostYVelocity;
    }

    float glidingXEquation(float timeGliding)
    {
        float h = Mathf.Log((-Mathf.Exp(-999999999) + 1) / lastBoostXVelocity - .00001f);
        float k = lastBoostXVelocity - Mathf.Exp(-h);
        float result = Mathf.Exp((1 / glideSpeed) * -timeGliding - h) + k;
        if (result < minXSpeed)
        {
            result = minXSpeed;
        }
        return result;
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
        if (previousState == falconStates.gliding)
        { 
            return (diveYSteepness * timeDiving) + lastGlideYVelocity;
        }
        else if (previousState == falconStates.boosting)
        {
            return (2 * diveYSteepness * timeDiving) + lastBoostYVelocity;
        }
        else
        {
            return (diveYSteepness * timeDiving) + lastTransitionYVelocity;
        }
    }

    float divingXEquation(float timeDiving)
    {
        float result;
        if (previousState == falconStates.gliding)
        {
            result = (diveXSteepness * -timeDiving) + lastGlideXVelocity;
        }
        else if (previousState == falconStates.boosting)
        {
            result = (diveXSteepness * -timeDiving) + lastBoostXVelocity;
        }
        else
        {
            result = (diveXSteepness * -timeDiving) + lastTransitionXVelocity;
        }

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
            if (currentState == falconStates.gliding)
            {
                previousState = falconStates.gliding;
            }
            else if (currentState == falconStates.boosting)
            {
                previousState = falconStates.boosting;
            }
            else if (currentState == falconStates.transition)
            {
                previousState = falconStates.transition;
            }
            currentState = falconStates.diving;
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            if (currentState == falconStates.diving)
            {
                currentState = falconStates.transition;
            }
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

        /***********TRANSITION STATE****************/
        else if (currentState == falconStates.transition)
        {
            timeTransition += Time.deltaTime;

            //If transition over, return to boost
            if (timeTransition >= diveToBoostTransitionTime)
            {
                currentState = falconStates.boosting;
                boostYEnergyAfterTransition = boostEnergy;
                boostXEnergyAfterTransition = boostEnergy;
                timeBoost = 0.0f;
            }

            currentYVelocity = diveTransitionYEquation(timeTransition);
            currentXVelocity = diveTransitionXEquation(timeTransition);
            lastTransitionXVelocity = currentXVelocity;
            lastTransitionYVelocity = currentYVelocity;
            boostEnergy -= Time.deltaTime;

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
            timeTransition = 0.0f; //Reset transition time

            movementVect = new Vector2(currentXVelocity, -currentYVelocity);
        }

        /**************DIVE STATE**************/
        else if (currentState == falconStates.diving)
        {
            timeDive += Time.deltaTime;
            boostEnergy = boostEnergyMultiplier * timeDive; //Convert time diving into boost power
            boostEnergyTotal = boostEnergy; //Stores the maximum boost energy gained from dive
            initialBoostX = timeDive * boostXScale;
            timeGlide = 0.0f; //Reset glide counter to 0
            timeBoost = 0.0f; //Reset boost counter to 0
            timeTransition = 0.0f; //Reset transition counter to 0
            diveToBoostTransitionDone = false; //Reset transition toggle
            diveToBoostTransitionTime = boostEnergyTotal / transitionFactor; //What portion of the boost should be transition

            currentYVelocity = divingYEquation(timeDive);
            currentXVelocity = divingXEquation(timeDive);
            lastDiveYVelocity = currentYVelocity;
            lastDiveXVelocity = currentXVelocity;

            movementVect = new Vector2(currentXVelocity, -currentYVelocity);
        }

        Debug.Log(movementVect);
        Debug.Log(currentState);
        transform.Translate(movementVect * Time.deltaTime, Space.World);
	}
}
