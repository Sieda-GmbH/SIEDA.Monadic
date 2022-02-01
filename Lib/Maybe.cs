using System;
using SIEDA.Monadic.SwitchCase;

namespace SIEDA.Monadic
{
   /// <summary> Represents a value that may be present ("is some (value)") or absent ("is none (of the possible values)"). </summary>
   /// <typeparam name="TValue"> The type of the value contained within, if any. </typeparam>
   public readonly struct Maybe<TValue>
   {
      #region State

      private readonly TValue _value;
      private readonly MybType _type;

      #endregion State

      #region Construction

      /// <summary> Empty instance, no value present, value absent. </summary>
      public static readonly Maybe<TValue> None = new Maybe<TValue>( MybType.None, default );

      /// <summary> Creates a <see cref="Maybe{T}"/> with value <paramref name="value"/>. </summary>
      /// <exception cref="MaybeSomeConstructionException"> if <paramref name="value"/> == <see langword="null"/>. </exception>
      public static Maybe<TValue> Some( TValue value )
      {
         if( value == null )
         {
            throw new MaybeSomeConstructionException( typeValue: typeof( TValue ) );
         }

         return new Maybe<TValue>( MybType.Some, value );
      }

      /// <summary> Creates a <see cref="Maybe{T}"/> with the value of the <see cref="Nullable{U}"/><paramref name="nullableValue"/>. </summary>
      /// <exception cref="MaybeSomeConstructionException"> if <paramref name="nullableValue"/> == <see langword="null"/> (has no value). </exception>
      public static Maybe<TValue> Some<U>( U? nullableValue ) where U : struct, TValue
      {
         if( !nullableValue.HasValue )
         {
            throw new MaybeSomeConstructionException( typeValue: typeof( TValue ) );
         }

         return new Maybe<TValue>( MybType.Some, nullableValue.Value );
      }

      /// <summary>
      /// <para> Creates and returns a <see cref="Maybe{T}"/> with value <paramref name="value"/> if <paramref name="value"/> != <see langword="null"/>. </para>
      /// <para> Returns <see cref="None"/> if <paramref name="value"/> == <see langword="null"/>. </para>
      /// </summary>
      public static Maybe<TValue> From( TValue value ) => ReferenceEquals( value, null ) ? None : new Maybe<TValue>( MybType.Some, value );

      /// <summary>
      /// <para> Creates and returns a <see cref="Maybe{T}"/> with the value of the <see cref="Nullable{U}"/><paramref name="nullableValue"/>
      ///        if <paramref name="nullableValue"/> != <see langword="null"/> (has a value).</para>
      /// <para> Returns <see cref="None"/> if <paramref name="nullableValue"/> == <see langword="null"/> (has no value). </para>
      /// </summary>
      public static Maybe<TValue> From<U>( U? nullableValue ) where U : struct, TValue => nullableValue.HasValue ? new Maybe<TValue>( MybType.Some, nullableValue.Value ) : None;

      private Maybe( MybType mbyType, TValue value )
      {
         _type = mbyType;
         _value = value;
      }

      #endregion Construction

      #region Properties

      /// <summary> Returns an appropriate <see cref="MybType"/> for this instance, useful in case you want to use a switch-case. </summary>
      public MybType Enum { get => _type; }

      /// <summary> <see langword="true"/>, if this instance has a value, aka is "some", its value is present. </summary>
      public bool IsSome { get => _type == MybType.Some; }

      /// <summary> <see langword="true"/>, if this instance is <see cref="None"/>, has no value, its value is absent. </summary>
      public bool IsNone { get => _type == MybType.None; }

      #endregion Properties

      #region Mapping

      /// <summary>
      /// Maps this instance by using its value (if any) as an argument for <paramref name="func"/> and returning a <see cref="Maybe{U}"/> created from the result.
      /// <para> <paramref name="func"/> is only called if <see cref="IsSome"/> == <see langword="true"/>.</para>
      /// <para> Returns <see cref="Maybe{U}.None"/> if <see cref="IsNone"/> == <see langword="true"/> or the result of <paramref name="func"/> == <see langword="null"/>. </para>
      /// </summary>
      /// <typeparam name="U"> The type of the new value. </typeparam>
      /// <param name="func"> The delegate that provides the new value. </param>
      public Maybe<U> Map<U>( Func<TValue, U> func ) => IsSome ? Maybe<U>.From( func( _value ) ) : Maybe<U>.None;

      /// <summary>
      /// Maps this instance by using its value (if any) as an argument for <paramref name="func"/>
      /// and returning the result as a "flat" <see cref="Maybe{U}"/> (instead of a "Maybe of a Maybe").
      /// <para><paramref name="func"/> is only called if <see cref="IsSome"/> == <see langword="true"/>.</para>
      /// <para> Returns <see cref="Maybe{U}.None"/> if <see cref="IsNone"/> == <see langword="true"/> or it is the result of <paramref name="func"/>. </para>
      /// </summary>
      /// <typeparam name="U"> The type of the new value. </typeparam>
      /// <param name="func"> The delegate that provides the new value which may be absent. </param>
      public Maybe<U> FlatMap<U>( Func<TValue, Maybe<U>> func ) => IsSome ? func( _value ) : Maybe<U>.None;

      /// <summary> <see cref="Maybe{T}"/>-compatible equality-check for values. </summary>
      /// <param name="value"> Value to check for equality. </param>
      /// <returns>
      /// <see langword="true"/> iff <see cref="IsSome"/> == <see langword="true"/> <c>and</c> the <see cref="object.Equals(object)"/> override of this instance's value
      /// returns <see langword="true"/> for <paramref name="value"/>, otherwise <see langword="false"/>.
      /// </returns>
      public bool Is( TValue value )
      {
         if( !IsSome ) return false;
         if( _value is string _strValue && value is string otherStrValue )
            return _strValue.Equals( otherStrValue, StringComparison.Ordinal );
         else
            return _value.Equals( value );
      }

      /// <summary> <see cref="Maybe{T}"/>-compatible inequality-check for values. </summary>
      /// <param name="value">Value to check for inequality.</param>
      /// <returns>
      /// <see langword="true"/> iff <see cref="IsSome"/> == <see langword="true"/> <c>and</c> the
      /// <see cref="object.Equals(object)"/> override of this instance's value returns <see langword="false"/> for <paramref name="value"/>, otherwise <see langword="false"/>.
      /// </returns>
      public bool IsNot( TValue value )
      {
         if( !IsSome ) return false;
         if( _value is string _strValue && value is string otherStrValue )
            return !_strValue.Equals( otherStrValue, StringComparison.Ordinal );
         else
            return !_value.Equals( value );
      }

      /// <summary> Monadic predicate check for values. </summary>
      /// <param name="predicate">The delegate that checks the predicate.</param>
      /// <returns>
      /// <see langword="true"/> iff <see cref="IsSome"/> == <see langword="true"/> <c>and</c> <paramref name="predicate"/> returns <see langword="true"/> for
      /// this instance's value. <see langword="false"/> in *all* other cases..
      /// </returns>
      public bool Holds( Func<TValue, bool> predicate ) => IsSome && predicate( _value );

      /// <summary>
      /// <para>Negated monadic predicate check for values. Basically syntactic sugar, equivalent code is:</para>
      /// <para><see cref="IsSome"/> <see langword="and"/> ( <see langword="not"/> <see cref="Holds(Func{TValue, bool})"/> )</para>
      /// </summary>
      /// <param name="predicate">The delegate that checks the predicate and is subsequently negated.</param>
      /// <returns>
      /// <see langword="true"/> iff <see cref="IsSome"/> == <see langword="true"/> <c>and</c> <paramref name="predicate"/> returns <see langword="false"/> for
      /// this instance's value. <see langword="false"/> in *all* other cases..
      /// </returns>
      public bool HoldsNot( Func<TValue, bool> predicate ) => IsSome && !predicate( _value );

      #endregion Mapping

      #region Accessing Value

      /// <summary> Returns this instance's value if <see cref="IsSome"/> == <see langword="true"/>, otherwise <paramref name="otherwise"/>. </summary>
      /// <param name="otherwise">The desired value if this instance has no value.</param>
      public TValue Or( TValue otherwise ) => IsSome ? _value : otherwise;

      /// <summary> Returns this instance's value if <see cref="IsSome"/> == <see langword="true"/>, otherwise throws a <see cref="MaybeNoneException"/>. </summary>
      /// <exception cref="MaybeNoneException"/>
      public TValue OrThrow() => IsSome ? _value : throw new MaybeNoneException( typeValue: typeof( TValue ) );

      #pragma warning disable CS0618 // Type or member is obsolete
      /// <summary>
      /// Returns this instance's value if <see cref="IsSome"/> == <see langword="true"/>, otherwise
      /// throws a <see cref="MaybeNoneException"/> with the message <paramref name="msg"/>.
      /// </summary>
      /// <exception cref="MaybeNoneException"/>
      public TValue OrThrow( string msg ) => IsSome ? _value : throw new MaybeNoneException( customMessage: msg );
      #pragma warning restore CS0618

      #pragma warning disable CS0618 // Type or member is obsolete
      /// <summary>
      /// Returns this instance's value if <see cref="IsSome"/> == <see langword="true"/>, otherwise throws a <see cref="MaybeNoneException"/> with the
      /// formatted message <paramref name="msg"/> using the message arguments <paramref name="args"/>.
      /// </summary>
      /// <exception cref="MaybeNoneException"/>
      public TValue OrThrow( string msg, params string[] args ) => IsSome ? _value : throw new MaybeNoneException( customMessage: string.Format( msg, args ) );
      #pragma warning restore CS0618 // MaybeNoneException( string ) obsolete

      /// <summary> Returns this instance's value if <see cref="IsSome"/> == <see langword="true"/>, otherwise throws the exception <paramref name="e"/>. </summary>
      /// <param name="e">The desired exception if this instance has no value.</param>
      /// <exception cref="Exception"/>
      public TValue OrThrow( Exception e ) => IsSome ? _value : throw e;

      /// <summary> Writes this instance's value into the <see langword="out"/> parameter <paramref name="value"/> and returns <see langword="true"/> if <see cref="IsSome"/> ==
      /// <see langword="true"/>, otherwise <paramref name="value"/> will be set to the <see langword="default"/> value of <typeparamref name="TValue"/> and the method returns
      /// <see langword="false"/>. </summary>
      /// <param name="value"><see langword="out"/> parameter for this instance's value.</param>
      /// <remarks>Not particularly functional, but a concession to typical .NET methods.</remarks>
      public bool TryGetValue( out TValue value )
      {
         value = IsSome ? _value : default;
         return IsSome;
      }

      #endregion Accessing Value

      #region Converters
      /// <summary> Converts this instance into an appropriate <see cref="Failable{TValue, TFail}"/>. </summary>
      /// <typeparam name="TFail"> The type of "failure" for the new <see cref="Failable{TValue, TFail}"/>. </typeparam>
      /// <param name="failure"> An object representing a "failure", used in case <see cref="IsSome"/> == <see langword="false"/> </param>
      /// <returns> <see cref="Failable{TValue, TFail}.Success(TValue)"/> if <see cref="IsSome"/> == <see langword="true"/> for this instance and
      /// <see cref="Failable{TValue, TFail}.Failure(TFail)"/> containing <paramref name="failure"/> otherwise. </returns>
      public Failable<TValue, TFail> ToFailable<TFail>( TFail failure ) => IsSome ? Failable<TValue, TFail>.Success( _value ) : Failable<TValue, TFail>.Failure( failure );

      /// <summary> Converts this instance into an appropriate <see cref="Failable{TValue, TFail}"/> using a function if necessary. </summary>
      /// <typeparam name="TFail"> The type of "failure" for the new <see cref="Failable{TValue, TFail}"/>. </typeparam>
      /// <param name="func"> An function producting a "failure", used in case <see cref="IsSome"/> == <see langword="false"/> </param>
      /// <returns> <see cref="Failable{TValue, TFail}.Success(TValue)"/> if <see cref="IsSome"/> == <see langword="true"/> for this instance and
      /// <see cref="Failable{TValue, TFail}.Failure(TFail)"/> containing the result of <paramref name="func"/> otherwise. </returns>
      public Failable<TValue, TFail> ToFailable<TFail>( Func<TFail> func ) => IsSome ? Failable<TValue, TFail>.Success( _value ) : Failable<TValue, TFail>.Failure( func() );

      /// <summary> Converts this instance into an appropriate <see cref="EFailable{TValue}"/>. </summary>
      /// <param name="exc"> An object representing a "failure", used in case <see cref="IsSome"/> == <see langword="false"/> </param>
      /// <returns> <see cref="EFailable{TValue}.Success(TValue)"/> if <see cref="IsSome"/> == <see langword="true"/> for this instance and
      /// <see cref="EFailable{TValue}.Failure(Exception)"/> containing <paramref name="exc"/> otherwise. </returns>
      public EFailable<TValue> ToEFailable( Exception exc ) => IsSome ? EFailable<TValue>.Success( _value ) : EFailable<TValue>.Failure( exc );

      /// <summary> Converts this instance into an appropriate <see cref="EFailable{TValue}"/> using a function if necessary. </summary>
      /// <param name="func"> An function producting a "failure", used in case <see cref="IsSome"/> == <see langword="false"/> </param>
      /// <returns> <see cref="EFailable{TValue}.Success(TValue)"/> if <see cref="IsSome"/> == <see langword="true"/> for this instance and
      /// <see cref="EFailable{TValue}.Failure(Exception)"/> containing the result of <paramref name="func"/> otherwise. </returns>
      public EFailable<TValue> ToEFailable( Func<Exception> func ) => IsSome ? EFailable<TValue>.Success( _value ) : EFailable<TValue>.Failure( func() );

      /// <summary>
      /// <para>Converts this instance into an appropriate <see cref="Option{TValue, TFail}"/>.</para>
      /// <para> Note that <typeparamref name="TFail"/> is relevant for typing the new instance only. </para>
      /// </summary>
      /// <typeparam name="TFail"> The type of "failure" for the new <see cref="Option{TValue, TFail}"/>. </typeparam>
      /// <returns> <see cref="Option{TValue, TFail}.Some(TValue)"/> if <see cref="IsSome"/> == <see langword="true"/> for this instance
      /// and <see cref="Option{TValue, TFail}.None"/> otherwise. </returns>
      public Option<TValue, TFail> ToOption<TFail>() => IsSome ? Option<TValue, TFail>.Some( _value ) : Option<TValue, TFail>.None;


      /// <summary> Converts this instance into an appropriate <see cref="EOption{TValue}"/>. </summary>
      /// <returns> <see cref="EOption{TValue}.Some(TValue)"/> if <see cref="IsSome"/> == <see langword="true"/> for this instance and
      /// <see cref="EOption{TValue}.None"/> otherwise. </returns>
      public EOption<TValue> ToEOption() => IsSome ? EOption<TValue>.Some( _value ) : EOption<TValue>.None;

      //MAYBEs cannot be converted into Validations, that makes no sense!

      #endregion Converters

      #region Object

      /// <summary> Custom implementation of <see cref="object.ToString()"/>, wrapping a call to this instance's value, if any. </summary>
      public override string ToString() =>
         IsSome
            ? $"[Maybe<{typeof( TValue ).Name}>.Some: {_value}]"
            : $"[Maybe<{typeof( TValue ).Name}>.None]";

      /// <summary> Returns <see langword="true"/> iff <see cref="IsSome"/> returns the same boolean for both objects and, in case of that
      /// value being <see langword="true"/>, the method <see cref="object.Equals(object)"/> of the two instance's values also returns <see langword="true"/>. </summary>
      public override bool Equals( object obj ) =>
         ( obj is Maybe<TValue> other )
         && ( IsSome == other.IsSome )
         && ( IsNone || Is( other._value ) );

      /// <summary>
      /// <para> Custom implementation of <see cref="object.GetHashCode()"/>, wrapping a call to this instance's value if any. </para>
      /// <para> Note that all <see cref="Maybe{T}"/>-objects, regardless of type <typeparamref name="TValue"/>, have the same hash code when there is no
      /// value, aka when the instance is <see cref="None"/>. </para>
      /// </summary>
      public override int GetHashCode() => IsSome ? _value.GetHashCode() : int.MinValue;

      #endregion Object
   }
}