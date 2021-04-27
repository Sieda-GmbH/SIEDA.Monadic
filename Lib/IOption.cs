using System;

namespace SIEDA.Monadic
{
   /// <summary>
   /// <para>
   /// Interface for a object that is a sort of combination of a <see cref="Maybe{TValue}"/> and a <see
   /// cref="IFailable{TValue, TFail}"/>: Represents the "result" of an operation that might have
   /// failed with a value that may be absent even if the operation did not fail.
   /// </para>
   /// <para>
   /// If the operation did not fail, there may be either "some" value of type <typeparamref
   /// name="TValue"/> or no value ("none (of the possible values)") without a failure. If the
   /// operation was indeed a "failure", there is a value of type <typeparamref name="TFail"/>.
   /// </para>
   /// </summary>
   /// <typeparam name="TValue">The type of the value.</typeparam>
   /// <typeparam name="TFail">The type of the "failed" value.</typeparam>
   public interface IOption<TValue, TFail>
   {
      #region Properties

      /// <summary>
      /// <see langword="true"/>, if this instance has a value, is "some", its value is present.
      /// </summary>
      bool IsSome { get; }

      /// <summary>
      /// <see langword="true"/>, if this instance has no value,
      /// its value is absent, but it is still no failure.
      /// </summary>
      bool IsNone { get; }

      /// <summary>
      /// <see langword="true"/>, if this instance is a "failure", aka has a value of type
      /// <typeparamref name="TFail"/>.
      /// </summary>
      bool IsFailure { get; }

      /// <summary>
      /// <see langword="true"/> if this instance has no value, meaning either <see cref="IsNone"/>
      /// or <see cref="IsFailure"/> is <see langword="true"/>.
      /// </summary>
      bool IsNotSome { get; }

      /// <summary>
      /// <see langword="true"/>, if this instance is no "failure", meaning either <see
      /// cref="IsSome"/> or <see cref="IsNone"/> is <see langword="true"/>.
      /// </summary>
      bool IsNotFailure { get; }

      #endregion Properties

      #region Mapping

      /// <summary>
      /// Maps this instance by using its value (if any) as an argument for <paramref name="func"/>
      /// and returning an <see cref="IOption{TNewValue, TFail}"/> created from the result.
      /// <para><paramref name="func"/> is only called if <see cref="IsSome"/> == <see langword="true"/>.</para>
      /// <para>
      /// Returns <see cref="Option{TNewValue, TFail}.None"/> if <see cref="IsNone"/> == <see
      /// langword="true"/> or the result of <paramref name="func"/> == <see langword="null"/>.
      /// </para>
      /// <para>
      /// Returns <see cref="Option{TNewValue, TFail}.Failure(TFail)"/> if <see cref="IsFailure"/>
      /// == <see langword="true"/> with this instance's "failed" value being unchanged.
      /// </para>
      /// </summary>
      /// <typeparam name="TNewValue">The type of the new value.</typeparam>
      /// <param name="func">The delegate that provides the new value.</param>
      IOption<TNewValue, TFail> Map<TNewValue>( Func<TValue, TNewValue> func );

      /// <summary>
      /// Maps this instance by using its value (if any) as an argument for <paramref name="func"/>
      /// and returning the result as a "flat" <see cref="IOption{TNewValue, TFail}"/> (instead of
      /// an "Option of an Option").
      /// <para><paramref name="func"/> is only called if <see cref="IsSome"/> == <see langword="true"/>.</para>
      /// <para>
      /// Returns <see cref="Option{TNewValue, TFail}.None"/> if <see cref="IsNone"/> == <see
      /// langword="true"/> or it is the result of <paramref name="func"/>.
      /// </para>
      /// <para>
      /// Returns <see cref="Option{TNewValue, TFail}.Failure(TFail)"/> if <see cref="IsFailure"/>
      /// == <see langword="true"/> or it is the result of <paramref name="func"/>.
      /// </para>
      /// </summary>
      /// <typeparam name="TNewValue">The type of the new value.</typeparam>
      /// <param name="func">
      /// The delegate that provides the new value which may fail or result in an absent value.
      /// </param>
      IOption<TNewValue, TFail> FlatMap<TNewValue>( Func<TValue, IOption<TNewValue, TFail>> func );

      /// <summary>
      /// <see cref="IOption{TValue, TFail}"/>-compatible equality-check for values.
      /// </summary>
      /// <param name="value">Value to check for equality.</param>
      /// <returns>
      /// <see langword="true"/> iff <see cref="IsSome"/> == <see langword="true"/><c>and</c> the
      /// <see cref="object.Equals(object)"/> override of this instance's value returns <see
      /// langword="true"/> for <paramref name="value"/>, otherwise <see langword="false"/>.
      /// </returns>
      bool Is( TValue value );

