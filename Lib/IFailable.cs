using System;

namespace SIEDA.Monadic
{
   /// <summary>
   /// Interface for a object that represents the "result" of an operation that might have failed,
   /// containing either a value of type <typeparamref name="TValue"/> if the operation "was
   /// successful" or a different value of type <typeparamref name="TFail"/> if the operation "was a
   /// failure". Neither of these are ever <see langword="null"/>.
   /// </summary>
   /// <typeparam name="TValue">The type of the "successful" value.</typeparam>
   /// <typeparam name="TFail">The type of the "failed" value.</typeparam>
   public interface IFailable<TValue, TFail>
   {
      #region Properties

      /// <summary>
      /// <see langword="true"/>, if this instance is a "success", aka has a value of type
      /// <typeparamref name="TValue"/>.
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
      /// Maps this instance by using its "successful" value (if any) as an argument for <paramref
      /// name="func"/> and returning a <see cref="Failable{TNewValue, TFail}"/> created from the result.
      /// <para><paramref name="func"/> is only called if <see cref="IsSuccess"/> == <see langword="true"/>.</para>
      /// <para>
      /// Returns <see cref="Failable{TNewValue, TFail}.Failure(TFail)"/> if <see cref="IsFailure"/>
      /// == <see langword="true"/> with this instance's "failed" value being unchanged.
      /// </para>
      /// </summary>
      /// <typeparam name="TNewValue">The type of the new "successful" value.</typeparam>
      /// <param name="func">The delegate that provides the new value.</param>
      IFailable<TNewValue, TFail> Map<TNewValue>( Func<TValue, TNewValue> func );

      /// <summary>
      /// Maps this instance by using its "successful" value (if any) as an argument for <paramref
      /// name="func"/> and returning the result as a "flat" <see cref="IFailable{TNewValue,
      /// TFail}"/> (instead of a "Failable of a Failable").
      /// <para><paramref name="func"/> is only called if <see cref="IsSuccess"/> == <see langword="true"/>.</para>
      /// <para>
      /// Returns <see cref="Failable{TNewValue, TFail}.Failure(TFail)"/> if <see cref="IsFailure"/>
      /// == <see langword="true"/> or it is the result of <paramref name="func"/>.
      /// </para>
      /// </summary>
      /// <typeparam name="TNewValue">The type of the new "successful" value.</typeparam>
      /// <param name="func">The delegate that provides the new value which may fail.</param>

      IFailable<TNewValue, TFail> FlatMap<TNewValue>( Func<TValue, IFailable<TNewValue, TFail>> func );

      /// <summary>
      /// <see cref="IFailable{TValue, TFail}"/>-compatible equality-check for "successful" values.
      /// </summary>
      /// <param name="value">"Successful" value to check for equality.</param>
      /// <returns>
      /// <see langword="true"/> iff <see cref="IsSuccess"/> == <see langword="true"/><c>and</c> the
      /// <see cref="object.Equals(object)"/> override of this instance's value returns <see
      /// langword="true"/> for <paramref name="value"/>, otherwise <see langword="false"/>.
      /// </returns>
      bool Is( TValue value );

      /// <summary>
      /// Monadic predicate check for "successful" values.
      /// </summary>
      /// <param name="predicate">The delegate that checks the predicate.</param>
      /// <returns>
      /// <see langword="true"/> iff <see cref="IsSuccess"/> == <see
      /// langword="true"/><c>and</c><paramref name="predicate"/> returns <see langword="true"/> for
      /// this instance's "successful" value, otherwise <see langword="false"/>.
      /// </returns>
      bool Holds( Func<TValue, bool> predicate );

      #endregion Mapping

      #region Accessing Success

      /// <summary>
      /// Returns this instance's "successful" value if <see cref="IsSuccess"/> == <see langword="true"/>,
      /// otherwise <paramref name="otherwise"/>.
      /// </summary>
      /// <param name="otherwise">The desired value if this instance represents a "failure".</param>
      TValue Or( TValue otherwise );

      /// <summary>
      /// Returns this instance's "successful" value if <see cref="IsSuccess"/> == <see
      /// langword="true"/>, otherwise throws a <see cref="FailableFailureException"/>.
      /// </summary>
      /// <exception cref="FailableFailureException"/>
      TValue OrThrow();

      /// <summary>
      /// Returns this instance's "successful" value if <see cref="IsSuccess"/> == <see
      /// langword="true"/>, otherwise throws a <see cref="FailableFailureException"/> with the
      /// message <paramref name="msg"/>.
      /// </summary>
      /// <exception cref="FailableFailureException"/>
      TValue OrThrow( string msg );

      /// <summary>
      /// Returns this instance's "successful" value if <see cref="IsSuccess"/> == <see
      /// langword="true"/>, otherwise throws a <see cref="FailableFailureException"/> with the
      /// formatted message <paramref name="msg"/> using the message arguments <paramref name="args"/>.
      /// </summary>
      /// <exception cref="FailableFailureException"/>
      TValue OrThrow( string msg, params string[] args );

