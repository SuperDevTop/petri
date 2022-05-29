using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PetriNet : MonoBehaviour, IPointerDownHandler, IPointerClickHandler,
    IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject toolPanel;
    public Text InputText;
    public Image transitionImage;
    public Image tokenImage;
    public Image positionImage;
    public Image arrowImage;
    public Image deleteImage;
    public Image moveImage;
    public GameObject DialogImage;
    public GameObject InputDialogImage;
    public Sprite[] deleteSprite;
    public Sprite[] moveSprite;
    public float start_x, start_y, end_x, end_y, force_x, force_y;    

    GraphicRaycaster raycaster;
    GameObject deleteGameObject;
    GameObject moveGameObject;
    GameObject resizeObject;
    GameObject startGameObject;
    GameObject endGameObject;
    GameObject firstSimulatedPosition;    
    Image arrow;
    int closeButtonClickCount;
    int positionNumber;
    int tokenNumber;
    int transitionNumber;
    int arrowWeight;
    int arrowNumber;
    int interval;
    public static int weight1;
    public static  int weight2;
    int positionCount;
    bool isCreatePosition;
    bool isCreateToken;
    bool isConnect;
    bool isCreateTransition;
    bool isMove;
    bool isResize;
    public static bool isSimulation;
    public static bool isManualSimulation;
    bool isAction;

    //foramt initial UI.
    void Start()
    {
        DialogImage.SetActive(false);
        InputDialogImage.SetActive(false);
        startGameObject = null;
        endGameObject = null;
        deleteImage.GetComponent<Button>().enabled = false;
        moveImage.GetComponent<Button>().enabled = false;
       
    }

    //Find selected Element and place on current mouse position.
    public void FindElement(string str, int number)
    {
        for (int i = 0; i < GameObject.FindGameObjectsWithTag(str).Length; i++)
        {
            if (GameObject.FindGameObjectsWithTag(str)[i].GetComponentInChildren<Text>().text == "" + number)
            {
                //print("!!!!!!!!!");
                GameObject.FindGameObjectsWithTag(str)[i].transform.position = Input.mousePosition;                
            }
        }
    }

    //Delete Selected Element.
    public void DestroyElement(string str, int number)
    {
        for (int i = 0; i < GameObject.FindGameObjectsWithTag(str).Length; i++)
        {
            if (GameObject.FindGameObjectsWithTag(str)[i].GetComponentInChildren<Text>().text == "" + number)
            {                                
                Destroy(GameObject.FindGameObjectsWithTag(str)[i].gameObject);
            }
        }
    }

    //FInd every position and each position's token mnubers are all changed by Petri net logic.
    public void findConnectedPosition(GameObject gameObject)
    {
        //print(12312312312);
        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Arrow").Length; i++)
        {
            if (GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject == gameObject && GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponent<Position>().tokenNumber > 0)
            {
                weight1 = GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().weight;

                for (int j = 0; j < GameObject.FindGameObjectsWithTag("Transition").Length; j++)
                {
                    if (GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().endGameObject == GameObject.FindGameObjectsWithTag("Transition")[j])
                    {
                        for (int k = 0; k < GameObject.FindGameObjectsWithTag("Arrow").Length; k++)
                        {
                            if (GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().startGameObject == GameObject.FindGameObjectsWithTag("Transition")[j])
                            {
                                weight2 = GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().weight;

                                if (weight1 >= weight2)
                                {
                                    if (GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponent<Position>().tokenNumber >= weight2)
                                    {
                                        GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponent<Position>().tokenNumber -= weight2;
                                        GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponent<Position>().tokenNumber += weight2;

                                        for (int o = 0; o < GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponentsInChildren<Image>().Length; o++)
                                        {
                                            if (GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponentsInChildren<Image>()[o].tag == "Cell")
                                            {
                                                GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponentsInChildren<Image>()[o].enabled = false;
                                            }

                                        }

                                        for (int p = 0; p < GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponentsInChildren<Image>().Length; p++)
                                        {
                                            if (GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponentsInChildren<Image>()[p].tag == "Cell")
                                            {
                                                GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponentsInChildren<Image>()[p].enabled = false;
                                            }

                                        }

                                        for (int l = 1; l < GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponent<Position>().tokenNumber + 1; l++)
                                        {
                                            GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponentsInChildren<Image>()[l].enabled = true;
                                        }

                                        for (int m = 1; m < GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponent<Position>().tokenNumber + 1; m++)
                                        {
                                            GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponentsInChildren<Image>()[m].enabled = true;
                                        }
                                    }
                                    else
                                    {
                                        GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponent<Position>().tokenNumber += GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponent<Position>().tokenNumber;
                                        GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponent<Position>().tokenNumber = 0;

                                        for (int o = 0; o < GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponentsInChildren<Image>().Length; o++)
                                        {
                                            if (GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponentsInChildren<Image>()[o].tag == "Cell")
                                            {
                                                GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponentsInChildren<Image>()[o].enabled = false;
                                            }

                                        }

                                        for (int p = 0; p < GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponentsInChildren<Image>().Length; p++)
                                        {
                                            if (GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponentsInChildren<Image>()[p].tag == "Cell")
                                            {
                                                GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponentsInChildren<Image>()[p].enabled = false;
                                            }

                                        }

                                        for (int m = 1; m < GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponent<Position>().tokenNumber + 1; m++)
                                        {
                                            GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponentsInChildren<Image>()[m].enabled = true;
                                        }
                                    }
                                }
                                else
                                {
                                    if (GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponent<Position>().tokenNumber >= weight1)
                                    {
                                        GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponent<Position>().tokenNumber -= weight1;
                                        GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponent<Position>().tokenNumber += weight1;

                                        for (int o = 0; o < GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponentsInChildren<Image>().Length; o++)
                                        {
                                            if (GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponentsInChildren<Image>()[o].tag == "Cell")
                                            {
                                                GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponentsInChildren<Image>()[o].enabled = false;
                                            }

                                        }

                                        for (int p = 0; p < GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponentsInChildren<Image>().Length; p++)
                                        {
                                            if (GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponentsInChildren<Image>()[p].tag == "Cell")
                                            {
                                                GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponentsInChildren<Image>()[p].enabled = false;
                                            }

                                        }

                                        for (int l = 1; l < GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponent<Position>().tokenNumber + 1; l++)
                                        {
                                            GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponentsInChildren<Image>()[l].enabled = true;
                                        }

                                        for (int m = 1; m < GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponent<Position>().tokenNumber + 1; m++)
                                        {
                                            GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponentsInChildren<Image>()[m].enabled = true;
                                        }
                                    }
                                    else
                                    {
                                        GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponent<Position>().tokenNumber += GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponent<Position>().tokenNumber;
                                        GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponent<Position>().tokenNumber = 0;

                                        for (int o = 0; o < GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponentsInChildren<Image>().Length; o++)
                                        {
                                            if (GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponentsInChildren<Image>()[o].tag == "Cell")
                                            {
                                                GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponentsInChildren<Image>()[o].enabled = false;
                                            }

                                        }

                                        for (int p = 0; p < GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponentsInChildren<Image>().Length; p++)
                                        {
                                            if (GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponentsInChildren<Image>()[p].tag == "Cell")
                                            {
                                                GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponentsInChildren<Image>()[p].enabled = false;
                                            }

                                        }

                                        for (int m = 1; m < GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponent<Position>().tokenNumber + 1; m++)
                                        {
                                            GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponentsInChildren<Image>()[m].enabled = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

       // isAction = false;       
    }

    public static void findConnectedPositionManual(GameObject gameObject)
    {
        //print(12312312312);
        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Arrow").Length; i++)
        {
            if (GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().endGameObject == gameObject && GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponent<Position>().tokenNumber > 0)
            {
                weight1 = GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().weight;

                for (int j = 0; j < GameObject.FindGameObjectsWithTag("Transition").Length; j++)
                {
                    if (GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().endGameObject == GameObject.FindGameObjectsWithTag("Transition")[j])
                    {
                        for (int k = 0; k < GameObject.FindGameObjectsWithTag("Arrow").Length; k++)
                        {
                            if (GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().startGameObject == GameObject.FindGameObjectsWithTag("Transition")[j])
                            {
                                weight2 = GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().weight;

                                if (weight1 >= weight2)
                                {
                                    if (GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponent<Position>().tokenNumber >= weight2)
                                    {
                                        GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponent<Position>().tokenNumber -= weight2;
                                        GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponent<Position>().tokenNumber += weight2;

                                        for (int o = 0; o < GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponentsInChildren<Image>().Length; o++)
                                        {
                                            if (GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponentsInChildren<Image>()[o].tag == "Cell")
                                            {
                                                GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponentsInChildren<Image>()[o].enabled = false;
                                            }

                                        }

                                        for (int p = 0; p < GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponentsInChildren<Image>().Length; p++)
                                        {
                                            if (GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponentsInChildren<Image>()[p].tag == "Cell")
                                            {
                                                GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponentsInChildren<Image>()[p].enabled = false;
                                            }

                                        }

                                        for (int l = 1; l < GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponent<Position>().tokenNumber + 1; l++)
                                        {
                                            GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponentsInChildren<Image>()[l].enabled = true;
                                        }

                                        for (int m = 1; m < GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponent<Position>().tokenNumber + 1; m++)
                                        {
                                            GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponentsInChildren<Image>()[m].enabled = true;
                                        }
                                    }
                                    else
                                    {
                                        GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponent<Position>().tokenNumber += GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponent<Position>().tokenNumber;
                                        GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponent<Position>().tokenNumber = 0;

                                        for (int o = 0; o < GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponentsInChildren<Image>().Length; o++)
                                        {
                                            if (GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponentsInChildren<Image>()[o].tag == "Cell")
                                            {
                                                GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponentsInChildren<Image>()[o].enabled = false;
                                            }

                                        }

                                        for (int p = 0; p < GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponentsInChildren<Image>().Length; p++)
                                        {
                                            if (GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponentsInChildren<Image>()[p].tag == "Cell")
                                            {
                                                GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponentsInChildren<Image>()[p].enabled = false;
                                            }

                                        }

                                        for (int m = 1; m < GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponent<Position>().tokenNumber + 1; m++)
                                        {
                                            GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponentsInChildren<Image>()[m].enabled = true;
                                        }
                                    }
                                }
                                else
                                {
                                    if (GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponent<Position>().tokenNumber >= weight1)
                                    {
                                        GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponent<Position>().tokenNumber -= weight1;
                                        GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponent<Position>().tokenNumber += weight1;

                                        for (int o = 0; o < GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponentsInChildren<Image>().Length; o++)
                                        {
                                            if (GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponentsInChildren<Image>()[o].tag == "Cell")
                                            {
                                                GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponentsInChildren<Image>()[o].enabled = false;
                                            }

                                        }

                                        for (int p = 0; p < GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponentsInChildren<Image>().Length; p++)
                                        {
                                            if (GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponentsInChildren<Image>()[p].tag == "Cell")
                                            {
                                                GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponentsInChildren<Image>()[p].enabled = false;
                                            }

                                        }

                                        for (int l = 1; l < GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponent<Position>().tokenNumber + 1; l++)
                                        {
                                            GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponentsInChildren<Image>()[l].enabled = true;
                                        }

                                        for (int m = 1; m < GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponent<Position>().tokenNumber + 1; m++)
                                        {
                                            GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponentsInChildren<Image>()[m].enabled = true;
                                        }
                                    }
                                    else
                                    {
                                        GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponent<Position>().tokenNumber += GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponent<Position>().tokenNumber;
                                        GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponent<Position>().tokenNumber = 0;

                                        for (int o = 0; o < GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponentsInChildren<Image>().Length; o++)
                                        {
                                            if (GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponentsInChildren<Image>()[o].tag == "Cell")
                                            {
                                                GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject.GetComponentsInChildren<Image>()[o].enabled = false;
                                            }

                                        }

                                        for (int p = 0; p < GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponentsInChildren<Image>().Length; p++)
                                        {
                                            if (GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponentsInChildren<Image>()[p].tag == "Cell")
                                            {
                                                GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponentsInChildren<Image>()[p].enabled = false;
                                            }

                                        }

                                        for (int m = 1; m < GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponent<Position>().tokenNumber + 1; m++)
                                        {
                                            GameObject.FindGameObjectsWithTag("Arrow")[k].GetComponent<Arrow>().endGameObject.GetComponentsInChildren<Image>()[m].enabled = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // isAction = false;       
    }

    void Update()
    {
        //Start simulation
        if (isSimulation)
        {
            if (interval == 100)
            {
                //print(interval);
                interval = 0;
                //print(positionCount % GameObject.FindGameObjectsWithTag("Position").Length);
                findConnectedPosition(GameObject.FindGameObjectsWithTag("Position")[positionCount % GameObject.FindGameObjectsWithTag("Position").Length]);
               // isAction = true;
                positionCount++;                                
            }
            else
            {
                interval++;
            }
        }   

        //Press Esc key
        if (Input.GetKeyDown("escape"))
        {
            deleteImage.GetComponent<Button>().enabled = false;
            deleteImage.sprite = deleteSprite[0];
            deleteGameObject = null;
            isMove = false;            
            moveImage.sprite = moveSprite[0];
            
            //moveGameObject.transform.position = Input.mousePosition;
            moveGameObject = null;
            resizeObject = null;

            if (isCreateToken)
            {
                DestroyElement("Token", tokenNumber);                
            }
            else if (isCreateTransition)
            {
                DestroyElement("Transition", transitionNumber);
                transitionNumber--;
            }
            else if (isConnect)
            {
                DestroyElement("Arrow", arrowNumber);
                arrowNumber--;
            }
            else if (isCreatePosition)
            {
                DestroyElement("Position", positionNumber);
                positionNumber--;
            }
        }
        else
        {
            //Click create position button
            if (isCreatePosition)
            {
                FindElement("Position", positionNumber);
            }
            //Click create token button
            else if (isCreateToken)
            {
                FindElement("Token", tokenNumber);
            }
            //Click create transition button
            else if (isCreateTransition)
            {
                FindElement("Transition", transitionNumber);
            }   
            //Click move button        
            else if (isMove)
            {                
                moveGameObject.transform.position = Input.mousePosition;
            }
        }        
    }

    //Format all elements
    public void NewButtonClick()
    {        
        Application.LoadLevel("Petri");
    }

    //Quit App
    public void QuitButtonClick()
    {
        Application.Quit();
    }

    //Click help dialog
    public void HelpButtonClick()
    {
        DialogImage.SetActive(true);
    }

    //Click help close to quit
    public void HelpCloseButtonClick()
    {
        DialogImage.SetActive(false);
    }

    //Click Position to create
    public void CreatePositionButtonClick()
    {
    

        isCreateToken = false;
        isCreateTransition = false;
        isConnect = false;
        isCreatePosition = true;
        positionNumber++;
        Image image = Instantiate(positionImage, new Vector2(0, 0), Quaternion.identity);
        image.transform.SetParent(GameObject.FindGameObjectWithTag("PositionContainer").transform, false);
        image.GetComponentInChildren<Text>().text = "" + positionNumber;
        print(GameObject.FindGameObjectsWithTag("Position")[0].GetComponentsInChildren<Image>()[1].name); 
    }

    
    //Click Token to create
    public void CreateTokenButtonClick()
    {

      
        isCreateToken = true;
        isCreateTransition = false;
        isConnect = false;
        isCreatePosition = false;
        tokenNumber = 1;
        Image image = Instantiate(tokenImage, new Vector2(0, 0), Quaternion.identity);
        image.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
        image.GetComponentInChildren<Text>().text = "" + tokenNumber;
    }

    //Click Tramsition to create
    public void CreateTransitionButtonClick()
    {      

        isCreateToken = false;
        isCreateTransition = true;
        isConnect = false;
        isCreatePosition = false;
        transitionNumber++;
        Image image = Instantiate(transitionImage, new Vector2(0, 0), Quaternion.identity);
        image.transform.SetParent(GameObject.FindGameObjectWithTag("TransitionContainer").transform, false);
        image.GetComponentInChildren<Text>().text = "" + transitionNumber;        
    }

    //Click Arrow to create
    public void ConnectButtonClick()
    {       
        isCreateToken = false;
        isCreateTransition = false;
        isConnect = false;
        isCreatePosition = false;

        InputDialogImage.SetActive(true);      
    }

    //Delete slected element
    public void DeleteButtonClick()
    {
        deleteImage.GetComponent<Button>().enabled = false;
        deleteImage.sprite = deleteSprite[0];
        moveImage.GetComponent<Button>().enabled = false;
        moveImage.sprite = moveSprite[0];

        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Arrow").Length; i++)
        {
            print(12312312);
            if (GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().startGameObject == deleteGameObject.gameObject || GameObject.FindGameObjectsWithTag("Arrow")[i].GetComponent<Arrow>().endGameObject == deleteGameObject.gameObject)
            {
                Destroy(GameObject.FindGameObjectsWithTag("Arrow")[i]);
            }
        }

        Destroy(deleteGameObject.gameObject);

        if (deleteGameObject.gameObject.tag == "Transition")
        {
            transitionNumber--;
        }
        else if (deleteGameObject.gameObject.tag == "Position")
        {
            positionNumber--;
        }
    }

    //Click move button
    public void MoveButtonclick()
    {
        deleteImage.GetComponent<Button>().enabled = false;
        deleteImage.sprite = deleteSprite[0];
        moveImage.GetComponent<Button>().enabled = false;
        moveImage.sprite = moveSprite[0];
       
        isMove = true;
    }

    
    /*public void ResizeButtonClick()
    {
        arrowWeight = Int32.Parse(InputText.text);
        InputDialogImage.SetActive(false);
        isConnect = false;
        start_x = startGameObject.transform.position.x;
        start_y = startGameObject.transform.position.y;
        end_x = endGameObject.transform.position.x;
        end_y = endGameObject.transform.position.y;
        arrowNumber++;
        arrow = Instantiate(arrowImage, new Vector2(0, 0), Quaternion.identity);
        arrow.transform.SetParent(GameObject.FindGameObjectWithTag("ArrowContainer").transform, false);
        arrow.GetComponentsInChildren<Text>()[0].text = "" + arrowNumber;
        arrow.GetComponentsInChildren<Text>()[1].text = "" + arrowWeight;
        arrow.rectTransform.sizeDelta = new Vector2(Mathf.Sqrt(Mathf.Pow(end_x - start_x, 2) + Mathf.Pow(end_y - start_y, 2)), arrow.rectTransform.sizeDelta.y);
        arrow.transform.position = new Vector2((start_x + end_x) / 2, (start_y + end_y) / 2);

        if (end_x - start_x > 0)
        {
            arrow.rectTransform.Rotate(0, 0, Mathf.Atan((end_y - start_y) / (end_x - start_x)) * 180 / 3.14f);
        }
        else
        {
            arrow.rectTransform.Rotate(0, 0, Mathf.Atan((end_y - start_y) / (end_x - start_x)) * 180 / 3.14f + 180);
        }

        startGameObject = null;
        endGameObject = null;
    }*/

    //Hide toolbar
    public void CloseButtonClick()
    {       
        if (closeButtonClickCount % 2 == 0)
        {
            toolPanel.SetActive(false);
        }
        else
        {
            toolPanel.SetActive(true);
        }

        closeButtonClickCount++;
    }

    //Click left mouse
    public void OnPointerDown(PointerEventData eventData)
    {
        //Click any element
        if (!isCreatePosition && !isCreateToken && !isCreateTransition && !isConnect)
        {
            if (eventData.pointerCurrentRaycast.gameObject.name != "Canvas" && eventData.pointerCurrentRaycast.gameObject.name != "ToolPanel" && eventData.pointerCurrentRaycast.gameObject.name != "Close")
            {
                deleteImage.GetComponent<Button>().enabled = true;
                deleteImage.sprite = deleteSprite[1];
                moveImage.GetComponent<Button>().enabled = true;
                moveImage.sprite = moveSprite[1];                
                deleteGameObject = eventData.pointerCurrentRaycast.gameObject;
                moveGameObject = eventData.pointerCurrentRaycast.gameObject;

                if (eventData.pointerCurrentRaycast.gameObject.name == "Arrow")
                {
                   
                    resizeObject = eventData.pointerCurrentRaycast.gameObject;
                }
            }
            else
            {
                deleteImage.GetComponent<Button>().enabled = false;
                deleteImage.sprite = deleteSprite[0];
                deleteGameObject = null;
            }
        }
      /*  else if (!isCreatePosition && !isCreateToken && !isCreateTransition && isConnect && startGameObject == null)
        {
            startGameObject = eventData.pointerCurrentRaycast.gameObject;
        }
        else if (!isCreatePosition && !isCreateToken && !isCreateTransition && isConnect && startGameObject != null && startGameObject.tag == "Position")
        {
            print("12312312312312");
            endGameObject = eventData.pointerCurrentRaycast.gameObject;

            if (eventData.pointerCurrentRaycast.gameObject.tag == "Transition")
            {
                CreateArrow();
            }
        }
        else if (!isCreatePosition && !isCreateToken && !isCreateTransition && isConnect && startGameObject != null && startGameObject.tag == "Transition")
        {
            print(432423423432);
            endGameObject = eventData.pointerCurrentRaycast.gameObject;

            if (eventData.pointerCurrentRaycast.gameObject.tag == "Position")
            {
                CreateArrow();
            }
        }   */     
    }

   /* public void CreateArrow()
    {
        InputDialogImage.SetActive(true);
       
    }*/

    /* public void WeightOkButtonClick()
     {
         arrowWeight = Int32.Parse(InputText.text);
         InputDialogImage.SetActive(false);
         isConnect = false;
         start_x = startGameObject.transform.position.x;
         start_y = startGameObject.transform.position.y;
         end_x = endGameObject.transform.position.x;
         end_y = endGameObject.transform.position.y;
         arrowNumber++;
         arrow = Instantiate(arrowImage, new Vector2(0, 0), Quaternion.identity);
         arrow.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
         arrow.GetComponentsInChildren<Text>()[0].text = "" + arrowNumber;
         arrow.GetComponentsInChildren<Text>()[1].text = "" + arrowWeight;
         arrow.rectTransform.sizeDelta = new Vector2(Mathf.Sqrt(Mathf.Pow(end_x - start_x, 2) + Mathf.Pow(end_y - start_y, 2)), arrow.rectTransform.sizeDelta.y);
         arrow.transform.position = new Vector2((start_x + end_x) / 2, (start_y + end_y) / 2);

         if (end_x - start_x > 0)
         {
             arrow.rectTransform.Rotate(0, 0, Mathf.Atan((end_y - start_y) / (end_x - start_x)) * 180 / 3.14f);
         }
         else
         {
             arrow.rectTransform.Rotate(0, 0, Mathf.Atan((end_y - start_y) / (end_x - start_x)) * 180 / 3.14f + 180);
         }

         startGameObject = null;
         endGameObject = null;
     }*/

    //Define Arrow's weight
    public void WeightOkButtonClick()
    {
        if (Int32.Parse(InputText.text) > 0)
        {
            InputText.text = "1";
            arrowWeight = Int32.Parse(InputText.text);
            InputDialogImage.SetActive(false);
            isConnect = true;
        }
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }

    
    public void OnPointerUp(PointerEventData eventData)
    {
        //create position
        if (isCreatePosition)
        {
            FindElement("Position", positionNumber);

            for (int j = 0; j < eventData.pointerCurrentRaycast.gameObject.GetComponentsInChildren<Image>().Length; j++)
            {
                if (eventData.pointerCurrentRaycast.gameObject.GetComponentsInChildren<Image>()[j].tag == "Cell")
                {
                    eventData.pointerCurrentRaycast.gameObject.GetComponentsInChildren<Image>()[j].enabled = false;
                }
                
            }
        }
        //create token
        else if (isCreateToken)
        {
            for (int i = 0; i < GameObject.FindGameObjectsWithTag("Position").Length; i++)
            {
                if (Vector3.Distance(GameObject.FindGameObjectsWithTag("Position")[i].gameObject.transform.position, Input.mousePosition) <= 50.0f && GameObject.FindGameObjectsWithTag("Position")[i].gameObject.GetComponent<Position>().tokenNumber < 15)
                {
                    GameObject.FindGameObjectsWithTag("Position")[i].gameObject.GetComponent<Position>().tokenNumber++;

                    for (int j = 1; j < GameObject.FindGameObjectsWithTag("Position")[i].gameObject.GetComponent<Position>().tokenNumber + 1; j++)
                    {
                        GameObject.FindGameObjectsWithTag("Position")[i].gameObject.GetComponentsInChildren<Image>()[j].enabled = true;
                    }
                }
            }
                Destroy(eventData.pointerCurrentRaycast.gameObject);
        }
        //create transition
        else if (isCreateTransition)
        {
            FindElement("Transition", transitionNumber);
        }    
        //Move element    
        else if (isMove)
        {
            moveImage.GetComponent<Button>().enabled = false;
            moveImage.sprite = moveSprite[0];
            deleteImage.GetComponent<Button>().enabled = false;
            deleteImage.sprite = deleteSprite[0];
            
            moveGameObject.transform.position = Input.mousePosition;
            isMove = false;
            moveGameObject = null;
        }

        isCreatePosition = false;        
        isCreateToken = false;
        isCreateTransition = false;  
    }

    public void OnPointerExit(PointerEventData eventData)
    {
       
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    //Start mouse drag to create arrow:define arrow's start point
    public void OnBeginDrag(PointerEventData eventData)
    {
        //when start element is position
        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Position").Length; i++)
        {
            //print(Vector3.Distance(GameObject.FindGameObjectsWithTag("Position")[i].gameObject.GetComponentsInChildren<Image>()[0].transform.position, Input.mousePosition));
            //print(Vector3.Distance(GameObject.FindGameObjectsWithTag("Position")[i].gameObject.GetComponentsInChildren<Image>()[1].transform.position, Input.mousePosition));

            for (int j = 16; j < 20; j++)
            {
                print(GameObject.FindGameObjectsWithTag("Position")[i].gameObject.GetComponentsInChildren<Image>()[j].name);
                if (isConnect && Vector3.Distance(GameObject.FindGameObjectsWithTag("Position")[i].gameObject.GetComponentsInChildren<Image>()[j].transform.position, Input.mousePosition) <= 20.0f)
                {

                    start_x = GameObject.FindGameObjectsWithTag("Position")[i].gameObject.GetComponentsInChildren<Image>()[j].transform.position.x;
                    start_y = GameObject.FindGameObjectsWithTag("Position")[i].gameObject.GetComponentsInChildren<Image>()[j].transform.position.y;
                    end_x = GameObject.FindGameObjectsWithTag("Position")[i].gameObject.GetComponentsInChildren<Image>()[j].transform.position.x;
                    end_y = GameObject.FindGameObjectsWithTag("Position")[i].gameObject.GetComponentsInChildren<Image>()[j].transform.position.y;
                    /* arrowNumber++;
                   arrow = Instantiate(arrowImage, new Vector2(0, 0), Quaternion.identity);
                   arrow.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
                   arrow.GetComponentsInChildren<Text>()[0].text = "" + arrowNumber;
                   arrow.GetComponentsInChildren<Text>()[1].text = "" + arrowWeight;
                   arrow.rectTransform.sizeDelta = new Vector2(Mathf.Sqrt(Mathf.Pow(end_x - start_x, 2) + Mathf.Pow(end_y - start_y, 2)), arrow.rectTransform.sizeDelta.y);
                   arrow.transform.position = new Vector2((start_x + end_x) / 2, (start_y + end_y) / 2);*/

                    /* if (end_x - start_x > 0)
                     {
                         arrow.rectTransform.Rotate(0, 0, Mathf.Atan((end_y - start_y) / (end_x - start_x)) * 180 / 3.14f);
                     }
                     else
                     {
                         arrow.rectTransform.Rotate(0, 0, Mathf.Atan((end_y - start_y) / (end_x - start_x)) * 180 / 3.14f + 180);
                     }*/

                    startGameObject = GameObject.FindGameObjectsWithTag("Position")[i].gameObject;
                    endGameObject = null;
                }
            }           
        }

        //when start element is transition
        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Transition").Length; i++)
        {
            for (int j = 1; j < 11; j++)
            {
                print(GameObject.FindGameObjectsWithTag("Transition")[i].gameObject.GetComponentsInChildren<Image>()[j].name);
                if (isConnect && Vector3.Distance(GameObject.FindGameObjectsWithTag("Transition")[i].gameObject.GetComponentsInChildren<Image>()[j].transform.position, Input.mousePosition) <= 20.0f)
                {

                    start_x = GameObject.FindGameObjectsWithTag("Transition")[i].gameObject.GetComponentsInChildren<Image>()[j].transform.position.x;
                    start_y = GameObject.FindGameObjectsWithTag("Transition")[i].gameObject.GetComponentsInChildren<Image>()[j].transform.position.y;
                    end_x = GameObject.FindGameObjectsWithTag("Transition")[i].gameObject.GetComponentsInChildren<Image>()[j].transform.position.x;
                    end_y = GameObject.FindGameObjectsWithTag("Transition")[i].gameObject.GetComponentsInChildren<Image>()[j].transform.position.y;
                    /* arrowNumber++;
                    arrow = Instantiate(arrowImage, new Vector2(0, 0), Quaternion.identity);
                    arrow.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
                    arrow.GetComponentsInChildren<Text>()[0].text = "" + arrowNumber;
                    arrow.GetComponentsInChildren<Text>()[1].text = "" + arrowWeight;
                    arrow.rectTransform.sizeDelta = new Vector2(Mathf.Sqrt(Mathf.Pow(end_x - start_x, 2) + Mathf.Pow(end_y - start_y, 2)), arrow.rectTransform.sizeDelta.y);
                    arrow.transform.position = new Vector2((start_x + end_x) / 2, (start_y + end_y) / 2);*/

                    /* if (end_x - start_x > 0)
                     {
                         arrow.rectTransform.Rotate(0, 0, Mathf.Atan((end_y - start_y) / (end_x - start_x)) * 180 / 3.14f);
                     }
                     else
                     {
                         arrow.rectTransform.Rotate(0, 0, Mathf.Atan((end_y - start_y) / (end_x - start_x)) * 180 / 3.14f + 180);
                     }*/

                    startGameObject = GameObject.FindGameObjectsWithTag("Transition")[i].gameObject;
                    endGameObject = null;
                }
            }
         
        }


    }

    //is OnDrag
    public void OnDrag(PointerEventData eventData)
    {        
        if (isConnect && startGameObject != null)
        {
            end_x = Input.mousePosition.x;
            end_y = Input.mousePosition.y;
            /* arrow.rectTransform.sizeDelta = new Vector2(Mathf.Sqrt(Mathf.Pow(end_x - start_x, 2) + Mathf.Pow(end_y - start_y, 2)), arrow.rectTransform.sizeDelta.y);
             arrow.transform.position = new Vector2((start_x + end_x) / 2, (start_y + end_y) / 2);*/

            /* if (end_x - start_x > 0)
             {
                 arrow.rectTransform.Rotate(0, 0, Mathf.Atan((end_y - start_y) / (end_x - start_x)) * 180 / 3.14f);
             }
             else
             {
                 arrow.rectTransform.Rotate(0, 0, Mathf.Atan((end_y - start_y) / (end_x - start_x)) * 180 / 3.14f + 180);
             }*/
        }
    }

    //End mouse drag
    public void OnEndDrag(PointerEventData eventData)
    {
        if (isConnect && startGameObject != null)
        {
            //when start element is transition
            if (startGameObject.tag == "Transition")
            {
                for (int i = 0; i < GameObject.FindGameObjectsWithTag("Position").Length; i++)
                {
                    for (int j = 16; j < 20; j++)
                    {
                        if (isConnect && Vector3.Distance(GameObject.FindGameObjectsWithTag("Position")[i].gameObject.GetComponentsInChildren<Image>()[j].transform.position, Input.mousePosition) <= 20.0f)
                        {
                            isConnect = false;
                            end_x = GameObject.FindGameObjectsWithTag("Position")[i].gameObject.GetComponentsInChildren<Image>()[j].transform.position.x;
                            end_y = GameObject.FindGameObjectsWithTag("Position")[i].gameObject.GetComponentsInChildren<Image>()[j].transform.position.y;
                            arrowNumber++;
                            arrow = Instantiate(arrowImage, new Vector2(0, 0), Quaternion.identity);
                            arrow.transform.SetParent(GameObject.FindGameObjectWithTag("ArrowContainer").transform, false);
                            arrow.GetComponentsInChildren<Text>()[0].text = "" + arrowNumber;
                            arrow.GetComponentsInChildren<Text>()[1].text = "" + arrowWeight;
                            arrow.GetComponentsInChildren<Text>()[2].text = "" + arrowWeight;
                            arrow.rectTransform.sizeDelta = new Vector2(Mathf.Sqrt(Mathf.Pow(end_x - start_x, 2) + Mathf.Pow(end_y - start_y, 2)), arrow.rectTransform.sizeDelta.y);
                            arrow.transform.position = new Vector2((start_x + end_x) / 2, (start_y + end_y) / 2);
                            arrow.GetComponent<Arrow>().startGameObject = startGameObject;
                            arrow.GetComponent<Arrow>().endGameObject = GameObject.FindGameObjectsWithTag("Position")[i].gameObject;
                            arrow.GetComponent<Arrow>().weight = arrowWeight;

                            if (end_x - start_x > 0)
                            {
                                arrow.GetComponentsInChildren<Image>()[0].rectTransform.Rotate(0, 0, Mathf.Atan((end_y - start_y) / (end_x - start_x)) * 180 / 3.14f);
                            }
                            else
                            {
                                arrow.GetComponentsInChildren<Image>()[0].rectTransform.Rotate(0, 0, Mathf.Atan((end_y - start_y) / (end_x - start_x)) * 180 / 3.14f + 180);
                            }
                        }
                    }
                    
                }
            }
            //when start position is position
            else if (startGameObject.tag == "Position")
            {
                for (int i = 0; i < GameObject.FindGameObjectsWithTag("Transition").Length; i++)
                {
                    for (int j = 1; j < 11; j++)
                    {
                        if (isConnect && Vector3.Distance(GameObject.FindGameObjectsWithTag("Transition")[i].gameObject.GetComponentsInChildren<Image>()[j].transform.position, Input.mousePosition) <= 20.0f)
                        {
                            isConnect = false;
                            end_x = GameObject.FindGameObjectsWithTag("Transition")[i].gameObject.GetComponentsInChildren<Image>()[j].transform.position.x;
                            end_y = GameObject.FindGameObjectsWithTag("Transition")[i].gameObject.GetComponentsInChildren<Image>()[j].transform.position.y;
                            arrowNumber++;
                            arrow = Instantiate(arrowImage, new Vector2(0, 0), Quaternion.identity);
                            arrow.transform.SetParent(GameObject.FindGameObjectWithTag("ArrowContainer").transform, false);
                            arrow.GetComponentsInChildren<Text>()[0].text = "" + arrowNumber;
                            arrow.GetComponentsInChildren<Text>()[1].text = "" + arrowWeight;
                            arrow.GetComponentsInChildren<Text>()[2].text = "" + arrowWeight;
                            arrow.rectTransform.sizeDelta = new Vector2(Mathf.Sqrt(Mathf.Pow(end_x - start_x, 2) + Mathf.Pow(end_y - start_y, 2)), arrow.rectTransform.sizeDelta.y);
                            arrow.transform.position = new Vector2((start_x + end_x) / 2, (start_y + end_y) / 2);
                            arrow.GetComponent<Arrow>().startGameObject = startGameObject;
                            arrow.GetComponent<Arrow>().endGameObject = GameObject.FindGameObjectsWithTag("Transition")[i].gameObject;
                            arrow.GetComponent<Arrow>().weight = arrowWeight;

                            if (end_x - start_x > 0)
                            {
                                arrow.GetComponentsInChildren<Image>()[0].rectTransform.Rotate(0, 0, Mathf.Atan((end_y - start_y) / (end_x - start_x)) * 180 / 3.14f);
                            }
                            else
                            {
                                arrow.GetComponentsInChildren<Image>()[0].rectTransform.Rotate(0, 0, Mathf.Atan((end_y - start_y) / (end_x - start_x)) * 180 / 3.14f + 180);
                            }
                        }
                    }

                   
                }
            }           
        }

        startGameObject = null;

    }

    //Click simulation button
    public void PlaySimulationClick()
    {
        isSimulation = true;
        isManualSimulation = false;

        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Transition").Length; i++)
        {
            GameObject.FindGameObjectsWithTag("Transition")[i].gameObject.GetComponent<Button>().enabled = false;
        }

        /*for (int i = 0; i < GameObject.FindGameObjectsWithTag("Position").Length; i++)
        {
            firstSimulatedPosition = GameObject.FindGameObjectsWithTag("Position")[i].gameObject;

            if (firstSimulatedPosition.GetComponent<Position>().tokenNumber < GameObject.FindGameObjectsWithTag("Position")[i].gameObject.GetComponent<Position>().tokenNumber)
            {
                firstSimulatedPosition = GameObject.FindGameObjectsWithTag("Position")[i].gameObject;

                if (firstSimulatedPosition.GetComponent<Position>().tokenNumber != 0)
                {
                    isSimulation = true;
                }
            }
        }*/
    }

    //Click pause button
    public void PauseSimulationClick()
    {
        isSimulation = false;
        isManualSimulation = true;

        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Transition").Length; i++)
        {
            GameObject.FindGameObjectsWithTag("Transition")[i].gameObject.GetComponent<Button>().enabled = true;
        }
        interval = 0;
        positionCount = 0;        
    }

    //Click stop button
    public void StopSimulationClick()
    {
        isSimulation = false;
        interval = 0;
        positionCount = 0;
        isAction = false;
    }  
}
