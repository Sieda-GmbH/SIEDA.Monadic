using System;
using System.Runtime.CompilerServices;
using Monadic.SwitchCase;

namespace SIEDA.Monadic
{
   /// <summary>
   /// <para>
   /// Represents the "result" of a check that might have failed, that is an operation that either
   /// succeeds without any error or fails with some <see cref="Exception"/> if the operation
   /// "was a failure". Never <see langword="null"/>.
   /// </para>
   /// <para>
   /// One can think of this as an "inverted" <see cref="Maybe{TValue}"/>, although this obviously comes
   /// with different intended semantics and API restrictions. An alternative interpretion of this
   /// class is a <see cref="EFailable{TValue}"/> without a value when it is successful.
   /// </para>
   /// </summary>
   public readonly struct EValidation
   {
      #region State

      private readonly Exception _error;
      private readonly VldType _type;

      #endregion State

      #region Construction

      /// <summary>
      /// The <see cref="EValidation"/> with a "successful" outcome.
      /// </summary>
      public static readonly EValidation Success = new EValidation( VldType.Success, default );

      /// <summary>
      /// Creates a <see cref="EValidation"/> with a <paramref name="failure"/>-value, which
      /// must not be <see langword="null"/>.
      /// </summary>
      /// <exception cref="ValidationFailureConstructionException">
      /// if <paramref name="failure"/> == <see langword="null"/>.
      /// </exception>
      public static EValidation Failure( Exception failure )
      {
         if( ReferenceEquals( failure, null ) )
         {
            throw new ValidationFailureConstructionException( typeFailure: typeof( Exception ) );
         }

         return new EValidation( VldType.Failure, failure );
      }

      private EValidation( VldType vldType, Exception failure )
      {
         _type = vldType;
         _error = failure;
      }

      #endregion Construction

      #region Properties
      /// <summary> Returns an appropriate <see cref="VldType"/> for this instance, useful in case you want to use a switch-case.</summary>
      public VldType Enum { get => _type; }

      /// <summary>
      /// <see langword="true"/>, if this instance is a "success".
      /// </summary>
      public bool IsSuccess { get => _type == VldType.Success; }

      /// <summary>
      /// <see langword="true"/>, if this instance is a "failure", aka has a value of type <see cref="Exception"/>.
      /// </summary>
      public bool IsFailure { get => _type == VldType.Failure; }
      #endregion Properties

      #region Accessing Failure
      /// <summary>
      /// Returns this instance's "failed" value if <see cref="IsFailure"/> == <see langword="true"/>,
      /// otherwise throws a <see cref="ValidationNoFailureException"/>.
      /// </summary>
      /// <exception cref="ValidationNoFailureException"/>
      public Exception FailureOrThrow() =>
         IsFailure ? _error : throw new ValidationNoFailureException( typeof( Exception ) );

      #endregion Accessing Failure

      #region Mapping
      /// <summary>
      /// Maps this instance by using its "failed" value (if any) as an argument for <paramref name="func"/>
      /// and returning a <see cref="EValidation"/> created from the result.
      /// <para><paramref name="func"/> is only called if <see cref="IsSuccess"/> == <see langword="false"/>.</para>
      /// <para>Returns <see cref="EValidation.Success"/> if <see cref="IsSuccess"/> == <see langword="true"/>.</para>
      /// </summary>
      /// <param name="func">The delegate that provides the new failure.</param>
      public EValidation FailMap( Func<Exception, Exception> func ) => IsSuccess ? EValidation.Success : EValidation.Failure( func( _error ) );
      #endregion Mapping

      #region Converters

      /// <summary>
      /// Converts this instance into an appropriate <see cref="Validation{Exception}"/>.
      /// </summary>
      /// <returns>
      /// <see cref="Validation{Exception}"/>.
      /// </returns>
      public Validation<Exception> ToValidation() =>
         IsSuccess ? Validation<Exception>.Success : Validation<Exception>.Failure( _error );

      /// <summary>
      /// Converts this instance into a <see cref="EOption{Object}"/>, which is either a
      /// failure or empty (but never defined).
      /// </summary>
      /// <returns>
      /// <see cref="EOption{Object}"/> with its some-type being 'object' and its failure-type
      /// being <see cref="Exception"/>.
      /// </returns>
      public EOption<object> ToEOption() =>
         IsSuccess ? EOption<object>.None : EOption<object>.Failure( _error );

      /// <summary>
      /// Converts this instance into a <see cref="Option{Object, Exception}"/>, which is either a
      /// failure or empty (but never defined).
      /// </summary>
      /// <returns>
      /// <see cref="Option{Object, Exception}"/> with its some-type being 'object' and its failure-type
      /// being <see cref="Exception"/>.
      /// </returns>
      public Option<object, Exception> ToOption() =>
         IsSuccess ? Option<object, Exception>.None : Option<object, Exception>.Failure( _error );

      #endregion Converters

      #region Object

      /// <summary>
      /// Custom implementation of <see cref="object.ToString()"/>, wrapping the corresponding call
      /// to this instance's value, be it a "success" or a "failure".
      /// </summary>
      public override string ToString() =>
         IsSuccess
         ? $"[EValidation.Success]"
         : $"[EValidation.Failure: { _error }]";


      /// <summary>
      /// <para>Returns <see langword="true"/> iff <see cref="IsSuccess"/> returns the same boolean for
      /// both objects and both instances reference the same <see cref="Exception"/> internally.
      /// being <see langword="false"/>.</para>
      /// <para>Supports cross-class checks with <see cref="Validation{Exception}"/> following the same semantics as above!</para>
      /// </summary>
      public override bool Equals( object obj ) =>
            ( ( obj is EValidation otherE )
            && ( IsSuccess == otherE.IsSuccess )
            && ( IsSuccess || _error == otherE.FailureOrThrow() ) )
         || ( ( obj is Validation<Exception> otherV )
            && ( IsSuccess == otherV.IsSuccess )
            && ( IsSuccess || _error == otherV.FailureOrThrow() ) );

      /// <summary>
      /// Custom implementation of <see cref="object.GetHashCode()"/>, wrapping a call to this
      /// instance's value, be it a "success" or a "failure".
      /// </summary>
      public override int GetHashCode() => IsSuccess ? int.MaxValue : _error.GetHashCode();

      #endregion Object
   }
}