      /// <summary>
      /// Monadic predicate check for values.
      /// </summary>
      /// <param name="predicate">The delegate that checks the predicate.</param>
      /// <returns>
      /// <see langword="true"/> iff <see cref="IsSome"/> == <see
      /// langword="true"/><c>and</c><paramref name="predicate"/> returns <see langword="true"/> for
      /// this instance's value, otherwise <see langword="false"/>.
      /// </returns>
      bool Holds( Func<TValue, bool> predicate );

      #endregion Mapping

      #region Accessing Value

      /// <summary>
      /// Returns this instance's value if <see cref="IsSome"/> == <see langword="true"/>, otherwise
      /// <paramref name="otherwise"/>.
      /// </summary>
      /// <param name="otherwise">
      /// The desired value if this instance has no value (with or without failure).
      /// </param>
      TValue Or( TValue otherwise );

      /// <summary>
      /// Returns this instance's value if <see cref="IsSome"/> == <see langword="true"/>, otherwise
      /// throws an <see cref="OptionNoneException"/> if <see cref="IsNone"/> == <see
      /// langword="true"/> or an <see cref="OptionFailureException"/> if <see cref="IsFailure"/> ==
      /// <see langword="true"/>.
      /// </summary>
      /// <exception cref="OptionNoneException"/>
      /// <exception cref="OptionFailureException"/>
      TValue OrThrow();

      /// <summary>
      /// Returns this instance's value if <see cref="IsSome"/> == <see langword="true"/>, otherwise
      /// throws an <see cref="OptionNoneException"/> if <see cref="IsNone"/> == <see
      /// langword="true"/> or an <see cref="OptionFailureException"/> if <see cref="IsFailure"/> ==
      /// <see langword="true"/>, both with the message <paramref name="msg"/>.
      /// </summary>
      /// <exception cref="OptionNoneException"/>
      /// <exception cref="OptionFailureException"/>
      TValue OrThrow( string msg );

      /// <summary>
      /// Returns this instance's value if <see cref="IsSome"/> == <see langword="true"/>, otherwise
      /// throws an <see cref="OptionNoneException"/> if <see cref="IsNone"/> == <see
      /// langword="true"/> or an <see cref="OptionFailureException"/> if <see cref="IsFailure"/> ==
      /// <see langword="true"/>, both with the formatted message <paramref name="msg"/> using the
      /// message arguments <paramref name="args"/>.
      /// </summary>
      /// <exception cref="OptionNoneException"/>
      /// <exception cref="OptionFailureException"/>
      TValue OrThrow( string msg, params string[] args );

      /// <summary>
      /// Returns this instance's value if <see cref="IsSome"/> == <see langword="true"/>, otherwise
      /// throws the exception <paramref name="e"/>.
      /// </summary>
      /// <param name="e">
      /// The desired exception if this instance has no value (with or without failure).
      /// </param>
      /// <exception cref="Exception"/>
      TValue OrThrow( Exception e );

      /// <summary>
      /// Writes this instance's value into the <see langword="out"/> parameter <paramref
      /// name="value"/> and returns <see langword="true"/> if <see cref="IsSome"/> == <see
      /// langword="true"/>, otherwise <paramref name="value"/> will be set to the <see
      /// langword="default"/> value of <typeparamref name="TValue"/> and the method returns <see langword="false"/>.
      /// </summary>
      /// <param name="value"><see langword="out"/> parameter for this instance's value.</param>
      /// <remarks>Not particularly functional, but a concession to typical .NET methods.</remarks>
      bool TryGetValue( out TValue value );

      #endregion Accessing Value

      #region Accessing Failure

      /// <summary>
      /// Returns this instance's "failed" value if <see cref="IsFailure"/> == <see
      /// langword="true"/>, otherwise throws a <see cref="OptionNoFailureException"/>.
      /// </summary>
      /// <exception cref="OptionNoFailureException"/>
      TFail FailureOrThrow();

      /// <summary>
      /// Writes this instance's "failed" value into the <see langword="out"/> parameter <paramref
      /// name="failure"/> and returns <see langword="true"/> if <see cref="IsFailure"/> == <see
      /// langword="true"/>, otherwise <paramref name="failure"/> will be set to the <see
      /// langword="default"/> value of <typeparamref name="TFail"/> and the method returns <see langword="false"/>.
      /// </summary>
      /// <param name="failure"><see langword="out"/> parameter for this instance's "failed" value.</param>
      /// <remarks>Not particularly functional, but a concession to typical .NET methods.</remarks>
      bool TryGetFailure( out TFail failure );

