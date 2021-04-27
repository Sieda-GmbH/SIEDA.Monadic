using System;

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
   public interface IValidation<TFail>
   {
      #region Properties

      /// <summary>
      /// <see langword="true"/>, if this instance is a "success".
      /// </summary>
      bool IsSuccess { get; }

      /// <summary>
      /// <see langword="true"/>, if this instance is a "failure", aka has a value of type
      /// <typeparamref name="TFail"/>.
      /// </summary>
      bool IsFailure { get; }

      #endregion Properties

      #region Mapping
      /// <summary>
      /// Maps this instance by using its "failed" value (if any) as an argument for <paramref name="func"/>
      /// and returning a <see cref="Validation{TNewFail}"/> created from the result.
      /// <para><paramref name="func"/> is only called if <see cref="IsSuccess"/> == <see langword="false"/>.</para>
      /// <para>Returns <see cref="Validation{TNewValue}.Success"/> if <see cref="IsSuccess"/> == <see langword="true"/>.</para>
      /// </summary>
      /// <typeparam name="TNewFail">The type of the new "failed" value.</typeparam>
      /// <param name="func">The delegate that provides the new failure.</param>
      Validation<TNewFail> FailMap<TNewFail>( Func<TFail, TNewFail> func );
      #endregion Mapping

      #region Accessing Failure

      /// <summary>
      /// Returns this instance's "failed" value if <see cref="IsFailure"/> == <see langword="true"/>,
      /// otherwise throws a <see cref="ValidationNoFailureException"/>.
      /// </summary>
      /// <exception cref="ValidationNoFailureException"/>
      TFail FailureOrThrow();

      /// <summary>
      /// Writes this instance's "failed" value into the <see langword="out"/> parameter <paramref
      /// name="value"/> and returns <see langword="true"/> if <see cref="IsFailure"/> == <see
      /// langword="true"/>, otherwise <paramref name="value"/> will be set to the <see
      /// langword="default"/> value of <typeparamref name="TFail"/> and the method returns <see langword="false"/>.
      /// </summary>
      /// <param name="value"><see langword="out"/> parameter for this instance's "failed" value.</param>
      bool TryGetFailure( out TFail value );

      #endregion Accessing Failure

      #region Converters

      /// <summary>
      /// Converts this instance into a <see cref="IOption{TValue, TFail}"/>, which is either a
      /// failure or empty (but never defined).
      /// </summary>
      /// <returns>
      /// <see cref="IOption{TValue, TFail}"/> with its some-type being 'object' and its failure-type
      /// being <typeparamref name="TFail"/>.
      /// </returns>
      IOption<object, TFail> ToOption();

      #endregion Converters

      #region Object

      /// <summary>
      /// Custom implementation of <see cref="object.ToString()"/>, wrapping the corresponding call
      /// to this instance's value, be it a "success" or a "failure".
      /// </summary>
      string ToString();

      /// <summary>
      /// <para>Returns <see langword="true"/> iff <see cref="IsSuccess"/> returns the same boolean for
      /// both objects and the method <see cref="object.Equals(object, object)"/> returns <see
      /// langword="true"/> the two instances' "failed" values in case of <see cref="IsSuccess"/>
      /// being <see langword="false"/>.</para>
      /// <para>Respects type, a <see cref="IValidation{A}"/> and a <see cref="IValidation{B}"/> are never equal!</para>
      /// </summary>
      bool Equals( object obj );

      /// <summary>
      /// Custom implementation of <see cref="object.GetHashCode()"/>, wrapping a call to this
      /// instance's value, be it a "success" or a "failure".
      /// </summary>
      int GetHashCode();

      #endregion Object
   }
}