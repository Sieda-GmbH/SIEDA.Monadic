# SIEDA.Monadic

Implements several functional [Monadic Types](https://en.wikipedia.org/wiki/Monad_(functional_programming)) which enable clean API- and method-design.

## Where can I get/download it?

You can find a NuGet-Package at [www.nuget.org](https://www.nuget.org/packages/SIEDA.Monadic/), containing binaries for different frameworks. There are no special dependencies.

## What is this for?

Consider of your code's contracts, such as the fact that an operation could fail or that a required value might not be present. In traditional C#, these contracts are _implicit_, for example realized through an exception which might be thrown (immediately aborting the current execution path) or by _null_-values which are supposed to carry that semantic. In short, your programmers must always be aware and on the look-out for these implicitly agreed-upon contracts. Usage of _null_ is particular problematic in this context, as this overloads its semantics! A _null_ now means both _not initialized yet_ and _edge/exception-case_.

Through the classes within this library, you can leverage the type system of C# to enforce the basic nature of such contracts, which means that you code does not compile until the programmer has ensured that he or she is dealing with e.g. the fact that this particular operation might fail (and what to do in this case).

### TLDR; What is this for?
Defining expressive method contracts, avoiding any usage of *null*-values in method calls, clearly expressing failure states and overall just writing better code by leveraging C#'s type system.

## What functionality is accessible?
Contains a number of **functional classes** of **three basic archetypes**, intended to be used in a monadic manner. This lib contains:

### Unary Monadic Archetypes:
#### Maybe
A *Maybe* indicates that a value **may** be present, aka there is either a value or there is no value (but obviously never both). A *Maybe* is therefore either a **Some(X)** or a **None** of a type X.

**Example:** When querying a data-structure for the presence of some value that is associated with a key, that query-getter may return a *Maybe*, which is a *Some* of that value if an association exists.

#### Validation
A *Validation* indicates that an operation **may** have failed, aka there is either no value because everything was fine or there is a value signaling a failure (but obviously never both). A *Validation* is therefore either a **Success** or a **Failure(X)** of a type X.

**Example:** When triggering a write-operation on your file-system, the corresponding function may return a *Validation*, which is a *Failure* containing a description of the problem if the write was not successful (e.g. due to permission problems).

#### EValidation

See [here](https://github.com/Sieda-GmbH/SIEDA.Monadic/blob/master/README.md#e-variants).

### Binary Monadic Archetypes:
#### Failable
A *Failable* indicates that either a value is "present" or something went wrong and a different value of a different type is "present". A *Failable* is therefore either a **Success(X)** or a **Failure(E)** of a type X, with E usually being some exception. Note however that such an exception is **not** thrown, as such no code paths are erroneously skipped and our contract is clean.

**Example:** When parsing an XML-serialized data-structure, that method may return a *Failable*, which is a *Success* containing the deserialized DTO-object if your input was satisfactory. Otherwise, the *Failure* models what was wrong with the string.

#### EFailable

See [here](https://github.com/Sieda-GmbH/SIEDA.Monadic/blob/master/README.md#e-variants).

### Ternary Monadic Archetypes:
#### Option
An *Option* represents the combination of a *Maybe* and *Failable*, indicating that a value is present or not present or that something went wrong. That means, in contrast to the two other classes, this is a ternary instead of a binary result type. An *Option* is therefore either a **Some(X)** or **None** or a **Failure(E)** of a type X, with E typically being some exception.

**Example:** When asking some persistence-wrapper for a DTO identified by a specific id via an appropriate method, this wrapper may return an *Option*. That *Option* is a *Some* if your id matches a datapoint in the persistence wrapped by this class (like a database), a *None* if there is nothing present for that identifier and a *Failure* if the persistence-layer encountered a technical error (like the database-connection being closed).

#### EOption

See [here](https://github.com/Sieda-GmbH/SIEDA.Monadic/blob/master/README.md#e-variants).

## E-Variants
*Validation*, *Failable* and *Option* exist in *E*-variants, namely *EValidation*, *EFailable* and *EOption*. These classes are basically similar to their regular counterparts, but with the right-side type fixed to Exception. Therefore, these three classes allow for shorter, more readable code and are the recommended way to employ these concepts, unless your use-cases differ significantly from _"modelling the result of operations"_.

## Examples:

### Declaring instances of monadic objects:

```csharp
var maybeA = Maybe<int>.Some( 1 );
var maybeB = Maybe<int>.None;

var validationA = Validation<string>.Success();
var validationB = Validation<string>.Failure( "Oh no, something went wrong!" );

var failableA = Failable<float, string>.Success( 2.0f );
var failableB = Failable<float, string>.Failure( "Oh no, something went wrong!" );

var optionA = Option<long, string>.Some( 3 );
var optionB = Option<long, string>.None;
var optionC = Option<long, string>.Failure( "Oh no, something went wrong!" );
```

For the majority of usecases, you will probably employ the E-Variants, which ends up looking like this for example:

```csharp
var efailableA = EFailable<float>.Success( 2.0f );
var efailableB = EFailable<float>.Failure( new Exception( "Oh no, something went wrong!" ) );
```

### Leveraging Monadics for API-contracts:

```csharp
//if we assume this is our contract for some authentication storage...
public interface SessionManager
{
   EFailable<UserSessionKey> CreateSession();
   Maybe<UserSession> GetSession( UserSessionKey key ); //return type clearly expresses that such a session may be non-existent!
   EValidation EndSession( UserSessionKey key );
}

//...we can for example leverage this when employing the monadic-based contract in e.g. REST-API endpoint to write very clean code:
[HttpPost( "usersession/{sessionId}/do_something" )]
[Produces( MediaTypeConstants.Application.Json )]
public ActionResult<ResultOfSomething> ExecuteForExistingUserSession( int sessionId, [FromBody] OtherParams otherParams ) =>
   SessionManager.GetSession( new UserSessionKey( sessionId ) )
      .Map( session => DoSomething( session, otherParams ) ) //returns a 'ResultOfSomething' containing the result
      .Or( ResultOfSomething.MakeError( $" Session '{sessionId}' not found!" ) } );
```

### Avoiding 'null'-values in internal logic:
```csharp
public class ClockForTests
{
   private Maybe<DateTime> _currentTime; //monadic object

   //CTOR for real-time clock:
   public ClockForTests() => _currentTime = Maybe<DateTime>.None;

   //CTOR for fixed-time instance:
   public ClockForTests( DateTime definedNow ) => _currentTime = Maybe<DateTime>.Some( definedNow );
   
   //accessor-methods for all testsuites to obtain the currently canonical concept of "the current time"
   public DateTime Now => _currentTime.Or( DateTime.Now );
   public DateTime UtcNow => _currentTime.Map( d => d.AsUtc() ).Or( DateTime.UtcNow );
   
   //setter-methods to set the currently canonical "current time", depending on the testcase's demands
   public void SetCurrentTimeTo( DateTime d ) => _currentTime = Maybe<DateTime>.Some( d );
   public void SetToRealTime() => _currentTime = Maybe<DateTime>.None;
}
```

### Stepping into the monadic world from your non-monadic source code:
```csharp
var efailable = EFailable<MyOjectReturnedMyFunction>.Wrapping( () => MyFunctionThatEitherReturnsAnObjectOrThrows( ... ) );
```

### Having clean contracts for your persistence layer (a class wrapping your SQL code):
```csharp
namespace Persistence.Layer.of.my.Application
{
   public interface IUserProvider
   {
      /// <summary>Obtain data based on the <paramref name="id"/>.</summary>
      /// <returns>
      /// <para>* <see cref="EOption{UserData}.Some(UserData)"/> containing the data identified by <paramref name="id"/>.</para>
      /// <para>* <see cref="EOption{UserData}.None"/> if no data exists on the database. </para>
      /// <para>* <see cref="EOption{UserData}.Failure(System.Data.SqlClient.SqlException)"/> if any technical problem occurred (e.g. connection loss). </para>
      /// </returns>
      EOption<UserData> GetUserData( User id );
   }
}
```

