using System;

namespace SIEDA.Monadic
{
   /// <summary>
   /// An implementation of <see cref="IValidation{TFail}"/>.
   /// </summary>
   public readonly struct Validation<TFail> : IValidation<TFail>
   {
      #region State

      private readonly TFail _error;

      // Property IsSuccess is also "State", and it is relevant for 'Equals(...)'.

      #endregion State

      #region Construction

      /// <summary>
      /// The <see cref="Validation{TFail}"/> with a "successful" outcome.
      /// </summary>
      public static readonly Validation<TFail> Success = new Validation<TFail>( true, default );

      /// <summary>
      /// Creates a <see cref="Validation{TFail}"/> with a <paramref name="failure"/>-value, which
      /// must not be <see langword="null"/>.
      /// </summary>
      /// <exception cref="ValidationFailureConstructionException">
      /// if <paramref name="failure"/> == <see langword="null"/>.
      /// </exception>
      public static Validation<TFail> Failure( TFail failure )
      {
         if( ReferenceEquals( failure, null ) )
         {
            throw new ValidationFailureConstructionException( typeFailure: typeof( TFail ) );
         }

         return new Validation<TFail>( false, failure );
      }

      private Validation( bool isSuccess, TFail failure )
      {
         IsSuccess = isSuccess;
         _error = failure;
      }

      #endregion Construction

      #region Properties
      ///<inheritdoc/>
      public bool IsSuccess { get; }

      ///<inheritdoc/>
      public bool IsFailure => !IsSuccess;

      #endregion Properties

      #region Accessing Failure
      ///<inheritdoc/>
      public TFail FailureOrThrow() =>
         IsFailure ? _error : throw new ValidationNoFailureException( typeof( TFail ) );
      ///<inheritdoc/>
      public bool TryGetFailure( out TFail value )
      {
         value = IsFailure ? _error : default;
         return IsFailure;
      }

      #endregion Accessing Failure

      #region Mapping
      ///<inheritdoc/>
      public Validation<TNewFail> FailMap<TNewFail>( Func<TFail, TNewFail> func ) => IsSuccess ? Validation<TNewFail>.Success : Validation<TNewFail>.Failure( func( _error ) );
      #endregion Mapping

      #region Converters

      ///<inheritdoc/>
      public IOption<object, TFail> ToOption() =>
         IsSuccess ? Option<object, TFail>.None : Option<object, TFail>.Failure( _error );

      #endregion Converters

      #region Object

      ///<inheritdoc/>
      public override string ToString() =>
         IsSuccess
         ? $"[Validation<{typeof( TFail ).Name}>.Success]"
         : $"[Validation<{typeof( TFail ).Name}>.Failure: { _error }]";


      ///<inheritdoc/>
      public override bool Equals( object obj ) =>
         ( obj is IValidation<TFail> other )
            && ( IsSuccess == other.IsSuccess )
            && ( IsSuccess || _error.Equals( other.FailureOrThrow() ) );

      ///<inheritdoc/>
      public override int GetHashCode() => IsSuccess ? int.MaxValue : _error.GetHashCode();

      #endregion Object
   }
}