# SIEDA.Monadic

Implements several functional [Monadic Types](https://en.wikipedia.org/wiki/Monad_(functional_programming)) which enable clean API- and method-design.

## What is this for?
Defining expressive method contracts, avoiding any usage of *null*-values in method calls, clearing expressing failure states and leveraging C#'s type system and overall just writing better code.

## What functionality is accessible?
Contains five **functional classes** of **three basic archetypes, intended to be used in a monadic manner, described below:

### Maybe
A *Maybe* indicates that a value **may** be present, aka there is either a value or there is no value (but obviously never both). A *Maybe* is therefore either a **Some(X)** or a **None** of a type X.

**Example:** When querying a data-structure for the presence of some value that is associated with a key, that Query-Getter may return a *Maybe*, which is a *Some* of that value if an association exists.

### Failable
A *Failable* indicates that either a value is "present" or something went wrong, which is typically represented by an exception. Note however that this exception was **never thrown**, as such no code paths have been erroneously skipped. A *Failable* is therefore either a **Success(X)** or a **Failure(E)** of a type X, with E usually being some exception.

**Example:** When calling a computation method of some service with arguments, that method may return a *Failable*, which is a *Success* containing the computation result if your input was satisfactory.

### Option
An *Option* represents the combination of a *Maybe* and *Failable*, indicating that a value is present, or not present or that something went wrong. That means, in contrast to the two other classes, this is a ternary instead of a binary result type. An *Option* is therefore either a **Some(X)** or **None** or a **Failure(E)** of a type X, with E typically being some exception.

**Example:** When asking some persistence-wrapper for a DTO identified by a specific id via an appropriate method, this wrapper may return an *Option*. That *Option* is a *Some* if your ID matches a datapoint in the persistence wrapped by this class (like a database), a *None* if there is nothing present for that identifier and a *Failure* if the persistence-layer encountered a technical error (like the database-connection being closed).

### E-Variants
Both *Failable* and *Option* exist in an *E*-Variant, namely *EFailable* and *EOption*. These two classes are basically similar to their regular counterparts but with the right-side type fixed to Exception. Therefore, the two classes allow for shorter, more readable code and are the recommended way to employ these concepts, unless your use-cases differs.

## Examples:

### Declaring monadic instances:

```csharp
var maybe = Maybe<int>.Some( 1 );
var failable = Failable<int, string>.Success( 2 );
var option = Option<int, string>.Some( 3 );
``` 

### Leveraging Monadics for API-contracts:

```csharp
//if we assume this is our contract for some authentication storage...
public interface SessionManager
{
   UserSessionKey CreateSession();
   Maybe<UserSession> GetSession( UserSessionKey key ); //return type clearly expresses that no such session could not exist!
   bool EndSession( UserSessionKey key );
}

//...we can for example leverage this when employing this monadic-based contract in a Rest-API endpoint:
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
   private Maybe<DateTime> _currentTime;

   public ClockForTests()
   {
      _currentTime = Maybe<DateTime>.None;
   }

   public ClockForTests( DateTime definedNow )
   {
      _currentTime = Maybe<DateTime>.Some( definedNow );
   }

   public void SetCurrentNow( DateTime d ) { _currentTime = Maybe<DateTime>.Some( d ); }

   public void RemoveCurrentNow() { _currentTime = Maybe<DateTime>.None; }

   public DateTime Now => _currentTime.Or( DateTime.Now );

   public DateTime UtcNow => _currentTime.Map( d => d.AsUtc() ).Or( DateTime.UtcNow );
}
```