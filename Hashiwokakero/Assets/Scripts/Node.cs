using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class States
{
    public static string blank = "blank";
    public static string bridge = "bridge";
    public static string island = "island";
}

public static class Dirs
{
    public static string up = "up";
    public static string right = "right";
    public static string down = "down";
    public static string left = "left";

    public static Dictionary<string, Vector2Int> dir_to_vector = new Dictionary<string, Vector2Int>()
    {
        {"up", new Vector2Int(0, -1)},
        {"right", new Vector2Int(1, 0)},
        {"down", new Vector2Int(0, 1)},
        {"left", new Vector2Int(-1, 0)}
    };
}

public class Directions
{
    public bool up = true;
    public bool right = true;
    public bool down = true;
    public bool left = true;

    public bool IsAllFalse()
    {
        return (!up && !right && !down && !left);
    }
    public bool[] AllDirections()
    {
        return new bool[]{ up, right, down, left};
    }
}


public struct Node
{

    public int x;
    public int y;
    public string state; //bridge || island || blank
    
    //island variables
    public int value; //if it's an island, the value is the number of bridges the island must contain
    public int current_value;
    public Directions directions;

    //bridge variables
    public bool is_double;
    public bool is_horizontal;

    public Node(int x, int y)
    {
        this.x = x;
        this.y = y;
        state = States.blank;

        value = 0;
        current_value = 0;
        directions = new Directions();

        is_double = false;
        is_horizontal = false;
    }

    public void MakeIsland(int value, int current_value)
    {
        this.value = value;
        this.current_value = current_value;
        state = States.island;
    }

    public void MakeBridge(bool is_double, bool is_horizontal)
    {
        this.is_double = is_double;
        this.is_horizontal = is_horizontal;
        state = States.bridge;
    }

    public void IncreaseCurrentValue(int amount)
    {
        current_value += amount;
    }
}
