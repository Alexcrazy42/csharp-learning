using System.Xml;

namespace csharp_learning.Part3.BasicDataTypes.Chapter14_Symbols_Strings_TextProcessing;

public class Chapter14Class
{
    public void Execute()
    {
        Double d;
        d = Char.GetNumericValue('\u0033'); // '\u0033 ' - это " цифра 3"
        Console.WriteLine(d.ToString());

        d = char.GetNumericValue('\u00bc'); // '\uOObc' - это "простая дробь одна четвертая ('1/4 ')"
        Console.WriteLine(d.ToString());

        d = Char.GetNumericValue('A');
        Console.WriteLine(d.ToString());


        Console.WriteLine("-----------------------------------");
        Char c;
        Int32 n;

        // приведение типов
        c = (Char)65;
        Console.WriteLine(c);

        n = (Int32)c;
        c = unchecked((Char)(65536 + 65));
        Console.WriteLine(c);

        // Convert
        c = Convert.ToChar(65);
        Console.WriteLine(c);

        n = Convert.ToInt32(c);
        Console.WriteLine(n);

        try
        {
            c = Convert.ToChar(20000000); // слишком много для 16 разрядов
            Console.WriteLine(c);
        }
        catch (OverflowException)
        {
            Console.WriteLine("Can't convert this to a Char");
        }

        c = ((IConvertible)65).ToChar(null);
        Console.WriteLine(c);

        n = ((IConvertible)c).ToInt32(null);
        Console.WriteLine(n);

        Console.WriteLine("-------------------------------------");

        // результат один и тот же. Однако символ @ перед строкой во втором случае
        // сообщает компилятору, что перед ним буквальная строка и он должен 
        // рассматривать символ обратного слэша как таковой, а не как признак управляющей
        // последовательности, благодаря чему путь выглядит привычнее
        String file = "C:\\Windows\\System32\\Notepad.exe";
        Console.WriteLine(file);
        file = @"C:\Windows\System32\Notepad.exe";
        Console.WriteLine(file);
    }
}
