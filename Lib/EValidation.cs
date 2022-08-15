using System;
using System.Runtime.CompilerServices;
using SIEDA.Monadic.SwitchCase;

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

      private readonly Exception _failure;
      private readonly VldType _type;

      #endregion State

      #region Construction

      /// <summary>
      /// Creates an appropriate <see cref="EValidation"/> wrapping either an exception thrown by <paramref name="toExecute"/> or simply being <see cref="Success"/>.
      /// </summary>
      /// <param name="toExecute">Function executed and wrapped by this monadic instance.</param>
      /// <exception cref="ValidationFromWrappedConstructionException"> if <paramref name="toExecute"/> == <see langword="null"/>. </exception>
      public static EValidation Wrapping<U>( Func<U> toExecute )
      {
         if( ReferenceEquals( toExecute, null ) ) throw new ValidationFromWrappedConstructionException();
         try
         {
            toExecute();
            return Success;
         }
         catch( Exception e )
         {
            return Failure( e );
         }
      }

      /// <summary>
      /// Creates an appropriate <see cref="EValidation"/> wrapping either an exception thrown by <paramref name="toExecute"/> or simply being <see cref="Success"/>.
      /// </summary>
      /// <param name="toExecute">Function executed and wrapped by this monadic instance.</param>
      /// <exception cref="ValidationFromWrappedConstructionException"> if <paramref name="toExecute"/> == <see langword="null"/>. </exception>
      public static EValidation Wrapping( Action toExecute )
      {
         if( ReferenceEquals( toExecute, null ) ) throw new ValidationFromWrappedConstructionException();
         try
         {
            toExecute();
            return Success;
         }
         catch( Exception e )
         {
            return Failure( e );
         }
      }

      /// <summary> The <see cref="EValidation"/> with a "successful" outcome. </summary>
      public static readonly EValidation Success = new EValidation( VldType.Success, default );

      /// <summary> Creates a <see cref="EValidation"/> with a <paramref name="failure"/>-value, which must not be <see langword="null"/>. </summary>
      /// <exception cref="ValidationFailureConstructionException"> if <paramref name="failure"/> == <see langword="null"/>. </exception>
      public static EValidation Failure( Exception failure )
      {
         if( ReferenceEquals( failure, null ) ) throw new ValidationFailureConstructionException( typeFailure: typeof( Exception ) );
         return new EValidation( VldType.Failure, failure );
      }

      private EValidation( VldType vldType, Exception failure )
      {
         _type = vldType;
         _failure = failure;
      }

      #endregion Construction

      #region Properties
      /// <summary> Returns an appropriate <see cref="VldType"/> for this instance, useful in case you want to use a switch-case.</summary>
      public VldType Enum { get => _type; }

      /// <summary> <see langword="true"/>, if this instance is a "success". </summary>
      public bool IsSuccess { get => _type == VldType.Success; }

      /// <summary> <see langword="true"/>, if this instance is a "failure", aka has a value of type <see cref="Exception"/>. </summary>
      public bool IsFailure { get => _type == VldType.Failure; }
      #endregion Properties

      #region Accessing Failure
      /// <summary> Returns this instance's "failed" value if <see cref="IsFailure"/> == <see langword="true"/>, otherwise throws a <see cref="ValidationNoFailureException"/>. </summary>
      /// <exception cref="ValidationNoFailureException"/>
      public Exception FailureOrThrow() =>
         IsFailure ? _failure : throw new ValidationNoFailureException( typeof( Exception ) );

      /// <summary>
      /// <para>Method exclusive to the E-Variants of all the Monads: Does nothing except throwing the contained exception if <see cref="IsFailure"/> is <see langword="true"/>.</para>
      /// <para>Use this method with care, exceptions should be wrapped inside monadic concepts for a reason!</para>
      /// </summary>
      public void ThrowContainedIfFailed() { if( IsFailure ) throw _failure; }

      #endregion Accessing Failure

      #region Mapping
      /// <summary>
      /// <para>Maps this instance by using its "failed" value (if any) as an argument for <paramref name="func"/> and returning a <see cref="EValidation"/> created from the result.</para>
      /// <para><paramref name="func"/> is only called if <see cref="IsFailure"/> == <see langword="true"/>.</para>
      /// <para>Otherwise, this instance is left untouched.</para>
      /// </summary>
      /// <param name="func">The delegate that provides the new failure.</param>
      public EValidation ExceptionMap( Func<Exception, Exception> func ) => IsSuccess ? Success : Failure( func( _failure ) );

      /// <summary>
      /// <para>Executes a side-effect, represented as the function <paramref name="func"/>, if and only if <see cref="IsFailure"/> == <see langword="true"/>.</para>
      /// <para>In any case, this instance is left untouched (but returned for easy functional chaining).</para>
      /// <para>*BEST PRACTICE:* Use this function for logging only!</para>
      /// </summary>
      /// <param name="func">The action to execute as side effect.</param>
      public EValidation ExceptionSideEffect( Action<Exception> func )
      {
         if( IsFailure ) func( _failure );
         return this;
      }
      #endregion Mapping

      #region Converters

      //no 'ToMaybe()', that really makes no sense for this class!

      /// <summary>
      /// Converts this instance into a <see cref="EFailable{TValue}"/>, which is then either a failure containing this instance's failure if
      /// <see cref="IsSuccess"/> == <see langword="false"/> or a success containing <paramref name="valueOnSuccess"/>
      /// otherwise.
      /// </summary>
      /// <param name="valueOnSuccess"> value to employ in case <see cref="IsSuccess"/> == <see langword="true"/>. </param>
      public EFailable<T> ToEFailable<T>( T valueOnSuccess ) => IsSuccess ? EFailable<T>.Success( valueOnSuccess ) : EFailable<T>.Failure( _failure );

      /// <summary>
      /// Converts this instance into a <see cref="EFailable{TValue}"/>, which is then either a failure containing this instance's failure if
      /// <see cref="IsSuccess"/> == <see langword="false"/> or a success containing the result of <paramref name="func"/>
      /// otherwise.
      /// </summary>
      /// <param name="func"> value to employ in case <see cref="IsSuccess"/> == <see langword="true"/>. </param>
      public EFailable<T> ToEFailable<T>( Func<T> func ) => IsSuccess ? EFailable<T>.Success( func() ) : EFailable<T>.Failure( _failure );

      /// <summary> Converts this instance into an appropriate <see cref="Validation{Exception}"/>. </summary>
      /// <returns> <see cref="Validation{Exception}"/>. </returns>
      public Validation<Exception> ToValidation() => IsSuccess ? Validation<Exception>.Success : Validation<Exception>.Failure( _failure );

      /// <summary> Converts this instance into a <see cref="EOption{Object}"/>, which is either a failure or empty (but never defined). </summary>
      /// <returns> <see cref="EOption{Object}"/> with its some-type being 'object' and its failure-type being <see cref="Exception"/>. </returns>
      public EOption<object> ToEOption() =>
         IsSuccess ? EOption<object>.None : EOption<object>.Failure( _failure );

      /// <summary> Converts this instance into a <see cref="Option{Object, Exception}"/>, which is either a failure or empty (but never defined). </summary>
      /// <returns> <see cref="Option{Object, Exception}"/> with its some-type being 'object' and its failure-type being <see cref="Exception"/>. </returns>
      public Option<object, Exception> ToOption() =>
         IsSuccess ? Option<object, Exception>.None : Option<object, Exception>.Failure( _failure );

      #endregion Converters

      #region Object

      /// <summary> Custom implementation of <see cref="object.ToString()"/>, wrapping the corresponding call to this instance's value, be it a "success" or a "failure". </summary>
      public override string ToString() =>
         IsSuccess
         ? $"[EValidation.Success]"
         : $"[EValidation.Failure: { _failure }]";


      /// <summary>
      /// <para>Returns <see langword="true"/> iff <see cref="IsSuccess"/> returns the same boolean for both objects and both instances reference the same
      /// <see cref="Exception"/> internally being <see langword="false"/>.</para>
      /// <para>Supports cross-class checks with <see cref="Validation{Exception}"/> following the same semantics as above!</para>
      /// </summary>
      public override bool Equals( object obj )
      {
         if( obj is EValidation otherE )
         {
            if( Enum != otherE.Enum ) return false;
            if( IsSuccess ) return true;
            else
            {
               var otherFail = otherE.FailureOrThrow();
               return ReferenceEquals( otherFail, _failure );
            }
         }
         else if( obj is Validation<Exception> otherV )
         {
            if( Enum != otherV.Enum ) return false;
            if( IsSuccess ) return true;
            else
            {
               var otherFail = otherV.FailureOrThrow();
               return ReferenceEquals( otherFail, _failure );
            }
         }
         else return false;
      }

      /// <summary>
      /// Custom implementation of <see cref="object.GetHashCode()"/>, wrapping a call to this instance's value, be it a "success" or a "failure".
      /// </summary>
      public override int GetHashCode() => IsSuccess ? int.MaxValue : _failure.GetHashCode();

      #endregion Object
   }
}