      /// <summary>
      /// Returns this instance's "successful" value if <see cref="IsSuccess"/> == <see
      /// langword="true"/>, otherwise throws the exception <paramref name="e"/>.
      /// </summary>
      /// <param name="e">The desired exception if this instance represents a "failure".</param>
      /// <exception cref="Exception"/>
      TValue OrThrow( Exception e );

      /// <summary>
      /// <para>
      /// Returns this instance's "successful" value if <see cref="IsSuccess"/> == <see
      /// langword="true"/>, otherwise <paramref name="otherwiseFunc"/> applied to the "failed" value.
      /// </para>
      /// <para>
      /// This method is exclusive to <see cref="IFailable{TValue, TFail}"/> since it only makes
      /// sense for this class. Its usage should be carefully considered.
      /// </para>
      /// </summary>
      /// <param name="otherwiseFunc">The desired value if this instance represents a "failure".</param>
      TValue OrUse( Func<TFail, TValue> otherwiseFunc );

      /// <summary>
      /// Writes this instance's "successful" value into the <see langword="out"/> parameter
      /// <paramref name="value"/> and returns <see langword="true"/> if <see cref="IsSuccess"/> ==
      /// <see langword="true"/>, otherwise <paramref name="value"/> will be set to the <see
      /// langword="default"/> value of <typeparamref name="TValue"/> and the method returns <see langword="false"/>.
      /// </summary>
      /// <param name="value">
      /// <see langword="out"/> parameter for this instance's "successful" value.
      /// </param>
      bool TryGetValue( out TValue value );

      #endregion Accessing Success

      #region Accessing Failure

      /// <summary>
      /// Returns this instance's "failed" value if <see cref="IsFailure"/> == <see
      /// langword="true"/>, otherwise throws a <see cref="FailableNoFailureException"/>.
      /// </summary>
      /// <exception cref="FailableNoFailureException"/>
      TFail FailureOrThrow();

      /// <summary>
      /// Writes this instance's "failed" value into the <see langword="out"/> parameter <paramref
      /// name="failure"/> and returns <see langword="true"/> if <see cref="IsFailure"/> == <see
      /// langword="true"/>, otherwise <paramref name="failure"/> will be set to the <see
      /// langword="default"/> value of <typeparamref name="TFail"/> and the method returns <see langword="false"/>.
      /// </summary>
      /// <param name="failure"><see langword="out"/> parameter for this instance's "failed" value.</param>
      bool TryGetFailure( out TFail failure );

      #endregion Accessing Failure

      #region Converters

      /// <summary>
      /// Converts this instance into a <see cref="Maybe{TValue}"/>.
      /// </summary>
      /// <returns>
      /// <see cref="Maybe{TValue}.Some(TValue)"/> if <see cref="IsSuccess"/> == <see
      /// langword="true"/> for this instance and <see cref="Maybe{TValue}.None"/> otherwise, thus
      /// *LOSING* the "failed" value of this instance.
      /// </returns>
      Maybe<TValue> ToMaybe();

      /// <summary>
      /// Converts this instance into an appropriate <see cref="Option{TValue, TFail}"/>.
      /// </summary>
      /// <returns>
      /// <see cref="Option{TValue, TFail}.Some(TValue)"/> if <see cref="IsSuccess"/> == <see
      /// langword="true"/> for this instance and <see cref="Option{TValue, TFail}.Failure(TFail)"/> otherwise.
      /// </returns>
      IOption<TValue, TFail> ToOption();

      /// <summary>
      /// Converts this instance into an appropriate <see cref="Validation{TFail}"/>. />.
      /// </summary>
      /// <returns>
      /// <see cref="Validation{TFail}.Success"/> if <see cref="IsSuccess"/> == <see langword="true"/>, thus *LOSING* the
      /// "successful" value of this instance. If <see cref="IsSuccess"/> == <see langword="false"/>, this method returns
      /// <see cref="Validation{TFail}.Failure(TFail)"/> with this instance's "failed" value instead.
      /// </returns>
      IValidation<TFail> ToValidation();

      #endregion Converters

      #region Object

      /// <summary>
      /// Custom implementation of <see cref="object.ToString()"/>, wrapping the corresponding call
      /// to this instance's value, be it a "success" or a "failure". This will return an internal
      /// representation of this instances state, suitable for debugging only.
      /// </summary>
      string ToString();

      /// <summary>
      /// <para>
      /// Returns <see langword="true"/> if <see cref="IsSuccess"/> returns the same boolean for
      /// both objects and
      /// </para>
      /// <para>
      /// - if <see cref="IsSuccess"/> being <see langword="true"/>: result of method <see
      ///   cref="object.Equals(object)"/> for the two instances' "some" values
      /// </para>
      /// <para>
      /// - if <see cref="IsSuccess"/> being <see langword="false"/>: result of method <see
      ///   cref="object.Equals(object)"/> for the two instances' "failed" values
      /// </para>
      /// <para>Respects type, a <see cref="IFailable{A, B}"/> and a <see cref="IFailable{C,D}"/> are never equal!</para>
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