using System.Net.Http.Headers;
using System.Runtime.ConstrainedExecution;

namespace csharp_learning.Part3.BasicDataTypes.Chapter16_Arrays;

public class Chapter16Class
{
    public void Execute()
    {
        Int32[] myInts; // объявление ссылки на массивы
        // поскольку массивы относятся к ссылочным типам данных, блок памяти
        // для хранение 100 неупакованных экземпляров типа Int32 выдяделяется
        // в упавляемой куче. Также в этом блоке размещается указатель на объект-тип
        // индекс блока синхронизации
        myInts = new Int32[100]; // создание массивы типа Int32 из 100 элементов

        var names = new String[] { "Aidan", "Grant" };
        // names[2] = "234"; OutOfRange

        // создание двумерного массива FileStream
        FileStream[,] fs2dim = new FileStream[5, 10];

        // неявное приведение к массиву типа Object
        Object[,] o2dim = fs2dim;

        // невозможно приведение двухмерного массива к одномерному
        // Ошибка компиляции CS0030: невозможно преобразовать тип 'object[*,*]'
        // в System.IO.Stream[]
        //Stream[] s1dim = (Stream[])o2dim;

        // явное приведение к двухмерному массиву Stream
        Stream[,] s2dim = (Stream[,])o2dim;

        // явное приведение к двухмерному массиву String
        // Компилируется, но во время исполнения - InvalidCastException
        // String[,] str2dim = (String[,])o2dim;

        // Создание одномерного массива Int32 (значимый тип)
        Int32[] i1dim = new Int32[5];

        // Невозможно приведение массива значимого типа
        // Ошибка компиляции CS0030: невозможно преобразовать
        // тип 'int[]' в 'object[]'
        // Object[] o1dim = (Object[])i1dim;

        // Создание нового массива и приведение элементов к нужному типу
        // при помощи метода Array.Copy
        // Создаем массив ссылок на упакованные элементы типа Int32
        Object[] ob1dim = new Object[i1dim.Length];
        Array.Copy(i1dim, ob1dim, i1dim.Length);

        String[] sa = new string[100];

        Object[] oa = sa; // oa ссылается на массив элементов типа String

        oa[5] = "Jeff"; // CLR проверяет принадлежность oa к типу String
                        // проверка проходит успешно
        oa[3] = 5; // CLR проверяет принадлежность oa к типу Int32;
                   // Генерируется исключение ArrayTypeMismatchException

        // В этом коде переменная oa, тип которой определен как Object[], ссылается
        // на массив типа String[]. Затем вы пытаетесь присвоить одному из элементов этого 
        // массива значение 5, относящегося к типу Int32, производному от типа Object. 
        // Естественно CLR проверяет корректность такого присваивания, то есть в процессе 
        // выполнения контролирует наличие в массиве элементов типа Int32. В данном случае
        // такие элементы отсутствуют, что и становится причиной исключения ArrayTypeMismatchException
    }
}
