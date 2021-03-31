using System;

namespace SIEDA.Monadic
{
   /// <summary>
   /// <para>Represents the "result" of check that might have failed, that is an operation that either succeeds without any error
   /// or fails with an <see cref="Exception"/>.</para>
   /// <para>One can think of this as an "inverted" <see cref="Maybe{T}"/>, although this obviously comes with different intended
   /// semantics and API restrictions. An alternative interpretion of this class is a <see cref="EFailable{TValue}"/> without a 
   /// value when it is successful.</para>
   /// </summary>
   public readonly struct EValidation
   {
      #region State
      private readonly Exception _error;
      // Property IsSuccess is also "State", and it is relevant for 'Equals(...)'.

      #endregion State

      #region Construction

      /// <summary>The <see cref="EValidation"/> with a "successful" outcome.</summary>
      public static readonly EValidation Success = new EValidation( null );

      /// <summary>Creates a <see cref="EValidation"/> with a <paramref name="error"/>-value, which must not be <see langword="null"/>.</summary>
      /// <exception cref="EValidationFailureConstructionException">if <paramref name="error"/> == <see langword="null"/>.</exception>
      public static EValidation Failure( Exception error )
      {
         if( ReferenceEquals( error, null ) ) throw new EValidationFailureConstructionException();
         return new EValidation( error );
      }

      private EValidation( Exception error )
      {
         _error = error;
      }

      #endregion Construction

      #region Properties

      /// <summary><see langword="true"/>, if this instance is a "success".</summary>
      public bool IsSuccess { get => _error == null; }

      /// <summary><see langword="true"/>, if this instance is a "failure", aka has a value of type <typeparamref name="TFail"/>.</summary>
      public bool IsFailure => !IsSuccess;

      #endregion Properties

      #region Accessing Failure

      /// <summary>
      /// Returns this instance's "failed" value if <see cref="IsFailure"/> == <see langword="true"/>,
      /// otherwise throws a <see cref="EValidationNoFailureException"/>.
      /// </summary>
      /// <exception cref="EValidationNoFailureException"/>
      public Exception FailureOrThrow() => IsFailure ? _error : throw new EValidationNoFailureException();

      /// <summary>
      /// Writes this instance's "failed" value into the <see langword="out"/> parameter <paramref name="value"/>
      /// and returns <see langword="true"/> if <see cref="IsFailure"/> == <see langword="true"/>,
      /// otherwise <paramref name="value"/> will be set to the <see langword="null"/> and the method returns <see langword="false"/>.
      /// </summary>
      /// <param name="value"><see langword="out"/> parameter for this instance's "failed" value.</param>
      /// <remarks>Not particularly functional, but a concession to typical .NET methods.</remarks>
      public bool TryGetFailure( out Exception value )
      {
         value = IsFailure ? _error : null;
         return IsFailure;
      }

      #endregion Accessing Failure

      /// <summary>
      /// Maps this instance by using its error (if any) as an argument for <paramref name="func"/>
      /// and returning the result as a "flat" <see cref="EValidation"/> (instead of a "EValidation of an EValidation").
      /// <para><paramref name="func"/> is only called if <see cref="IsFailure"/> == <see langword="true"/>.</para>
      /// <para>Returns this instance if <see cref="IsSuccess"/> == <see langword="true"/> or it is the result
      /// of <paramref name="func"/>.</para>
      /// </summary>
      /// <typeparam name="U">The type of the new value.</typeparam>
      /// <param name="func">The delegate that provides the new value, if relevant.</param>
      public EValidation FlatMap<U>( Func<Exception, EValidation> func ) => IsFailure ? func( _error ) : this;

      #region Converters

      /// <summary>Converts this instance into a <see cref="Option{TValue, TFail}"/>, which is either a failure or empty (but never defined).</summary>
      /// <returns><see cref="Option{TValue, TFail}"/> with its some-type being 'object' and its failure-type being <see cref="Exception"/>.</returns>
      public Option<object, Exception> ToOption() => IsSuccess ? Option<object, Exception>.None : Option<object, Exception>.Failure( _error );

      /// <summary>Converts this instance into a <see cref="EOption{TValue}"/>, which is either a failure or empty (but never defined).</summary>
      /// <returns><see cref="EOption{TValue}"/> with its some-type being 'object'.</returns>
      public EOption<object> ToEOption() => IsSuccess ? EOption<object>.None : EOption<object>.Failure( _error );

      #endregion Converters

      #region Object

      ///<summary>
      /// Custom implementation of <see cref="object.ToString()"/>, wrapping the corresponding call to this instance's value,
      /// be it a "success" or a "failure".
      ///</summary>
      public override string ToString() =>
         IsSuccess //the 'failure' is never null!
         ? $"[EValidation.Success]"
         : $"[EValidation.Failure: { _error }]";

      ///<summary>
      /// Returns <see langword="true"/> iff <see cref="IsSuccess"/> returns the same boolean for both objects
      /// and the operator '==' returns <see langword="true"/> the two instances' <see cref="Exception"/>-instances
      /// in case of <see cref="IsSuccess"/> being <see langword="false"/>.
      ///</summary>
      public override bool Equals( object obj ) =>
         ( obj is EValidation other )
            && ( IsSuccess == other.IsSuccess )
            && ( IsSuccess || _error == other._error );

      /// <summary>
      /// Custom implementation of <see cref="object.GetHashCode()"/>, wrapping a call to this instance's value,
      /// be it a "success" or a "failure".
      /// </summary>
      public override int GetHashCode() => IsSuccess ? int.MaxValue : _error.GetHashCode();

      #endregion Object
   }
}