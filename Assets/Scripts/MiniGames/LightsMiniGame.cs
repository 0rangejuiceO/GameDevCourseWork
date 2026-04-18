using UnityEngine;
using UnityEngine.Rendering;

public class LightsMiniGame : MonoBehaviour
{

    private GameObject[] lights;
    [SerializeField]private int numRounds = 3;
    [SerializeField]private float timeBetweenRounds = 3f;
    [SerializeField]private float timeToShowLights = 2f;
    [SerializeField]private float timeBetweenLights = 1f;
    private int[] lightsOrder;

    private float timeSinceLastAction = 0f;
    private int lastAction;
    private int round;
    private int currentLight;
    public MiniGameHandler miniGameHandler;
    private bool reportedEnd = false;



    void Start()
    {
        MiniGameLight[] lightsComponents = gameObject.GetComponentsInChildren<MiniGameLight>();
        lights = new GameObject[lightsComponents.Length];

        for(int i = 0; i<lightsComponents.Length; i++)
        {
            lights[i] = lightsComponents[i].gameObject;
        }


        newOrder();
        miniGameHandler = GameObject.Find("MiniGameHandler").GetComponent<MiniGameHandler>();
    }

    void Update()
    {

        if (lastAction == 0)
        {
            if(timeSinceLastAction >= timeBetweenRounds)
            {
                lights[lightsOrder[0]].GetComponent<MiniGameLight>().TurnOn();
                lastAction = 1;
                timeSinceLastAction = 0f;
            }

        }
        else if (lastAction == 1)
        {
            if(timeSinceLastAction >= timeToShowLights)
            {
                lights[lightsOrder[currentLight]].GetComponent<MiniGameLight>().TurnOff();
                if(currentLight== round)
                {
                    currentLight = 0;
                    foreach(GameObject light in lights)
                    {
                        light.GetComponent<MiniGameLight>().BecomeInteractable();
                    }
                    lastAction = 3;
                }
                else
                {
                    lastAction = 2;
                }
                timeSinceLastAction = 0f;
            }
        }
        else if(lastAction == 2)
        {
            if(timeSinceLastAction >= timeBetweenLights)
            {
                currentLight++;
                lights[lightsOrder[currentLight]].GetComponent<MiniGameLight>().TurnOn();
                lastAction = 1;
                timeSinceLastAction = 0f;
            }
        }
        
        timeSinceLastAction += Time.deltaTime;

        if(lastAction == -1 && !reportedEnd)
        {
            reportedEnd = true;
            miniGameHandler.EndMiniGame(true);
        }
    }

    public void LightPressed(int index)
    {
        if(index == lightsOrder[currentLight])
        {
            currentLight++;
            if(currentLight > round)
            {

                round++;
                currentLight = 0;
                lastAction = 0;
                timeSinceLastAction = 0;
                if (round == numRounds)
                {
                    Debug.Log("Game Won!");
                    lastAction = -1;
                }
            }
        }
    }

    public void RestartGame()
    {
        newOrder();
    }

    private void newOrder()
    {
        lightsOrder = new int[numRounds];
        string debugString="New order: ";
        for (int i = 0; i < numRounds; i++)
        {
            lightsOrder[i] = Random.Range(0, lights.Length);
            debugString += lightsOrder[i] + " ";
        }
        Debug.Log(debugString);
        round = 0;
        currentLight = 0;
        lastAction = 0;
        timeSinceLastAction = 0;
    }
}
