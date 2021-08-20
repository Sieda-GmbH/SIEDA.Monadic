using System;
using System.Runtime.CompilerServices;
using Monadic.SwitchCase;

namespace SIEDA.Monadic
{
   /// <summary>
   /// <para>
   /// Interface for a object that is a sort of combination of a <see cref="Maybe{TValue}"/> and a <see
   /// cref="Failable{TValue, TFail}"/>: Represents the "result" of an operation that might have
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
   public readonly struct Option<TValue, TFail>
   {
      #region State

      private readonly TValue _value;
      private readonly TFail _failure;
      private readonly OptType _type;

      #endregion State

      #region Construction

      /// <summary>
      /// Empty instance, no value present, failure absent.
      /// </summary>
      public static readonly Option<TValue, TFail> None = new Option<TValue, TFail>( OptType.None, default, default );

      /// <summary>
      /// Creates an <see cref="Option{TValue, TFail}"/> with value <paramref name="value"/>.
      /// </summary>
      /// <exception cref="OptionSomeConstructionException">
      /// if <paramref name="value"/> == <see langword="null"/>.
      /// </exception>
      public static Option<TValue, TFail> Some( TValue value )
      {
         if( ReferenceEquals( value, null ) )
         {
            throw new OptionSomeConstructionException( typeValue: typeof( TValue ), typeFailure: typeof( TFail ) );
         }

         return new Option<TValue, TFail>( OptType.Some, value, default );
      }

      /// <summary>
      /// Creates an <see cref="Option{TValue, TFail}"/> with the value of the <see
      /// cref="Nullable{T}"/><paramref name="nullableValue"/>.
      /// </summary>
      /// <exception cref="OptionSomeConstructionException">
      /// if <paramref name="nullableValue"/> == <see langword="null"/> (has no value).
      /// </exception>
      public static Option<TValue, TFail> Some<T>( T? nullableValue ) where T : struct, TValue
      {
         if( !nullableValue.HasValue )
         {
            throw new OptionSomeConstructionException( typeValue: typeof( TValue ), typeFailure: typeof( TFail ) );
         }

         return new Option<TValue, TFail>( OptType.Some, nullableValue.Value, default );
      }

      /// <summary>
      /// <para>
      /// Creates and returns an <see cref="Option{TValue, TFail}"/> with value <paramref
      /// name="value"/> if <paramref name="value"/> != <see langword="null"/>.
      /// </para>
      /// <para>Returns <see cref="None"/> if <paramref name="value"/> == <see langword="null"/>.</para>
      /// </summary>
      public static Option<TValue, TFail> From( TValue value ) =>
         ReferenceEquals( value, null ) ? None : new Option<TValue, TFail>( OptType.Some, value, default );

      /// <summary>
      /// <para>
      /// Creates and returns an <see cref="Option{TValue, TFail}"/> with the value of the <see
      /// cref="Nullable{T}"/><paramref name="nullableValue"/> if <paramref name="nullableValue"/>
      /// != <see langword="null"/> (has a value).
      /// </para>
      /// <para>
      /// Returns <see cref="None"/> if <paramref name="nullableValue"/> == <see langword="null"/>
      /// (has no value).
      /// </para>
      /// </summary>
      public static Option<TValue, TFail> From<T>( T? nullableValue ) where T : struct, TValue =>
          nullableValue.HasValue ? new Option<TValue, TFail>( OptType.Some, nullableValue.Value, default ) : None;

      /// <summary>
      /// Creates an <see cref="Option{TValue, TFail}"/> with a <paramref name="failure"/>-value,
      /// which must not be <see langword="null"/>.
      /// </summary>
      /// <exception cref="OptionFailureConstructionException">
      /// if <paramref name="failure"/> == <see langword="null"/>.
      /// </exception>
      public static Option<TValue, TFail> Failure( TFail failure )
      {
         if( ReferenceEquals( failure, null ) )
         {
            throw new OptionFailureConstructionException( typeValue: typeof( TValue ), typeFailure: typeof( TFail ) );
         }

         return new Option<TValue, TFail>( OptType.Failure, default, failure );
      }

      private Option( OptType optType, TValue value, TFail failure )
      {
         _type = optType;
         _value = value;
         _failure = failure;
      }

      #endregion Construction

      #region Properties
      /// <summary> Returns an appropriate <see cref="OptType"/> for this instance, useful in case you want to use a switch-case.</summary>
      public OptType Enum { get => _type; }

      /// <summary>
      /// <see langword="true"/>, if this instance has a value, is "some", its value is present.
      /// </summary>
      public bool IsSome { get => _type == OptType.Some; }

      /// <summary>
      /// <see langword="true"/>, if this instance has no value,
      /// its value is absent, but it is still no failure.
      /// </summary>
      public bool IsNone { get => _type == OptType.None; }

      /// <summary>
      /// <see langword="true"/>, if this instance is a "failure", aka has a value of type
      /// <typeparamref name="TFail"/>.
      /// </summary>
      public bool IsFailure { get => _type == OptType.Failure; }

      /// <summary>
      /// <see langword="true"/> if this instance has no value, meaning either <see cref="IsNone"/>
      /// or <see cref="IsFailure"/> is <see langword="true"/>.
      /// </summary>
      public bool IsNotSome => !IsSome;

      /// <summary>
      /// <see langword="true"/>, if this instance is no "failure", meaning either <see
      /// cref="IsSome"/> or <see cref="IsNone"/> is <see langword="true"/>.
      /// </summary>
      public bool IsNotFailure => !IsFailure;

      #endregion Properties

      #region Mapping

      /// <summary>
      /// Maps this instance by using its value (if any) as an argument for <paramref name="func"/>
      /// and returning an <see cref="Option{TNewValue, TFail}"/> created from the result.
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
      public Option<TNewValue, TFail> Map<TNewValue>( Func<TValue, TNewValue> func ) =>
         IsSome
            ? Option<TNewValue, TFail>.From( func( _value ) )
            : IsNone
               ? Option<TNewValue, TFail>.None
               : Option<TNewValue, TFail>.Failure( _failure );

      /// <summary>
      /// Maps this instance by using its "failed" value (if any) as an argument for <paramref
      /// name="func"/> and returning a <see cref="Option{TValue, TNewFail}"/> created from the result.
      /// <para><paramref name="func"/> is only called if <see cref="IsFailure"/> == <see langword="true"/>.</para>
      /// <para>
      /// Returns <see cref="Option{TValue, TNewFail}.Some(TValue)"/> if <see cref="IsSome"/>
      /// == <see langword="true"/> with this instance's "successful" value being unchanged and returns
      /// <see cref="Option{TValue, TNewFail}.None"/> if <see cref="IsNone"/> == <see langword="true"/>.
      /// </para>
      /// </summary>
      /// <typeparam name="TNewFail">The type of the new "failed" value.</typeparam>
      /// <param name="func">The delegate that provides the new failure.</param>
      public Option<TValue, TNewFail> MapFailure<TNewFail>( Func<TFail, TNewFail> func ) =>
         IsFailure
         ? Option<TValue, TNewFail>.Failure( func( _failure ) )
         : IsNone
               ? Option<TValue, TNewFail>.None
               : Option<TValue, TNewFail>.Some( _value );

      /// <summary>
      /// Maps this instance by using its value (if any) as an argument for <paramref name="func"/>
      /// and returning the result as a "flat" <see cref="Option{TNewValue, TFail}"/> (instead of
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
      public Option<TNewValue, TFail> FlatMap<TNewValue>( Func<TValue, Option<TNewValue, TFail>> func ) =>
         IsSome
            ? func( _value )
            : IsNone
               ? Option<TNewValue, TFail>.None
               : Option<TNewValue, TFail>.Failure( _failure );

      /// <summary>
      /// <see cref="Option{TValue, TFail}"/>-compatible equality-check for values.
      /// </summary>
      /// <param name="value">Value to check for equality.</param>
      /// <returns>
      /// <see langword="true"/> iff <see cref="IsSome"/> == <see langword="true"/><c>and</c> the
      /// <see cref="object.Equals(object)"/> override of this instance's value returns <see
      /// langword="true"/> for <paramref name="value"/>, otherwise <see langword="false"/>.
      /// </returns>
      public bool Is( TValue value ) => IsSome && _value.Equals( value );


      /// <summary>
      /// <see cref="Option{TValue, TFail}"/>-compatible inequality-check for values.
      /// </summary>
      /// <param name="value">Value to check for inequality.</param>
      /// <returns>
      /// <see langword="true"/> iff <see cref="IsSome"/> == <see langword="true"/><c>and</c> the
      /// <see cref="object.Equals(object)"/> override of this instance's value returns <see
      /// langword="false"/> for <paramref name="value"/>, otherwise <see langword="false"/>.
      /// </returns>
      public bool IsNot( TValue value ) => IsSome && !_value.Equals( value );

      /// <summary>
      /// Monadic predicate check for values.
      /// </summary>
      /// <param name="predicate">The delegate that checks the predicate.</param>
      /// <returns>
      /// <see langword="true"/> iff <see cref="IsSome"/> == <see
      /// langword="true"/><c>and</c><paramref name="predicate"/> returns <see langword="true"/> for
      /// this instance's value, otherwise <see langword="false"/>.
      /// </returns>
      public bool Holds( Func<TValue, bool> predicate ) => IsSome && predicate( _value );

      #endregion Mapping

      #region Accessing Value

      private TValue ThrowNotSome()
      {
         // The return-type is just a hack, this method NEVER returns anything.
         if( IsFailure )
         {
            throw new OptionFailureException( typeValue: typeof( TValue ), typeFailure: typeof( TFail ) );
         }
         else
         {
            throw new OptionNoneException( typeValue: typeof( TValue ), typeFailure: typeof( TFail ) );
         }
      }

      private TValue ThrowNotSome( string customMessage )
      {
         // The return-type is just a hack, this method NEVER returns anything.
         #pragma warning disable CS0618 // Type or member is obsolete
         if( IsFailure )
         {
            throw new OptionFailureException( customMessage: customMessage );
         }
         else
         {
            throw new OptionNoneException( customMessage: customMessage );
         }
         #pragma warning restore CS0618
      }

      /// <summary>
      /// Returns this instance's value if <see cref="IsSome"/> == <see langword="true"/>, otherwise
      /// <paramref name="otherwise"/>.
      /// </summary>
      /// <param name="otherwise">
      /// The desired value if this instance has no value (with or without failure).
      /// </param>
      public TValue Or( TValue otherwise ) => IsSome ? _value : otherwise;

      /// <summary>
      /// Returns this instance's value if <see cref="IsSome"/> == <see langword="true"/>, otherwise
      /// throws an <see cref="OptionNoneException"/> if <see cref="IsNone"/> == <see
      /// langword="true"/> or an <see cref="OptionFailureException"/> if <see cref="IsFailure"/> ==
      /// <see langword="true"/>.
      /// </summary>
      /// <exception cref="OptionNoneException"/>
      /// <exception cref="OptionFailureException"/>
      public TValue OrThrow() => IsSome ? _value : ThrowNotSome();

      /// <summary>
      /// Returns this instance's value if <see cref="IsSome"/> == <see langword="true"/>, otherwise
      /// throws an <see cref="OptionNoneException"/> if <see cref="IsNone"/> == <see
      /// langword="true"/> or an <see cref="OptionFailureException"/> if <see cref="IsFailure"/> ==
      /// <see langword="true"/>, both with the message <paramref name="msg"/>.
      /// </summary>
      /// <exception cref="OptionNoneException"/>
      /// <exception cref="OptionFailureException"/>
      public TValue OrThrow( string msg ) => IsSome ? _value : ThrowNotSome( msg );

      /// <summary>
      /// Returns this instance's value if <see cref="IsSome"/> == <see langword="true"/>, otherwise
      /// throws an <see cref="OptionNoneException"/> if <see cref="IsNone"/> == <see
      /// langword="true"/> or an <see cref="OptionFailureException"/> if <see cref="IsFailure"/> ==
      /// <see langword="true"/>, both with the formatted message <paramref name="msg"/> using the
      /// message arguments <paramref name="args"/>.
      /// </summary>
      /// <exception cref="OptionNoneException"/>
      /// <exception cref="OptionFailureException"/>
      public TValue OrThrow( string msg, params string[] args ) =>
         IsSome ? _value : ThrowNotSome( string.Format( msg, args ) );

      /// <summary>
      /// Returns this instance's value if <see cref="IsSome"/> == <see langword="true"/>, otherwise
      /// throws the exception <paramref name="e"/>.
      /// </summary>
      /// <param name="e">
      /// The desired exception if this instance has no value (with or without failure).
      /// </param>
      /// <exception cref="Exception"/>
      public TValue OrThrow( Exception e ) => IsSome ? _value : throw e;

      /// <summary>
      /// Writes this instance's value into the <see langword="out"/> parameter <paramref
      /// name="value"/> and returns <see langword="true"/> if <see cref="IsSome"/> == <see
      /// langword="true"/>, otherwise <paramref name="value"/> will be set to the <see
      /// langword="default"/> value of <typeparamref name="TValue"/> and the method returns <see langword="false"/>.
      /// </summary>
      /// <param name="value"><see langword="out"/> parameter for this instance's value.</param>
      /// <remarks>Not particularly functional, but a concession to typical .NET methods.</remarks>
      public bool TryGetValue( out TValue value )
      {
         value = IsSome ? _value : default;
         return IsSome;
      }

      #endregion Accessing Value

      #region Accessing Failure

      /// <summary>
      /// Returns this instance's "failed" value if <see cref="IsFailure"/> == <see
      /// langword="true"/>, otherwise throws a <see cref="OptionNoFailureException"/>.
      /// </summary>
      /// <exception cref="OptionNoFailureException"/>
      public TFail FailureOrThrow() =>
         IsFailure ? _failure : throw new OptionNoFailureException( typeof( TValue ), typeof( TFail ) );

      /// <summary>
      /// Writes this instance's "failed" value into the <see langword="out"/> parameter <paramref
      /// name="failure"/> and returns <see langword="true"/> if <see cref="IsFailure"/> == <see
      /// langword="true"/>, otherwise <paramref name="failure"/> will be set to the <see
      /// langword="default"/> value of <typeparamref name="TFail"/> and the method returns <see langword="false"/>.
      /// </summary>
      /// <param name="failure"><see langword="out"/> parameter for this instance's "failed" value.</param>
      /// <remarks>Not particularly functional, but a concession to typical .NET methods.</remarks>
      public bool TryGetFailure( out TFail failure )
      {
         failure = IsFailure ? _failure : default;
         return IsFailure;
      }

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
      public Maybe<TValue> ToMaybe() => IsSome ? Maybe<TValue>.Some( _value ) : Maybe<TValue>.None;

      /// <summary>
      /// Converts this instance into an appropriate <see cref="Failable{TValue, TFail}"/>.
      /// </summary>
      /// <param name="failureOnNone">
      /// An object representing a "failure", used in case <see cref="IsNone"/> == <see langword="true"/>
      /// </param>
      /// <returns>
      /// <see cref="Failable{TValue, TFail}.Success(TValue)"/> if <see cref="IsSome"/> == <see
      /// langword="true"/> for this instance, otherwise <see cref="Failable{TValue,
      /// TFail}.Failure(TFail)"/> containing <paramref name="failureOnNone"/> if <see cref="IsNone"/>
      /// == <see langword="true"/> or this instance's "failed" value if <see cref="IsFailure"/> ==
      /// <see langword="true"/>.
      /// </returns>
      public Failable<TValue, TFail> ToFailable( TFail failureOnNone ) =>
         IsSome
            ? Failable<TValue, TFail>.Success( _value )
            : Failable<TValue, TFail>.Failure( IsFailure ? _failure : failureOnNone );

      /// <summary>
      /// Converts this instance into an appropriate <see cref="Failable{TValue, TFail}"/>.
      /// </summary>
      /// <param name="funcForNone">
      /// A function producing a "failure", used in case <see cref="IsNone"/> == <see langword="true"/>
      /// </param>
      /// <returns>
      /// <see cref="Failable{TValue, TFail}.Success(TValue)"/> if <see cref="IsSome"/> == <see
      /// langword="true"/> for this instance, otherwise <see cref="Failable{TValue,
      /// TFail}.Failure(TFail)"/> containing the result of <paramref name="funcForNone"/> if
      /// <see cref="IsNone"/> == <see langword="true"/> or this instance's "failed" value if <see cref="IsFailure"/> ==
      /// <see langword="true"/>.
      /// </returns>
      public Failable<TValue, TFail> ToFailable( Func<TFail> funcForNone ) =>
         IsSome
            ? Failable<TValue, TFail>.Success( _value )
            : Failable<TValue, TFail>.Failure( IsFailure ? _failure : funcForNone() );

      /// <summary>
      /// <para>Converts this instance into an appropriate <see cref="EFailable{TValue}"/>.</para>
      /// <para>Note that any "failed" value this instance might have is lost in the conversion
      ///       and the exception given via <paramref name="exc"/> is used instead.</para>
      /// </summary>
      /// <param name="exc">
      /// An object representing a "failure", used in case <see cref="IsSome"/> == <see langword="false"/>
      /// </param>
      /// <returns><see cref="EFailable{TValue}.Success(TValue)"/> if <see cref="IsSome"/> == <see langword="true"/> for
      /// this instance and <see cref="EFailable{TValue}.Failure(Exception)"/> containing <paramref name="exc"/> otherwise.</returns>
      public EFailable<TValue> ToEFailable( Exception exc ) => IsSome ? EFailable<TValue>.Success( _value ) : EFailable<TValue>.Failure( exc );

      /// <summary>
      /// Converts this instance into an appropriate <see cref="EFailable{TValue}"/>.
      /// </summary>
      /// <param name="funcForNone">
      /// A function producing a "failure", used in case <see cref="IsNone"/> == <see langword="true"/>
      /// </param>
      /// <param name="funcForFailure">
      /// A function producing a "failure", used in case <see cref="IsFailure"/> == <see langword="true"/> and
      /// called with the result of <see cref="FailureOrThrow"/>.
      /// </param>
      /// <returns><see cref="EFailable{TValue}.Success(TValue)"/> if <see cref="IsSome"/> == <see langword="true"/> for
      /// this instance and <see cref="EFailable{TValue}.Failure(Exception)"/> containing the result of
      /// <paramref name="funcForNone"/> or <paramref name="funcForFailure"/>, depending on the state of this instance. </returns>
      public EFailable<TValue> ToEFailable( Func<Exception> funcForNone, Func<TFail, Exception> funcForFailure )
      {
         if( IsSome )
            return EFailable<TValue>.Success( _value );
         else {
            Exception e = IsFailure ? funcForFailure( _failure ) : funcForNone();
            return EFailable<TValue>.Failure( e );
         }
      }

      /// <summary>
      /// <para>Converts this instance into an appropriate <see cref="Validation{TFail}"/>. </para>
      /// <para>NOTE: For <see cref="IsNone"/> == <see langword="true"/>, a <see cref="Validation{TFail}.Success"/> is returned by this method.
      /// If this behavior is not desired, use <see cref="ToValidation(TFail)"/> instead.</para>
      /// </summary>
      /// <returns>
      /// <see cref="Validation{TFail}.Success"/> if <see cref="IsFailure"/> == <see langword="false"/>, thus *LOSING* a
      /// "successful value" or "no value" state of this instance. If <see cref="IsFailure"/> == <see langword="true"/>, this method returns
      /// <see cref="Validation{TFail}.Failure(TFail)"/> with this instance's "failed" value instead.
      /// </returns>
      public Validation<TFail> ToValidation() =>
         IsFailure
            ? Validation<TFail>.Failure( _failure )
            : Validation<TFail>.Success;

      /// <summary>
      /// Converts this instance into an appropriate <see cref="Validation{TFail}"/>. />.
      /// </summary>
      /// <returns>
      /// <see cref="Validation{TFail}.Success"/> if <see cref="IsSome"/> == <see langword="true"/>, thus *LOSING* a
      /// "successful value" of this instance. If <see cref="IsNone"/> == <see langword="true"/>, <paramref name="errorOnNone"/>
      /// is used to construct and return <see cref="Validation{TFail}.Failure(TFail)"/> and, finally, if instead <see cref="IsFailure"/>
      /// == <see langword="true"/>, this method returns with this instance's "failed" value instead.
      /// </returns>
      public Validation<TFail> ToValidation( TFail errorOnNone ) =>
         IsSome
            ? Validation<TFail>.Success
            : Validation<TFail>.Failure(IsFailure? _failure : errorOnNone );

      /// <summary>
      /// Converts this instance into an appropriate <see cref="Validation{TFail}"/>. />.
      /// </summary>
      /// <param name="funcForNone">
      /// A function producing a "failure", used in case <see cref="IsNone"/> == <see langword="true"/>.
      /// </param>
      /// <returns>
      /// <see cref="Validation{TFail}.Success"/> if <see cref="IsSome"/> == <see langword="true"/>, thus *LOSING* a
      /// "successful value" of this instance. If <see cref="IsNone"/> == <see langword="true"/>, <paramref name="funcForNone"/>
      /// is used to construct and return <see cref="Validation{TFail}.Failure(TFail)"/> and, finally, if instead <see cref="IsFailure"/>
      /// == <see langword="true"/>, this method returns with this instance's "failed" value instead.
      /// </returns>
      public Validation<TFail> ToValidation( Func<TFail> funcForNone ) =>
         IsSome
            ? Validation<TFail>.Success
            : Validation<TFail>.Failure( IsFailure ? _failure : funcForNone() );

      /// <summary>
      /// <para>Converts this instance into an appropriate <see cref="EValidation"/>.</para>
      /// <para>Note that any value this instance might have, be it "some", "none", or "failure" is lost in
      ///       the conversion and the exception given via <paramref name="exc"/> is used instead.</para>
      /// </summary>
      /// <param name="exc">
      /// An object representing a "failure", used in case <see cref="IsSome"/> == <see langword="false"/>
      /// </param>
      /// <returns>
      /// <see cref="EValidation.Success"/> if <see cref="IsSome"/> == <see langword="true"/>, thus *LOSING* the
      /// "successful" value of this instance. If <see cref="IsSome"/> == <see langword="false"/>, this method returns
      /// <see cref="EValidation.Failure(Exception)"/> containing <paramref name="exc"/> otherwise.
      /// </returns>
      public EValidation ToEValidation( Exception exc ) => IsSome ? EValidation.Success : EValidation.Failure( exc );

      /// <summary>
      /// Converts this instance into an appropriate <see cref="EValidation"/>.
      /// </summary>
      /// <param name="funcForNone">
      /// A function producing a "failure", used in case <see cref="IsNone"/> == <see langword="true"/>
      /// </param>
      /// <param name="funcForFailure">
      /// A function producing a "failure", used in case <see cref="IsFailure"/> == <see langword="true"/> and
      /// called with the result of <see cref="FailureOrThrow"/>.
      /// </param>
      /// <returns>
      /// <see cref="EValidation.Success"/> if <see cref="IsSome"/> == <see langword="true"/>, thus *LOSING* the
      /// "successful" value of this instance. If <see cref="IsSome"/> == <see langword="false"/>, this method returns
      /// <see cref="EValidation.Failure(Exception)"/>  containing the result of <paramref name="funcForNone"/> or
      /// <paramref name="funcForFailure"/>, depending on the state of this instance. </returns>
      public EValidation ToEValidation( Func<Exception> funcForNone, Func<TFail, Exception> funcForFailure )
      {
         if( IsSome )
            return EValidation.Success;
         else
         {
            Exception e = IsFailure ? funcForFailure( _failure ) : funcForNone();
            return EValidation.Failure( e );
         }
      }

      /// <summary>
      /// <para>Converts this instance into an appropriate <see cref="EOption{TValue}"/>.</para>
      /// <para>Note that any "failed" value this instance might have is lost in the conversion
      ///       and the exception given via <paramref name="exc"/> is used instead.</para>
      /// </summary>
      /// <param name="exc">
      /// An object representing a "failure", used in case <see cref="IsSome"/> == <see langword="false"/>
      /// </param>
      /// <returns><see cref="EOption{TValue}.Some(TValue)"/> if <see cref="IsSome"/> == <see langword="true"/> for this instance,
      /// <see cref="EOption{TValue}.None"/> if <see cref="IsNone"/> == <see langword="true"/> for this instance and
      /// <see cref="EOption{TValue}.Failure(Exception)"/> containing <paramref name="exc"/>
      /// if <see cref="IsFailure"/> == <see langword="true"/> for this instance.</returns>
      public EOption<TValue> ToEOption( Exception exc ) =>
         IsSome ? EOption<TValue>.Some( _value ) : ( IsFailure ? EOption<TValue>.Failure( exc ) : EOption<TValue>.None );

      /// <summary>
      /// Converts this instance into an appropriate <see cref="EOption{TValue}"/>.
      /// </summary>
      /// <param name="func">
      /// A function producing a "failure", used in case <see cref="IsFailure"/> == <see langword="true"/> and
      /// called with the result of <see cref="FailureOrThrow"/>.
      /// </param>
      /// <returns><see cref="EOption{TValue}.Some(TValue)"/> if <see cref="IsSome"/> == <see langword="true"/> for this instance,
      /// <see cref="EOption{TValue}.None"/> if <see cref="IsNone"/> == <see langword="true"/> for this instance and
      /// <see cref="EOption{TValue}.Failure(Exception)"/> containing the result of <paramref name="func"/>
      /// if <see cref="IsFailure"/> == <see langword="true"/> for this instance.</returns>
      public EOption<TValue> ToEOption( Func<TFail,Exception> func ) =>
         IsSome ? EOption<TValue>.Some( _value ) : ( IsFailure ? EOption<TValue>.Failure( func( _failure ) ) : EOption<TValue>.None );

      /// <summary>
      /// <para>Converts this instance into an appropriate <see cref="EOption{TValue}"/> and performs an operation akin
      /// to <see cref="EOption{TValue}.Map{TNewValue}(Func{TValue, TNewValue})"/> while converting.</para>
      /// <para>Note that any "failed" value this instance might have is lost in the conversion
      /// and the exception given via <paramref name="exc"/> is used instead.</para>
      /// </summary>
      /// <param name="func">
      /// A function producting the new "value", used in case <see cref="IsSome"/> == <see langword="true"/>
      /// </param>
      /// <param name="exc">
      /// An object representing a "failure", used in case <see cref="IsFailure"/> == <see langword="true"/>
      /// </param>
      /// <returns><see cref="EOption{TValue}.Some(TValue)"/> if <see cref="IsSome"/> == <see langword="true"/> for
      /// this instance, <see cref="EOption{TValue}.None"/>  if <see cref="IsNone"/> == <see langword="true"/> and
      /// <see cref="EFailable{TValue}.Failure(Exception)"/> containing <paramref name="exc"/> otherwise.</returns>
      public EOption<TNewValue> ToEOptionWith<TNewValue>( Func<TValue, TNewValue> func, Exception exc ) =>
         IsSome ? EOption<TNewValue>.Some( func( _value ) ) : ( IsNone ? EOption<TNewValue>.None : EOption<TNewValue>.Failure( exc ) );

      /// <summary>
      /// Converts this instance into an appropriate <see cref="EOption{TValue}"/> using a function if necessary.
      /// While converting, this method performs an operation akin to <see cref="EOption{TValue}.Map{TNewValue}(Func{TValue, TNewValue})"/>.
      /// </summary>
      /// <param name="func">
      /// A function producting the new "value", used in case <see cref="IsSome"/> == <see langword="true"/>
      /// </param>
      /// <param name="excFunc">
      /// A function producting the new "failure", used in case <see cref="IsFailure"/> == <see langword="true"/>
      /// </param>
      /// <returns><see cref="EOption{TValue}.Some(TValue)"/> if <see cref="IsSome"/> == <see langword="true"/> for
      /// this instance, <see cref="EOption{TValue}.None"/>  if <see cref="IsNone"/> == <see langword="true"/> and
      /// <see cref="EFailable{TValue}.Failure(Exception)"/> containing the result of <paramref name="excFunc"/>
      /// called with the result of <see cref="FailureOrThrow"/> otherwise.</returns>
      public EOption<TNewValue> ToEOptionWith<TNewValue>( Func<TValue, TNewValue> func, Func<TFail, Exception> excFunc ) =>
         IsSome ? EOption<TNewValue>.Some( func( _value ) ) : ( IsNone ? EOption<TNewValue>.None : EOption<TNewValue>.Failure( excFunc( _failure ) ) );

      #endregion Converters

      #region Object

      /// <summary>
      /// Custom implementation of <see cref="object.ToString()"/>, wrapping the corresponding call
      /// to this instance's value, if any, be it a "failure" or not. This will return an internal
      /// representation of this instances state, suitable for debugging only.
      /// </summary>
      public override string ToString() =>
              IsSome ? $"[Option<{typeof( TValue ).Name}, {typeof( TFail ).Name}>.Some: {_value}]"
         : IsFailure ? $"[Option<{typeof( TValue ).Name}, {typeof( TFail ).Name}>.Failure: {_failure}]"
                     : $"[Option<{typeof( TValue ).Name}, {typeof( TFail ).Name}>.None]";

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
      /// <para>Respects type, a <see cref="Option{A, B}"/> and a <see cref="Option{C,D}"/> are never equal!</para>
      /// <para>Supports cross-class checks with <see cref="EOption{TValue}"/>, calling <see cref="EOption{TValue}.Equals(object)"/>-method.</para>
      /// </summary>
      public override bool Equals( object obj ) =>
         ( ( obj is Option<TValue, TFail> otherO )
            && ( IsSome == otherO.IsSome )
            && ( IsNone == otherO.IsNone )
            && ( IsNotSome || _value.Equals( otherO.OrThrow() ) )
            && ( IsNotFailure || _failure.Equals( otherO.FailureOrThrow() ) ) )
         || ( obj is EOption<TValue> otherE && otherE.Equals( this ) );

      /// <summary>
      /// <para>
      /// Custom implementation of <see cref="object.GetHashCode()"/>, wrapping a call to this
      /// instance's value, if any, be it a "failure" or not.
      /// </para>
      /// <para>
      /// Note that all <see cref="Option{TValue, TFail}"/>-objects, regardless of type
      /// <typeparamref name="TValue"/>, have the same hash code when there is no value and no
      /// failure, aka when <see cref="IsNone"/> == <see langword="true"/>.
      /// </para>
      /// </summary>
      public override int GetHashCode() =>
         IsSome
            ? _value.GetHashCode()
            : IsFailure ? _failure.GetHashCode() : int.MinValue;

      #endregion Object
   }
}