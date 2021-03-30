using System;

namespace SIEDA.Monadic
{
   /// <summary>Represents a value that may be present ("is some (value)") or absent ("is none (of the possible values)").</summary>
   /// <typeparam name="T">The type of the value contained within, if any.</typeparam>
   public class Maybe<T>
   {
      #region State

      private readonly T _value;

      // Property IsSome is also "State", and it is relevant for 'Equals(...)'.

      #endregion State

      #region Construction

      /// <summary>Empty instance, no value present, value absent.</summary>
      public static readonly Maybe<T> None = new Maybe<T>( false, default );

      /// <summary>Creates a <see cref="Maybe{T}"/> with value <paramref name="value"/>.</summary>
      /// <exception cref="MaybeSomeConstructionException">if <paramref name="value"/> == <see langword="null"/>.</exception>
      public static Maybe<T> Some( T value )
      {
         if( value == null ) throw new MaybeSomeConstructionException( typeValue: typeof( T ) );
         return new Maybe<T>( true, value );
      }

      /// <summary>Creates a <see cref="Maybe{T}"/> with the value of the <see cref="Nullable{U}"/> <paramref name="nullableValue"/>.</summary>
      /// <exception cref="MaybeSomeConstructionException">if <paramref name="nullableValue"/> == <see langword="null"/> (has no value).</exception>
      public static Maybe<T> Some<U>( U? nullableValue ) where U : struct, T
      {
         if( !nullableValue.HasValue ) throw new MaybeSomeConstructionException( typeValue: typeof( T ) );
         return new Maybe<T>( true, nullableValue.Value );
      }

      /// <summary>
      /// <para>Creates and returns a <see cref="Maybe{T}"/> with value <paramref name="value"/> if <paramref name="value"/> != <see langword="null"/>.</para>
      /// <para>Returns <see cref="None"/> if <paramref name="value"/> == <see langword="null"/>.</para>
      /// </summary>
      public static Maybe<T> From( T value ) => ReferenceEquals( value, null ) ? None : new Maybe<T>( true, value );

      /// <summary>
      /// <para>Creates and returns a <see cref="Maybe{T}"/> with the value of the <see cref="Nullable{U}"/> <paramref name="nullableValue"/>
      ///       if <paramref name="nullableValue"/> != <see langword="null"/> (has a value).</para>
      /// <para>Returns <see cref="None"/> if <paramref name="nullableValue"/> == <see langword="null"/> (has no value).</para>
      /// </summary>
      public static Maybe<T> From<U>( U? nullableValue ) where U : struct, T => nullableValue.HasValue ? new Maybe<T>( true, nullableValue.Value ) : None;

      private Maybe( bool hasValue, T value )
      {
         IsSome = hasValue;
         _value = value;
      }

      #endregion Construction

      #region Properties

      /// <summary><see langword="true"/>, if this instance has a value, is "some", its value is present.</summary>
      public bool IsSome { get; }

      /// <summary><see langword="true"/>, if this instance is <see cref="None"/>, has no value, its value is absent.</summary>
      public bool IsNone => !IsSome;

      #endregion Properties

      #region Mapping

      /// <summary>
      /// Maps this instance by using its value (if any) as an argument for <paramref name="func"/>
      /// and returning a <see cref="Maybe{U}"/> created from the result.
      /// <para><paramref name="func"/> is only called if <see cref="IsSome"/> == <see langword="true"/>.</para>
      /// <para>Returns <see cref="Maybe{U}.None"/> if <see cref="IsNone"/> == <see langword="true"/>
      ///       or the result of <paramref name="func"/> == <see langword="null"/>.</para>
      /// </summary>
      /// <typeparam name="U">The type of the new value.</typeparam>
      /// <param name="func">The delegate that provides the new value.</param>
      public Maybe<U> Map<U>( Func<T, U> func ) => IsSome ? Maybe<U>.From( func( _value ) ) : Maybe<U>.None;

      /// <summary>
      /// Maps this instance by using its value (if any) as an argument for <paramref name="func"/>
      /// and returning the result as a "flat" <see cref="Maybe{U}"/> (instead of a "Maybe of a Maybe").
      /// <para><paramref name="func"/> is only called if <see cref="IsSome"/> == <see langword="true"/>.</para>
      /// <para>Returns <see cref="Maybe{U}.None"/> if <see cref="IsNone"/> == <see langword="true"/>
      ///       or it is the result of <paramref name="func"/>.</para>
      /// </summary>
      /// <typeparam name="U">The type of the new value.</typeparam>
      /// <param name="func">The delegate that provides the new value which may be absent.</param>
      public Maybe<U> FlatMap<U>( Func<T, Maybe<U>> func ) => IsSome ? func( _value ) : Maybe<U>.None;

