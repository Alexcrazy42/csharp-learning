using System;

namespace CompanyA;

public class Phone
{
    public void Dial()
    {
        Console.WriteLine("Phone.Dial");
        EstablishConnection();
        // действия по набору телефонного номера
    }


    protected virtual void EstablishConnection()
    {
        Console.WriteLine("Phone.EstablishConnection");
        // действия по установлению соединения
    }
}
