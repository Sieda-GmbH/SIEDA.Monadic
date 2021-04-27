using System;

namespace SIEDA.Monadic
{
   /// <summary>
   /// An implementation of <see cref="IOption{TValue, TFail}"/>, with <c>TFail</c> being <see cref="Exception"/>.
   /// </summary>
   /// <remarks>This class acts like a transparent wrapper around an <see cref="Option{TValue, TFail}"/>.</remarks>
   /// <typeparam name="TValue">The type of the value.</typeparam>
   public readonly struct EOption<TValue> : IOption<TValue, Exception>
   {
      #region State

      private readonly IOption<TValue, Exception> _innerOption;

      #endregion State

      #region Construction

      /// <summary>
      /// Empty instance, no value present, failure absent.
      /// </summary>
      public static readonly EOption<TValue> None = new EOption<TValue> ( Option<TValue, Exception>.None );

      /// <summary>
      /// Creates an <see cref="EOption{TValue}"/> with value <paramref name="value"/>.
      /// </summary>
      /// <exception cref="OptionSomeConstructionException">
      /// if <paramref name="value"/> == <see langword="null"/>.
      /// </exception>
      public static EOption<TValue> Some( TValue value ) => new EOption<TValue>( Option<TValue, Exception>.Some( value ) );

      /// <summary>
      /// Creates an <see cref="EOption{TValue}"/> with the value of the <see
      /// cref="Nullable{T}"/><paramref name="nullableValue"/>.
      /// </summary>
      /// <exception cref="OptionSomeConstructionException">
      /// if <paramref name="nullableValue"/> == <see langword="null"/> (has no value).
      /// </exception>
      public static EOption<TValue> Some<T>( T? nullableValue ) where T : struct, TValue => new EOption<TValue>( Option<TValue, Exception>.Some( nullableValue ) );

      /// <summary>
      /// <para>
      /// Creates and returns an <see cref="EOption{TValue}"/> with value <paramref name="value"/>
      /// if <paramref name="value"/> != <see langword="null"/>.
      /// </para>
      /// <para>Returns <see cref="None"/> if <paramref name="value"/> == <see langword="null"/>.</para>
      /// </summary>
      public static EOption<TValue> From( TValue value ) => new EOption<TValue>( Option<TValue, Exception>.From( value ) );

      /// <summary>
      /// <para>
      /// Creates and returns an <see cref="EOption{TValue}"/> with the value of the <see
      /// cref="Nullable{T}"/><paramref name="nullableValue"/> if <paramref name="nullableValue"/>
      /// != <see langword="null"/> (has a value).
      /// </para>
      /// <para>
      /// Returns <see cref="None"/> if <paramref name="nullableValue"/> == <see langword="null"/>
      /// (has no value).
      /// </para>
      /// </summary>
      public static EOption<TValue> From<T>( T? nullableValue ) where T : struct, TValue => new EOption<TValue>( Option<TValue, Exception>.From( nullableValue ) );

      /// <summary>
      /// Creates an <see cref="EOption{TValue}"/> with a <paramref name="failure"/>-value, which
      /// must not be <see langword="null"/>.
      /// </summary>
      /// <exception cref="OptionFailureConstructionException">
      /// if <paramref name="failure"/> == <see langword="null"/>.
      /// </exception>
      public static EOption<TValue> Failure( Exception failure ) => new EOption<TValue>( Option<TValue, Exception>.Failure( failure ) );

      private EOption( IOption<TValue, Exception> option ) => _innerOption = option;

      #endregion Construction

      #region Properties

      ///<inheritdoc/>
      public bool IsSome => _innerOption.IsSome;

      ///<inheritdoc/>
      public bool IsNone => _innerOption.IsNone;

      ///<inheritdoc/>
      public bool IsFailure => _innerOption.IsFailure;

      ///<inheritdoc/>
      public bool IsNotSome => _innerOption.IsNotSome;

      ///<inheritdoc/>
      public bool IsNotFailure => _innerOption.IsNotFailure;

      #endregion Properties

      #region Mapping
      ///<inheritdoc/>
      public IOption<TNewValue, Exception> Map<TNewValue>( Func<TValue, TNewValue> func ) => _innerOption.Map( func );

      ///<inheritdoc/>
      public IOption<TNewValue, Exception> FlatMap<TNewValue>( Func<TValue, IOption<TNewValue, Exception>> func ) => _innerOption.FlatMap( func );

      ///<inheritdoc/>
      public bool Is( TValue value ) => _innerOption.Is( value );

      ///<inheritdoc/>
      public bool Holds( Func<TValue, bool> predicate ) => _innerOption.Holds( predicate );

      #endregion Mapping

      #region Accessing Value

      ///<inheritdoc/>
      public TValue Or( TValue otherwise ) => _innerOption.Or( otherwise );

      ///<inheritdoc/>
      public TValue OrThrow() => _innerOption.OrThrow();

      ///<inheritdoc/>
      public TValue OrThrow( string msg ) => _innerOption.OrThrow( msg );

      ///<inheritdoc/>
      public TValue OrThrow( string msg, params string[] args ) => _innerOption.OrThrow( msg, args );

      ///<inheritdoc/>
      public TValue OrThrow( Exception e ) => _innerOption.OrThrow( e );

      ///<inheritdoc/>
      public bool TryGetValue( out TValue value ) => _innerOption.TryGetValue( out value );

      #endregion Accessing Value

      #region Accessing Failure

      ///<inheritdoc/>
      public Exception FailureOrThrow() => _innerOption.FailureOrThrow();

      ///<inheritdoc/>
      public bool TryGetFailure( out Exception exception ) => _innerOption.TryGetFailure( out exception );

      #endregion Accessing Failure

      #region Converters

      ///<inheritdoc/>
      public Maybe<TValue> ToMaybe() => _innerOption.ToMaybe();

      ///<inheritdoc/>
      public IFailable<TValue, Exception> ToFailable( Exception errorOnNone ) => _innerOption.ToFailable( errorOnNone );

      ///<inheritdoc/>
      public IValidation<Exception> ToValidation() => _innerOption.ToValidation();

      ///<inheritdoc/>
      public IValidation<Exception> ToValidation( Exception errorOnNone ) => _innerOption.ToValidation();
      #endregion Converters

      #region Object
      ///<inheritdoc/>
      public override string ToString() => _innerOption.ToString();

      ///<inheritdoc/>
      public override bool Equals( object obj ) => _innerOption.Equals( obj );

      ///<inheritdoc/>
      public override int GetHashCode() => _innerOption.GetHashCode();
      #endregion Object
   }
}