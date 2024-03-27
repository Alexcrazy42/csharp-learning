using CompanyA;

namespace CompanyB;

internal class BetterPhone : Phone
{
    //// здесь добавлено ключевое слово new, чтобы указать, что этот метод не связан с методом Dial базового типа
    //public new void Dial()
    //{
    //    Console.WriteLine("BetterPhone.Dial");
    //    EstablishConnection();
    //    base.Dial();
    //}

    // здесь добавлено ключевое слово new, чтобы указать, что этот метод не связан с методом EstablishConnection базового типа
    // без ключевого слова new разработчики BetterPhone компилятор бы вы
    protected override void EstablishConnection()
    {
        Console.WriteLine("BetterPhone.EstablishConnection");
        // выполнить действия по набору номера
    }
}
