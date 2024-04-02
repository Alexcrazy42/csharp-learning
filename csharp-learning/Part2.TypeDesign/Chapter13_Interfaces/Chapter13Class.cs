namespace csharp_learning.Part2.TypeDesign.Chapter13_Interfaces;

public class Chapter13Class
{
    public void Execute()
    {
        Point[] points = new Point[]
        {
            new Point(3, 3),
            new Point(1, 2)
        };
        bool firstBiggerThenTwo = points[0].CompareTo(points[1]) == 1 ? true : false;
        Console.WriteLine($"Больше точка: {(firstBiggerThenTwo ? points[0].ToString() : points[1].ToString())}");

        Console.WriteLine("------------------------------------------------");
        Console.WriteLine("Наследование интерфейсов");
        Base b = new Base();

        // вызов реализации Dispose в типе Base
        b.Dispose();
        // Вызов реализации Dispose в типе объекта Base
        ((IDisposable)b).Dispose();

        Derived d = new();

        // вызов реализации Dispose в типе Derived
        d.Dispose();
        // вызов реализации Dispose в типе объекта Derived
        ((IDisposable)d).Dispose();

        b = new Derived();

        // вызов реализации Dispose в типе Base
        b.Dispose();

        // вызов реализации Dispose в типе объекта Derived
        ((IDisposable)b).Dispose();

        Console.WriteLine("---------------------------------------");
        var st = new SimpleType();
        st.Dispose();
        ((IDisposable)st).Dispose();

        Console.WriteLine("--------------------------------------");
        int x = 1, y = 2;
        IComparable c = x;

        // CompareTop ожидает Object, но вполне допустимо передать переменную типа Int32, 
        // предварительно упаковав
        c.CompareTo(y);

        // CompareTo ожидает Object
        // при передаче "2" (String) компиляция выполняется нормально,
        // но во время выполнения вбрасывается ArgumentException
        // c.CompareTo("2");

        // предпочтительнее обеспечить более строгий контроль типов в интерфейсном методе, поэтому FCL содержит обобщенный 
        // интерфейс IComparable<T>.
        IComparable<int> c1 = x;

        c1.CompareTo(y);

        // компиляция не проходит
        // c1.CompareTo("123");

        Pizzeria p = new();
        p.GetMenu(); // вызов открытогго метода GetMenu, который не имеет отношения к интерфейсам

        (p as IWindow).GetMenu(); // без упаковки вызов метода IWindow.GetMenu
        (p as IRest).GetMenu(); // без упаковки вызов метода IRest.GetMenu

        Console.WriteLine("----------------------------------");

        SomeValueType v = new(0);
        object o = new();

        int n = v.CompareTo(v); // нежелательная упаковка
                                  // n = v.CompareTo(o); // исключение InvalidCastException в рантайм для структуры без EIMI
                                  // для структуры с IEMI - ошибка компиляции

        int x1 = 5;
        // Single s = x.ToSingle(null); // попытка вызвать метод интерфейса IConvertible

        float s = ((IConvertible)x).ToSingle(null); // приведение Int32 к IConvertible, при этом происходит упаковка

        Derived1 d1 = new();

        object o1 = new();

        d1.CompareTo(o1);


    }
}


public sealed class Point : IComparable<Point>
{
    private int x, y;

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    // этот методе реализует IComparable<T> в Point
    public int CompareTo(Point other)
    {
        return Math.Sign(Math.Sqrt(x * x + y * y) - Math.Sqrt(other.x * other.x + other.y * other.y));
    }

    public override string ToString()
    {
        return $"Точка: ({x};{y})";
    }
}

public class Base : IDisposable
{
    public void Dispose()
    {
        Console.WriteLine("Base's Dispose");
    }
}

public class Derived : Base, IDisposable
{
    // этот метод не может переопределить базовый метод Dispose
    // ключевое слово new указывает на то, что этот метод
    // повторно реализует метод Dispose интерфейса IDisposable
    new public void Dispose()
    {
        Console.WriteLine("Derived'd Dispose");

        // как вызвать реализацию базового класса (если нужно)
        //base.Dispose();
    }
}

public sealed class SimpleType : IDisposable
{
    public void Dispose() { Console.WriteLine("public Dispose"); }

    void IDisposable.Dispose() { Console.WriteLine("IDisposable.Dispose"); }
}

public interface IWindow
{
    void GetMenu();
}

public interface IRest
{
    void GetMenu();
}

public class Pizzeria : IWindow, IRest
{
    public void GetMenu()
    {
        Console.WriteLine("Get menu from instanse");
    }

    void IWindow.GetMenu()
    {
        Console.WriteLine("Get menu from IWindow");
    }

    void IRest.GetMenu()
    {
        Console.WriteLine("Get menu from IRest");
    }
}


// вариант без EIMI

//public struct SomeValueType : IComparable
//{
//    private Int32 x;
//    public SomeValueType(Int32 x)
//    {
//        this.x = x;
//    }

//    public Int32 CompareTo(Object other)
//    {
//        return x - ((SomeValueType)other).x;
//    }
//}

// вариант с EIMI
public struct SomeValueType : IComparable
{
    private int x;
    public SomeValueType(int x)
    {
        this.x = x;
    }

    public int CompareTo(SomeValueType other)
    {
        return x - other.x;
    }

    // Примечание: в следующей строке не используется public/private
    int IComparable.CompareTo(object other)
    {
        return CompareTo((SomeValueType)other);
    }
}


public class Base1 : IComparable
{
    // явная реализация интерфейсного метода (EIMI)
    int IComparable.CompareTo(object? o)
    {
        Console.WriteLine("Base's CompareTo");
        return 0;
    }

    // лучший способ - в дополнение к явно реализованному интерфейсному методу создать
    // в базовом классе виртуальный метод, который будет реализовываться явно, а затем в классе наследника 
    // переопределить этот виртуальный метод
    public virtual int CompareTo(object o)
    {
        Console.WriteLine("Base's virtual CompareTo");

        return 0;
    }
}

public class Derived1 : Base1, IComparable
{
    public override int CompareTo(object o)
    {
        Console.WriteLine("Derived's CompareTo");

        // эта попытка вызвать EIMI базового класса приводит к ошибке:
        // CS0117: Base1 does not contain a definition for CompareTo
        // проблема заключается в том, что в классе Base1 нет открытого или защищенного
        // метода CompareTo, который он мог бы вызвать
        // base.CompareTo(o);

        // здесь this приводится к типу IComparable, а затем вызывается метод CompareTo
        // Однако открытый метод класса Derived1 является реализацией метода CompareTo интерфейса IComparable
        // класса Derived1, поэтому возникает бесконечная рекурсия
        // IComparable c = this;
        // c.CompareTo(o);


        return base.CompareTo(o);
    }
}