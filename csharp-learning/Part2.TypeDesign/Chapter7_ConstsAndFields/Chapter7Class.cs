namespace csharp_learning.Part2.TypeDesign.Chapter7_ConstsAndFields;

public class Chapter7Class
{
    public void Execute()
    {
        SomeType.numberOfWrites = 1; // изменит static поле типа

        // SomeType.random = new Random(); // присваивание значения доступному только для чтения 
        // статическому полю допускается только в статическом конструкторе и в инициализаторе переменных

        // ok
        AType.InvalidChars[0] = 'X';
        AType.InvalidChars[1] = 'Y';
        AType.InvalidChars[2] = 'Z';

        // присваивание значения доступному только для чтения 
        // статическому полю допускается только в статическом конструкторе и в инициализаторе переменных
        // не компилируется, т.к. InvalidChars ссылается на неизменяемое
        //AType.InvalidChars = new Char[] { 'X', 'Y', 'Z' };
    }
}

public sealed class SomeLibraryType
{
    // модификатор static необходим, чтобы ассоциировать поле с его типом
    public static readonly int MaxEntriesInList = 50;
}

public sealed class SomeType
{
    // это статическое неизменяемое поле. Его значение рассчиывается 
    // и сохраняется в памяти при инициализации класса во время выполнения
    public static readonly Random random = new Random();

    // это статическое изменяемое поле
    public static int numberOfWrites = 0;

    // неизменяемое экземплярное поле
    public readonly string PathName = "pathName";

    // изменяемое экземплярное поле
    public FileStream fs;

    public SomeType(string pathName)
    {
        // изменяется значение неизменяемого поля
        // в данном случае это возможно, т.к. это происходит в конструкторе
        PathName = pathName;
    }

    public string DoSomething()
    {
        // эта строка читает и записывает значение статического изменяемого типа
        numberOfWrites += 1;
        // читает значение неизменяемого экземплярного поля
        return PathName;
    }
}

public sealed class AType
{
    public static readonly char[] InvalidChars = new char[] { 'A', 'B', 'C' };
}