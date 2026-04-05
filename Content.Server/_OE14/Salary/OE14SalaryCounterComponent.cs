namespace Content.Server._OE14.Salary;

[RegisterComponent, Access(typeof(OE14SalarySystem))]
public sealed partial class OE14SalaryCounterComponent : Component
{
    [DataField, AutoNetworkedField]
    public TimeSpan NextSalaryTime = TimeSpan.Zero;

    [DataField]
    public TimeSpan Frequency = TimeSpan.FromMinutes(20);

    [DataField]
    public int Salary = 100;

    [DataField]
    public int UnpaidSalary = 0;
}
