using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csharp_learning.Part2.TypeDesign.Chapter10_Properties;

public class Chapter10Class
{
    public void Execute()
    {
        Employee e = new Employee();
        e.Name = "Name";
        e.Age = 41;
        e.Age = -5; // следствие нарушения принципа инкапсуляции

        EmployeeWithProperies ewp = new EmployeeWithProperies("Name", 19);
        Console.WriteLine(ewp.Name);
        // ewp.Age = -1; // ArgumentOutOfRangeException
        //ewp.get_Age() // CS0571: явный вызов оператора или метода невозможен

        EmployeeWithAip ewAip = new();
        ewAip.Age = 5;
        ewAip.Name = "Name2";

        Console.WriteLine("---------------- Анонимные типы -------------------");
        var o1 = new { Name = "Jeff", Year = 1964 };
        Console.WriteLine(o1.Name);
        Console.WriteLine(o1.Year);

        string Name = "Grant";
        DateTime dt = DateTime.Now;

        // анонимный тип с двумя свойствами
        // 1. Строковому свойству Name назначено значение Grant
        // 2. Свойству Year типа Int32 Year назначен год внутри dt
        // компилятор определяет, что первое свойствое должно называться Name
        // так как Name - имя локальной переменной, то компилятор устанавливает значение типа свойства аналогично типу локальной переменной, то есть String
        // Для второго свойства компилятор использует имя поля/свойства: Year, типа Int32
        var o2 = new { Name, dt.Year };

        // выделить массив BitArray, которые может хранить 14 бит
        BitArray ba = new BitArray(14);

        for (int x = 0; x < 14; x++)
        {
            Console.WriteLine($"Bit {x} is {(ba[x] ? "On" : "Off")}");
        }


    }
}

// определение типа с помощью полей
class Employee
{
    public string Name;

    public int Age;
}

// при компиляции этого типа csc обнаружит свойства Name и Age
// поскольку у обоих есть методы-аксессоры get и set, csc генерирует в типе четеры определения методов:
// get_Name, set_Name, get_Age, set_Age, такие методы нельзя создать
// csc автоматически генерирует имена этих методов, прибавляя приставки get_ и set_ к имени свойства.
public class EmployeeWithProperies
{
    private string name;
    private int age;

    public string Name
    {
        get { return name; }
        set { name = value; } // ключевое слово value индентифицирует новое значение
    }

    public int Age
    {
        get { return age; }
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException($"{value}", "Возвраст должно быть больше 0");
            }
            age = value;
        }
    }

    public EmployeeWithProperies(string name, int age)
    {
        Name = name;
        Age = age;
    }
}

public sealed class EmployeeWithAip
{
    public string Name { get; set; } // csc автоматически создаст нужные аксессоры

    private int age;

    public int Age
    {
        get { return age; }
        set
        {
            if (value < 0) throw new ArgumentOutOfRangeException("value", "Возвраст должен быть больше 0!");
            age = value;
        }
    }
}

public class SomeType
{
    private static string Name
    {
        get
        { return null; }
        set { }
    }

    public static void MethodWithOutParam(out string n) { n = null; }

    public static void M()
    {
        // компилятор вернет сообщение об ошибке:
        // CS0206: свойство или индексатор не могут передаваться как 
        //MethodWithOutParam(out Name);
    }
}

public class BitArray
{
    private byte[] byteArray;
    private int numBits;

    // конструктор, выделяющий память для байтового массива и устанавливающий все биты в 0
    public BitArray(int numBits)
    {
        if (numBits <= 0)
        {
            throw new ArgumentOutOfRangeException("numBits", $"numBits must be > 0");
        }

        this.numBits = numBits;
        byteArray = new byte[(numBits + 7) / 8];
    }

    // Это индексатор (свойство с параметрами)
    public bool this[int bitPos]
    {
        get
        {
            if (bitPos < 0 || bitPos >= numBits)
            {
                throw new ArgumentOutOfRangeException("bitPos");
            }
            return (byteArray[bitPos / 8] & 1 << bitPos % 8) != 0;
        }
        set
        {
            if (bitPos < 0 || bitPos >= numBits)
            {
                throw new ArgumentOutOfRangeException("bitPos");
            }
            if (value)
            {
                byteArray[bitPos / 8] = (byte)(byteArray[bitPos / 8] | 1 << bitPos % 8);
            }
            else
            {
                byteArray[bitPos / 8] = (byte)(byteArray[bitPos / 8] & ~(1 << bitPos % 8));
            }
        }
    }
}