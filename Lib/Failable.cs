using System;
using Monadic.SwitchCase;

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
   public readonly struct Failable<TValue, TFail>
   {
      #region State

      private readonly TValue _value;
      private readonly TFail _failure;
      private readonly FlbType _type;

      #endregion State

      #region Construction

      /// <summary>
      /// Creates a <see cref="Failable{TValue, TFail}"/> with a "successful" <paramref
      /// name="value"/>, which must not be <see langword="null"/>.
      /// </summary>
      /// <exception cref="FailableSuccessConstructionException">
      /// if <paramref name="value"/> == <see langword="null"/>.
      /// </exception>
      public static Failable<TValue, TFail> Success( TValue value )
      {
         if( ReferenceEquals( value, null ) )
         {
            throw new FailableSuccessConstructionException( typeValue: typeof( TValue ), typeFailure: typeof( TFail ) );
         }

         return new Failable<TValue, TFail>( FlbType.Success, value, default );
      }

      /// <summary>
      /// Creates a <see cref="Failable{TValue, TFail}"/> with the value of the <see
      /// cref="Nullable{T}"/><paramref name="nullableValue"/>.
      /// </summary>
      /// <exception cref="FailableSuccessConstructionException">
      /// if <paramref name="nullableValue"/> == <see langword="null"/> (has no value).
      /// </exception>
      public static Failable<TValue, TFail> Success<T>( T? nullableValue ) where T : struct, TValue
      {
         if( !nullableValue.HasValue )
         {
            throw new FailableSuccessConstructionException( typeValue: typeof( TValue ), typeFailure: typeof( TFail ) );
         }

         return new Failable<TValue, TFail>( FlbType.Success, nullableValue.Value, default );
      }

      /// <summary>
      /// Creates a <see cref="Failable{TValue, TFail}"/> with a <paramref name="failure"/>-value,
      /// which must not be <see langword="null"/>.
      /// </summary>
      /// <exception cref="FailableFailureConstructionException">
      /// if <paramref name="failure"/> == <see langword="null"/>.
      /// </exception>
      public static Failable<TValue, TFail> Failure( TFail failure )
      {
         if( ReferenceEquals( failure, null ) )
         {
            throw new FailableFailureConstructionException( typeValue: typeof( TValue ), typeFailure: typeof( TFail ) );
         }

         return new Failable<TValue, TFail>( FlbType.Failure, default, failure );
      }

      /// <summary>
      /// Creates a <see cref="Failable{TValue, TFail}"/> with the failure value of the <see
      /// cref="Nullable{T}"/> <paramref name="nullableFailure"/>.
      /// </summary>
      /// <exception cref="FailableFailureConstructionException">
      /// if <paramref name="nullableFailure"/> == <see langword="null"/> (has no value).
      /// </exception>
      public static Failable<TValue, TFail> Failure<T>( T? nullableFailure ) where T : struct, TFail
      {
         if( !nullableFailure.HasValue )
         {
            throw new FailableFailureConstructionException( typeValue: typeof( TValue ), typeFailure: typeof( TFail ) );
         }

         return new Failable<TValue, TFail>( FlbType.Failure, default, nullableFailure.Value );
      }

      private Failable( FlbType flbType, TValue value, TFail failure )
      {
         _type = flbType;
         _value = value;
         _failure = failure;
      }

      #endregion Construction

      #region Properties
      /// <summary> Returns an appropriate <see cref="FlbType"/> for this instance, useful in case you want to use a switch-case.</summary>
      public FlbType Enum { get => _type; }

      /// <summary>
      /// <see langword="true"/>, if this instance is a "success", aka has a value of type
      /// <typeparamref name="TValue"/>.
      /// </summary>
      public bool IsSuccess { get => _type == FlbType.Success; }

      /// <summary>
      /// <see langword="true"/>, if this instance is a "failure", aka has a value of type
      /// <typeparamref name="TFail"/>.
      /// </summary>
      public bool IsFailure { get => _type == FlbType.Failure; }

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
      public Failable<TNewValue, TFail> Map<TNewValue>( Func<TValue, TNewValue> func ) =>
         IsSuccess ? Failable<TNewValue, TFail>.Success( func( _value ) ) : Failable<TNewValue, TFail>.Failure( _failure );

      /// <summary>
      /// Maps this instance by using its "failed" value (if any) as an argument for <paramref
      /// name="func"/> and returning a <see cref="Failable{TValue, TNewFail}"/> created from the result.
      /// <para><paramref name="func"/> is only called if <see cref="IsFailure"/> == <see langword="true"/>.</para>
      /// <para>
      /// Returns <see cref="Failable{TValue, TNewFail}.Success(TValue)"/> if <see cref="IsSuccess"/>
      /// == <see langword="true"/> with this instance's "successful" value being unchanged.
      /// </para>
      /// </summary>
      /// <typeparam name="TNewFail">The type of the new "failed" value.</typeparam>
      /// <param name="func">The delegate that provides the new failure.</param>
      public Failable<TValue, TNewFail> MapFailure<TNewFail>( Func<TFail, TNewFail> func ) =>
         IsFailure ? Failable<TValue, TNewFail>.Failure( func( _failure ) ) : Failable<TValue, TNewFail>.Success( _value );

      /// <summary>
      /// Maps this instance by using its "successful" value (if any) as an argument for <paramref
      /// name="func"/> and returning the result as a "flat" <see cref="Failable{TNewValue,
      /// TFail}"/> (instead of a "Failable of a Failable").
      /// <para><paramref name="func"/> is only called if <see cref="IsSuccess"/> == <see langword="true"/>.</para>
      /// <para>
      /// Returns <see cref="Failable{TNewValue, TFail}.Failure(TFail)"/> if <see cref="IsFailure"/>
      /// == <see langword="true"/> or it is the result of <paramref name="func"/>.
      /// </para>
      /// </summary>
      /// <typeparam name="TNewValue">The type of the new "successful" value.</typeparam>
      /// <param name="func">The delegate that provides the new value which may fail.</param>
      public Failable<TNewValue, TFail> FlatMap<TNewValue>( Func<TValue, Failable<TNewValue, TFail>> func ) =>
         IsSuccess ? func( _value ) : Failable<TNewValue, TFail>.Failure( _failure );

      /// <summary>
      /// <para>Maps this instance by using its "successful" value (if any) as an argument for <paramref
      /// name="func"/> and returning the result as an <see cref="Option{TNewValue,TFail}"/>
      /// (instead of a "Failable of an Option").</para>
      /// <para><paramref name="func"/> is only called if <see cref="IsSuccess"/> == <see langword="true"/>.</para>
      /// <para>
      /// Returns <see cref="Option{TNewValue, TFail}.Failure(TFail)"/> if <see cref="IsFailure"/>
      /// == <see langword="true"/> or it is the result of <paramref name="func"/>.
      /// </para>
      /// <para>Note that this type of special flat-map only exists for <see cref="Failable{TValue, TFail}"/>
      /// and <see cref="EFailable{TValue}"/> as all other monadic types are imcompatible. For these types,
      /// simply perform a conversion first.</para>
      /// </summary>
      /// <typeparam name="TNewValue">The type of the new "successful" value.</typeparam>
      /// <param name="func">The delegate that provides the new value which may fail.</param>
      public Option<TNewValue, TFail> FlatMap<TNewValue>( Func<TValue, Option<TNewValue, TFail>> func ) =>
         IsSuccess ? func( _value ) : Option<TNewValue, TFail>.Failure( _failure );

      /// <summary>
      /// <see cref="Failable{TValue, TFail}"/>-compatible equality-check for "successful" values.
      /// </summary>
      /// <param name="value">"Successful" value to check for equality.</param>
      /// <returns>
      /// <see langword="true"/> iff <see cref="IsSuccess"/> == <see langword="true"/> <c>and</c> the
      /// <see cref="object.Equals(object)"/> override of this instance's value returns <see
      /// langword="true"/> for <paramref name="value"/>, otherwise <see langword="false"/>.
      /// </returns>
      public bool Is( TValue value ) => IsSuccess && _value.Equals( value );

      /// <summary>
      /// Monadic predicate check for "successful" values.
      /// </summary>
      /// <param name="predicate">The delegate that checks the predicate.</param>
      /// <returns>
      /// <see langword="true"/> iff <see cref="IsSuccess"/> == <see
      /// langword="true"/> <c>and</c><paramref name="predicate"/> returns <see langword="true"/> for
      /// this instance's "successful" value, otherwise <see langword="false"/>.
      /// </returns>
      public bool Holds( Func<TValue, bool> predicate ) => IsSuccess && predicate( _value );

      #endregion Mapping

      #region Accessing Success

      /// <summary>
      /// Returns this instance's "successful" value if <see cref="IsSuccess"/> == <see langword="true"/>,
      /// otherwise <paramref name="otherwise"/>.
      /// </summary>
      /// <param name="otherwise">The desired value if this instance represents a "failure".</param>
      public TValue Or( TValue otherwise ) => IsSuccess ? _value : otherwise;

      /// <summary>
      /// Returns this instance's "successful" value if <see cref="IsSuccess"/> == <see
      /// langword="true"/>, otherwise throws a <see cref="FailableFailureException"/>.
      /// </summary>
      /// <exception cref="FailableFailureException"/>
      public TValue OrThrow() => IsSuccess ? _value : throw new FailableFailureException( typeValue: typeof( TValue ), typeFailure: typeof( TFail ) );

      #pragma warning disable CS0618 // Type or member is obsolete
      /// <summary>
      /// Returns this instance's "successful" value if <see cref="IsSuccess"/> == <see
      /// langword="true"/>, otherwise throws a <see cref="FailableFailureException"/> with the
      /// message <paramref name="msg"/>.
      /// </summary>
      /// <exception cref="FailableFailureException"/>
      public TValue OrThrow( string msg ) => IsSuccess ? _value : throw new FailableFailureException( customMessage: msg );
      #pragma warning restore CS0618

      #pragma warning disable CS0618 // Type or member is obsolete
      /// <summary>
      /// Returns this instance's "successful" value if <see cref="IsSuccess"/> == <see
      /// langword="true"/>, otherwise throws a <see cref="FailableFailureException"/> with the
      /// formatted message <paramref name="msg"/> using the message arguments <paramref name="args"/>.
      /// </summary>
      /// <exception cref="FailableFailureException"/>
      public TValue OrThrow( string msg, params string[] args ) => IsSuccess ? _value : throw new FailableFailureException( customMessage: string.Format( msg, args ) );
      #pragma warning restore CS0618

      /// <summary>
      /// Returns this instance's "successful" value if <see cref="IsSuccess"/> == <see
      /// langword="true"/>, otherwise throws the exception <paramref name="e"/>.
      /// </summary>
      /// <param name="e">The desired exception if this instance represents a "failure".</param>
      /// <exception cref="Exception"/>
      public TValue OrThrow( Exception e ) => IsSuccess ? _value : throw e;

      /// <summary>
      /// <para>
      /// Returns this instance's "successful" value if <see cref="IsSuccess"/> == <see
      /// langword="true"/>, otherwise <paramref name="otherwiseFunc"/> applied to the "failed" value.
      /// </para>
      /// <para>
      /// This method is exclusive to <see cref="Failable{TValue, TFail}"/> since it only makes
      /// sense for this class. Its usage should be carefully considered.
      /// </para>
      /// </summary>
      /// <param name="otherwiseFunc">The desired value if this instance represents a "failure".</param>
      public TValue OrUse( Func<TFail, TValue> otherwiseFunc ) => IsSuccess ? _value : otherwiseFunc( _failure );

      /// <summary>
      /// Writes this instance's "successful" value into the <see langword="out"/> parameter
      /// <paramref name="value"/> and returns <see langword="true"/> if <see cref="IsSuccess"/> ==
      /// <see langword="true"/>, otherwise <paramref name="value"/> will be set to the <see
      /// langword="default"/> value of <typeparamref name="TValue"/> and the method returns <see langword="false"/>.
      /// </summary>
      /// <param name="value">
      /// <see langword="out"/> parameter for this instance's "successful" value.
      /// </param>
      public bool TryGetValue( out TValue value )
      {
         value = IsSuccess ? _value : default;
         return IsSuccess;
      }

      #endregion Accessing Success

      #region Accessing Failure

      /// <summary>
      /// Returns this instance's "failed" value if <see cref="IsFailure"/> == <see
      /// langword="true"/>, otherwise throws a <see cref="FailableNoFailureException"/>.
      /// </summary>
      /// <exception cref="FailableNoFailureException"/>
      public TFail FailureOrThrow() => IsFailure ? _failure : throw new FailableNoFailureException( typeof( TValue ), typeof( TFail ) );

      /// <summary>
      /// Writes this instance's "failed" value into the <see langword="out"/> parameter <paramref
      /// name="failure"/> and returns <see langword="true"/> if <see cref="IsFailure"/> == <see
      /// langword="true"/>, otherwise <paramref name="failure"/> will be set to the <see
      /// langword="default"/> value of <typeparamref name="TFail"/> and the method returns <see langword="false"/>.
      /// </summary>
      /// <param name="failure"><see langword="out"/> parameter for this instance's "failed" value.</param>
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
      /// <see cref="Maybe{TValue}.Some(TValue)"/> if <see cref="IsSuccess"/> == <see
      /// langword="true"/> for this instance and <see cref="Maybe{TValue}.None"/> otherwise, thus
      /// *LOSING* the "failed" value of this instance.
      /// </returns>
      public Maybe<TValue> ToMaybe() =>
         IsSuccess ? Maybe<TValue>.Some( _value ) : Maybe<TValue>.None;

      /// <summary>
      /// <para>Converts this instance into an appropriate <see cref="EFailable{TValue}"/>.</para>
      /// <para>Note that any "failed" value this instance might have is lost in the conversion
      ///       and the exception given via <paramref name="exc"/> is used instead.</para>
      /// </summary>
      /// <param name="exc">
      /// An object representing a "failure", used in case <see cref="IsSuccess"/> == <see langword="false"/>
      /// </param>
      /// <returns><see cref="EFailable{TValue}.Success(TValue)"/> if <see cref="IsSuccess"/> == <see langword="true"/> for
      /// this instance and <see cref="EFailable{TValue}.Failure(Exception)"/> containing <paramref name="exc"/> otherwise.</returns>
      public EFailable<TValue> ToEFailable( Exception exc ) => IsSuccess ? EFailable<TValue>.Success( _value ) : EFailable<TValue>.Failure( exc );

      /// <summary>
      /// Converts this instance into an appropriate <see cref="EFailable{TValue}"/> using a function if necessary.
      /// </summary>
      /// <param name="func">
      /// A function producting the new "failure", used in case <see cref="IsSuccess"/> == <see langword="false"/>
      /// </param>
      /// <returns><see cref="EFailable{TValue}.Success(TValue)"/> if <see cref="IsSuccess"/> == <see langword="true"/> for
      /// this instance and <see cref="EFailable{TValue}.Failure(Exception)"/> containing the result of <paramref name="func"/>
      /// called with the result of <see cref="FailureOrThrow"/> otherwise.</returns>
      public EFailable<TValue> ToEFailable( Func<TFail, Exception> func ) => IsSuccess ? EFailable<TValue>.Success( _value ) : EFailable<TValue>.Failure( func( _failure ) );

      /// <summary>
      /// <para>Converts this instance into an appropriate <see cref="EFailable{TValue}"/> and performs an operation akin
      /// to <see cref="EFailable{TValue}.Map{TNewValue}(Func{TValue, TNewValue})"/> while converting.</para>
      /// <para>Note that any "failed" value this instance might have is lost in the conversion
      /// and the exception given via <paramref name="exc"/> is used instead.</para>
      /// </summary>
      /// <param name="func">
      /// A function producting the new "value", used in case <see cref="IsSuccess"/> == <see langword="true"/>
      /// </param>
      /// <param name="exc">
      /// An object representing a "failure", used in case <see cref="IsSuccess"/> == <see langword="false"/>
      /// </param>
      /// <returns><see cref="EFailable{TValue}.Success(TValue)"/> if <see cref="IsSuccess"/> == <see langword="true"/> for
      /// this instance and <see cref="EFailable{TValue}.Failure(Exception)"/> containing <paramref name="exc"/> otherwise.</returns>
      public EFailable<TNewValue> ToEFailableWith<TNewValue>( Func<TValue, TNewValue> func, Exception exc ) =>
         IsSuccess ? EFailable<TNewValue>.Success( func( _value ) ) : EFailable<TNewValue>.Failure( exc );

      /// <summary>
      /// Converts this instance into an appropriate <see cref="EFailable{TValue}"/> using a function if necessary.
      /// While converting, this method performs an operation akin to <see cref="EFailable{TValue}.Map{TNewValue}(Func{TValue, TNewValue})"/>.
      /// </summary>
      /// <param name="func">
      /// A function producting the new "value", used in case <see cref="IsSuccess"/> == <see langword="true"/>
      /// </param>
      /// <param name="excFunc">
      /// A function producting the new "failure", used in case <see cref="IsSuccess"/> == <see langword="false"/>
      /// </param>
      /// <returns><see cref="EFailable{TValue}.Success(TValue)"/> if <see cref="IsSuccess"/> == <see langword="true"/> for
      /// this instance and <see cref="EFailable{TValue}.Failure(Exception)"/> containing the result of <paramref name="excFunc"/>
      /// called with the result of <see cref="FailureOrThrow"/> otherwise.</returns>
      public EFailable<TNewValue> ToEFailableWith<TNewValue>( Func<TValue, TNewValue> func, Func<TFail, Exception> excFunc ) =>
         IsSuccess ? EFailable<TNewValue>.Success( func( _value ) ) : EFailable<TNewValue>.Failure( excFunc( _failure ) );

      /// <summary>
      /// Converts this instance into an appropriate <see cref="Option{TValue, TFail}"/>.
      /// </summary>
      /// <returns>
      /// <see cref="Option{TValue, TFail}.Some(TValue)"/> if <see cref="IsSuccess"/> == <see
      /// langword="true"/> for this instance and <see cref="Option{TValue, TFail}.Failure(TFail)"/> otherwise.
      /// </returns>
      public Option<TValue, TFail> ToOption() => IsSuccess ? Option<TValue, TFail>.Some( _value ) : Option<TValue, TFail>.Failure( _failure );

      /// <summary>
      /// Converts this instance into an appropriate <see cref="Option{TValue, TFail}"/> and performs an operation akin
      /// to <see cref="Option{TValue, TFail}.Map{TNewValue}(Func{TValue, TNewValue})"/> while converting.
      /// </summary>
      /// <param name="func">
      /// A function producting the new "value", used in case <see cref="IsSuccess"/> == <see langword="true"/>
      /// </param>
      /// <returns>
      /// <see cref="Option{TNewValue, TFail}.Some(TNewValue)"/> if <see cref="IsSuccess"/> == <see
      /// langword="true"/> for this instance and <see cref="Option{TNewValue, TFail}.Failure(TFail)"/> otherwise.
      /// </returns>
      public Option<TNewValue, TFail> ToOptionWith<TNewValue>( Func<TValue, TNewValue> func ) =>
         IsSuccess ? Option<TNewValue, TFail>.Some( func( _value ) ) : Option<TNewValue, TFail>.Failure( _failure );

      /// <summary>
      /// <para>Converts this instance into an appropriate <see cref="EOption{TValue}"/>.</para>
      /// <para>Note that any "failed" value this instance might have is lost in the conversion
      ///       and the exception given via <paramref name="exc"/> is used instead.</para>
      /// </summary>
      /// <param name="exc">
      /// An object representing a "failure", used in case <see cref="IsSuccess"/> == <see langword="false"/>
      /// </param>
      /// <returns><see cref="EOption{TValue}.Some(TValue)"/> if <see cref="IsSuccess"/> == <see langword="true"/> for
      /// this instance and <see cref="EOption{TValue}.Failure(Exception)"/> containing <paramref name="exc"/> otherwise.</returns>
      public EOption<TValue> ToEOption( Exception exc ) => IsSuccess ? EOption<TValue>.Some( _value ) : EOption<TValue>.Failure( exc );

      /// <summary>
      /// Converts this instance into an appropriate <see cref="EOption{TValue}"/> using a function if necessary.
      /// </summary>
      /// <param name="func">
      /// A function producting the new "failure", used in case <see cref="IsSuccess"/> == <see langword="false"/>
      /// </param>
      /// <returns><see cref="EOption{TValue}.Some(TValue)"/> if <see cref="IsSuccess"/> == <see langword="true"/> for
      /// this instance and <see cref="EOption{TValue}.Failure(Exception)"/> containing the result of <paramref name="func"/>
      /// called with the result of <see cref="FailureOrThrow"/> otherwise.</returns>
      public EOption<TValue> ToEOption( Func<TFail, Exception> func ) => IsSuccess ? EOption<TValue>.Some( _value ) : EOption<TValue>.Failure( func( _failure ) );

      /// <summary>
      /// <para>Converts this instance into an appropriate <see cref="EOption{TValue}"/> and performs an operation akin
      /// to <see cref="EOption{TValue}.Map{TNewValue}(Func{TValue, TNewValue})"/> while converting.</para>
      /// <para>Note that any "failed" value this instance might have is lost in the conversion
      /// and the exception given via <paramref name="exc"/> is used instead.</para>
      /// </summary>
      /// <param name="func">
      /// A function producting the new "value", used in case <see cref="IsSuccess"/> == <see langword="true"/>
      /// </param>
      /// <param name="exc">
      /// An object representing a "failure", used in case <see cref="IsSuccess"/> == <see langword="false"/>
      /// </param>
      /// <returns><see cref="EOption{TValue}.Some(TValue)"/> if <see cref="IsSuccess"/> == <see langword="true"/> for
      /// this instance and <see cref="EOption{TValue}.Failure(Exception)"/> containing the result of <paramref name="func"/>
      /// called with the result of <see cref="FailureOrThrow"/> otherwise.</returns>
      public EOption<TNewValue> ToEOptionWith<TNewValue>( Func<TValue, TNewValue> func, Exception exc ) =>
         IsSuccess ? EOption<TNewValue>.Some( func( _value ) ) : EOption<TNewValue>.Failure( exc );

      /// <summary>
      /// Converts this instance into an appropriate <see cref="EOption{TValue}"/> using a function if necessary.
      /// While converting, this method performs an operation akin to <see cref="EOption{TValue}.Map{TNewValue}(Func{TValue, TNewValue})"/>.
      /// </summary>
      /// <param name="func">
      /// A function producting the new "value", used in case <see cref="IsSuccess"/> == <see langword="true"/>
      /// </param>
      /// <param name="excFunc">
      /// A function producting the new "failure", used in case <see cref="IsSuccess"/> == <see langword="false"/>
      /// </param>
      /// <returns><see cref="EOption{TValue}.Some(TValue)"/> if <see cref="IsSuccess"/> == <see langword="true"/> for
      /// this instance and <see cref="EOption{TValue}.Failure(Exception)"/> containing the result of <paramref name="excFunc"/>
      /// called with the result of <see cref="FailureOrThrow"/> otherwise.</returns>
      public EOption<TNewValue> ToEOptionWith<TNewValue>( Func<TValue, TNewValue> func, Func<TFail, Exception> excFunc ) =>
         IsSuccess ? EOption<TNewValue>.Some( func( _value ) ) : EOption<TNewValue>.Failure( excFunc( _failure ) );

      /// <summary>
      /// Converts this instance into an appropriate <see cref="Validation{TFail}"/>. />.
      /// </summary>
      /// <returns>
      /// <see cref="Validation{TFail}.Success"/> if <see cref="IsSuccess"/> == <see langword="true"/>, thus *LOSING* the
      /// "successful" value of this instance. If <see cref="IsSuccess"/> == <see langword="false"/>, this method returns
      /// <see cref="Validation{TFail}.Failure(TFail)"/> with this instance's "failed" value instead.
      /// </returns>
      public Validation<TFail> ToValidation() => IsSuccess ? Validation<TFail>.Success : Validation<TFail>.Failure( _failure );

      /// <summary>
      /// <para>Converts this instance into an appropriate <see cref="EValidation"/>.</para>
      /// <para>Note that any "failed" value this instance might have is lost in the conversion
      ///       and the exception given via <paramref name="exc"/> is used instead.</para>
      /// </summary>
      /// <param name="exc">
      /// An object representing a "failure", used in case <see cref="IsSuccess"/> == <see langword="false"/>
      /// </param>
      /// <returns>
      /// <see cref="EValidation.Success"/> if <see cref="IsSuccess"/> == <see langword="true"/>, thus *LOSING* the
      /// "successful" value of this instance. If <see cref="IsSuccess"/> == <see langword="false"/>, this method returns
      /// <see cref="EValidation.Failure(Exception)"/> containing <paramref name="exc"/> otherwise.
      /// </returns>
      public EValidation ToEValidation( Exception exc ) => IsSuccess ? EValidation.Success : EValidation.Failure( exc );

      /// <summary>
      /// Converts this instance into an appropriate <see cref="EValidation"/>.
      /// </summary>
      /// <param name="func">
      /// A function producting the new "failure", used in case <see cref="IsSuccess"/> == <see langword="false"/>
      /// </param>
      /// <returns>
      /// <see cref="EValidation.Success"/> if <see cref="IsSuccess"/> == <see langword="true"/>, thus *LOSING* the
      /// "successful" value of this instance. If <see cref="IsSuccess"/> == <see langword="false"/>, this method returns
      /// <see cref="EValidation.Failure(Exception)"/> containing the result of <paramref name="func"/>
      /// called with the result of <see cref="FailureOrThrow"/> otherwise.</returns>
      public EValidation ToEValidation( Func<TFail, Exception> func ) => IsSuccess ? EValidation.Success : EValidation.Failure( func( _failure ) );

      #endregion Converters

      #region Object

      /// <summary>
      /// Custom implementation of <see cref="object.ToString()"/>, wrapping the corresponding call
      /// to this instance's value, be it a "success" or a "failure". This will return an internal
      /// representation of this instances state, suitable for debugging only.
      /// </summary>
      public override string ToString() =>
         IsSuccess //neither 'value' nor 'failure' is ever null!
         ? $"[Failable<{typeof( TValue ).Name}, {typeof( TFail ).Name}>.Success: { _value }]"
         : $"[Failable<{typeof( TValue ).Name}, {typeof( TFail ).Name}>.Failure: { _failure }]";


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
      /// <para>Respects type, a <see cref="Failable{A, B}"/> and a <see cref="Failable{C,D}"/> are never equal!</para>
      /// <para>Supports cross-class checks with <see cref="EFailable{TValue}"/>, calling <see cref="EFailable{TValue}.Equals(object)"/>-method.</para>
      /// </summary>
      public override bool Equals( object obj ) =>
            ( ( obj is Failable<TValue, TFail> otherF )
            && ( IsSuccess == otherF.IsSuccess )
            && ( IsFailure || _value.Equals( otherF.OrThrow() ) )
            && ( IsSuccess || _failure.Equals( otherF.FailureOrThrow() ) ) )
         || ( obj is EFailable<TValue> otherE && otherE.Equals( this ) );

      /// <summary>
      /// Custom implementation of <see cref="object.GetHashCode()"/>, wrapping a call to this
      /// instance's value, be it a "success" or a "failure".
      /// </summary>
      public override int GetHashCode() => IsSuccess ? _value.GetHashCode() : _failure.GetHashCode();

      #endregion Object
   }
}