      /// <summary><see cref="Maybe{T}"/>-compatible equality-check for values.</summary>
      /// <param name="value">Value to check for equality.</param>
      /// <returns>
      /// <see langword="true"/> iff <see cref="IsSome"/> == <see langword="true"/>
      /// <c>and</c> the <see cref="object.Equals(object)"/> override of this instance's value
      /// returns <see langword="true"/> for <paramref name="value"/>, otherwise <see langword="false"/>.
      /// </returns>
      public bool Is( T value ) => IsSome && _value.Equals( value );

      /// <summary>Monadic predicate check for values.</summary>
      /// <param name="predicate">The delegate that checks the predicate.</param>
      /// <returns>
      /// <see langword="true"/> iff <see cref="IsSome"/> == <see langword="true"/>
      /// <c>and</c> <paramref name="predicate"/> returns <see langword="true"/> for this instance's value,
      /// otherwise <see langword="false"/>.
      /// </returns>
      public bool Holds( Func<T, bool> predicate ) => IsSome && predicate( _value );

      #endregion Mapping

      #region Accessing Value

      /// <summary>
      /// Returns this instance's value if <see cref="IsSome"/> == <see langword="true"/>,
      /// otherwise <paramref name="otherwise"/>.
      /// </summary>
      /// <param name="otherwise">The desired value if this instance has no value.</param>
      public T Or( T otherwise ) => IsSome ? _value : otherwise;

      /// <summary>
      /// Returns this instance's value if <see cref="IsSome"/> == <see langword="true"/>,
      /// otherwise throws a <see cref="MaybeNoneException"/>.
      /// </summary>
      /// <exception cref="MaybeNoneException"/>
      public T OrThrow() => IsSome ? _value : throw new MaybeNoneException( typeValue: typeof( T ) );

      /// <summary>
      /// Returns this instance's value if <see cref="IsSome"/> == <see langword="true"/>,
      /// otherwise throws a <see cref="MaybeNoneException"/> with the message <paramref name="msg"/>.
      /// </summary>
      /// <exception cref="MaybeNoneException"/>
      #pragma warning disable CS0618 // MaybeNoneException( string ) is marked obsolete to discourage explicit use outside of this class.
      public T OrThrow( string msg ) => IsSome ? _value : throw new MaybeNoneException( customMessage: msg );
      #pragma warning restore CS0618 // MaybeNoneException( string ) obsolete

      /// <summary>
      /// Returns this instance's value if <see cref="IsSome"/> == <see langword="true"/>,
      /// otherwise throws a <see cref="MaybeNoneException"/> with the formatted message <paramref name="msg"/>
      /// using the message arguments <paramref name="args"/>.
      /// </summary>
      /// <exception cref="MaybeNoneException"/>
      public T OrThrow( string msg, params string[] args ) =>
         #pragma warning disable CS0618 // MaybeNoneException( string ) is marked obsolete to discourage explicit use outside of this class.
         IsSome ? _value : throw new MaybeNoneException( customMessage: string.Format( msg, args ) );
         #pragma warning restore CS0618 // MaybeNoneException( string ) obsolete

      /// <summary>
      /// Returns this instance's value if <see cref="IsSome"/> == <see langword="true"/>,
      /// otherwise throws the exception <paramref name="e"/>.
      /// </summary>
      /// <param name="e">The desired exception if this instance has no value.</param>
      /// <exception cref="Exception"/>
      public T OrThrow( Exception e ) => IsSome ? _value : throw e;

      /// <summary>
      /// Writes this instance's value into the <see langword="out"/> parameter <paramref name="value"/>
      /// and returns <see langword="true"/> if <see cref="IsSome"/> == <see langword="true"/>,
      /// otherwise <paramref name="value"/> will be set to the <see langword="default"/> value of <typeparamref name="T"/>
      /// and the method returns <see langword="false"/>.
      /// </summary>
      /// <param name="value"><see langword="out"/> parameter for this instance's value.</param>
      /// <remarks>Not particularly functional, but a concession to typical .NET methods.</remarks>
      public bool TryGetValue( out T value )
      {
         value = IsSome ? _value : default;
         return IsSome;
      }

