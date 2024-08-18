namespace csharp_learning.Part3.BasicDataTypes.Chapter15_EnumTypesAndBitFlags;

public class Chapter15Class
{
    public void Execute()
    {
        Color c = Color.Blue;
        Console.WriteLine(Enum.GetUnderlyingType(typeof(Color))); // Int32
        Console.WriteLine(c); // "Blue" общий формат
        Console.WriteLine(c.ToString()); // "Blue" общий формат
        Console.WriteLine(c.ToString("G")); // "Blue" общий формат
        Console.WriteLine(c.ToString("D")); // "3" (десятичный формат)
        Console.WriteLine(c.ToString("X")); // "03" (шестнадцатеричный формат)

        Console.WriteLine("------------------------------");
        Color[] colors = (Color[])Enum.GetValues(typeof(Color));
        Console.WriteLine($"Number of symbols: {colors.Length}");
        Console.WriteLine("Value\tSymbol\n------\t-------");
        foreach (Color c1 in colors)
        {
            Console.WriteLine("{0,5:D}\t{0:G}", c1);
        }

        Console.WriteLine("----------------------------");
        Color c2 = (Color)Enum.Parse(typeof(Color), "orange", true);
        Console.WriteLine(c2);

        // Brown в Color не определен, но мы его пытаемся получить, будет исключение
        // ArgumentException
        // c2 = (Color)Enum.Parse(typeof(Color), "Brown", false);

        // создается экземпляр перечисления Color со значением 1
        Enum.TryParse<Color>("1", false, out c2);
        Console.WriteLine(c2);
        // создается экземпляр перечисления Color со значением 23
        Enum.TryParse<Color>("23", false, out c2);
        Console.WriteLine(c2);

        Console.WriteLine("---------------------------------");
        Console.WriteLine(Enum.IsDefined(typeof(Color), 1)); // true
        Console.WriteLine(Enum.IsDefined(typeof(Color), "White")); // true
        Console.WriteLine(Enum.IsDefined(typeof(Color), "white")); // false, тк проверка с учетом регистра производится
        Console.WriteLine(Enum.IsDefined(typeof(Color), 10)); // false, тк нет идентификатора 10

        Console.WriteLine("---------------------------------");
        Actions actions = Actions.Read | Actions.Delete;
        Console.WriteLine(actions.ToString());

        Console.WriteLine("--------------------------------");
        Actions a = (Actions)Enum.Parse(typeof(Actions), "Query", true);
        Console.WriteLine(a.ToString());

        Enum.TryParse<Actions>("Query, Read", false, out a);
        Console.WriteLine(a.ToString());

        Console.WriteLine("------------------------------");
        var fa = FileAttributes.System;
        fa = fa.Set(FileAttributes.ReadOnly);
        fa = fa.Set(FileAttributes.Normal);
        fa = fa.Clear(FileAttributes.System);
        fa.ForEach(f => Console.WriteLine(f));
    }
}

internal enum Color
{
    White,
    Red,
    Green,
    Blue, 
    Orange
}

internal enum Color1 : Int32 // раньше нельзя было так объявлять
{
    White, 
    Red
}

[Flags]
internal enum Actions
{
    None = 0,
    Read = 1,
    Write = 2,
    Delete = 4,
    Query = 8
}

internal static class FileAttributesExtension
{
    public static Boolean IsSet(this FileAttributes flags, FileAttributes flagToTest)
    {
        if (flagToTest == 0)
        {
            throw new ArgumentOutOfRangeException("Value must not be 0");
        }
        return !IsSet(flags, flagToTest);
    }

    public static Boolean IsClear(this FileAttributes flags, FileAttributes testFlags)
    {
        return ((flags & testFlags) != 0);
    }

    public static FileAttributes Set(this FileAttributes flags, FileAttributes setFlags)
    {
        return flags | setFlags;
    }

    public static FileAttributes Clear(this FileAttributes flags, FileAttributes clearFlags)
    {
        return flags & ~clearFlags;
    }

    public static void ForEach(this FileAttributes flags, Action<FileAttributes> processFlag)
    {
        if (processFlag == null)
        {
            throw new ArgumentNullException("processFlag");
        }

        for (UInt32 bit = 1; bit != 0; bit <<= 1)
        {
            UInt32 temp = ((UInt32)flags) & bit;
            if (temp != 0)
            {
                processFlag((FileAttributes)temp);
            }
        }
    }
}