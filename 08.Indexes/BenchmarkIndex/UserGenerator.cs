using Bogus;

namespace Benchmark;

public static class UserGenerator
{
    public static IEnumerable<(string Name, DateTime Dob)> GenerateUsers(int count)
    {
        var faker = new Faker();
        return Enumerable.Range(1, count).Select(_ =>
            (faker.Name.FullName(), faker.Date.Between(new DateTime(1950, 1, 1), new DateTime(2010, 12, 31))));
    }
}
