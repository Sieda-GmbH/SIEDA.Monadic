using System;

namespace SIEDA.Monadic
{
   /// <summary>
   /// Represents the "result" of an operation that might have failed, containing either a value of type 
   /// <typeparamref name="TValue"/> if the operation "was successful" or a different value of type 
   /// <typeparamref name="TFail"/> if the operation "was a failure". Neither of these are ever <see langword="null"/>.
   /// </summary>
   /// <typeparam name="TValue">The type of the "successful" value.</typeparam>
   /// <typeparam name="TFail">The type of the "failed" value.</typeparam>
   public readonly struct Failable<TValue, TFail>
   {
      #region State

      private readonly TValue _value;
      private readonly TFail _error;

      // Property IsSuccess is also "State", and it is relevant for 'Equals(...)'.

      #endregion State

      #region Construction

      /// <summary>Creates a <see cref="Failable{TValue, TFail}"/> with a "successful" <paramref name="value"/>, which must not be <see langword="null"/>.</summary>
      /// <exception cref="FailableSuccessConstructionException">if <paramref name="value"/> == <see langword="null"/>.</exception>
      public static Failable<TValue, TFail> Success( TValue value )
      {
         if( ReferenceEquals( value, null ) ) throw new FailableSuccessConstructionException( typeValue: typeof( TValue ), typeFailure: typeof( TFail ) );
         return new Failable<TValue, TFail>( true, value, default );
      }

      /// <summary>Creates a <see cref="Failable{TValue, TFail}"/> with a <paramref name="failure"/>-value, which must not be <see langword="null"/>.</summary>
      /// <exception cref="FailableFailureConstructionException">if <paramref name="failure"/> == <see langword="null"/>.</exception>
      public static Failable<TValue, TFail> Failure( TFail failure )
      {
         if( ReferenceEquals( failure, null ) ) throw new FailableFailureConstructionException( typeValue: typeof( TValue ), typeFailure: typeof( TFail ) );
         return new Failable<TValue, TFail>( false, default, failure );
      }

      private Failable( bool hasValue, TValue value, TFail failure )
      {
         IsSuccess = hasValue;
         _value = value;
         _error = failure;
      }

      #endregion Construction

      #region Properties

      /// <summary><see langword="true"/>, if this instance is a "success", aka has a value of type <typeparamref name="TValue"/>.</summary>
      public bool IsSuccess { get; }

      /// <summary><see langword="true"/>, if this instance is a "failure", aka has a value of type <typeparamref name="TFail"/>.</summary>
      public bool IsFailure => !IsSuccess;

      #endregion Properties

      #region Mapping

      /// <summary>
      /// Maps this instance by using its "successful" value (if any) as an argument for <paramref name="func"/>
      /// and returning a <see cref="Failable{TNewValue, TFail}"/> created from the result.
      /// <para><paramref name="func"/> is only called if <see cref="IsSuccess"/> == <see langword="true"/>.</para>
      /// <para>Returns <see cref="Failable{TNewValue, TFail}.Failure(TFail)"/> if <see cref="IsFailure"/> == <see langword="true"/>
      ///       with this instance's "failed" value being unchanged.</para>
      /// </summary>
      /// <typeparam name="TNewValue">The type of the new "successful" value.</typeparam>
      /// <param name="func">The delegate that provides the new value.</param>
      public Failable<TNewValue, TFail> Map<TNewValue>( Func<TValue, TNewValue> func ) =>
         IsSuccess ? Failable<TNewValue, TFail>.Success( func( _value ) ) : Failable<TNewValue, TFail>.Failure( _error );

      /// <summary>
      /// Maps this instance by using its "successful" value (if any) as an argument for <paramref name="func"/>
      /// and returning the result as a "flat" <see cref="Failable{TNewValue, TFail}"/> (instead of a "Failable of a Failable").
      /// <para><paramref name="func"/> is only called if <see cref="IsSuccess"/> == <see langword="true"/>.</para>
      /// <para>Returns <see cref="Failable{TNewValue, TFail}.Failure(TFail)"/> if <see cref="IsFailure"/> == <see langword="true"/>
      ///       or it is the result of <paramref name="func"/>.</para>
      /// </summary>
      /// <typeparam name="TNewValue">The type of the new "successful" value.</typeparam>
      /// <param name="func">The delegate that provides the new value which may fail.</param>
      public Failable<TNewValue, TFail> FlatMap<TNewValue>( Func<TValue, Failable<TNewValue, TFail>> func ) =>
         IsSuccess ? func( _value ) : Failable<TNewValue, TFail>.Failure( _error );

      /// <summary><see cref="Failable{TValue, TFail}"/>-compatible equality-check for "successful" values.</summary>
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
      /// <c>and</c> <paramref name="predicate"/> returns <see langword="true"/> for this instance's "successful"
      /// value, otherwise <see langword="false"/>.
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
      /// otherwise throws a <see cref="FailableFailureException"/>.
      /// </summary>
      /// <exception cref="FailableFailureException"/>
      public TValue OrThrow() => IsSuccess ? _value : throw new FailableFailureException( typeValue: typeof( TValue ), typeFailure: typeof( TFail ) );

      /// <summary>
      /// Returns this instance's "successful" value if <see cref="IsSuccess"/> == <see langword="true"/>,
      /// otherwise throws a <see cref="FailableFailureException"/> with the message <paramref name="msg"/>.
      /// </summary>
      /// <exception cref="FailableFailureException"/>
      #pragma warning disable CS0618 // FailableFailureException( string ) is marked obsolete to discourage explicit use outside of this class.
      public TValue OrThrow( string msg ) => IsSuccess ? _value : throw new FailableFailureException( customMessage: msg );
      #pragma warning restore CS0618 // FailableFailureException( string ) obsolete

      /// <summary>
      /// Returns this instance's "successful" value if <see cref="IsSuccess"/> == <see langword="true"/>,
      /// otherwise throws a <see cref="FailableFailureException"/> with the formatted message <paramref name="msg"/>
      /// using the message arguments <paramref name="args"/>.
      /// </summary>
      /// <exception cref="FailableFailureException"/>
      public TValue OrThrow( string msg, params string[] args ) =>
         #pragma warning disable CS0618 // FailableFailureException( string ) is marked obsolete to discourage explicit use outside of this class.
         IsSuccess ? _value : throw new FailableFailureException( customMessage: string.Format( msg, args ) );
         #pragma warning restore CS0618 // FailableFailureException( string ) obsolete

      /// <summary>
      /// Returns this instance's "successful" value if <see cref="IsSuccess"/> == <see langword="true"/>,
      /// otherwise throws the exception <paramref name="e"/>.
      /// </summary>
      /// <param name="e">The desired exception if this instance represents a "failure".</param>
      /// <exception cref="Exception"/>
      public TValue OrThrow( Exception e ) => IsSuccess ? _value : throw e;

      /// <summary>
      /// <para>Returns this instance's "successful" value if <see cref="IsSuccess"/> == <see langword="true"/>,
      /// otherwise <paramref name="otherwiseFunc"/> applied to the "failed" value.</para>
      /// <para>This method is exclusive to <see cref="Failable{TValue, TFail}"/> since it only makes sense
      /// for this class. Its usage should be carefully considered.</para>
      /// </summary>
      /// <param name="otherwiseFunc">The desired value if this instance represents a "failure".</param>
      public TValue OrUse( Func<TFail, TValue> otherwiseFunc ) => IsSuccess ? _value : otherwiseFunc( _error );

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
      /// otherwise throws a <see cref="FailableNoFailureException"/>.
      /// </summary>
      /// <exception cref="FailableNoFailureException"/>
      public TFail FailureOrThrow() => IsFailure ? _error : throw new FailableNoFailureException( typeof( TValue ), typeof( TFail ) );

      /// <summary>
      /// Writes this instance's "failed" value into the <see langword="out"/> parameter <paramref name="value"/>
      /// and returns <see langword="true"/> if <see cref="IsFailure"/> == <see langword="true"/>,
      /// otherwise <paramref name="value"/> will be set to the <see langword="default"/> value of <typeparamref name="TFail"/>
      /// and the method returns <see langword="false"/>.
      /// </summary>
      /// <param name="value"><see langword="out"/> parameter for this instance's "failed" value.</param>
      /// <remarks>Not particularly functional, but a concession to typical .NET methods.</remarks>
      public bool TryGetFailure( out TFail value )
      {
         value = IsFailure ? _error : default;
         return IsFailure;
      }

      #endregion Accessing Failure

      #region Converters

      /// <summary>Converts this instance into a <see cref="Maybe{TValue}"/>.</summary>
      /// <returns><see cref="Maybe{TValue}.Some(TValue)"/> if <see cref="IsSuccess"/> == <see langword="true"/> for
      /// this instance and <see cref="Maybe{TValue}.None"/> otherwise, thus *LOSING* the "failed" value of this instance.</returns>
      public Maybe<TValue> ToMaybe() => IsSuccess ? Maybe<TValue>.Some( _value ) : Maybe<TValue>.None;

      /// <summary>
      /// <para>Converts this instance into an appropriate <see cref="EFailable{TValue}"/>.</para>
      /// <para>Note that any "failed" value this instance might have is lost in the conversion
      ///       and the exception given via <paramref name="error"/> is used instead.</para>
      /// </summary>
      /// <returns><see cref="EFailable{TValue}.Success(TValue)"/> if <see cref="IsSuccess"/> == <see langword="true"/> for
      /// this instance and <see cref="EFailable{TValue}.Failure(Exception)"/> containing <paramref name="error"/> otherwise.</returns>
      public EFailable<TValue> ToEFailable( Exception error ) => IsSuccess ? EFailable<TValue>.Success( _value ) : EFailable<TValue>.Failure( error );

      /// <summary>Converts this instance into an appropriate <see cref="Option{TValue, TFail}"/>.</summary>
      /// <returns><see cref="EOption{TValue}.Some(TValue)"/> if <see cref="IsSuccess"/> == <see langword="true"/> for this instance and
      /// <see cref="Option{TValue, TFail}.Failure(TFail)"/> otherwise.</returns>
      public Option<TValue, TFail> ToOption() => IsSuccess ? Option<TValue, TFail>.Some( _value ) : Option<TValue, TFail>.Failure( _error );

      /// <summary>
      /// <para>Converts this instance into an appropriate <see cref="EOption{TValue}"/>.</para>
      /// <para>Note that any "failed" value this instance might have is lost in the conversion
      ///       and the exception given via <paramref name="error"/> is used instead.</para>
      /// </summary>
      /// <returns><see cref="EOption{TValue}.Some(TValue)"/> if <see cref="IsSuccess"/> == <see langword="true"/> for this instance and
      /// <see cref="EOption{TValue}.Failure(Exception)"/> containing <paramref name="error"/> otherwise.</returns>
      public EOption<TValue> ToEOption( Exception error ) => IsSuccess ? EOption<TValue>.Some( _value ) : EOption<TValue>.Failure( error );

      #endregion Converters

      #region Object

      ///<summary>
      /// Custom implementation of <see cref="object.ToString()"/>, wrapping the corresponding call to this instance's value,
      /// be it a "success" or a "failure".
      ///</summary>
      public override string ToString() =>
         IsSuccess //neither 'value' nor 'failure' is ever null!
         ? $"[Failable<{typeof( TValue ).Name}, {typeof( TFail ).Name}>.Success: { _value }]"
         : $"[Failable<{typeof( TValue ).Name}, {typeof( TFail ).Name}>.Failure: { _error }]";

      ///<summary>
      /// Returns <see langword="true"/> iff <see cref="IsSuccess"/> returns the same boolean for both objects
      /// and the method <see cref="object.Equals(object, object)"/> returns <see langword="true"/>
      /// for the two instances' "successful" values in case of <see cref="IsSuccess"/> being <see langword="true"/>
      /// and for the two instances' "failed" values in case of <see cref="IsSuccess"/> being <see langword="false"/>.
      ///</summary>
      public override bool Equals( object obj ) =>
         ( obj is Failable<TValue, TFail> other )
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