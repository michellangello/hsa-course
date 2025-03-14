using Bogus;

public class Employee
{
	public int Id { get; set; }
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public Address Address { get; set; }
	public Job Job { get; set; }
}

public class Address
{
	public string Street { get; set; }
	public string City { get; set; }
	public string ZipCode { get; set; }
}

public class Job
{
	public string Title { get; set; }
	public Department Department { get; set; }
}

public class Department
{
	public string Name { get; set; }
	public string Floor { get; set; }
}

public class DepartmentFaker : Faker<Department>
{
	public DepartmentFaker()
	{
		RuleFor(d => d.Name, f => f.Commerce.Department());
		RuleFor(d => d.Floor, f => f.Random.Number(1, 20).ToString());
	}
}

public class JobFaker : Faker<Job>
{
	public JobFaker()
	{
		RuleFor(j => j.Title, f => f.Name.JobTitle());
		RuleFor(j => j.Department, f => new DepartmentFaker().Generate());
	}
}

public class AddressFaker : Faker<Address>
{
	public AddressFaker()
	{
		RuleFor(a => a.Street, f => f.Address.StreetName());
		RuleFor(a => a.City, f => f.Address.City());
		RuleFor(a => a.ZipCode, f => f.Address.ZipCode());
	}
}

public class EmployeeFaker : Faker<Employee>
{
	public EmployeeFaker()
	{
		RuleFor(e => e.Id, f => f.Random.Number(int.MinValue, int.MaxValue));
		RuleFor(e => e.FirstName, f => f.Name.FirstName());
		RuleFor(e => e.LastName, f => f.Name.LastName());
		RuleFor(e => e.Address, f => new AddressFaker().Generate());
		RuleFor(e => e.Job, f => new JobFaker().Generate());
	}
}
