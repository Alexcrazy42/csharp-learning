using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace cscharp_learning.Chapter8;

public class Chapter8Class
{
    public void Execute()
    {
        SomeType st = new SomeType("Hello world");
        Rectangle rectangle = new Rectangle();
        SomeValType[] a = new SomeValType[10];
        a[0].x = 123;
        Console.WriteLine(a[0].x);


        Int32 iterations = 1_000_000_000;

        // обнаружив в коде класс со статическими полями, где имеет место
        // встроенная инициализация (BeforeFieldInit), csc создает в таблице
        // определений класса запись без такого флага. Логика создателей этого
        // алгоритма проста: статические поля должны быть инициализированы до 
        // обращения к ним, а явно заданный конструктор типа может содержать 
        // дополнительный код, способный делать определенную видимую работу, 
        // поэтому его нужно выполнять в заданное разработчиком время
        //PerfTest1(iterations); // тут тело метода происходит медленее
        //PerfTest2(iterations);

        Complex c1 = new Complex()
        {
            x = 1,
            y = 2
        };
        Complex c2 = new Complex()
        {
            x = 2, 
            y = 3
        };
        Complex c3 = c1 + c2;
        c1 = c3 * 3;

        Rational r = 5; // неявное приведение Int32 к Rational
        Int32 x = (Int32)r; // явное приведение Rational к Int32, 
                            // интересно то, что компилятор не подсвечивает ситуации
                            // когда мы пытаемся неявно сделать приведение
                            // а в типе задано только явное приведение

        
        StringBuilder sb = new("Hello world");
        // сначала компилятор проверит StringBuilder и все его базовые классы, пытаяся найти методы
        // c названием IndexOf и единственным параметром char. Если таковые существуют, csc сгенерирует IL-код
        // если же не существует, csc будет искать любой статический класс с определенным методом indexOf, у которого первый параметр соответсвует типу выражения, используемого при вызове метода
        // этот тип должен быть помечен при помощи ключевого слова this. В данном примере выражением является sb типа StringBuilder
        // в этом случае компилятор ищет метод indexOf с двумя параметрами: StringBuilder отмеченный словом this и Char
        Console.WriteLine(sb.IndexOf('l')); // 2

        sb = null;
        // sb.IndexOf('a'); // ошибки компилятора нет
        // sb.Replace('1', '2'); // тоже нет ошибки компилятора


        // создание делегата Action, ссылающегося на статический метод расширения
        // ShowItems, и его первый аргумент инициализируется ссылкой на строку "Jeff"
        // После создания делегата конструктор передается в вызываемый метод, также передается
        // ссылка на объект, который должен быть передан в этот метод в качестве скрытого параметра
        // Обычно, когда мы создаем делегат, ссылающейся на статический метод, объектная ссылка
        // равна null, потому что статический метод не имеет этого параметра.
        Action action = "Jeff".ShowItems;

        // вызов делегата, вызывающего ShowItems и передающего ссылку на строку "Jeff"
        action();

        Base1 base1 = new();
        base1.Name = "123";
    }

    // при jit компиляции этого метода конструкторы типов для классов 
    // BeforeFieldInit и Precise еще не выполнены, поэтому вызовы этих конструкторов
    // встроены в код метода
    // что снижает эффективность программы
    private static void PerfTest1(Int32 iterations)
    {
        Stopwatch sw = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            // JIT компилятор создает код вызова конструктора типа
            // BeforeFieldInit, чтобы он выполнился до начала цикла
            BeforeFieldInit.x = 1;
        }
        Console.WriteLine($"PerfTest1: {sw.Elapsed} BeforeFieldInit");

        sw = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            // JIT-компилятора создает кода вызова конструктора типа Precise, 
            // чтобы тот проверил, нужно ли вызывать конструктор в каждом цикле
            Precise.x = 1;
        }
        Console.WriteLine($"PerfTest1: {sw.Elapsed} Precise");
    }

    // При JIT компиляции этого метода конструкторы типов для классов BeforeFieldInit
    // и Precise уже завершили работу, поэтому вызовы этих конструкторов
    // не встраиваются в коде метода, благодаря чему он исполняется быстрее
    private static void PerfTest2(Int32 iterations)
    {
        Stopwatch sw = Stopwatch.StartNew();

        for (int i = 0; i < iterations; i++)
        {
            BeforeFieldInit.x = 1;
        }
        Console.WriteLine($"PerfTest2: {sw.Elapsed} BeforeFieldInit");

        sw = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            Precise.x = 1;
        }
        Console.WriteLine($"PerfTest2: {sw.Elapsed} Precise");
    }
}

