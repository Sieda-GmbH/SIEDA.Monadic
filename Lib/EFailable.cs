
using System;

namespace SIEDA.Monadic
{
   /// <summary>
   /// An implementation of <see cref="IFailable{TValue, TFail}"/>, with <c>TFail</c> beeing <see cref="Exception"/>.
   /// </summary>
   /// <remarks>This class acts like a transparent wrapper around an <see cref="Option{TValue, TFail}"/>.</remarks>
   /// <typeparam name="TValue">The type of the "successful" value.</typeparam>
   public readonly struct EFailable<TValue> : IFailable<TValue, Exception>
   {
      #region State

      private readonly IFailable<TValue, Exception> _innerFailable;

      #endregion State

      #region Construction

      /// <summary>
      /// Creates an <see cref="EFailable{TValue}"/> with a "successful" <paramref
      /// name="value"/>, which must not be <see langword="null"/>.
      /// </summary>
      /// <exception cref="FailableSuccessConstructionException">
      /// if <paramref name="value"/> == <see langword="null"/>.
      /// </exception>
      public static EFailable<TValue> Success( TValue value ) => new EFailable<TValue>( Failable<TValue, Exception>.Success( value ) );

      /// <summary>
      /// Creates a <see cref="EFailable{TValue}"/> with the value of the <see
      /// cref="Nullable{T}"/><paramref name="nullableValue"/>.
      /// </summary>
      /// <exception cref="FailableSuccessConstructionException">
      /// if <paramref name="nullableValue"/> == <see langword="null"/> (has no value).
      /// </exception>
      public static EFailable<TValue> Success<T>( T? nullableValue ) where T : struct, TValue => new EFailable<TValue>( Failable<TValue, Exception>.Success( nullableValue ) );

      /// <summary>
      /// Creates an <see cref="EFailable{TValue}"/> with a <paramref name="failure"/>-exception,
      /// which must not be <see langword="null"/>.
      /// </summary>
      /// <exception cref="FailableFailureConstructionException">
      /// if <paramref name="failure"/> == <see langword="null"/>.
      /// </exception>
      public static EFailable<TValue> Failure( Exception failure ) => new EFailable<TValue>( Failable<TValue, Exception>.Failure( failure ) );

      private EFailable( IFailable<TValue, Exception> failable ) => _innerFailable = failable;

      #endregion Construction

      #region Properties

      ///<inheritdoc/>
      public bool IsSuccess => _innerFailable.IsSuccess;

      ///<inheritdoc/>
      public bool IsFailure => _innerFailable.IsFailure;

      #endregion Properties

      #region Mapping
      ///<inheritdoc/>
      public IFailable<TNewValue, Exception> Map<TNewValue>( Func<TValue, TNewValue> func ) => _innerFailable.Map( func );

      ///<inheritdoc/>
      public IFailable<TNewValue, Exception> FlatMap<TNewValue>( Func<TValue, IFailable<TNewValue, Exception>> func ) => _innerFailable.FlatMap( func );

      ///<inheritdoc/>
      public bool Is( TValue value ) => _innerFailable.Is( value );

      ///<inheritdoc/>
      public bool Holds( Func<TValue, bool> predicate ) => _innerFailable.Holds( predicate );

      #endregion Mapping

      #region Accessing Success

      ///<inheritdoc/>
      public TValue Or( TValue otherwise ) => _innerFailable.Or( otherwise );

      ///<inheritdoc/>
      public TValue OrThrow() => _innerFailable.OrThrow();

      ///<inheritdoc/>
      public TValue OrThrow( string msg ) => _innerFailable.OrThrow( msg );

      ///<inheritdoc/>
      public TValue OrThrow( string msg, params string[] args ) => _innerFailable.OrThrow( msg, args );

      ///<inheritdoc/>
      public TValue OrThrow( Exception e ) => _innerFailable.OrThrow( e );

      ///<inheritdoc/>
      public TValue OrUse( Func<Exception, TValue> otherwiseFunc ) => _innerFailable.OrUse( otherwiseFunc );

      ///<inheritdoc/>
      public bool TryGetValue( out TValue value ) => _innerFailable.TryGetValue( out value );

      #endregion Accessing Success

      #region Accessing Failure

      ///<inheritdoc/>
      public Exception FailureOrThrow() => _innerFailable.FailureOrThrow();

      ///<inheritdoc/>
      public bool TryGetFailure( out Exception exception ) => _innerFailable.TryGetFailure( out exception );

      #endregion Accessing Failure

      #region Converters

      ///<inheritdoc/>
      public Maybe<TValue> ToMaybe() => _innerFailable.ToMaybe();

      ///<inheritdoc/>
      public IOption<TValue, Exception> ToOption() => _innerFailable.ToOption();

      ///<inheritdoc/>
      public IValidation<Exception> ToValidation() => _innerFailable.ToValidation();

      #endregion Converters

      #region Object

      ///<inheritdoc/>
      public override string ToString() => _innerFailable.ToString();

      ///<inheritdoc/>
      public override bool Equals( object obj ) => _innerFailable.Equals( obj );

      ///<inheritdoc/>
      public override int GetHashCode() => _innerFailable.GetHashCode();

      #endregion Object
   }
}