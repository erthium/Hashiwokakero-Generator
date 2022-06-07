using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Manager : MonoBehaviour
{
    [Header("Assignments")]
    public GameObject nodePrefab;
    public Transform parentObject;

    [Header("Board")]
    public int width = 10;
    public int height = 10;
    GameObject[,] nodeObjects;

    //[Header("Difficulty")]
    

    //definitions
    public Generator generator;

    void Start()
    {
        Initialize();
    }


    void Update()
    {
        
    }

    public void Initialize()
    {
        generator = new Generator(width, height);

        if (nodeObjects != null)
        {
            for (int x_ = 0; x_ < width; x_++)
            {
                for (int y_ = 0; y_ < height; y_++)
                {
                    Destroy(nodeObjects[x_, y_]);
                }
            }
        }

        nodeObjects = new GameObject[width, height];
    }

    public void Generate()
    {
        generator.Generate();

        for (int x_ = 0; x_ < width; x_++)
        {
            for (int y_ = 0; y_ < height; y_++)
            {
                GameObject new_node = Instantiate(nodePrefab, new Vector3(x_, -y_, 0), Quaternion.identity, parentObject);
                new_node.name = string.Format("{0}x{1}", x_, y_);
                nodeObjects[x_, y_] = new_node;
                if (generator.board[x_, y_].state == States.blank)
                {
                    new_node.GetComponentInChildren<TMP_Text>().text = " ";
                }
                else if(generator.board[x_, y_].state == States.bridge)
                {
                    if (generator.board[x_, y_].is_double)
                    {
                        new_node.GetComponentInChildren<TMP_Text>().text = "||";
                    }
                    else
                    {
                        new_node.GetComponentInChildren<TMP_Text>().text = "-";
                    } 
                }
                else
                {
                    new_node.GetComponentInChildren<TMP_Text>().text = generator.board[x_, y_].value.ToString();
                }

            }
        }
    }

}
