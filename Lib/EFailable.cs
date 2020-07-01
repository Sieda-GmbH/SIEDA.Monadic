using System;

namespace SIEDA.Monadic
{
   /// <summary>
   /// Represents the "result" of an operation that might have failed, containing either a value of type <typeparamref name="TValue"/> if the operation "was successful" 
   /// or wrapping an internal <see cref="Exception"/> if the operation "was a failure". Note that this internal exception is not thrown but contained within this
   /// instance, just as a value representing a "success" would. Neither of these are ever <see langword="null"/>.
   /// </summary>
   /// <typeparam name="TValue">The type of the "successful" value.</typeparam>
   public readonly struct EFailable<TValue>
   {
      #region State

      private readonly TValue _value;
      private readonly Exception _error;

      // Property IsSuccess is also "State", and it is relevant for 'Equals(...)'.

      #endregion State

      #region Construction

      /// <summary>Creates an <see cref="EFailable{TValue}"/> with a "successful" <paramref name="value"/>, which must not be <see langword="null"/>.</summary>
      /// <exception cref="EFailableSuccessConstructionException">if <paramref name="value"/> == <see langword="null"/>.</exception>
      public static EFailable<TValue> Success( TValue value )
      {
         if( ReferenceEquals( value, null ) ) throw new EFailableSuccessConstructionException( typeValue: typeof( TValue ) );
         return new EFailable<TValue>( true, value, default );
      }

      /// <summary>Creates an <see cref="EFailable{TValue}"/> with a <paramref name="failure"/>-exception, which must not be <see langword="null"/>.</summary>
      /// <exception cref="EFailableFailureConstructionException">if <paramref name="failure"/> == <see langword="null"/>.</exception>
      public static EFailable<TValue> Failure( Exception failure )
      {
         if( ReferenceEquals( failure, null ) ) throw new EFailableFailureConstructionException( typeValue: typeof( TValue ) );
         return new EFailable<TValue>( false, default, failure );
      }

      private EFailable( bool hasValue, TValue value, Exception failure )
      {
         IsSuccess = hasValue;
         _value = value;
         _error = failure;
      }

      #endregion Construction

      #region Properties

      /// <summary><see langword="true"/>, if this instance is a "success", aka has a value of type <typeparamref name="TValue"/>.</summary>
      public bool IsSuccess { get; }

      /// <summary><see langword="true"/>, if this instance is a "failure", aka wraps an <see cref="Exception"/>.</summary>
      public bool IsFailure => !IsSuccess;

      #endregion Properties

      #region Mapping

      /// <summary>
      /// Maps this instance by using its "successful" value (if any) as an argument for <paramref name="func"/>
      /// and returning an <see cref="EFailable{TNewValue}"/> created from the result.
      /// <para><paramref name="func"/> is only called if <see cref="IsSuccess"/> == <see langword="true"/>.</para>
      /// <para>Returns <see cref="EFailable{TNewValue}.Failure(Exception)"/> if <see cref="IsFailure"/> == <see langword="true"/>
      ///       with this instance's internal exception being unchanged.</para>
      /// </summary>
      /// <typeparam name="TNewValue">The type of the new "successful" value.</typeparam>
      /// <param name="func">The delegate that provides the new value.</param>
      public EFailable<TNewValue> Map<TNewValue>( Func<TValue, TNewValue> func ) =>
         IsSuccess ? EFailable<TNewValue>.Success( func( _value ) ) : EFailable<TNewValue>.Failure( _error );

      /// <summary>
      /// Maps this instance by using its "successful" value (if any) as an argument for <paramref name="func"/>
      /// and returning the result as a "flat" <see cref="EFailable{TNewValue}"/> (instead of a "EFailable of a EFailable").
      /// <para><paramref name="func"/> is only called if <see cref="IsSuccess"/> == <see langword="true"/>.</para>
      /// <para>Returns <see cref="EFailable{TNewValue}.Failure(Exception)"/> if <see cref="IsFailure"/> == <see langword="true"/>
      ///       or it is the result of <paramref name="func"/>.</para>
      /// </summary>
      /// <typeparam name="TNewValue">The type of the new "successful" value.</typeparam>
      /// <param name="func">The delegate that provides the new value which may fail.</param>
      public EFailable<TNewValue> FlatMap<TNewValue>( Func<TValue, EFailable<TNewValue>> func ) =>
         IsSuccess ? func( _value ) : EFailable<TNewValue>.Failure( _error );

      /// <summary><see cref="EFailable{TValue}"/>-compatible equality-check for "successful" values.</summary>
      /// <param name="value">"Successful" value to check for equality.</param>
      /// <returns>
      /// <see langword="true"/> iff <see cref="IsSuccess"/> == <see langword="true"/>
      /// <c>and</c> the <see cref="object.Equals(object)"/> override of this instance's value
      /// returns <see langword="true"/> for <paramref name="value"/>, otherwise <see langword="false"/>.
      /// </returns>
      public bool Is( TValue value ) => IsSuccess && _value.Equals( value );

      /// <summary>Monadic predicate check for "successful" values.</summary>
      /// <param name="predicate">The delegate that checks the predicate.</param>
      /// <returns>
      /// <see langword="true"/> iff <see cref="IsSuccess"/> == <see langword="true"/>
      /// <c>and</c> <paramref name="predicate"/> returns <see langword="true"/> for this
      /// instance's "successful" value, otherwise <see langword="false"/>.
      /// </returns>
      public bool Holds( Func<TValue, bool> predicate ) => IsSuccess && predicate( _value );

      #endregion Mapping

      #region Accessing Success

      /// <summary>
      /// Returns this instance's "successful" value if <see cref="IsSuccess"/> == <see langword="true"/>,
      /// otherwise <paramref name="otherwise"/>.
      /// </summary>
      /// <param name="otherwise">The desired value if this instance represents a "failure".</param>
      public TValue Or( TValue otherwise ) => IsSuccess ? _value : otherwise;

      /// <summary>
      /// Returns this instance's "successful" value if <see cref="IsSuccess"/> == <see langword="true"/>,
      /// otherwise throws an <see cref="EFailableFailureException"/>.
      /// </summary>
      /// <exception cref="EFailableFailureException"/>
      public TValue OrThrow() => IsSuccess ? _value : throw new EFailableFailureException( typeValue: typeof( TValue ) );

      /// <summary>
      /// Returns this instance's "successful" value if <see cref="IsSuccess"/> == <see langword="true"/>,
      /// otherwise throws an <see cref="EFailableFailureException"/> with the message <paramref name="msg"/>.
      /// </summary>
      /// <exception cref="EFailableFailureException"/>
      #pragma warning disable CS0618 // EFailableFailureException( string ) is marked obsolete to discourage explicit use outside of this class.
      public TValue OrThrow( string msg ) => IsSuccess ? _value : throw new EFailableFailureException( customMessage: msg );
      #pragma warning restore CS0618 // EFailableFailureException( string ) obsolete

      /// <summary>
      /// Returns this instance's "successful" value if <see cref="IsSuccess"/> == <see langword="true"/>,
      /// otherwise throws an <see cref="EFailableFailureException"/> with the formatted message <paramref name="msg"/>
      /// using the message arguments <paramref name="args"/>.
      /// </summary>
      /// <exception cref="EFailableFailureException"/>
      public TValue OrThrow( string msg, params string[] args ) =>
         #pragma warning disable CS0618 // EFailableFailureException( string ) is marked obsolete to discourage explicit use outside of this class.
         IsSuccess ? _value : throw new EFailableFailureException( customMessage: string.Format( msg, args ) );
         #pragma warning restore CS0618 // EFailableFailureException( string ) obsolete

      /// <summary>
      /// Returns this instance's "successful" value if <see cref="IsSuccess"/> == <see langword="true"/>,
      /// otherwise throws the given exception <paramref name="e"/>.
      /// </summary>
      /// <param name="e">The desired exception if this instance represents a "failure".</param>
      /// <exception cref="Exception"/>
      public TValue OrThrow( Exception e ) => IsSuccess ? _value : throw e;

      /// <summary>
      /// Writes this instance's "successful" value into the <see langword="out"/> parameter <paramref name="value"/>
      /// and returns <see langword="true"/> if <see cref="IsSuccess"/> == <see langword="true"/>,
      /// otherwise <paramref name="value"/> will be set to the <see langword="default"/> value of <typeparamref name="TValue"/>
      /// and the method returns <see langword="false"/>.
      /// </summary>
      /// <param name="value"><see langword="out"/> parameter for this instance's "successful" value.</param>
      /// <remarks>Not particularly functional, but a concession to typical .NET methods.</remarks>
      public bool TryGetValue( out TValue value )
      {
         value = IsSuccess ? _value : default;
         return IsSuccess;
      }

      #endregion Accessing Success

      #region Accessing Failure

      /// <summary>
      /// Returns this instance's "failed" value if <see cref="IsFailure"/> == <see langword="true"/>,
      /// otherwise throws an <see cref="EFailableNoFailureException"/>.
      /// </summary>
      /// <exception cref="EFailableNoFailureException"/>
      // There is **EXPLICITLY** no short-hand method to throw the inner exception, as that would easily lead to mistakes!
      public Exception FailureOrThrow() => IsFailure ? _error : throw new EFailableNoFailureException( typeof( TValue ) );

      /// <summary>
      /// Writes this instance's "failed" value into the <see langword="out"/> parameter <paramref name="exception"/>
      /// and returns <see langword="true"/> if <see cref="IsFailure"/> == <see langword="true"/>,
      /// otherwise <paramref name="exception"/> will be set to the <see langword="null"/> and the method returns <see langword="false"/>.
      /// </summary>
      /// <param name="exception"><see langword="out"/> parameter for this instance's internal exception.</param>
      /// <remarks>Not particularly functional, but a concession to typical .NET methods.</remarks>
      public bool TryGetFailure( out Exception exception )
      {
         exception = IsFailure ? _error : null;
         return IsFailure;
      }

      #endregion Accessing Failure

      #region Converters

      /// <summary>Converts this instance into a <see cref="Maybe{TValue}"/>.</summary>
      /// <returns><see cref="Maybe{TValue}.Some(TValue)"/> if <see cref="IsSuccess"/> == <see langword="true"/> for
      /// this instance and <see cref="Maybe{TValue}.None"/> otherwise, thus *LOSING* the "failed" value of this instance.</returns>
      public Maybe<TValue> ToMaybe() => IsSuccess ? Maybe<TValue>.Some( _value ) : Maybe<TValue>.None;

      /// <summary>Converts this instance into an appropriate <see cref="Failable{TValue, Exception}"/></summary>
      /// <returns><see cref="Failable{TValue, Exception}.Success(TValue)"/> if <see cref="IsSuccess"/> == <see langword="true"/> for
      /// this instance and <see cref="Failable{TValue, Exception}.Failure(Exception)"/> otherwise.</returns>
      public Failable<TValue, Exception> ToFailable() => IsSuccess ? Failable<TValue, Exception>.Success( _value ) : Failable<TValue, Exception>.Failure( _error );

      /// <summary>Converts this instance into an appropriate <see cref="Option{TValue, Exception}"/>.</summary>
      /// <returns><see cref="Option{TValue, Exception}.Some(TValue)"/> if <see cref="IsSuccess"/> == <see langword="true"/> for this instance and
      /// <see cref="Option{TValue, Exception}.Failure(Exception)"/> otherwise.</returns>
      public Option<TValue, Exception> ToOption() => IsSuccess ? Option<TValue, Exception>.Some( _value ) : Option<TValue, Exception>.Failure( _error );

      /// <summary>Converts this instance into an appropriate <see cref="EOption{TValue}"/>.</summary>
      /// <returns><see cref="EOption{TValue}.Some(TValue)"/> if <see cref="IsSuccess"/> == <see langword="true"/> for this instance and
      /// <see cref="EOption{TValue}.Failure(Exception)"/> otherwise.</returns>
      public EOption<TValue> ToEOption() => IsSuccess ? EOption<TValue>.Some( _value ) : EOption<TValue>.Failure( _error );

      #endregion Converters

      #region Object

      ///<summary>
      /// Custom implementation of <see cref="object.ToString()"/>, wrapping the corresponding call to this instance's value,
      /// be it a "success" or a "failure".
      ///</summary>
      public override string ToString() =>
         IsSuccess //both success and failed value are never null!
         ? $"[EFailable<{typeof( TValue ).Name}>.Success: { _value }]"
         : $"[EFailable<{typeof( TValue ).Name}>.Failure: { _error }]";

      ///<summary>
      /// Returns <see langword="true"/> iff <see cref="IsSuccess"/> returns the same boolean for both objects
      /// and the method <see cref="object.Equals(object, object)"/> returns <see langword="true"/>
      /// for the two instances' "successful" values in case of <see cref="IsSuccess"/> being <see langword="true"/>
      /// and for the two instances' internal exceptions in case of <see cref="IsSuccess"/> being <see langword="false"/>.
      ///</summary>
      public override bool Equals( object obj ) =>
         ( obj is EFailable<TValue> other )
            && ( IsSuccess == other.IsSuccess )
            && ( IsFailure == other.IsFailure )
            && ( IsFailure || Equals( _value, other._value ) )
            && ( IsSuccess || Equals( _error, other._error ) );

      /// <summary>
      /// Custom implementation of <see cref="object.GetHashCode()"/>, wrapping a call to this instance's value,
      /// be it a "success" or a "failure".
      /// </summary>
      public override int GetHashCode() => IsSuccess ? _value.GetHashCode() : _error.GetHashCode();

      #endregion Object
   }
}