      #endregion Accessing Value

      #region Converters

      /// <summary>Converts this instance into an appropriate <see cref="Failable{TValue, TFail}"/>.</summary>
      /// <typeparam name="TFail">The type of "failure" for the new <see cref="Failable{TValue, TFail}"/>.</typeparam>
      /// <param name="error">An object representing a "failure", used in case <see cref="IsSome"/> == <see langword="false"/></param>
      /// <returns><see cref="Failable{TValue, TFail}.Success(TValue)"/> if <see cref="IsSome"/> == <see langword="true"/> for
      /// this instance and <see cref="Failable{TValue, TFail}.Failure(TFail)"/> containing <paramref name="error"/> otherwise.</returns>
      public Failable<T, TFail> ToFailable<TFail>( TFail error ) => IsSome ? Failable<T, TFail>.Success( _value ) : Failable<T, TFail>.Failure( error );

      /// <summary>Converts this instance into an appropriate <see cref="EFailable{TValue}"/>.</summary>
      /// <param name="error">An object representing a "failure", used in case <see cref="IsSome"/> == <see langword="false"/></param>
      /// <returns><see cref="EFailable{TValue}.Success(TValue)"/> if <see cref="IsSome"/> == <see langword="true"/> for
      /// this instance and <see cref="EFailable{TValue}.Failure(Exception)"/> containing <paramref name="error"/> otherwise.</returns>
      public EFailable<T> ToEFailable( Exception error ) => IsSome ? EFailable<T>.Success( _value ) : EFailable<T>.Failure( error );

      /// <summary>
      /// <para>Converts this instance into an appropriate <see cref="Option{TValue, WhateverErrorType}"/>.</para>
      /// <para>Note that <typeparamref name="WhateverErrorType"/> is relevant for typing the new instance only.</para>
      /// </summary>
      /// <typeparam name="WhateverErrorType">The type of "failure" for the new <see cref="Option{TValue, WhateverErrorType}"/>.</typeparam>
      /// <returns><see cref="Option{TValue, TFail}.Some(TValue)"/> if <see cref="IsSome"/> == <see langword="true"/> for
      /// this instance and <see cref="Option{TValue, TFail}.None"/> otherwise.</returns>
      public Option<T, WhateverErrorType> ToOption<WhateverErrorType>() =>
         IsSome ? Option<T, WhateverErrorType>.Some( _value ) : Option<T, WhateverErrorType>.None;

      /// <summary>Converts this instance into an appropriate <see cref="EOption{TValue}"/>.</summary>
      /// <returns><see cref="EOption{TValue}.Some(TValue)"/> if <see cref="IsSome"/> == <see langword="true"/> for
      /// this instance and <see cref="EOption{TValue}.None"/> otherwise.</returns>
      public EOption<T> ToEOption() => IsSome ? EOption<T>.Some( _value ) : EOption<T>.None;

      #endregion Converters

      #region Overrides

      ///<summary>Custom implementation of <see cref="object.ToString()"/>, wrapping a call to this instance's value, if any.</summary>
      public override string ToString() => IsSome ? $"[Maybe<{typeof( T ).Name}>.Some: {_value}]" : $"[Maybe<{typeof( T ).Name}>.None]";

      ///<summary>
      ///Returns <see langword="true"/> iff <see cref="IsSome"/> returns the same boolean for both objects and, in case of that value being
      ///<see langword="true"/>, the method <see cref="object.Equals(object)"/> of the two instance's values also returns <see langword="true"/>.
      ///</summary>
      public override bool Equals( object obj ) =>
         ( obj is Maybe<T> other )
         && ( IsSome == other.IsSome )
         && ( IsNone || _value.Equals( other._value ) );

      /// <summary>
      /// <para>Custom implementation of <see cref="object.GetHashCode()"/>, wrapping a call to this instance's value if any.</para>
      /// <para>Note that all <see cref="Maybe{T}"/>-objects, regardless of type <typeparamref name="T"/>,
      ///       have the same hash code when there is no value, aka when the instance is <see cref="None"/>.</para>
      /// </summary>
      public override int GetHashCode() => IsSome ? _value.GetHashCode() : int.MinValue;

      #endregion Overrides
   }
}