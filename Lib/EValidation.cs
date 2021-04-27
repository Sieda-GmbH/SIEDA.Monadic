using System;

namespace SIEDA.Monadic
{
   /// <summary>
   /// An implementation of <see cref="IValidation{TFail}"/>, with <c>TFail</c> beeing <see cref="Exception"/>.
   /// </summary>
   public readonly struct EValidation : IValidation<Exception>
   {
      #region State

      private readonly IValidation<Exception> _innerValidation;

      #endregion State

      #region Construction

      /// <summary>
      /// The <see cref="EValidation"/> with a "successful" outcome.
      /// </summary>
      public static readonly EValidation Success = new EValidation( Validation<Exception>.Success );

      /// <summary>
      /// Creates a <see cref="EValidation"/> with a <paramref name="error"/>-value, which must not
      /// be <see langword="null"/>.
      /// </summary>
      /// <exception cref="ValidationFailureConstructionException">
      /// if <paramref name="error"/> == <see langword="null"/>.
      /// </exception>
      public static EValidation Failure( Exception error )
      {
         if( ReferenceEquals( error, null ) )
         {
            #pragma warning disable CS0618 // Type or member is obsolete
            throw new ValidationFailureConstructionException();
            #pragma warning restore CS0618
         }

         return new EValidation( Validation<Exception>.Failure( error ) );
      }

      private EValidation( IValidation<Exception> validation ) => _innerValidation = validation;

      #endregion Construction

      #region Properties
      ///<inheritdoc/>
      public bool IsSuccess => _innerValidation.IsSuccess;

      ///<inheritdoc/>
      public bool IsFailure => _innerValidation.IsFailure;

      #endregion Properties

      #region Mapping
      ///<inheritdoc/>
      public Validation<TNewFail> FailMap<TNewFail>( Func<Exception, TNewFail> func ) =>
         _innerValidation.IsSuccess ? Validation<TNewFail>.Success : Validation<TNewFail>.Failure( func( _innerValidation.FailureOrThrow() ) );
      #endregion Mapping

      #region Accessing Failure
      ///<inheritdoc/>
      public Exception FailureOrThrow() => _innerValidation.FailureOrThrow();

      ///<inheritdoc/>
      public bool TryGetFailure( out Exception failure ) => _innerValidation.TryGetFailure( out failure );

      #endregion Accessing Failure

      #region Converters

      ///<inheritdoc/>
      public IOption<object, Exception> ToOption() => _innerValidation.ToOption();

      #endregion Converters

      #region Object

      ///<inheritdoc/>
      public override string ToString() => _innerValidation.ToString();


      ///<inheritdoc/>
      public override bool Equals( object obj ) => _innerValidation.Equals( obj );

      ///<inheritdoc/>
      public override int GetHashCode() => _innerValidation.GetHashCode();

      #endregion Object
   }
}