public sealed class SomeType
{
    // csc предлагает простой синтаксис, позволяющий инициализировать поля по время создания объекта ссылочного типа
    // при создании объекта SomeType его поле x инициализируется значением 5
    private Int32 x;
    private String s;
    private Double d;
    private Byte b;

    // код этого конструтоар инициализирует поля значениями по умолчанию
    // этот конструктор должен вызываваться всеми остальными конструкторами
    public SomeType()
    {
        x = 5;
        s = "Hi where";
        d = 3.14159;
        b = 0xff;
    }

    // этот конструктор инициализирует поля значениями по умолчанию, 
    // а затем изменяет значения this.x
    public SomeType(Int32 x) : this()
    {
        this.x = x;
    }

    // этот конструтор инициализрует поля по умолчанию 
    // а затем изменяет значение this.s
    public SomeType(String s) : base()
    {
        this.s = s;
    }

    // этот конструтор инициализрует поля по умолчанию 
    // а затем изменяет значение this.s
    public SomeType(Int32 x, String s) : this()
    {
        this.x = x;
        this.s = s;
    }
}

// 
public struct Point
{
    public Int32 x, y;

    // конструктор выполняетася лишь при наличии кода, явно  вызывающего этот конструктор
    // поэтому в типе Rectangle csc не будет автоматически вызывать этот конструктор, это сделано для повышения быстродействия
    // у многих языков есть такое поведение, что для value type не будет вызываться неявно конструтор без параметров, даже если он задан в типе. Его надо вызывать явно
    // в книге сказано, что нельзя создавать конструкторы без параметров у значимого типа, но это добавили в C# 7.2
    public Point()
    {
        this.x = 5;
        //this.y = 5;
    }
}

// чтобы создать этот тип, надо использовать оператор new, указав конструктор, будет вызван конструтор, сгенерированный csc
// из соображений производительности, CLR не пытается вызвать конструктор для каждого экземпляра значимого типа, содержащегося в объекте ссылочного типа. 
// однако, как отмечалось ранее, поля значимого типа инициализируются нулевыми или пустыми значениями
public sealed class Rectangle
{
    public Point topLeft, bottomRight;

    // в csc оператор new, использованный для создания value type, вызывает конструтор для инициализации полей значимого типа

    public Rectangle()
    {
        this.topLeft = new Point();
    }
}

public sealed class SomeRefType
{
    static SomeRefType()
    {
        // используется при первом обращении к ссылочному типу SomeRefType
    }
}

internal struct SomeValType
{
    // csc на самом деле допускает определять для значимых типов конструкторы без параметров
    // Есть определенные особенности вызова конструктора типа:
    // При компиляции метода JIT-компилятор обнаруживает типы, на которые есть ссылки из кода
    // Если в каком-либо из типов определен ctor, JIT-компилятор проверяет, был ли исполнен ctor типа в данном домене приложений
    // Если нет, JIT-компилятор создает в IL-коде вызов конструктора типа
    // Если же код уже исполнялся, JIT-компилятор вызова конструктора типа не создает, так как "знает", что тип уже инициализирован
    // Если несколько потоков одновременно попытаются вызывать конструктор типа, только один получит такую возможность, остальные блокируются. Это назвается возможность взаимоисключающего запирания
    static SomeValType()
    {
        // исполняется при первом обращении к значимому типу SomeValType
        Console.WriteLine("This never gets displayed");
    }

    public Int32 x;
}

// так как в этом классе конструктор типа не задан явно, 
// C# отмечает определение типа в метаданных ключевым словом BeforeFieldInit
public sealed class BeforeFieldInit
{
    public static Int32 x = 123;
}

