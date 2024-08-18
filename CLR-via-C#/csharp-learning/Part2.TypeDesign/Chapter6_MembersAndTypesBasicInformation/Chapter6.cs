namespace csharp_learning.Part2.TypeDesign.Chapter6_MembersAndTypesBasicInformation;

public partial class Chapter6Class
{
    public int GetFive() => 5;

    public void Execute()
    {
        Console.WriteLine(); // вызов статического метода, csc использует для его вызова метод call 

        //Employee emp = null; // ошибка в runtime
        //Employee emp; // ошибка компиляции, использование локальной переменной, которой не присвоено значение
        Employee emp = new(); // ok
        var a = emp.GenProgressReportQ(); // вызов виртуального экземплярного метода, call
        var b = emp.GetYearsEmployedQ(); // вызов невиртуального экземплярного метода, callvirt
                                         // выглядит странно, потому что метод невиртуальный
                                         // тем не менее код работает, потому что во время JIT-компляции CLR выясняет, что
                                         // это невиртуальный метод, и вызывает его невиртуально
                                         // почему не вызывается инструкция call?
                                         // так решили разработчики C# - JIT-компилятор должен создавать код проверки, не равен ли null вызывающий объект
                                         // поэтому вызовы невиртуальных экземплярных методов выполняются чуть медленее, чем могли бы
        var c = Employee.Lookup("name"); // вызов статисческого метода
        Console.WriteLine(a); // "123"
        Console.WriteLine(b); // 1
        Console.WriteLine(c.Name); // "name"

        Phone p = null;
        //Int32 x = p.GetFive(); // NullReferenceException
        // для компилятора все ок - для вызова невиртуального экземплярного метода GetFive среде CLR необходимо узнать
        // только тип p, а это Chapter6Class. При вызове GetFive аргумент this равен null, но в методе GetFive он не используется, 
        // поэтому исключения нет. Однако csc вместо инструкции call вставляет callvirt, поэтому код вызовет NullRefException

        Point p1 = new Point(3, 4);

        // csc вставить здесь инструкцию callvirt
        // но JIT-компилятор оптимизирует этот вызов и сгенерирует код
        // для невиртуального вызова ToString
        // посколько p имеет тип Point, являющийся изолированным
        Console.WriteLine(p1.ToString());

        var pInh = new NotSealedPointInheritor(4, 3);
        var name = pInh.PrintName();

        BetterPhone bPhone = new BetterPhone();
        bPhone.Dial();


    }

    // static заставляет компилятор C# сделать этот класс абстрактным (abstract)
    // и изолированным (sealed). Более того, компилятор не создает в классе метод конструктора экземплятов (.ctor)
    public static class AStaticClass
    {
        public static void AStaticMethodQ()
        {

        }

        public static string AStaticProperty
        {
            get { return s_AStaticField; }
            private set { s_AStaticField = value; }
        }

        private static string s_AStaticField;

        public static event EventHandler AStaticEvent;
    }

    // при компиляции этого кода компилятор помещает три записи в таблицу определений методов сборки
    // каждая запись содрежит флаги, указывающие, является ли метод экземплярным, виртуальным или статическим
    // при компиляции кода, ссылающегося на эти методы, компилятор проверяет флаги в определении методов, чтобы выяснить, какой 
    // IL-код нужно вставить для корректного вызова методов
    internal class Employee
    {
        public string Name { get; private set; }

        // невиртуальный экземплярный метод
        public int GetYearsEmployedQ()
        {
            return 1;
        }

        // виртуальный метод (виртуальный, значит, экземплярный)
        public virtual string GenProgressReportQ()
        {
            return "123";
        }

        // статический метод
        public static Employee Lookup(string name)
        {
            return new Employee()
            {
                Name = name
            };

        }
    }

    public class SomeClass
    {
        // ToString - виртуальный метод базового класса Object
        public override string ToString()
        {
            // компилятор использует команду call для невиртуального вызова 
            // метода ToString класса Object

            // если бы компилятор вместе call использовал callvirt, этот метода 
            // продолжал бы рекурсивно вызывать сам себя до переполнения стека
            return base.ToString();
        }
    }

    public class Set
    {
        private int length = 0;

        // этот перегруженный метод - невиртуальный
        public int Find(object value)
        {
            return Find(value, 0, length);
        }

        // этот перегруженный метод - невиртуальный
        public int Find(object value, int startIndex)
        {
            return Find(value, 0, length);
        }

        // наиболее функциональный метод сделан виртуальным
        public virtual int Find(object value, int startIndex, int endIndex)
        {
            // настоящая реализация, которую можно переопределить
            return startIndex - endIndex;
        }
    }

    public sealed class Point
    {
        private int x, y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return $"{x} : {y}";
        }
    }

    public class NotSealedPoint
    {
        private int x, y;

        public NotSealedPoint(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return $"{x} : {y}";
        }

        public virtual string PrintName()
        {
            return "NotSealedPoint";
        }
    }

    public class NotSealedPointInheritor : NotSealedPoint
    {
        public NotSealedPointInheritor(int x, int y) : base(x, y)
        { }

        public override string PrintName()
        {
            return "NotSealedPointInheritor";
        }
    }

    public class SimulatedClosedClass
    {
        public sealed override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public sealed override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public sealed override string ToString()
        {
            return base.ToString();
        }

        // к сожалению C# не разрешает изолировать метод Finalize
        // определите дополнительные открытые или закрытые члены ...

        // не определяйте защищенные или виртуальные методы
    }
}