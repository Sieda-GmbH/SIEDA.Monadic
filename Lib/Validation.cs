using System;
using SIEDA.Monadic.SwitchCase;

namespace SIEDA.Monadic
{
   /// <summary>
   /// <para>
   /// Represents the "result" of a check that might have failed, that is an operation that either
   /// succeeds without any error or fails with some information <typeparamref name="TFail"/> if
   /// the operation "was a failure". Never <see langword="null"/>.
   /// </para>
   /// <para>
   /// One can think of this as an "inverted" <see cref="Maybe{TValue}"/>, although this obviously comes
   /// with different intended semantics and API restrictions. An alternative interpretion of this
   /// class is a <see cref="Failable{TValue, TFail}"/> without a value when it is successful.
   /// </para>
   /// </summary>
   /// <typeparam name="TFail">The type of the "failed" value.</typeparam>
   public readonly struct Validation<TFail>
   {
      #region State

      private readonly TFail _failure;
      private readonly VldType _type;

      #endregion State

      #region Construction

      /// <summary>
      /// The <see cref="Validation{TFail}"/> with a "successful" outcome.
      /// </summary>
      public static readonly Validation<TFail> Success = new Validation<TFail>( VldType.Success, default );

      /// <summary>
      /// Creates a <see cref="Validation{TFail}"/> with a <paramref name="failure"/>-value, which
      /// must not be <see langword="null"/>.
      /// </summary>
      /// <exception cref="ValidationFailureConstructionException">
      /// if <paramref name="failure"/> == <see langword="null"/>.
      /// </exception>
      public static Validation<TFail> Failure( TFail failure )
      {
         if( ReferenceEquals( failure, null ) ) throw new ValidationFailureConstructionException( typeFailure: typeof( TFail ) );
         return new Validation<TFail>( VldType.Failure, failure );
      }

      private Validation( VldType vldType, TFail failure )
      {
         _type = vldType;
         _failure = failure;
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
      /// <see langword="true"/>, if this instance is a "failure", aka has a value of type
      /// <typeparamref name="TFail"/>.
      /// </summary>
      public bool IsFailure { get => _type == VldType.Failure; }
      #endregion Properties

      #region Accessing Failure
      /// <summary>
      /// Returns this instance's "failed" value if <see cref="IsFailure"/> == <see langword="true"/>,
      /// otherwise throws a <see cref="ValidationNoFailureException"/>.
      /// </summary>
      /// <exception cref="ValidationNoFailureException"/>
      public TFail FailureOrThrow() =>
         IsFailure ? _failure : throw new ValidationNoFailureException( typeof( TFail ) );

      /// <summary>
      /// Writes this instance's "failed" value into the <see langword="out"/> parameter <paramref
      /// name="value"/> and returns <see langword="true"/> if <see cref="IsFailure"/> == <see
      /// langword="true"/>, otherwise <paramref name="value"/> will be set to the <see
      /// langword="default"/> value of <typeparamref name="TFail"/> and the method returns <see langword="false"/>.
      /// </summary>
      /// <param name="value"><see langword="out"/> parameter for this instance's "failed" value.</param>
      public bool TryGetFailure( out TFail value )
      {
         value = IsFailure ? _failure : default;
         return IsFailure;
      }

      #endregion Accessing Failure

      #region Mapping
      /// <summary>
      /// Maps this instance by using its "failed" value (if any) as an argument for <paramref name="func"/>
      /// and returning a <see cref="Validation{TNewFail}"/> created from the result.
      /// <para><paramref name="func"/> is only called if <see cref="IsSuccess"/> == <see langword="false"/>.</para>
      /// <para>Returns <see cref="Validation{TNewValue}.Success"/> if <see cref="IsSuccess"/> == <see langword="true"/>.</para>
      /// </summary>
      /// <typeparam name="TNewFail">The type of the new "failed" value.</typeparam>
      /// <param name="func">The delegate that provides the new failure.</param>
      public Validation<TNewFail> FailMap<TNewFail>( Func<TFail, TNewFail> func ) => IsSuccess ? Validation<TNewFail>.Success : Validation<TNewFail>.Failure( func( _failure ) );
      #endregion Mapping

      #region Converters

      //no 'ToMaybe()', that really makes no sense for this class!

      /// <summary>
      /// Converts this instance into a <see cref="Failable{TValue, TFail}"/>, which is then either a failure if
      /// <see cref="IsSuccess"/> == <see langword="false"/> or a success containing <paramref name="valueOnSuccess"/>
      /// otherwise.
      /// </summary>
      /// <param name="valueOnSuccess"> value to employ in case <see cref="IsSuccess"/> == <see langword="true"/>. </param>.
      public Failable<T, TFail> ToFailable<T>( T valueOnSuccess ) => IsSuccess ? Failable<T, TFail>.Success( valueOnSuccess ) : Failable<T, TFail>.Failure( _failure );

      /// <summary>
      /// Converts this instance into a <see cref="EFailable{TValue}"/>, which is then either a failure containing <paramref name="error"/> if
      /// <see cref="IsSuccess"/> == <see langword="false"/> or a success containing <paramref name="valueOnSuccess"/>
      /// otherwise.
      /// </summary>
      /// <param name="valueOnSuccess"> value to employ in case <see cref="IsSuccess"/> == <see langword="true"/>. </param>
      /// <param name="error"> value to employ in case <see cref="IsSuccess"/> == <see langword="false"/>. </param>
      public EFailable<T> ToEFailable<T>( T valueOnSuccess, Exception error ) => IsSuccess ? EFailable<T>.Success( valueOnSuccess ) : EFailable<T>.Failure( error );

      /// <summary>
      /// Converts this instance into a <see cref="EFailable{TValue}"/>, which is then either a failure containing the result of <paramref name="func"/> if
      /// <see cref="IsSuccess"/> == <see langword="false"/> or a success containing <paramref name="valueOnSuccess"/>
      /// otherwise.
      /// </summary>
      /// <param name="valueOnSuccess"> value to employ in case <see cref="IsSuccess"/> == <see langword="true"/>. </param>
      /// <param name="func"> function computing the value to employ in case <see cref="IsSuccess"/> == <see langword="false"/>. </param>
      public EFailable<T> ToEFailable<T>( T valueOnSuccess, Func<TFail, Exception> func ) => IsSuccess ? EFailable<T>.Success( valueOnSuccess ) : EFailable<T>.Failure( func( _failure ) );

      /// <summary>
      /// Converts this instance into a <see cref="Option{Object, TFail}"/>, which is either a
      /// failure or empty (but never defined).
      /// </summary>
      /// <returns>
      /// <see cref="Option{Object, TFail}"/> with its some-type being 'object' and its failure-type
      /// being <typeparamref name="TFail"/>.
      /// </returns>
      public Option<object, TFail> ToOption() => IsSuccess ? Option<object, TFail>.None : Option<object, TFail>.Failure( _failure );

      /// <summary>
      /// <para>Converts this instance into an appropriate <see cref="EOption{TValue}"/>.</para>
      /// <para>Note that any "failed" value this instance might have is lost in the conversion
      ///       and the exception given via <paramref name="error"/> is used instead.</para>
      /// </summary>
      /// <returns><see cref="EOption{Object}.None"/> if <see cref="IsSuccess"/> == <see langword="true"/> for
      /// this instance and <see cref="EOption{Object}.Failure(Exception)"/> containing <paramref name="error"/> otherwise.</returns>
      public EOption<object> ToEOption( Exception error ) => IsSuccess ? EOption<object>.None : EOption<object>.Failure( error );

      /// <summary>
      /// <para>Converts this instance into an appropriate <see cref="EOption{TValue}"/> using converter-function <paramref name="func"/>.</para>
      /// <para>Note that <paramref name="func"/> is only called if <see cref="IsSuccess"/> == <see langword="false"/>.</para>
      /// </summary>
      /// <returns><see cref="EOption{Object}.None"/> if <see cref="IsSuccess"/> == <see langword="true"/> for
      /// this instance and <see cref="EOption{Object}.Failure(Exception)"/> containing the result of <paramref name="func"/> applied
      /// to this instance's failure-value otherwise.</returns>
      public EOption<object> ToEOption( Func<TFail, Exception> func ) => IsSuccess ? EOption<object>.None : EOption<object>.Failure( func( _failure ) );

      /// <summary>
      /// <para>Converts this instance into an appropriate <see cref="EValidation"/>.</para>
      /// <para>Note that any "failed" value this instance might have is lost in the conversion
      ///       and the exception given via <paramref name="error"/> is used instead.</para>
      /// </summary>
      /// <returns><see cref="EValidation.Success"/> if <see cref="IsSuccess"/> == <see langword="true"/> for
      /// this instance and <see cref="EValidation.Failure(Exception)"/> containing <paramref name="error"/> otherwise.</returns>
      public EValidation ToEValidation( Exception error ) => IsSuccess ? EValidation.Success : EValidation.Failure( error );

      /// <summary>
      /// <para>Converts this instance into an appropriate <see cref="EValidation"/> using converter-function <paramref name="func"/>.</para>
      /// <para>Note that <paramref name="func"/> is only called if <see cref="IsSuccess"/> == <see langword="false"/>.</para>
      /// </summary>
      /// <returns><see cref="EValidation.Success"/> if <see cref="IsSuccess"/> == <see langword="true"/> for
      /// this instance and <see cref="EValidation.Failure(Exception)"/> containing the result of <paramref name="func"/> applied
      /// to this instance's failure-value otherwise.</returns>
      public EValidation ToEValidation( Func<TFail, Exception> func ) => IsSuccess ? EValidation.Success : EValidation.Failure( func( _failure ) );

      #endregion Converters

      #region Object

      /// <summary>
      /// Custom implementation of <see cref="object.ToString()"/>, wrapping the corresponding call
      /// to this instance's value, be it a "success" or a "failure".
      /// </summary>
      public override string ToString() =>
         IsSuccess
         ? $"[Validation<{typeof( TFail ).Name}>.Success]"
         : $"[Validation<{typeof( TFail ).Name}>.Failure: { _failure }]";


      /// <summary>
      /// <para>Returns <see langword="true"/> iff <see cref="IsSuccess"/> returns the same boolean for
      /// both objects and the method <see cref="object.Equals(object, object)"/> returns <see
      /// langword="true"/> the two instances' "failed" values in case of <see cref="IsSuccess"/>
      /// being <see langword="false"/>.</para>
      /// <para>Respects type, a <see cref="Validation{A}"/> and a <see cref="Validation{B}"/> are never equal!</para>
      /// <para>Supports cross-class checks with <see cref="EValidation"/>, calling <see cref="EValidation.Equals(object)"/>-method.</para>
      /// </summary>
      public override bool Equals( object obj )
      {
         if( obj is Validation<TFail> otherF )
         {
            if( Enum != otherF.Enum ) return false;
            if( IsSuccess ) return true;
            else
            {
               var otherFail = otherF.FailureOrThrow();
               if( _failure is string _strFail && otherFail is string otherStrFail )
                  return _strFail.Equals( otherStrFail, StringComparison.Ordinal );
               else
                  return _failure.Equals( otherFail );
            }
         }
         else if( obj is EValidation otherE )
            return otherE.Equals( this ); //only maintain one E-Equals-Implementation!
         else
            return false;
      }


      /// <summary>
      /// Custom implementation of <see cref="object.GetHashCode()"/>, wrapping a call to this
      /// instance's value, be it a "success" or a "failure".
      /// </summary>
      public override int GetHashCode() => IsSuccess ? int.MaxValue : _failure.GetHashCode();

      #endregion Object
   }
}