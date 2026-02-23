using UnityEngine;

public class Money : MonoBehaviour
{
    public int moneyAmount = 1000;

    public static Money instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void AddMoney(int amount)
    {
        moneyAmount += amount;
        Debug.Log("Money added: " + amount + ". Total money: " + moneyAmount);
    }
    public bool CheckMoney(int check)
    {
        int reamaning = moneyAmount -= check;

        if (reamaning >= 0)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
   public void RemoveMoney(int amount)
    {
        moneyAmount -= amount;
    }
}
