using System;

[Serializable]
public class Ingredient
{
    public enum EUnit { Spoon, Cup, Bowl, Piece }

    public string name;
    public int amount = 1;
    public EUnit unit;
}
