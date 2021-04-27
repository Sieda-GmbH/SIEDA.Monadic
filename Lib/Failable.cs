using System;

namespace SIEDA.Monadic
{
   /// <summary>
   /// An implementation of <see cref="IFailable{TValue, TFail}"/>.
   /// </summary>
   public readonly struct Failable<TValue, TFail> : IFailable<TValue, TFail>
   {
      #region State

      private readonly TValue _value;

      private readonly TFail _failure;

      // Property IsSuccess is also "State", and it is relevant for 'Equals(...)'.

      #endregion State

      #region Construction

      /// <summary>
      /// Creates a <see cref="Failable{TValue, TFail}"/> with a "successful" <paramref
      /// name="value"/>, which must not be <see langword="null"/>.
      /// </summary>
      /// <exception cref="FailableSuccessConstructionException">
      /// if <paramref name="value"/> == <see langword="null"/>.
      /// </exception>
      public static Failable<TValue, TFail> Success( TValue value )
      {
         if( ReferenceEquals( value, null ) )
         {
            throw new FailableSuccessConstructionException( typeValue: typeof( TValue ), typeFailure: typeof( TFail ) );
         }

         return new Failable<TValue, TFail>( true, value, default );
      }

      /// <summary>
      /// Creates a <see cref="Failable{TValue, TFail}"/> with the value of the <see
      /// cref="Nullable{T}"/><paramref name="nullableValue"/>.
      /// </summary>
      /// <exception cref="FailableSuccessConstructionException">
      /// if <paramref name="nullableValue"/> == <see langword="null"/> (has no value).
      /// </exception>
      public static Failable<TValue, TFail> Success<T>( T? nullableValue ) where T : struct, TValue
      {
         if( !nullableValue.HasValue )
         {
            throw new FailableSuccessConstructionException( typeValue: typeof( TValue ), typeFailure: typeof( TFail ) );
         }

         return new Failable<TValue, TFail>( true, nullableValue.Value, default );
      }

      /// <summary>
      /// Creates a <see cref="Failable{TValue, TFail}"/> with a <paramref name="failure"/>-value,
      /// which must not be <see langword="null"/>.
      /// </summary>
      /// <exception cref="FailableFailureConstructionException">
      /// if <paramref name="failure"/> == <see langword="null"/>.
      /// </exception>
      public static Failable<TValue, TFail> Failure( TFail failure )
      {
         if( ReferenceEquals( failure, null ) )
         {
            throw new FailableFailureConstructionException( typeValue: typeof( TValue ), typeFailure: typeof( TFail ) );
         }

         return new Failable<TValue, TFail>( false, default, failure );
      }

      /// <summary>
      /// Creates a <see cref="Failable{TValue, TFail}"/> with the failure value of the <see
      /// cref="Nullable{T}"/> <paramref name="nullableFailure"/>.
      /// </summary>
      /// <exception cref="FailableFailureConstructionException">
      /// if <paramref name="nullableFailure"/> == <see langword="null"/> (has no value).
      /// </exception>
      public static Failable<TValue, TFail> Failure<T>( T? nullableFailure ) where T : struct, TFail
      {
         if( !nullableFailure.HasValue )
         {
            throw new FailableFailureConstructionException( typeValue: typeof( TValue ), typeFailure: typeof( TFail ) );
         }

         return new Failable<TValue, TFail>( false, default, nullableFailure.Value );
      }

      private Failable( bool hasValue, TValue value, TFail failure )
      {
         IsSuccess = hasValue;
         _value = value;
         _failure = failure;
      }

      #endregion Construction

      #region Properties
      ///<inheritdoc/>
      public bool IsSuccess { get; }

      ///<inheritdoc/>
      public bool IsFailure => !IsSuccess;

      #endregion Properties

      #region Mapping

      ///<inheritdoc/>
      public IFailable<TNewValue, TFail> Map<TNewValue>( Func<TValue, TNewValue> func ) =>
         IsSuccess ? Failable<TNewValue, TFail>.Success( func( _value ) ) : Failable<TNewValue, TFail>.Failure( _failure );

      ///<inheritdoc/>
      public IFailable<TNewValue, TFail> FlatMap<TNewValue>( Func<TValue, IFailable<TNewValue, TFail>> func ) =>
         IsSuccess ? func( _value ) : Failable<TNewValue, TFail>.Failure( _failure );

      ///<inheritdoc/>
      public bool Is( TValue value ) => IsSuccess && _value.Equals( value );

      ///<inheritdoc/>
      public bool Holds( Func<TValue, bool> predicate ) => IsSuccess && predicate( _value );

      #endregion Mapping

      #region Accessing Success

      ///<inheritdoc/>
      public TValue Or( TValue otherwise ) => IsSuccess ? _value : otherwise;

      ///<inheritdoc/>
      public TValue OrThrow() => IsSuccess ? _value : throw new FailableFailureException( typeValue: typeof( TValue ), typeFailure: typeof( TFail ) );

      #pragma warning disable CS0618 // Type or member is obsolete
      ///<inheritdoc/>
      public TValue OrThrow( string msg ) => IsSuccess ? _value : throw new FailableFailureException( customMessage: msg );
      #pragma warning restore CS0618

      #pragma warning disable CS0618 // Type or member is obsolete
      ///<inheritdoc/>
      public TValue OrThrow( string msg, params string[] args ) => IsSuccess ? _value : throw new FailableFailureException( customMessage: string.Format( msg, args ) );
      #pragma warning restore CS0618

      ///<inheritdoc/>
      public TValue OrThrow( Exception e ) => IsSuccess ? _value : throw e;

      ///<inheritdoc/>
      public TValue OrUse( Func<TFail, TValue> otherwiseFunc ) => IsSuccess ? _value : otherwiseFunc( _failure );

      ///<inheritdoc/>
      public bool TryGetValue( out TValue value )
      {
         value = IsSuccess ? _value : default;
         return IsSuccess;
      }

      #endregion Accessing Success

      #region Accessing Failure

      ///<inheritdoc/>
      public TFail FailureOrThrow() => IsFailure ? _failure : throw new FailableNoFailureException( typeof( TValue ), typeof( TFail ) );

      ///<inheritdoc/>
      public bool TryGetFailure( out TFail failure )
      {
         failure = IsFailure ? _failure : default;
         return IsFailure;
      }

      #endregion Accessing Failure

      #region Converters

      ///<inheritdoc/>
      public Maybe<TValue> ToMaybe() =>
         IsSuccess ? Maybe<TValue>.Some( _value ) : Maybe<TValue>.None;

      ///<inheritdoc/>
      public IOption<TValue, TFail> ToOption() =>
         IsSuccess ? Option<TValue, TFail>.Some( _value ) : Option<TValue, TFail>.Failure( _failure );

      ///<inheritdoc/>
      public IValidation<TFail> ToValidation() =>
         IsSuccess ? Validation<TFail>.Success : Validation<TFail>.Failure( _failure );

      #endregion Converters

      #region Object

      ///<inheritdoc/>
      public override string ToString() =>
         IsSuccess //neither 'value' nor 'failure' is ever null!
         ? $"[Failable<{typeof( TValue ).Name}, {typeof( TFail ).Name}>.Success: { _value }]"
         : $"[Failable<{typeof( TValue ).Name}, {typeof( TFail ).Name}>.Failure: { _failure }]";


      ///<inheritdoc/>
      public override bool Equals( object obj ) =>
         ( obj is IFailable<TValue, TFail> other )
            && ( IsSuccess == other.IsSuccess )
            && ( IsFailure || _value.Equals( other.OrThrow() ) )
            && ( IsSuccess || _failure.Equals( other.FailureOrThrow() ) );

      ///<inheritdoc/>
      public override int GetHashCode() => IsSuccess ? _value.GetHashCode() : _failure.GetHashCode();

      #endregion Object
   }
}