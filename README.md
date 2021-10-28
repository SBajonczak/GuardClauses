

![Nuget](https://img.shields.io/nuget/v/Bajonczak.Guard?style=for-the-badge)
![Nuget](https://img.shields.io/nuget/dt/Bajonczak.Guard?style=for-the-badge)


# Guard Clauses

A simple extensible package with guard clause extensions.

A [guard clause](https://blog.bajonczak.com/GuardIntro) is a software pattern that simplifies complex functions by "failing fast", checking for invalid inputs up front and immediately failing if any are found.

## Give a Star! :star:

If you like or are using this project please give it a star. Thanks!

## Usage

```c#
public void ProcessOrder(Order order)
{
    Guard.Against.Null(order, nameof(order));

    // process order here
}

// OR

public class Order
{
    private string _name;
    private int _quantity;
    private long _max;
    private decimal _unitPrice;
    private DateTime _dateCreated;

    public Order(string name, int quantity, long max, decimal unitPrice, DateTime dateCreated)
    {
        _name = Guard.Against.NullOrWhiteSpace(name, nameof(name));
        _quantity = Guard.Against.NegativeOrZero(quantity, nameof(quantity));
        _max = Guard.Against.Zero(max, nameof(max));
        _unitPrice = Guard.Against.Negative(unitPrice, nameof(unitPrice));
        _dateCreated = Guard.Against.OutOfSQLDateRange(dateCreated, nameof(dateCreated));
    }
}
```

## Supported Guard Clauses

- **Guard.Against.Null** (throws if input is null)
- **Guard.Against.NullOrEmpty** (throws if string, guid or array input is null or empty)
- **Guard.Against.NullOrWhiteSpace** (throws if string input is null, empty or whitespace)
- **Guard.Against.OutOfRange** (throws if integer/DateTime/enum input is outside a provided range)
- **Guard.Against.OutOfSQLDateRange** (throws if DateTime input is outside the valid range of SQL Server DateTime values)
- **Guard.Against.Zero** (throws if number input is zero)

## Extending with your own Guard Clauses

To extend your own guards, you can do the following:

```c#
// Using the same namespace will make sure your code picks up your 
// extensions no matter where they are in your codebase.
namespace Bajonczak.GuardClauses
{
    public static class FooGuard
    {
        public static void Foo(this IGuardClause guardClause, string input, string parameterName)
        {
            if (input?.ToLower() == "foo")
                throw new ArgumentException("Should not have been foo!", parameterName);
        }
    }
}

// Usage
public void SomeMethod(string something)
{
    Guard.Against.Foo(something, nameof(something));
}
```


