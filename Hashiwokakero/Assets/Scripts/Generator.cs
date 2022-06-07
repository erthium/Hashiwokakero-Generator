using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Generator
{
    public int width;
    public int height;


    public Node[,] board;
    public List<Vector2Int> open = new List<Vector2Int>();


    public bool is_generated;

    public Generator(int width = 10, int height = 10)
    {
        this.width = width;
        this.height = height;


        //setting the board in starting position
        board = new Node[width, height];
        for (int x_ = 0; x_ < width; x_++)
        {
            for (int y_ = 0; y_ < height; y_++)
            {
                board[x_, y_] = new Node(x_, y_);
            }
        }

        is_generated = false;

    }

    public void Generate()
    //generating a random puzzle
    {

        //f1 - choosing a starting point and value randomly and adding the point to "open"
        /*
         Vector2Int starting_point = new Vector2Int(Random.range(0, width), Random.range(0, height)); 
        int starting_point = Random.range(1,9);
         */
        //for testing easily, starting point and value are predefined
        Vector2Int starting_point = new Vector2Int(5, 5);
        int starting_value = 1;

        board[starting_point.x, starting_point.y].MakeIsland(starting_value, 0);

        open.Add(starting_point);

        while(open.Count > 0)
        {
            //get the first node in open
            Vector2Int current_node = open[0];
            Debug.Log("Current Node Is: " + current_node);

            //f2 - find available directions, if there is none, remove the node from "open" and continue
            if (!FindPossibleDirections(current_node.x, current_node.y))
            {
                open.Remove(current_node);
                Debug.Log("Removed: " + current_node);
                Debug.Log("New Open Length = " + open.Count);
                continue;
            }

            //f3 - choose a random direction and choose one or two bridges
            string current_direction = ChooseRandomDirection(current_node.x, current_node.y);
            Vector2Int direction_vector = Dirs.dir_to_vector[current_direction];

            //f4 - if the bridges must be double, if there is no other way to complete the value of the node, they will be double
            int number_of_bridges;
            int needed_bridges = board[current_node.x, current_node.y].value - board[current_node.x, current_node.y].current_value;
            Debug.Log("Needed Bridges = " + needed_bridges);
            Debug.Log("That fella = " + NumberOfAvailableDirections(current_node.x, current_node.y) * 2);
            if (NumberOfAvailableDirections(current_node.x, current_node.y) * 2 == needed_bridges) //f4
            {
                number_of_bridges = 2;
            }
            else if(needed_bridges == 1)
            {
                number_of_bridges = 1;
            }
            else
            {
                number_of_bridges = Random.Range(1, 3);
            }
            Debug.Log("Number Of Bridges = " + number_of_bridges);



            //f5 - choose a valid random length, if it is 0, it means the current direction is
            //not available since it will block other nodes
            //maximum length is calculated considering that the next node after the maximum length will be an island
            //f6 - try to set the bridge, if it blocks an island try every possible length, if does not work, continue
            int current_length = ChooseLength(current_node.x, current_node.y, direction_vector);
            if (current_length == 0)
            {
                continue;
            }

            //f7 - set the bridges, give the new island a possible random value and insert it to the first index of "open"
            SetBridges(current_node.x, current_node.y, direction_vector, current_length, number_of_bridges);

        }
        is_generated = true;
    }

    public bool FindPossibleDirections(int x, int y) //f2
    //if two nodes in a row is blank in a direction, that direction is available
    //we must also check if 1 or 2 node in that direction is still in the board or not
    //this function is disabling the directions in island if they are not available
    //return false if there is no available direction
    {
        if(board[x, y].state == States.island && board[x, y].value == board[x, y].current_value)
        {
            return false;
        }

        //checking up
        if(y < 2)
        {
            board[x, y].directions.up = false;
        }
        else if(board[x, y - 1].state != States.blank || board[x, y - 2].state != States.blank)
        {
            board[x, y].directions.up = false;
        }

        //checking right
        if (x > width - 3)
        {
            board[x, y].directions.right = false;
        }
        else if (board[x + 1, y].state != States.blank || board[x + 2, y].state != States.blank)
        {
            board[x, y].directions.right = false;
        }

        //checking down
        if(y > height - 3)
        {
            board[x, y].directions.down = false;
        }
        else if (board[x, y + 1].state != States.blank || board[x, y + 2].state != States.blank)
        {
            board[x, y].directions.down = false;
        }

        //checking left
        if(x < 2)
        {
            board[x, y].directions.left = false;
        }
        else if (board[x - 1, y].state != States.blank || board[x - 2, y].state != States.blank)
        {
            board[x, y].directions.left = false;
        }

        //if none of the directions are available, function will return false, otherwise it will return true
        if (board[x, y].directions.IsAllFalse()) { return false; }
        return true;
    }

    public string ChooseRandomDirection(int x, int y) //f3
    //
    {
        string direction = string.Empty;
        bool[] available_directions = board[x, y].directions.AllDirections();
        while (true)
        {
            int random_number = Random.Range(0, 4);
            if (available_directions[random_number])
            {
                switch (random_number)
                {
                    case 0:
                        direction = Dirs.up;
                        //board[x, y].directions.up = false;
                        break;
                    case 1:
                        direction = Dirs.right;
                        //board[x, y].directions.right = false;
                        break;
                    case 2:
                        direction = Dirs.down;
                        //board[x, y].directions.down = false;
                        break;
                    case 3:
                        direction = Dirs.left;
                        //board[x, y].directions.left = false;
                        break;
                    default: break;
                }
                break;
            }
        }
        return direction;
    }



    public int NumberOfAvailableDirections(int x, int y) //f4
    {
        int available_directions_amount = 0;

        //checking up
        if (board[x, y].directions.up == true)
        {
            if (board[x, y - 1].state == States.blank && board[x, y - 2].state == States.blank)
            {
                available_directions_amount++;
            }
        }

        //checking right
        if (board[x, y].directions.right == true)
        {
            if (board[x + 1, y].state == States.blank && board[x + 2, y].state == States.blank)
            {
                available_directions_amount++;
            }
        }

        //checking down
        if (board[x, y].directions.down == true)
        {
            if (board[x, y + 1].state == States.blank && board[x, y + 2].state == States.blank)
            {
                available_directions_amount++;
            }
        }

        //checking left
        if (board[x, y].directions.left == true)
        {
            if (board[x - 1, y].state == States.blank && board[x - 2, y].state == States.blank)
            {
                available_directions_amount++;
            }
        }

        return available_directions_amount;
    }




    public int ChooseLength(int x, int y, Vector2Int direction_vector) //f5
    //first find the maximumu length, then create an array from 1 to the maximum length
    //then choose one random length from that array
    {
        int current_length = 0;
        //finding the maximum length by going forword in the current direction one by one
        Vector2Int current_position = new Vector2Int(x, y);
        int max_length = -1;
        while(current_position.x + direction_vector.x < width && current_position.y + direction_vector.y < height &&
            current_position.x + direction_vector.x > 0 && current_position.y + direction_vector.y > 0)
        {
            current_position += direction_vector;
            if(board[current_position.x, current_position.y].state != States.blank)
            {
                break;
            }
            max_length++;
        }

        //creating a list that contains all the possible lengths
        List<int> possible_lentghs = new List<int>();

        for (int i = 0; i < max_length; i++) 
        { 
            possible_lentghs.Add(i + 1);
        }

        //checking if these length are blocking any other node, in a random order
        while(possible_lentghs.Count > 0)
        {
            current_length = possible_lentghs[Random.Range(0, possible_lentghs.Count)];
            if (DoesItBlock(x, y, direction_vector, current_length)) //f6
            {
                possible_lentghs.Remove(current_length);
                current_length = 0;
                continue;
            }
            break;
        }

        return current_length;
    }

    public bool DoesItBlock(int x, int y, Vector2Int direction_vector, int length) //f6
    {
        Vector2Int current_position = new Vector2Int(x, y) + direction_vector; //position of the first bridge
        for (int i = 0; i < length; i++)
        {
            board[current_position.x, current_position.y].state = States.bridge;
            current_position += direction_vector;
        }
        board[current_position.x, current_position.y].state = States.island;
        //after this loop, the current_position is equal to the position of the new island

        //checking all the open islands accept the current one
        foreach (Vector2Int node_to_check in open.GetRange(1, open.Count - 1))
        {
            int available_direction_amount = NumberOfAvailableDirections(node_to_check.x, node_to_check.y); //f4
            int needed_bridges = board[node_to_check.x, node_to_check.y].value - board[node_to_check.x, node_to_check.y].current_value;
            if (available_direction_amount * 2 < needed_bridges)
            {
                //unsetting the bridges
                for (int i = 0; i < length; i++)
                {
                    board[current_position.x, current_position.y].state = States.blank;
                    current_position -= direction_vector;
                }
                board[current_position.x, current_position.y].state = States.blank;

                return true;
            }
        }

        //unsetting the bridges
        for (int i = 0; i < length; i++)
        {
            board[current_position.x, current_position.y].state = States.blank;
            current_position -= direction_vector;
        }
        board[current_position.x, current_position.y].state = States.blank;

        return false;
    }


    public void SetBridges(int x, int y, Vector2Int direction_vector, int length, int upcoming_bridge_number) //f7
    {
        board[x, y].current_value += upcoming_bridge_number;

        Vector2Int current_position = new Vector2Int(x, y) + direction_vector; //position of the first bridge
        for (int i = 0; i < length; i++)
        {
            board[current_position.x, current_position.y].MakeBridge((direction_vector.x != 0), (upcoming_bridge_number == 2));
            current_position += direction_vector;
        }
        
        //after this loop, the current_position is equal to the position of the new island

        //setting the new island and adding it to the top of "open"
        if(!FindPossibleDirections(current_position.x, current_position.y))
        {
            board[current_position.x, current_position.y].MakeIsland(upcoming_bridge_number, upcoming_bridge_number);
        }
        else
        {
            int max_value = NumberOfAvailableDirections(current_position.x, current_position.y) * 2 + upcoming_bridge_number;
            int random_value = Random.Range(upcoming_bridge_number, max_value + 1);
            board[current_position.x, current_position.y].MakeIsland(random_value, upcoming_bridge_number);

        }
        open.Insert(0, current_position);
        Debug.Log("Inserted: " + current_position);
        Debug.Log("New Open Length = " + open.Count);
    }



    public string CreateCode()
    //creating a code that can be decodable and turned into a board, code that contains the positions and values of the islands
    //similar to chess board notations, FEN (Forsyth-Edwards Notation)
    //example -> chess starting position: rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR
    {
        if (is_generated)
        {
            return "";
        }
        return "";
    }

}
