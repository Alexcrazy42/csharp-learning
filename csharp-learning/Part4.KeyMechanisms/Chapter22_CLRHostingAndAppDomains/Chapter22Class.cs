using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Remoting;

namespace csharp_learning.Part4.KeyMechanisms.Chapter22_CLRHostingAndAppDomains;

public class Chapter22Class
{
    public void Execute()
    {
        // получаем ссылку на домен, в котором исполняется вызывающий поток
        AppDomain adCallingThreadDomain = Thread.GetDomain();

        // каждому домену присваивается значимое имя, облегчающее отладку
        String callingDomaingName = adCallingThreadDomain.FriendlyName;
        Console.WriteLine($"Default AppDomain's friendly name = {callingDomaingName}");

        String exeAssembly = Assembly.GetEntryAssembly().FullName;
        Console.WriteLine($"main assembly = {exeAssembly}");

        // определяем локальную переменную, ссылающуся на домен
        AppDomain ad2 = null;

        // ПРИМЕР 1. Доступ к объектам другого домена приложений
        // с продвижением по ссылке
        Console.WriteLine($"{Environment.NewLine} Demo#1");

        // создаем новый домен (с теми эе параметрами защиты и конфигурации)
        ad2 = AppDomain.CreateDomain("AD #2");
        MarshalByRefType mbrt = null;

        // Загружаем нашу сборку в новый домен, конструируем объект
        // и продвигаем его обратно в наш домен
        // (в действительности мы получаем ссылку на представитель)
        mbrt = (MarshalByRefType) 
            ad2.CreateInstanceAndUnwrap(exeAssembly, "MarshalByRefType");

        Console.WriteLine($"Type={mbrt.GetType()}"); // CLR неверно определяет тип


        // Все выглядит так, как будто мы вызываем метод экземпляра
        // MarshalByRefType, но на самом деле мы вызываем метод тип
        // представителя. Именно представитель переносит поток в тот домен,
        // в котором находится объект, и вызывает метод для реального объекта
        mbrt.SomeMethod();

        // Выгружаем новый домен
        AppDomain.Unload(ad2);
        // mbrt ссылается на правильный объект-представитель
        // объект-представитель ссылается на неправильный домен

        try
        {
            // вызываем метод, определенный в типе представителя
            // поскольку домен приложений неправильный, появляется исключчение
            mbrt.SomeMethod();
            Console.WriteLine("Successful call");
        }
        catch (AppDomainUnloadedException)
        {
            Console.WriteLine("Failed call.");
        }

        // ПРИМЕР 2. Доступ к объектам другого домена
        // с продвижением по значению
        Console.WriteLine($"{Environment.NewLine}Demo #2");

        // Создаем новый домен (с такиим же параметрами защиты
        // и конфигурирования, как в текущем)
        ad2 = AppDomain.CreateDomain("AD #2");

        // Загружаем нашу сборку в новый домен, конструируем объект
        // и продвигаем его обратно в наш домен
        // (в действительности мы получаем ссылку на представитель)
        mbrt = (MarshalByRefType) 
            ad2.CreateInstanceAndUnwrap(exeAssembly, "MarshalByRefType");

        // Метод возвращает КОПИЮ возвращенного объекта;
        // продвижение объекта происходило по значению, а не по ссылке
        MarshalByValType mbvt = mbrt.MethodWithReturn();

        // Кажется, что мы вызываем метод экземпляра MarshalByRefType,
        // и это на самом деле так
        Console.WriteLine("Returned object created " + mbvt.ToString());
        
        // Выгружаем новый домен
        AppDomain.Unload(ad2);
        // mbrt ссылается на действительный объект;
        // выгрузка домена не имеет никакого эффекта

        try
        {
            // Вызываем метод объекта; исключение не генерируется
            Console.WriteLine("Returned object created " + mbvt.ToString());
            Console.WriteLine("Successful call.");
        }
        catch (AppDomainUnloadedException)
        {
            Console.WriteLine("Failed call.");
        }

        // ПРИМЕР 3. Доступ к объектам другого домена
        // без использования механизма продвижения
        Console.WriteLine("{0}Demo #3", Environment.NewLine);

        // Создаем новый домен (с такими же параметрами защиты
        // и конфигурирования, как в текущем)
        ad2 = AppDomain.CreateDomain("AD #2");

        // Загружаем нашу сборку в новый домен, конструируем объект
        // и продвигаем его обратно в наш домен
        // (в действительности мы получаем ссылку на представитель)
        mbrt = (MarshalByRefType)
            ad2.CreateInstanceAndUnwrap(exeAssembly, "MarshalByRefType");

        // Метод возвращает объект, продвижение которого невозможно
        // Генерируется исключение
        NonMarshalableType nmt = mbrt.MethodArgAndReturn(callingDomaingName);
        // До выполнения этого кода дело не дойдет...
    }
}

// Экземпляры допускают продвижение по ссылке через границы доменов
public sealed class MarshalByRefType : MarshalByRefObject
{
    public MarshalByRefType()
    {
        Console.WriteLine($"{this.GetType()} ctor running in {Thread.GetDomain().FriendlyName}");
    }

    public void SomeMethod()
    {
        Console.WriteLine("Executing in " + Thread.GetDomain().FriendlyName);
    }

    public MarshalByValType MethodWithReturn()
    {
        Console.WriteLine("Executing in " + Thread.GetDomain().FriendlyName);
        var t = new MarshalByValType();
        return t;
    }

    public NonMarshalableType MethodArgAndReturn(String callingDomainName)
    {
        // ПРИМЕЧАНИЕ: callingDomainName имеет атрибут [Serializable]
        Console.WriteLine($"Calling from '{callingDomainName}' to '{Thread.GetDomain().FriendlyName}'");
        var t = new NonMarshalableType();
        return t;
    }
}

// Экземпляры допускают продвижение по значению через границы доменов
[Serializable]
public sealed class MarshalByValType : Object
{
    private DateTime creationTime = DateTime.Now;
    // ПРИМЕЧАНИЕ: DateTime помечен атрибутом [Serializable]

    public MarshalByValType()
    {
        Console.WriteLine($"{this.GetType()} ctor running in {Thread.GetDomain().FriendlyName}, Created on {creationTime:D}");
    }

    public override string ToString()
    {
        return creationTime.ToLongDateString();
    }
}

// Экземпляры не допускают продвижение между доменами
// [Serializable]
public sealed class NonMarshalableType : Object
{
    public NonMarshalableType()
    {
        Console.WriteLine("Executing in " + Thread.GetDomain().FriendlyName);
    }
}