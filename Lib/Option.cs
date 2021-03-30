using System;

namespace SIEDA.Monadic
{
   /// <summary>
   /// <para>
   /// A sort of combination of <see cref="Maybe{TValue}"/> and <see cref="Failable{TValue, TFail}"/>:
   /// Represents the "result" of an operation that might have failed with a value that may be absent even if the operation did not fail.
   /// </para>
   /// <para>
   /// If the operation did not fail, there may be either "some" value of type <typeparamref name="TValue"/> or no value ("none (of the
   /// possible values)") without a failure. If the operation was indeed a "failure", there is a value of type <typeparamref name="TFail"/>.
   /// </para>
   /// </summary>
   /// <typeparam name="TValue">The type of the value.</typeparam>
   /// <typeparam name="TFail">The type of the "failed" value.</typeparam>
   public readonly struct Option<TValue, TFail>
   {
      #region State

      private readonly TValue _value;
      private readonly TFail _error;

      // Properties IsSome and IsFailure are also "State", and they are relevant for 'Equals(...)'.

      #endregion State

      #region Construction

      /// <summary>Empty instance, no value present, value absent.</summary>
      public static readonly Option<TValue, TFail> None = new Option<TValue, TFail>( hasValue: false, hasError: false, default, default );

      /// <summary>Creates an <see cref="Option{TValue, TFail}"/> with value <paramref name="value"/>.</summary>
      /// <exception cref="OptionSomeConstructionException">if <paramref name="value"/> == <see langword="null"/>.</exception>
      public static Option<TValue, TFail> Some( TValue value )
      {
         if( ReferenceEquals( value, null ) ) throw new OptionSomeConstructionException( typeValue: typeof( TValue ), typeFailure: typeof( TFail ) );
         return new Option<TValue, TFail>( hasValue: true, hasError: false, value, default );
      }

      /// <summary>Creates an <see cref="Option{TValue, TFail}"/> with the value of the <see cref="Nullable{T}"/> <paramref name="nullableValue"/>.</summary>
      /// <exception cref="OptionSomeConstructionException">if <paramref name="nullableValue"/> == <see langword="null"/> (has no value).</exception>
      public static Option<TValue, TFail> Some<T>( T? nullableValue ) where T : struct, TValue
      {
         if( !nullableValue.HasValue ) throw new OptionSomeConstructionException( typeValue: typeof( TValue ), typeFailure: typeof( TFail ) );
         return new Option<TValue, TFail>( hasValue: true, hasError: false, nullableValue.Value, default );
      }

      /// <summary>
      /// <para>Creates and returns an <see cref="Option{TValue, TFail}"/> with value <paramref name="value"/> if <paramref name="value"/> != <see langword="null"/>.</para>
      /// <para>Returns <see cref="None"/> if <paramref name="value"/> == <see langword="null"/>.</para>
      /// </summary>
      public static Option<TValue, TFail> From( TValue value ) =>
         ReferenceEquals( value, null ) ? None : new Option<TValue, TFail>( hasValue: true, hasError: false, value, default );

      /// <summary>
      /// <para>Creates and returns an <see cref="Option{TValue, TFail}"/> with the value of the <see cref="Nullable{T}"/> <paramref name="nullableValue"/>
      ///       if <paramref name="nullableValue"/> != <see langword="null"/> (has a value).</para>
      /// <para>Returns <see cref="None"/> if <paramref name="nullableValue"/> == <see langword="null"/> (has no value).</para>
      /// </summary>
      public static Option<TValue, TFail> From<T>( T? nullableValue ) where T : struct, TValue =>
          nullableValue.HasValue ? new Option<TValue, TFail>( hasValue: true, hasError: false, nullableValue.Value, default ) : None;

      /// <summary>Creates an <see cref="Option{TValue, TFail}"/> with a <paramref name="failure"/>-value, which must not be <see langword="null"/>.</summary>
      /// <exception cref="OptionFailureConstructionException">if <paramref name="failure"/> == <see langword="null"/>.</exception>
      public static Option<TValue, TFail> Failure( TFail failure )
      {
         if( ReferenceEquals( failure, null ) ) throw new OptionFailureConstructionException( typeValue: typeof( TValue ), typeFailure: typeof( TFail ) );
         return new Option<TValue, TFail>( hasValue: false, hasError: true, default, failure );
      }

      private Option( bool hasValue, bool hasError, TValue value, TFail error )
      {
         IsSome = hasValue;
         IsFailure = hasError;
         _value = value;
         _error = error;
      }

      #endregion Construction

      #region Properties

      /// <summary><see langword="true"/>, if this instance has a value, is "some", its value is present.</summary>
      public bool IsSome { get; }

      /// <summary><see langword="true"/>, if this instance is <see cref="None"/>, meaning it has no value, its value is absent, but it is still no failure.</summary>
      public bool IsNone => !IsSome && !IsFailure;

      /// <summary><see langword="true"/>, if this instance is a "failure", aka has a value of type <typeparamref name="TFail"/>.</summary>
      public bool IsFailure { get; }

      /// <summary><see langword="true"/>if this instance has no value, meaning either <see cref="IsNone" /> or <see cref="IsFailure" /> is <see langword="true"/>.</summary>
      public bool IsNotSome => !IsSome;

      /// <summary><see langword="true"/>, if this instance is no "failure", meaning either <see cref="IsSome" /> or <see cref="IsNone" /> is <see langword="true"/>.</summary>
      public bool IsNotFailure => !IsFailure;

      #endregion Properties

      #region Mapping

      /// <summary>
      /// Maps this instance by using its value (if any) as an argument for <paramref name="func"/>
      /// and returning an <see cref="Option{TNewValue, TFail}"/> created from the result.
      /// <para><paramref name="func"/> is only called if <see cref="IsSome"/> == <see langword="true"/>.</para>
      /// <para>Returns <see cref="Option{TNewValue, TFail}.None"/> if <see cref="IsNone"/> == <see langword="true"/>
      ///       or the result of <paramref name="func"/> == <see langword="null"/>.</para>
      /// <para>Returns <see cref="Option{TNewValue, TFail}.Failure(TFail)"/> if <see cref="IsFailure"/> == <see langword="true"/>
      ///       with this instance's "failed" value being unchanged.</para>
      /// </summary>
      /// <typeparam name="TNewValue">The type of the new value.</typeparam>
      /// <param name="func">The delegate that provides the new value.</param>
      public Option<TNewValue, TFail> Map<TNewValue>( Func<TValue, TNewValue> func ) =>
         IsSome
            ? Option<TNewValue, TFail>.From( func( _value ) )
            : IsNone ? Option<TNewValue, TFail>.None : Option<TNewValue, TFail>.Failure( _error );

      /// <summary>
      /// Maps this instance by using its value (if any) as an argument for <paramref name="func"/>
      /// and returning the result as a "flat" <see cref="Option{TNewValue, TFail}"/> (instead of an "Option of an Option").
      /// <para><paramref name="func"/> is only called if <see cref="IsSome"/> == <see langword="true"/>.</para>
      /// <para>Returns <see cref="Option{TNewValue, TFail}.None"/> if <see cref="IsNone"/> == <see langword="true"/>
      ///       or it is the result of <paramref name="func"/>.</para>
      /// <para>Returns <see cref="Option{TNewValue, TFail}.Failure(TFail)"/> if <see cref="IsFailure"/> == <see langword="true"/>
      ///       or it is the result of <paramref name="func"/>.</para>
      /// </summary>
      /// <typeparam name="TNewValue">The type of the new value.</typeparam>
      /// <param name="func">The delegate that provides the new value which may fail or result in an absent value.</param>
      public Option<TNewValue, TFail> FlatMap<TNewValue>( Func<TValue, Option<TNewValue, TFail>> func ) =>
         IsSome ? func( _value ) : IsNone ? Option<TNewValue, TFail>.None : Option<TNewValue, TFail>.Failure( _error );

      /// <summary><see cref="Option{TValue, TFail}"/>-compatible equality-check for values.</summary>
      /// <param name="value">Value to check for equality.</param>
      /// <returns>
      /// <see langword="true"/> iff <see cref="IsSome"/> == <see langword="true"/>
      /// <c>and</c> the <see cref="object.Equals(object)"/> override of this instance's value
      /// returns <see langword="true"/> for <paramref name="value"/>, otherwise <see langword="false"/>.
      /// </returns>
      public bool Is( TValue value ) => IsSome && _value.Equals( value );

      /// <summary>Monadic predicate check for values.</summary>
      /// <param name="predicate">The delegate that checks the predicate.</param>
      /// <returns>
      /// <see langword="true"/> iff <see cref="IsSome"/> == <see langword="true"/>
      /// <c>and</c> <paramref name="predicate"/> returns <see langword="true"/> for this instance's value,
      /// otherwise <see langword="false"/>.
      /// </returns>
      public bool Holds( Func<TValue, bool> predicate ) => IsSome && predicate( _value );

      #endregion Mapping

      #region Accessing Value

      private TValue ThrowNotSome()
      { //the return-type is just a hack, this method NEVER returns anything
         if( IsFailure ) throw new OptionFailureException( typeValue: typeof( TValue ), typeFailure: typeof( TFail ) );
         else throw new OptionNoneException( typeValue: typeof( TValue ), typeFailure: typeof( TFail ) );
      }

      private TValue ThrowNotSome( string customMessage )
      { //again, return type is just a hack
         #pragma warning disable CS0618 // OptionFailureException( string ) and OptionNoneException( string ) are marked obsolete to discourage explicit use outside of this class.
         if( IsFailure ) throw new OptionFailureException( customMessage: customMessage );
         else throw new OptionNoneException( customMessage: customMessage );
         #pragma warning restore CS0618 // OptionFailureException( string ) and OptionNoneException( string ) obsolete
      }

      /// <summary>
      /// Returns this instance's value if <see cref="IsSome"/> == <see langword="true"/>,
      /// otherwise <paramref name="otherwise"/>.
      /// </summary>
      /// <param name="otherwise">The desired value if this instance has no value (with or without failure).</param>
      public TValue Or( TValue otherwise ) => IsSome ? _value : otherwise;

      /// <summary>
      /// Returns this instance's value if <see cref="IsSome"/> == <see langword="true"/>,
      /// otherwise throws an <see cref="OptionNoneException"/> if <see cref="IsNone"/> == <see langword="true"/>
      /// or an <see cref="OptionFailureException"/> if <see cref="IsFailure"/> == <see langword="true"/>.
      /// </summary>
      /// <exception cref="OptionNoneException"/>
      /// <exception cref="OptionFailureException"/>
      public TValue OrThrow() => IsSome ? _value : ThrowNotSome();

      /// <summary>
      /// Returns this instance's value if <see cref="IsSome"/> == <see langword="true"/>,
      /// otherwise throws an <see cref="OptionNoneException"/> if <see cref="IsNone"/> == <see langword="true"/>
      /// or an <see cref="OptionFailureException"/> if <see cref="IsFailure"/> == <see langword="true"/>,
      /// both with the message <paramref name="msg"/>.
      /// </summary>
      /// <exception cref="OptionNoneException"/>
      /// <exception cref="OptionFailureException"/>
      public TValue OrThrow( string msg ) => IsSome ? _value : ThrowNotSome( msg );

      /// <summary>
      /// Returns this instance's value if <see cref="IsSome"/> == <see langword="true"/>,
      /// otherwise throws an <see cref="OptionNoneException"/> if <see cref="IsNone"/> == <see langword="true"/>
      /// or an <see cref="OptionFailureException"/> if <see cref="IsFailure"/> == <see langword="true"/>,
      /// both with the formatted message <paramref name="msg"/> using the message arguments <paramref name="args"/>.
      /// </summary>
      /// <exception cref="OptionNoneException"/>
      /// <exception cref="OptionFailureException"/>
      public TValue OrThrow( string msg, params string[] args ) =>
         IsSome ? _value : ThrowNotSome( string.Format( msg, args ) );

      /// <summary>
      /// Returns this instance's value if <see cref="IsSome"/> == <see langword="true"/>,
      /// otherwise throws the exception <paramref name="e"/>.
      /// </summary>
      /// <param name="e">The desired exception if this instance has no value (with or without failure).</param>
      /// <exception cref="Exception"/>
      public TValue OrThrow( Exception e ) => IsSome ? _value : throw e;

      /// <summary>
      /// Writes this instance's value into the <see langword="out"/> parameter <paramref name="value"/>
      /// and returns <see langword="true"/> if <see cref="IsSome"/> == <see langword="true"/>,
      /// otherwise <paramref name="value"/> will be set to the <see langword="default"/> value of <typeparamref name="TValue"/>
      /// and the method returns <see langword="false"/>.
      /// </summary>
      /// <param name="value"><see langword="out"/> parameter for this instance's value.</param>
      /// <remarks>Not particularly functional, but a concession to typical .NET methods.</remarks>
      public bool TryGetValue( out TValue value )
      {
         value = IsSome ? _value : default;
         return IsSome;
      }

      #endregion Accessing Value

      #region Accessing Failure

      /// <summary>
      /// Returns this instance's "failed" value if <see cref="IsFailure"/> == <see langword="true"/>,
      /// otherwise throws a <see cref="OptionNoFailureException"/>.
      /// </summary>
      /// <exception cref="OptionNoFailureException"/>
      public TFail FailureOrThrow() => IsFailure ? _error : throw new OptionNoFailureException( typeof( TValue ), typeof( TFail ) );

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
      /// <returns><see cref="Maybe{TValue}.Some(TValue)"/> if <see cref="IsSome"/> == <see langword="true"/> for
      /// this instance and <see cref="Maybe{TValue}.None"/> otherwise, thus *LOSING* the "failed" value of this instance.</returns>
      public Maybe<TValue> ToMaybe() => IsSome ? Maybe<TValue>.Some( _value ) : Maybe<TValue>.None;

      /// <summary>Converts this instance into an appropriate <see cref="Failable{TValue, TFail}"/>.</summary>
      /// <param name="errorOnNone">An object representing a "failure", used in case <see cref="IsNone"/> == <see langword="true"/></param>
      /// <returns><see cref="Failable{TValue, TFail}.Success(TValue)"/> if <see cref="IsSome"/> == <see langword="true"/> for this instance,
      /// otherwise <see cref="Failable{TValue, TFail}.Failure(TFail)"/> containing
      /// <paramref name="errorOnNone"/> if <see cref="IsNone"/> == <see langword="true"/> or
      /// this instance's "failed" value if <see cref="IsFailure"/> == <see langword="true"/>.</returns>
      public Failable<TValue, TFail> ToFailable( TFail errorOnNone ) =>
         IsSome ? Failable<TValue, TFail>.Success( _value ) : Failable<TValue, TFail>.Failure( IsFailure ? _error : errorOnNone );

      /// <summary>
      /// <para>Converts this instance into an appropriate <see cref="EFailable{TValue}"/>.</para>
      /// <para>Note that any "failed" value this instance might have is lost in the conversion
      ///       and the exception given via <paramref name="error"/> is used instead.</para>
      /// </summary>
      /// <returns><see cref="EFailable{TValue}.Success(TValue)"/> if <see cref="IsSome"/> == <see langword="true"/> for
      /// this instance and <see cref="EFailable{TValue}.Failure(Exception)"/> containing <paramref name="error"/> otherwise.</returns>
      public EFailable<TValue> ToEFailable( Exception error ) => IsSome ? EFailable<TValue>.Success( _value ) : EFailable<TValue>.Failure( error );

      /// <summary>
      /// <para>Converts this instance into an appropriate <see cref="EOption{TValue}"/>.</para>
      /// <para>Note that any "failed" value this instance might have is lost in the conversion
      ///       and the exception given via <paramref name="error"/> is used instead.</para>
      /// </summary>
      /// <returns><see cref="EOption{TValue}.Some(TValue)"/> if <see cref="IsSome"/> == <see langword="true"/> for this instance,
      /// <see cref="EOption{TValue}.None"/> if <see cref="IsNone"/> == <see langword="true"/> for this instance and
      /// <see cref="EOption{TValue}.Failure(Exception)"/> containing <paramref name="error"/>
      /// if <see cref="IsFailure"/> == <see langword="true"/> for this instance.</returns>
      public EOption<TValue> ToEOption( Exception error ) =>
         IsSome ? EOption<TValue>.Some( _value ) : ( IsFailure ? EOption<TValue>.Failure( error ) : EOption<TValue>.None );

      #endregion Converters

      #region Object

      ///<summary>
      /// Custom implementation of <see cref="object.ToString()"/>, wrapping the corresponding call to this instance's value,
      /// if any, be it a "failure" or not.
      ///</summary>
      public override string ToString() =>
              IsSome ? $"[Option<{typeof( TValue ).Name}, {typeof( TFail ).Name}>.Some: {_value}]"
         : IsFailure ? $"[Option<{typeof( TValue ).Name}, {typeof( TFail ).Name}>.Failure: {_error}]"
                     : $"[Option<{typeof( TValue ).Name}, {typeof( TFail ).Name}>.None]";

      ///<summary>
      /// Returns <see langword="true"/> iff <see cref="IsSome"/> returns the same boolean for both objects
      /// and <see cref="IsFailure"/> also returns the same boolean for both objects
      /// and the method <see cref="object.Equals(object, object)"/> returns <see langword="true"/>
      /// for the two instances' values in case of <see cref="IsSome"/> being <see langword="true"/>
      /// and for the two instances' "failed" values in case of <see cref="IsFailure"/> being <see langword="true"/>.
      ///</summary>
      public override bool Equals( object obj ) =>
         ( obj is Option<TValue, TFail> other )
            && ( IsSome == other.IsSome )
            && ( IsFailure == other.IsFailure )
            && ( IsNotSome || Equals( _value, other._value ) )
            && ( IsNotFailure || Equals( _error, other._error ) );

      /// <summary>
      /// <para>Custom implementation of <see cref="object.GetHashCode()"/>, wrapping a call to this instance's value,
      ///       if any, be it a "failure" or not.</para>
      /// <para>Note that all <see cref="Option{TValue, TFail}"/>-objects, regardless of type <typeparamref name="TValue"/>,
      ///       have the same hash code when there is no value and no failure, aka when the instance is <see cref="None"/>.</para>
      /// </summary>
      public override int GetHashCode() => IsSome ? _value.GetHashCode() : IsFailure ? _error.GetHashCode() : int.MinValue;

      #endregion Object
   }
}