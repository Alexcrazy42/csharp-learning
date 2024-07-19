using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace csharp_learning.Part4.KeyMechanisms.Chapter23_AssemblyLoadingAndReflection;

public class Chapter23Class
{
    private const BindingFlags bf = BindingFlags.Instance | BindingFlags.Public 
        | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Static;

    public void Execute()
    {
        //String dataAssembly = "System.Data, version=4.0.0.0, " +
        //    "culture=neutral, PublicKeyToken=b77a5c561934e089";
        //LoadAssemblyAndShowPublicTypes(dataAssembly);

        //LoadAssemblies();

        //var allTypes = (from a in AppDomain.CurrentDomain.GetAssemblies()
        //                from t in a.ExportedTypes
        //                where typeof(Exception).GetTypeInfo().IsAssignableFrom(t.GetTypeInfo())
        //                orderby t.Name
        //                select t).ToArray();

        // Построение и вывод иерархического дерева наследования
        //Console.WriteLine(WalkInhetanceHierarhy(new StringBuilder(), 0, typeof(Exception)));

        // Получаем ссылку на объект Type обобщенного типа
		Type openType = typeof(Dictionary<,>);

		// Закрываем обобщенный тип, используя TKey=String, TValue=Int32
		Type closedType = openType.MakeGenericType(
			new Type[] { typeof(String), typeof(Int32) });

		// Создаем экземпляр закрытого типа
		var o = (Dictionary <string,int>) Activator.CreateInstance(closedType);

		// Проверяем, работает ли наше решение
		Console.WriteLine(o.GetType());


        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        //foreach (Assembly a in assemblies)
        //{
        //    Show(0, "Assembly : {0}", a);

        //    foreach (Type t in a.ExportedTypes)
        //    {
        //        Show(1, "Type: {0}", t);

        //        // Получение информации о членах типа
        //        foreach (MemberInfo mi in t.GetTypeInfo().DeclaredMembers)
        //        {
        //            String typeName = String.Empty;
        //            if (mi is Type) typeName = "(Nested) Type";
        //            if (mi is FieldInfo) typeName = "FieldInfo";
        //            if (mi is MethodInfo) typeName = "MethodInfo";
        //            if (mi is ConstructorInfo) typeName = "ConstructoInfo";
        //            if (mi is PropertyInfo) typeName = "PropertyInfo";
        //            if (mi is EventInfo) typeName = "EventInfo";
        //            Show(2, "{0}: {1}", typeName, mi);
        //        }
        //    }
        //}


        Type t = typeof(SomeType);
        BindToMemberThenInvokeTheMember(t);
        Console.WriteLine();

        BindToMemberCreateDelegateToMemberThenInvokeTheMember(t);
        Console.WriteLine();

        UseDynamicToBindAndInvokeTheMember(t);
        Console.WriteLine();


        // Выводим размер кучи до отражения
        Show("Before doing anything");

        // Создаем кэш объектов MethodInfo для всех методов из MSCorlib.dll
        List<MethodBase> methodInfos = new List<MethodBase>();

        foreach (Type t2 in typeof(Object).Assembly.GetExportedTypes())
        {
            // Игнорируем обобщенные типы
            if (t2.IsGenericTypeDefinition) continue;

            MethodBase[] mb = t.GetMethods(bf);
            methodInfos.AddRange(mb);
        }

        // Выводим количество методов и размер кучи после привязки всех методов
        Console.WriteLine("# of methods={0:N0}", methodInfos.Count);
        Show("After building cache of MethodInfo objects");
        
        // Создаем кэш дескрипторов RuntimeMethodHandles
        // для всех объектов MethodInfo
        List<RuntimeMethodHandle> methodHandles =
            methodInfos.ConvertAll<RuntimeMethodHandle>(mb => mb.MethodHandle);
        
        Show("Holding MethodInfo and RuntimeMethodHandle cache");
        GC.KeepAlive(methodInfos); // Запрещаем уборку мусора в кэше

        methodInfos = null; // Разрешаем уборку мусора в кэше
        GC.Collect();
        Show("After freeing MethodInfo objects");

        methodInfos = new List<MethodBase>();

        foreach (Type t1 in typeof(Object).Assembly.GetExportedTypes())
        {
            // Игнорируем обобщенные типы
            if (t1.IsGenericTypeDefinition) continue;

            MethodBase[] mb = t1.GetMethods(bf);
            methodInfos.AddRange(mb);
        }

        Show("Size of heap after re-creating MethodInfo objects");
        GC.KeepAlive(methodHandles); // Запрещаем уборку мусора в кэше
        GC.KeepAlive(methodInfos); // Запрещаем уборку мусора в кэше
       
        methodHandles = null; // Разрешаем уборку мусора в кэше
        methodInfos = null; // Разрешаем уборку мусора в кэше
        GC.Collect();
        Show("After freeing MethodInfos and RuntimeMethodHandles");
    }