// так как в этом классе конструктор типа задан явно
// C# не отмечает определение типа ключевым словом BeforeFieldInit
public sealed class Precise
{
    public static Int32 x;

    static Precise() { x = 123; }
}

public sealed class Complex
{
    public Int32 x;
    public Int32 y;

    // csc генерирует определение метода op_Addition и устанавливает в записи с определением этого метода флаг specialname
    // когда компилятор видит в исходном коде оператор +, он исследует типы его операндов. При этом компилятор пытается выяснить, 
    // не определен ли для одного из них оператор op_Addition с флагом specialname, параметры которого
    // совместимы с типами операндов. Если такой код существует, компилятор генерирует код, вызывающий этот метод, иначе возникает ошибка компилятора

    public static Complex operator +(Complex x, Complex y)
    {
        return new Complex()
        {
            x = x.x + y.x,
            y = x.y + y.y
        };
    }

    public static Complex operator *(Complex x, Int32 y)
    {
        return new Complex()
        {
            x = x.x * y,
            y = x.y * y
        };
    }
}

public sealed class Rational
{
    public Int32 x;

    public Rational(Int32 num)
    {
        this.x = num;
    }

    public Rational(Double d)
    {
        this.x = (Int32)d;
    }

    public Int32 ToInt32()
    {
        return x;
    }

    public Double ToDouble()
    {
        return x;
    }

    // явно возвращает объект типа Rational из Int32, возвращает полученный объект
    // при определении метода для операторов преобразования следует указать, должен ли компилятор
    // генерировать код для их неявного вызова автоматические или лишь при наличии явного указания в исходном коде
    // ключевое слово implicit указывает csc, что наличие в исходном коде явного приведения не обязательно для генерации кода,
    // вызывающего метод оператора преобразования.
    // Ключевое слово explicit позволяет компилятору вызывать методе только тогда, когда в исходном тексте происходит явное приведение типов
    public static implicit operator Rational(Int32 num)
    {
        return new Rational(num);
    }

    // явно возвращает объекта типа Int32, полученный из Rational
    public static implicit operator Int32(Rational r)
    {
        return r.ToInt32();
    }


    // явно возвращает объект типа Double, полученный из Rational
    public static explicit operator Double(Rational r)
    {
        return r.ToDouble();
    }
}

public static class StringBuilderExtension
{
    public static Int32 IndexOf(this StringBuilder sb, Char value)
    {
        for (int index = 0; index < sb.Length; index++)
        {
            if (sb[index] == value)
            {
                return index;
            }
        }
        return -1;
    }
}

public static class EnumerableExtension
{
    public static void ShowItems<T>(this IEnumerable<T> collection)
    {
        foreach(var item in collection)
        {
            Console.WriteLine(item);
        }
    }
}

public class Base
{
    private String name;

    protected virtual void OnNameChanging(String newName)
    {

    }

    public String Name
    {
        get { return name; }
        set
        {
            OnNameChanging(value);
            name = value;
        }
    }
}


// к сожалению для представленного кода характерны две проблемы:
// 1) Тип не должен быть изолированным. Нельзя использовать этот подход для изолированных классов или для значимых типов
// (потому что значимые типы неявно изолированы). К тому же нельзя использовать этот подход для статических методов, потому что они не могут быть переопделены
// 2) Существует проблема эффективности. Тип, будучи переопределенным, переопределяет метод, что отнимает некоторое количество системных ресурсов. 
public sealed class Derived : Base
{
    protected override void OnNameChanging(string newName)
    {
        if (String.IsNullOrEmpty(newName))
        {
            throw new ArgumentNullException("value");
        }
    }
}

// сгенерированный при помощи инструмента программный еод
public sealed partial class Base1
{
    private String name;

    // Это объявление с определением частичного метода вызывается 
    // перед изменением поля name
    partial void OnNameChanging(String newName);

    public String Name
    {
        get { return name; }
        set
        {
            // информирование класса о потенциальном изменении
            OnNameChanging(value);
            name = value;
        }
    }
}

// написанный программистом код, содержащийся в другом файле
public sealed partial class Base1
{
    partial void OnNameChanging(String newName)
    {
        Console.WriteLine("Base1.OnNameChanging1");
    }
}