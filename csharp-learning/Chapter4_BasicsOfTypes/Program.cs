namespace csharp_learning.Chapter4;

class Chapter4Class
{
    public void Execute()
    {
        Employee e;
        int year;
        e = new Manager();
        e = Employee.Lookup("Joe");
        year = e.GetYearsEmployed();
        var s = e.GetProcessReport();
    }
}

class Employee
{
    public int GetYearsEmployed()
    {
        return 1;
    }

    public virtual string GetProcessReport()
    {
        return "employee";
    }

    public static Employee Lookup(string name)
    {
        return new Employee();
    }
}

sealed class Manager : Employee
{
    public override string GetProcessReport()
    {
        return "manager";
    }

    public new int GetYearsEmployed()
    {
        return 2;
    }
}