      #endregion Accessing Failure

      #region Converters

      /// <summary>
      /// Converts this instance into a <see cref="Maybe{TValue}"/>.
      /// </summary>
      /// <returns>
      /// <see cref="Maybe{TValue}.Some(TValue)"/> if <see cref="IsSome"/> == <see langword="true"/>
      /// for this instance and <see cref="Maybe{TValue}.None"/> otherwise, thus *LOSING* the
      /// "failed" value of this instance.
      /// </returns>
      Maybe<TValue> ToMaybe();

      /// <summary>
      /// Converts this instance into an appropriate <see cref="IFailable{TValue, TFail}"/>.
      /// </summary>
      /// <param name="errorOnNone">
      /// An object representing a "failure", used in case <see cref="IsNone"/> == <see langword="true"/>
      /// </param>
      /// <returns>
      /// <see cref="Failable{TValue, TFail}.Success(TValue)"/> if <see cref="IsSome"/> == <see
      /// langword="true"/> for this instance, otherwise <see cref="Failable{TValue,
      /// TFail}.Failure(TFail)"/> containing <paramref name="errorOnNone"/> if <see cref="IsNone"/>
      /// == <see langword="true"/> or this instance's "failed" value if <see cref="IsFailure"/> ==
      /// <see langword="true"/>.
      /// </returns>
      IFailable<TValue, TFail> ToFailable( TFail errorOnNone );

      /// <summary>
      /// Converts this instance into an appropriate <see cref="Validation{TFail}"/>. />.
      /// </summary>
      /// <returns>
      /// <see cref="Validation{TFail}.Success"/> if <see cref="IsFailure"/> == <see langword="false"/>, thus *LOSING* a
      /// "successful value" or "no value" state of this instance. If <see cref="IsFailure"/> == <see langword="true"/>, this method returns
      /// <see cref="Validation{TFail}.Failure(TFail)"/> with this instance's "failed" value instead.
      /// </returns>
      IValidation<TFail> ToValidation();

      /// <summary>
      /// Converts this instance into an appropriate <see cref="Validation{TFail}"/>. />.
      /// </summary>
      /// <returns>
      /// <see cref="Validation{TFail}.Success"/> if <see cref="IsSome"/> == <see langword="true"/>, thus *LOSING* a
      /// "successful value" of this instance. If <see cref="IsNone"/> == <see langword="true"/>, <paramref name="errorOnNone"/>
      /// is used to construct and return <see cref="Validation{TFail}.Failure(TFail)"/> and, finally, if instead <see cref="IsFailure"/>
      /// == <see langword="true"/>, this method returns with this instance's "failed" value instead.
      /// </returns>
      IValidation<TFail> ToValidation( TFail errorOnNone );

      #endregion Converters

      #region Object

      /// <summary>
      /// Custom implementation of <see cref="object.ToString()"/>, wrapping the corresponding call
      /// to this instance's value, if any, be it a "failure" or not. This will return an internal
      /// representation of this instances state, suitable for debugging only.
      /// </summary>
      string ToString();

      /// <summary>
      /// <para>
      /// Returns <see langword="true"/> iff <see cref="IsNone"/> return the same booleans for both
      /// objects and
      /// </para>
      /// <para>
      /// - if <see cref="IsSome"/> being <see langword="true"/>: result of method <see
      ///   cref="object.Equals(object)"/> for the two instances' "some" values
      /// </para>
      /// <para>- if <see cref="IsNone"/> being <see langword="true"/>: (no further condition)</para>
      /// <para>
      /// - if <see cref="IsFailure"/> being <see langword="true"/>: result of method <see
      ///   cref="object.Equals(object)"/> for the two instances' "failed" values
      /// </para>
      /// <para>Respects type, a <see cref="IOption{A, B}"/> and a <see cref="IOption{C,D}"/> are never equal!</para>
      /// </summary>
      bool Equals( object obj );

      /// <summary>
      /// <para>
      /// Custom implementation of <see cref="object.GetHashCode()"/>, wrapping a call to this
      /// instance's value, if any, be it a "failure" or not.
      /// </para>
      /// <para>
      /// Note that all <see cref="IOption{TValue, TFail}"/>-objects, regardless of type
      /// <typeparamref name="TValue"/>, have the same hash code when there is no value and no
      /// failure, aka when <see cref="IsNone"/> == <see langword="true"/>.
      /// </para>
      /// </summary>
      int GetHashCode();

      #endregion Object
   }
}