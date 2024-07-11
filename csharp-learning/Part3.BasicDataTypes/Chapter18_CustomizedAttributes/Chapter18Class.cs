namespace csharp_learning.Part3.BasicDataTypes.Chapter18_CustomizedAttributes;

public class Chapter18Class
{
    public void Execute()
    {
        Person tom = new Person("Tom", 35);
        Person bob = new Person("Bob", 16);
        bool tomIsValid = ValidateUser(tom);
        bool bobIsValid = ValidateUser(bob);

        Console.WriteLine($"Результат валидации Тома: {tomIsValid}");
        Console.WriteLine($"Результат валидации Боба: {bobIsValid}");
    }

    bool ValidateUser(Person person)
    {
        Type type = typeof(Person);
        object[] attributes = type.GetCustomAttributes(false);
        
        foreach (var attr in attributes)
        {
            if (attr is AgeValidationAttribute ageAttribute)
                return person.Age >= ageAttribute.Age;
        }
        return true;
    }
}

[AttributeUsage(AttributeTargets.Enum, Inherited = false)]
public class FlagsAttribute : System.Attribute
{
    public FlagsAttribute()
    {

    }
}

class AgeValidationAttribute : Attribute
{
    public int Age { get; }
    public AgeValidationAttribute() { }
    public AgeValidationAttribute(int age) => Age = age;
}

[AgeValidation(18)]
public class Person
{
    public string Name { get; }
    public int Age { get; set; }
    public Person(string name, int age)
    {
        Name = name;
        Age = age;
    }
}

