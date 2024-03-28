using System.Collections;

namespace csharp_learning.Chapter5;

public class Chapter5Class
{
    public void Execute()
    {
        // явное и неявное приведение между примитными числами
        Int32 i = 100;
        Int64 l = i;
        Byte b = (Byte)i;
        Int16 v = (Int16)i;

        // литералы
        Boolean found = false;
        Int32 x = 100 + 23; // компилятор видит: Int32 = 123;
        String s = "a" + "bcd"; // String s = "abcd";

        // overflow
        // b и 200 сначала преобразуются в 32 или 64 разрядные операнды, посколько 
        // все арифметичные операции в clr выполняются только над 32и 64 разрядными числами
        b = (Byte)(b + 200); // по умолчанию unchecked операция
        //b = checked ((Byte)(b + 300)); // OverflowException
        b = (Byte)checked(b + 200); // ошибки не будет, т.к. приведение к byte вывели из оператора checked

        checked
        {
            b = 100;
            //b += 200; // OverflowException
            SomeMethod(400); // ошибки не будет, т.к. оператор checked не влияет на работу тела метода
        }

        // value and reference type
        SomeRef r1 = new SomeRef();
        SomeVal v1 = new SomeVal();
        r1.x = 5; // разыменование указателя
        v1.x = 5; // изменение в стеке

        SomeRef r2 = r1; // копируется ссылка на r1 (указатель)
        SomeVal v2 = v1;
        r1.x = 6; // изменится объекта в heap, и все кто на него ссылаются, увидят изменения
        v1.x = 6; // изменится только v1 в стеке

        // boxing/unboxing
        ArrayList arrayList = new ArrayList();
        SomeVal point; // выделяется память под SomeVal в стеке
        for(int item = 0; item < 10; item++)
        {
            point.x = point.y = item; // инициализация членов в значимом типе
            arrayList.Add(point); // упаковка значимого типа и добавление ссылки в ArrayList

            // метод Add в качестве параметра нужен тип Object, то есть ссылка (указатель) на объект в управляемой куче
            // однако передается значимый тип. Чтобы код работал, нужно преобразовать значимый тип SameVal
            // в объект из heap и получить на него ссылку. 
            // Для преобразования значимого типа в ссылочный служит упаковка (boxing) (часть 8 README.md)
        }

        SomeVal p = (SomeVal)arrayList[0]; // распаковка (часть 9 README.md)

        Int32 x1 = 5;
        Object o = x;
        //Int16 x2 = (Int16)o; // InvalidCastException
        Int16 x3 = (Int16)(Int32)x1; // распаковка затем приведение типа

        SomeVal p1;
        p1.x = p1.y = 3;
        Object o1 = p1;
        p1.x = 2; // изменяется поле значимого типа p1 в стеке, никак не влияет на объект из heap
        o1 = p1; // упаковка, создающая новый объект в heap

        SomeVal p2 = new SomeVal();
        p2.x = p2.y = 1;

        Console.WriteLine(p1.ToString()); // p1 не пакуется для вызова ToString (виртуальный метод)
        Console.WriteLine(p1.GetType()); // p1 пакуется для вызова GetType (невиртуальный метод)
        Console.WriteLine(p1.CompareTo(p2)); // p1 не пакуется при вызове CompareTo, p2 не пакуется, потому что вызван CompareTo(SomeVal)

        // p1 пакуется, а ссылка размещается в c, т.к. интерфейсы по определению имеют ссылочный тип
        IComparable c = p1;
        Console.WriteLine(c.GetType()); // SomeVal, который размещен в куче

        // p1 не пакуется для вызова CompareTo
        // Посколько в CompareTo не передается переменная SomeVal
        // вызывается CompareTo(Object), которому нужна ссылка на упакованный SomeVal
        // c не пакуется, посколько уже ссылается на упакованный SomeVal
        Console.WriteLine(p1.CompareTo(c)); // 0

        // c не пакуется, потому что уже ссылается на упакованный SomeVal
        // p2 пакуется, потому что вызывается CompareTo(Object)
        Console.WriteLine(c.CompareTo(p2));

        // c распаковывается в p2
        p2 = (SomeVal)c;
        // убеждаемся, что поля скопированы в p2
        Console.WriteLine(p2.ToString());

        // изменение полей в упакорванных значимых типах посредством интерфейсов
        Point p3 = new Point(1, 1);

        Console.WriteLine(p3);
        p3.Change(2, 2);
        Console.WriteLine(p3);

        Object o3 = p3;
        Console.WriteLine(o3);

        ((Point)o3).Change(3, 3); // при таком приведении o3 распаковывается и поля упакованного объекта типа Point
                                  // копируются во временный объект типа Point
                                  // поля этого временного объекта становятся равными 3, но на упакованный значимый тип Point в object o
                                  // это никак не влияет
        Console.WriteLine(o3); // останется 2 : 2

        ((IChangedBoxedPoint)p3).Change(4, 4); // p3 упаковывается в ref type IChangedBoxedPoint
                                               // метод Change вызывается для этого упакованного значения и его поля становятся равными 4
                                               // но при возврате из Change на упакованный объект в heap перестает сразу действовать ссылка
        Console.WriteLine(p3); // 2 : 2

        ((IChangedBoxedPoint)o3).Change(5, 5); // упакованный тип Point на которой ссылается o3 приводится к типу IChangedBoxedPoint
                                               // упаковка не производится, т.к. тип o3 уже упакован
                                               // метод Change меняет поля упакованного типа на который ссылается o3, и они становятся равны 5
                                               // метод интерфейса может изменить поля в упакованном значимом типе. в C# сделать без интерфейса это нельзя
        Console.WriteLine(o3); // 5 : 5

        dynamic a = 5;
        // Print(a); // InvalidCastException в runtime, когда CLR проверит правильность приведения типов

    }

    public void SomeMethod(Int32 i)
    {
        Byte b = (Byte)i;
    }

    public class SomeRef
    {
        public Int32 x;
    }

    public struct SomeVal : IComparable
    {
        public Int32 x;
        public Int32 y;

        public override string ToString()
        {
            return $"({x} : {y})";
        }

        public Int32 CompareTo(SomeVal other)
        {
            return Math.Sign(Math.Sqrt(x*x + y*y) - Math.Sqrt(other.x * other.x + other.y * other.y));
        }

        public Int32 CompareTo(Object o)
        {
            if(GetType() != o.GetType())
            {
                throw new ArgumentException("o in not a SomeVal");
            }
            return CompareTo((SomeVal)o);
        }
    }

    public interface IChangedBoxedPoint
    {
        void Change(Int32 x, Int32 y);
    }

    public struct Point : IChangedBoxedPoint
    {
        private Int32 x, y;

        public Point(Int32 x, Int32 y)
        {
            this.x = x;
            this.y = y;
        }

        public void Change(Int32 x, Int32 y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return $"{x} : {y}";
        }
    }
    
    public void Print(string str)
    {
        Console.WriteLine(str);
    }
}