    private void BindToMemberThenInvokeTheMember(Type t)
    {
        Console.WriteLine("BindToMemberThenInvokeTheMember:");

        // создание экземпляра
        Type ctorArgument = Type.GetType("System.Int32&");
        // или typeof(Int32).MakeByRefType();

        ConstructorInfo ctor = t.GetTypeInfo().DeclaredConstructors.First(c =>
            c.GetParameters()[0].ParameterType == ctorArgument);
        Object[] args = new object[] { 12 }; // аргументы конструктора

        Console.WriteLine("x before constructor called: " + args[0]);

        Object obj = ctor.Invoke(args);
        Console.WriteLine("Type: " + obj.GetType());
        Console.WriteLine("x after contructor returns: " + args[0]);

        // чтение и запись в поле
        FieldInfo fi = obj.GetType().GetTypeInfo().GetDeclaredField("someField");
        fi.SetValue(obj, 33);
        Console.WriteLine("someField: " + fi.GetValue(obj));

        // вызов метода
        MethodInfo mi = obj.GetType().GetTypeInfo().GetDeclaredMethod("ToString");
        String s = (String) mi.Invoke(obj, null);
        Console.WriteLine("ToString: " + s);

        // чтение и запись свойства
        PropertyInfo pi = obj.GetType().GetTypeInfo().GetDeclaredProperty("SomeProp");
        try
        {
            pi.SetValue(obj, 0, null);
        }
        catch (TargetInvocationException e)
        {
            if (e.InnerException.GetType() != typeof(ArgumentOutOfRangeException)) throw;

            Console.WriteLine("Property set catch");
        }

        pi.SetValue(obj, 2, null);
        Console.WriteLine("SomeProp: " + pi.GetValue(obj, null));

        // добавление и удаление делегата для события
        EventInfo ei = obj.GetType().GetTypeInfo().GetDeclaredEvent("SomeEvent");
        EventHandler eh = new EventHandler(EventCallback); // смотри ei.EventHandlerType
        ei.AddEventHandler(obj, eh);
        ei.RemoveEventHandler(obj, eh);
    }

    private void EventCallback(Object sender, EventArgs e) {  }

    private void BindToMemberCreateDelegateToMemberThenInvokeTheMember(Type t)
    {
        Console.WriteLine("BindToMemberCreateDelegateToMemberThenInvokeTheMember: ");

        // создание экземпляры (нельзя создать делегата для конструктора)
        Object[] args = new object[] { 12 }; // аргументы конструктора

        Console.WriteLine("x before constructor called: " + args[0]);
        object obj = Activator.CreateInstance(t, args);
        Console.WriteLine("Type:" + obj.GetType().ToString());
        Console.WriteLine("x after constructor returns: " + args[0]);

        // ВНИМАНИЕ: нельзя создать делегата для поля

        // вызов метода
        MethodInfo mi = obj.GetType().GetTypeInfo().GetDeclaredMethod("ToString");
        var toString = mi.CreateDelegate<Func<String>>(obj);
        String s = toString();
        Console.WriteLine("ToString: " + s);

        // Чтение и запись свойства
        PropertyInfo pi = obj.GetType().GetTypeInfo().GetDeclaredProperty("SomeProp");
        var setSomeProp = pi.SetMethod.CreateDelegate<Action<Int32>>(obj);
        try
        {
            setSomeProp(0);
        }
        catch (ArgumentOutOfRangeException)
        {
            Console.WriteLine("Property set catch.");
        }
        setSomeProp(2);
        var getSomeProp = pi.GetMethod.CreateDelegate<Func<Int32>>(obj);
        Console.WriteLine("SomeProp: " + getSomeProp());

        // Добавление и удаление делегата для события
        EventInfo ei = obj.GetType().GetTypeInfo().GetDeclaredEvent("SomeEvent");
        var addSomeEvent = ei.AddMethod.CreateDelegate<Action<EventHandler>>(obj);
        addSomeEvent(EventCallback);
        var removeSomeEvent =
        ei.RemoveMethod.CreateDelegate<Action<EventHandler>>(obj);
        removeSomeEvent(EventCallback);
    }

