using System.Collections;

namespace CoreTests.Shared;

public class EmptyStringTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { "\n" };
        yield return new object[] { string.Empty };
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        yield return new object[] { null };
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }


    IEnumerator IEnumerable.GetEnumerator() 
        => GetEnumerator();
}