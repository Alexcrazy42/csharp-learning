using System.Globalization;
using System.Text;

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

        Console.WriteLine("-----------------------------------------");
        String s1 = "strasse";
        String s2 = "straße";
        Boolean eq;

        // CompareOrdinal возвращает ненулевое значение
        eq = String.Compare(s1, s2, StringComparison.Ordinal) == 0;
        Console.WriteLine($"Ordinal comparison {s1} {(eq ? "==" : "!=")} {s2}");

        // Сортировка строк для немецкого языка (de) в Германии (DE)
        CultureInfo ci = new CultureInfo("de-De");

        eq = String.Compare(s1, s2, true, ci) == 0;
        Console.WriteLine($"Cultural comparison: {s1} {(eq ? "==" : "!=")} {s2}");

        Console.WriteLine("------------------------------------------");
        String output = String.Empty;
        String[] symbol = new String[] { "<", "=", ">"};
        Int32 x;
        CultureInfo ci1;

        // Следующ ий код демонстрирует насколько отличается результат
        // сравнения строк для различных региональных стандартов
        String s11 = "coté";
        String s21 = "côte";

        // сортировка строк для французского языка во Франкции
        ci1 = new CultureInfo("fr-FR");
        x = Math.Sign(ci1.CompareInfo.Compare(s11, s21));
        output += $"{ci1.Name} Compare: {s11} {s21} {symbol[x + 1]}";
        output += Environment.NewLine;

        // сортировка для японского языка в Японии
        ci1 = new CultureInfo("ja-JP");
        x = Math.Sign(ci1.CompareInfo.Compare(s11, s21));
        output += $"{ci1.Name} Compare: {s11} {s21} {symbol[x + 1]}";
        output += Environment.NewLine;

        // региональные стандарты потока
        ci1 = Thread.CurrentThread.CurrentCulture;
        x = Math.Sign(ci1.CompareInfo.Compare(s11, s21));
        output += $"{ci1.Name} Compare: {s11} {s21} {symbol[x + 1]}";
        output += Environment.NewLine;
        Console.WriteLine(output);

        Console.WriteLine("----------------------------------------");
        String s111 = "Hello";
        String s112 = "Hello";
        Console.WriteLine(Object.ReferenceEquals(s111, s112));

        s111 = String.Intern(s111);
        s112 = String.Intern(s112);
        Console.WriteLine(Object.ReferenceEquals(s111, s112));

        Console.WriteLine("----------------------------------------");
        StringBuilder sb = new();
        
        // выполняем ряд действий со строками, используя SB
        sb.Append($"Jeffry Richter").Replace(" ", "-");

        // преобразуем SB в String, 
        // чтобы сделать все символы прописными
        String s = sb.ToString().ToUpper();

        // очищаем SB (выделяется память под новый массив Char
        sb.Length = 0;

        // загружаем строку с прописными String в SB
        // и выполняем остальные операции
        sb.Append(s).Insert(8, "Marc-");

        // преобразуем SB обратно в String
        s = sb.ToString();

        Console.WriteLine(s);
    }

    // Как видите, этот метод вызывает метод Equals типа Stri ng, который сравнивает
    // отдельные символы строк и проверяет, все ли символы совпадают. Это 
    // сравнение может выполняться медленно. Кроме того, массив wordlist может
    // иметь много элементов, которые ссылаются на многие объекты String, содержащие
    // тот же набор символов.Это означает, что в куче может существовать
    // множество идентичных строк, не подлежащих сборке в качестве мусора. 
    private static Int32 NumTimesWordAppearsEquals(String word, String[] wordList)
    {
        Int32 count = 0;
        for(Int32 i = 0; i < wordList.Length; i++)
        {
            if (word.Equals(wordList[i], StringComparison.Ordinal))
            {
                count++;
            }
        }
        return count;
    }

    // Этот метод интернирует слово и предполагает, что wordlist содержит ссылки на
    // интернированные строки.Во-первых, в этой версии экономится память,
    // если слово повторяется в списке слов, потому что теперь wordlist содержит
    // многочисленные ссылки на единственный объект Stri ng в куче. Во-вторых, эта
    // версия работает быстрее, потому что для выяснения, есть ли указанное слово
    // в массиве, достаточно сравнить указатели.
    // Хотя метод NumTimesWordAppearsIntern работает быстрее, чем NumTimesWordAppearsEquals,
    // общая производительность приложения может оказаться
    // ниже, чем при использовании метода NumTimesWordAppears intern из-за времени, которое
    // требуется на интернирование всех строк по мере добавления их
    // в массив wordlist.
    // Преимущества метода NumTimesWordAppearsintern - ускорение работы и снижение
    // потребления памяти - будут заметны, если приложению нужно множество раз вызывать метод,
    // передавая один и тот же массив wordlist.Этим обсуждением я хотел донести
    // до вас, что интернирование строк полезно, но использовать его нужно с осторожностью.
    // Собственно, именно по этой причине компилятор С# указывает, что не следует разрешать интернирование строк. 

    private static Int32 NumTimesWordAppearsIntern(String word, String[] wordList)
    {
        // В этом методе предполагается, что все элементы в wordList
        // ссылаются на интернированные строки
        word = String.Intern(word);
        Int32 count = 0;
        for (int i = 0; i < wordList.Length; i++)
        {
            if (Object.ReferenceEquals(word, wordList[i]))
            {
                count++;
            }
        }
        return count;

    }
}