    private void UseDynamicToBindAndInvokeTheMember(Type t)
    {
        Console.WriteLine("UseDynamicToBindAndInvokeTheMember");

        // Создание экземпляра (dynamic нельзя использовать для вызова конструктора)
        Object[] args = new Object[] { 12 }; // Аргументы конструктора
        Console.WriteLine("x before constructor called: " + args[0]);
        dynamic obj = Activator.CreateInstance(t, args);
        Console.WriteLine("Type: " + obj.GetType().ToString());
        Console.WriteLine("x after constructor returns: " + args[0]);

        // Чтение и запись поля
        try
        {
            obj.someField = 5;
            Int32 v = (Int32)obj.m_someField;
            Console.WriteLine("someField: " + v);
        }
        catch (RuntimeBinderException e)
        {
            // Получает управление, потому что поле является приватным
            Console.WriteLine("Failed to access field: " + e.Message);
        }

        // Вызов метода
        String s = (String)obj.ToString();
        Console.WriteLine("ToString: " + s);

        // Чтение и запись свойства
        try
        {
            obj.SomeProp = 0;
        }
        catch (ArgumentOutOfRangeException)
        {
            Console.WriteLine("Property set catch.");
        }
        obj.SomeProp = 2;
        Int32 val = (Int32)obj.SomeProp;
        Console.WriteLine("SomeProp: " + val);

        // Добавление и удаление делегата для события
        obj.SomeEvent += new EventHandler(EventCallback);
        obj.SomeEvent -= new EventHandler(EventCallback);
    }

    private void Show(string s)
    {
        var total = GC.GetTotalMemory(false);
        Console.WriteLine($"Heap size = {total} - {s}");
    }

    private void Show(Int32 indent, String format, params Object[] args)
    {
        Console.WriteLine(new String(' ', 3 * indent) + format, args);
    }

    private void LoadAssemblyAndShowPublicTypes(String assemId)
    {
        // явная загрузка сборки в домен приложений
        Assembly a = Assembly.Load(assemId);

        // цикл выполняется для каждого типа,
        // открыто экспортируемого загруженной сборкой
        foreach(Type t in a.ExportedTypes)
        {
            Console.WriteLine(t.FullName);
        }
    }

    private void LoadAssemblies()
    {
        String[] assemblies =
        {
             "System.Runtime",
                "System.Collections",
                "System.Linq",
                "System.Threading.Tasks",
                "System.Net.Http",
                "System.Xml.Linq",
                "System.Reflection",
                "System.Console",
                "System.Text.Json",
                "System.IO.FileSystem",
                "System.Buffers",
                "System.Text.Encoding.Extensions"
        };

        String EcmaPublicKeyToken = "b77a5c561934e089";
        String MSPublicKeyToken = "b03f5f7f11d50a3a";



        // Этот же номер версии предполагается для всех остальных сборок.
        Version version = typeof(Object).Assembly.GetName().Version;

        // Явная загрузка сборок
        foreach (String a in assemblies)
        {
            String AssemblyIdentity =
                String.Format(a, EcmaPublicKeyToken, MSPublicKeyToken) +
                    ", Culture=neutral, Version=" + version;
            Assembly.Load(AssemblyIdentity);
        }
    }

    private StringBuilder WalkInhetanceHierarhy(
        StringBuilder sb, Int32 indent, Type baseType, IEnumerable<Type> allTypes)
    {
        String spaces = new String(' ', indent * 3);
        sb.AppendLine(spaces + baseType.FullName);
        foreach (var t in allTypes)
        {
            if (t.GetTypeInfo().BaseType != baseType) continue;

            WalkInhetanceHierarhy(sb, indent + 1, t, allTypes);
        }

        return sb;
    }
}

internal static class ReflectionExtensions
{
    // Метод расширения, упрощающий синтаксис создания делегата
    public static TDelegate CreateDelegate<TDelegate>(this MethodInfo mi,
    Object target = null)
    {
        return (TDelegate)(Object)mi.CreateDelegate(typeof(TDelegate), target);
    }
}

// класс для демонстрации отражения
// у него есть поле, конструктор, метод, свойство и событие
internal sealed class SomeType
{
    private Int32 someField;

    public SomeType(ref Int32 x)
    {
        x *= 2;
    }

    public override string ToString()
    {
        return someField.ToString();
    }

    public Int32 SomeProp
    {
        get { return someField; } 
        
        set
        {
            if (value < 1)
            {
                throw new ArgumentOutOfRangeException("value");
            }
            someField = value;
        }
    }

    public event EventHandler SomeEvent;

    private void NoCompilerWarnings() { SomeEvent.ToString(); }
}