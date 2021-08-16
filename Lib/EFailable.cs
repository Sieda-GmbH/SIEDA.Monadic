using System;
using Monadic.SwitchCase;

namespace SIEDA.Monadic
{
   /// <summary>
   /// Interface for a object that represents the "result" of an operation that might have failed,
   /// containing either a value of type <typeparamref name="TValue"/> if the operation "was
   /// successful" or a different value of type <see cref="Exception"/> if the operation "was a
   /// failure". Neither of these are ever <see langword="null"/>.
   /// </summary>
   /// <typeparam name="TValue">The type of the "successful" value.</typeparam>
   public readonly struct EFailable<TValue>
   {
      #region State

      private readonly TValue _value;
      private readonly Exception _failure;
      private readonly FlbType _type;

      #endregion State

      #region Construction

      /// <summary>
      /// Creates an appropriate <see cref="EFailable{TValue}"/> wrapping the result of <paramref name="toExecute"/> or the exception that method threw.
      /// </summary>
      /// <param name="toExecute">Function executed and wrapped by this monadic instance.</param>
      /// <exception cref="FailableFromWrappedConstructionException">
      /// if <paramref name="toExecute"/> == <see langword="null"/>.
      /// </exception>
      public static EFailable<TValue> Wrapping( Func<TValue> toExecute )
      {
         if( ReferenceEquals( toExecute, null ) )
         {
            throw new FailableFromWrappedConstructionException( typeValue: typeof( TValue ) );
         }
         try
         {
            return Success( toExecute() );
         } catch( Exception e) {
            return Failure( e );
         }
      }

      /// <summary>
      /// Creates a <see cref="EFailable{TValue}"/> with a "successful" <paramref name="value"/>, which must not be <see langword="null"/>.
      /// </summary>
      /// <exception cref="FailableSuccessConstructionException">
      /// if <paramref name="value"/> == <see langword="null"/>.
      /// </exception>
      public static EFailable<TValue> Success( TValue value )
      {
         if( ReferenceEquals( value, null ) )
         {
            throw new FailableSuccessConstructionException( typeValue: typeof( TValue ), typeFailure: typeof( Exception ) );
         }

         return new EFailable<TValue>( FlbType.Success, value, default );
      }

      /// <summary>
      /// Creates a <see cref="EFailable{TValue}"/> with the value of the <see
      /// cref="Nullable{T}"/><paramref name="nullableValue"/>.
      /// </summary>
      /// <exception cref="FailableSuccessConstructionException">
      /// if <paramref name="nullableValue"/> == <see langword="null"/> (has no value).
      /// </exception>
      public static EFailable<TValue> Success<T>( T? nullableValue ) where T : struct, TValue
      {
         if( !nullableValue.HasValue )
         {
            throw new FailableSuccessConstructionException( typeValue: typeof( TValue ), typeFailure: typeof( Exception ) );
         }

         return new EFailable<TValue>( FlbType.Success, nullableValue.Value, default );
      }

      /// <summary>
      /// Creates a <see cref="EFailable{TValue}"/> with a <paramref name="failure"/>-value,
      /// which must not be <see langword="null"/>.
      /// </summary>
      /// <exception cref="FailableFailureConstructionException">
      /// if <paramref name="failure"/> == <see langword="null"/>.
      /// </exception>
      public static EFailable<TValue> Failure( Exception failure )
      {
         if( ReferenceEquals( failure, null ) )
         {
            throw new FailableFailureConstructionException( typeValue: typeof( TValue ), typeFailure: typeof( Exception ) );
         }

         return new EFailable<TValue>( FlbType.Failure, default, failure );
      }

      private EFailable( FlbType flbType, TValue value, Exception failure )
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
      /// <see langword="true"/>, if this instance is a "success", aka has a value of type <typeparamref name="TValue"/>.
      /// </summary>
      public bool IsSuccess { get => _type == FlbType.Success; }

      /// <summary>
      /// <see langword="true"/>, if this instance is a "failure", aka has a value of type <see cref="Exception"/>.
      /// </summary>
      public bool IsFailure { get => _type == FlbType.Failure; }

      #endregion Properties

      #region Mapping

      /// <summary>
      /// Maps this instance by using its "successful" value (if any) as an argument for <paramref
      /// name="func"/> and returning a <see cref="EFailable{TNewValue}"/> created from the result.
      /// <para><paramref name="func"/> is only called if <see cref="IsSuccess"/> == <see langword="true"/>.</para>
      /// <para>
      /// Returns <see cref="EFailable{TNewValue}.Failure(Exception)"/> if <see cref="IsFailure"/>
      /// == <see langword="true"/> with this instance's "failed" value being unchanged.
      /// </para>
      /// </summary>
      /// <typeparam name="TNewValue">The type of the new "successful" value.</typeparam>
      /// <param name="func">The delegate that provides the new value.</param>
      public EFailable<TNewValue> Map<TNewValue>( Func<TValue, TNewValue> func ) =>
         IsSuccess ? EFailable<TNewValue>.Success( func( _value ) ) : EFailable<TNewValue>.Failure( _failure );

      /// <summary>
      /// Maps this instance by using its "failed" value (if any) as an argument for <paramref name="func"/>
      /// and returning a <see cref="EFailable{TValue}"/> created from the result.
      /// <para><paramref name="func"/> is only called if <see cref="IsFailure"/> == <see langword="true"/>.</para>
      /// <para>Otherwise, this instance is left untouched.</para>
      /// </summary>
      /// <param name="func">The delegate that provides the new failure.</param>
      public EFailable<TValue> ExceptionMap( Func<Exception, Exception> func ) => IsFailure ? Failure( func( _failure ) ) : this;

      /// <summary>
      /// <para>Executes a side-effect, represented as the function <paramref name="func"/>, if and only if <see cref="IsFailure"/> == <see langword="true"/>.</para>
      /// <para>In any case, this instance is left untouched (but returned for easy functional chaining).</para>
      /// <para>*BEST PRACTICE:* Use this function for logging only!</para>
      /// </summary>
      /// <param name="func">The action to execute as side effect.</param>
      public EFailable<TValue> ExceptionSideEffect( Action<Exception> func )
      {
         if( IsFailure ) func( _failure );
         return this;
      }

      /// <summary>
      /// Maps this instance by using its "successful" value (if any) as an argument for <paramref
      /// name="func"/> and returning the result as a "flat" <see cref="EFailable{TNewValue}"/>
      /// (instead of a "Failable of a Failable").
      /// <para><paramref name="func"/> is only called if <see cref="IsSuccess"/> == <see langword="true"/>.</para>
      /// <para>
      /// Returns <see cref="EFailable{TNewValue}.Failure"/> if <see cref="IsFailure"/>
      /// == <see langword="true"/> or it is the result of <paramref name="func"/>.
      /// </para>
      /// </summary>
      /// <typeparam name="TNewValue">The type of the new "successful" value.</typeparam>
      /// <param name="func">The delegate that provides the new value which may fail.</param>
      public EFailable<TNewValue> FlatMap<TNewValue>( Func<TValue, EFailable<TNewValue>> func ) =>
         IsSuccess ? func( _value ) : EFailable<TNewValue>.Failure( _failure );

      /// <summary>
      /// <para>Maps this instance by using its "successful" value (if any) as an argument for <paramref
      /// name="func"/> and returning the result as an <see cref="EOption{TNewValue}"/>
      /// (instead of a "Failable of an Option").</para>
      /// <para><paramref name="func"/> is only called if <see cref="IsSuccess"/> == <see langword="true"/>.</para>
      /// <para>
      /// Returns <see cref="EOption{TNewValue}.Failure(Exception)"/> if <see cref="IsFailure"/>
      /// == <see langword="true"/> or it is the result of <paramref name="func"/>.
      /// </para>
      /// <para>Note that this type of special flat-map only exists for <see cref="Failable{TValue, TFail}"/>
      /// and <see cref="EFailable{TValue}"/> as all other monadic types are imcompatible. For these types,
      /// simply perform a conversion first.</para>
      /// </summary>
      /// <typeparam name="TNewValue">The type of the new "successful" value.</typeparam>
      /// <param name="func">The delegate that provides the new value which may fail.</param>
      public EOption<TNewValue> FlatMap<TNewValue>( Func<TValue, EOption<TNewValue>> func ) =>
         IsSuccess ? func( _value ) : EOption<TNewValue>.Failure( _failure );

      /// <summary>
      /// <para>Maps this instance by using its "successful" value (if any) as an argument for <paramref
      /// name="func"/> and returning the result as an <see cref="Option{TNewValue,Exception}"/>
      /// (instead of a "Failable of an Option").</para>
      /// <para><paramref name="func"/> is only called if <see cref="IsSuccess"/> == <see langword="true"/>.</para>
      /// <para>
      /// Returns <see cref="Option{TNewValue, Exception}.Failure(Exception)"/> if <see cref="IsFailure"/>
      /// == <see langword="true"/> or it is the result of <paramref name="func"/>.
      /// </para>
      /// <para>Note that this type of special flat-map only exists for <see cref="Failable{TValue, TFail}"/>
      /// and <see cref="EFailable{TValue}"/> as all other monadic types are imcompatible. For these types,
      /// simply perform a conversion first.</para>
      /// </summary>
      /// <typeparam name="TNewValue">The type of the new "successful" value.</typeparam>
      /// <param name="func">The delegate that provides the new value which may fail.</param>
      [Obsolete("Usage of this method is not recommended, an EOption is the type of monadic object you are looking for.")]
      public Option<TNewValue, Exception> FlatMap<TNewValue>( Func<TValue, Option<TNewValue,Exception>> func ) =>
         IsSuccess ? func( _value ) : Option<TNewValue, Exception>.Failure( _failure );

      /// <summary>
      /// <see cref="EFailable{TValue}"/>-compatible equality-check for "successful" values.
      /// </summary>
      /// <param name="value">"Successful" value to check for equality.</param>
      /// <returns>
      /// <see langword="true"/> iff <see cref="IsSuccess"/> == <see langword="true"/><c>and</c> the
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
      /// langword="true"/><c>and</c><paramref name="predicate"/> returns <see langword="true"/> for
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
      public TValue OrThrow() => IsSuccess ? _value : throw new FailableFailureException( typeValue: typeof( TValue ), typeFailure: typeof( Exception ) );

      /// <summary>
      /// Returns this instance's "successful" value if <see cref="IsSuccess"/> == <see
      /// langword="true"/>, otherwise throws the contained exception which would be returned
      /// by <see cref="FailureOrThrow"/> of this instance.
      /// </summary>
      public TValue OrThrowContained() => IsSuccess ? _value : throw _failure;

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
      public Exception FailureOrThrow() => IsFailure ? _failure : throw new FailableNoFailureException( typeof( TValue ), typeof( Exception ) );

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
      public Maybe<TValue> ToMaybe() => IsSuccess ? Maybe<TValue>.Some( _value ) : Maybe<TValue>.None;

      /// <summary>Converts this instance into an appropriate <see cref="Failable{TValue, Exception}"/></summary>
      /// <returns><see cref="Failable{TValue, Exception}.Success(TValue)"/> if <see cref="IsSuccess"/> == <see langword="true"/> for
      /// this instance and <see cref="Failable{TValue, Exception}.Failure(Exception)"/> otherwise.</returns>
      public Failable<TValue, Exception> ToFailable() => IsSuccess ? Failable<TValue, Exception>.Success( _value ) : Failable<TValue, Exception>.Failure( _failure );


      /// <summary>
      /// Converts this instance into an appropriate <see cref="Option{TValue, Exception}"/>.
      /// </summary>
      /// <returns>
      /// <see cref="Option{TValue, Exception}.Some(TValue)"/> if <see cref="IsSuccess"/> == <see
      /// langword="true"/> for this instance and <see cref="Option{TValue, Exception}.Failure(Exception)"/> otherwise.
      /// </returns>
      public Option<TValue, Exception> ToOption() => IsSuccess ? Option<TValue, Exception>.Some( _value ) : Option<TValue, Exception>.Failure( _failure );

      /// <summary>
      /// Converts this instance into an appropriate <see cref="EOption{TValue}"/>.
      /// </summary>
      /// <returns>
      /// <see cref="Option{TValue, Exception}.Some(TValue)"/> if <see cref="IsSuccess"/> == <see
      /// langword="true"/> for this instance and <see cref="EOption{TValue}.Failure(Exception)"/> otherwise.
      /// </returns>
      public EOption<TValue> ToEOption() => IsSuccess ? EOption<TValue>.Some( _value ) : EOption<TValue>.Failure( _failure );

      /// <summary>
      /// Converts this instance into an appropriate <see cref="Validation{Exception}"/>. />.
      /// </summary>
      /// <returns>
      /// <see cref="Validation{Exception}.Success"/> if <see cref="IsSuccess"/> == <see langword="true"/>, thus *LOSING* the
      /// "successful" value of this instance. If <see cref="IsSuccess"/> == <see langword="false"/>, this method returns
      /// <see cref="Validation{Exception}.Failure(Exception)"/> with this instance's "failed" value instead.
      /// </returns>
      public Validation<Exception> ToValidation() => IsSuccess ? Validation<Exception>.Success : Validation<Exception>.Failure( _failure );

      /// <summary>
      /// Converts this instance into an appropriate <see cref="EValidation"/>. />.
      /// </summary>
      /// <returns>
      /// <see cref="EValidation.Success"/> if <see cref="IsSuccess"/> == <see langword="true"/>, thus *LOSING* the
      /// "successful" value of this instance. If <see cref="IsSuccess"/> == <see langword="false"/>, this method returns
      /// <see cref="EValidation.Failure(Exception)"/> with this instance's "failed" value instead.
      /// </returns>
      public EValidation ToEValidation() => IsSuccess ? EValidation.Success : EValidation.Failure( _failure );

      #endregion Converters

      #region Object

      /// <summary>
      /// Custom implementation of <see cref="object.ToString()"/>, wrapping the corresponding call
      /// to this instance's value, be it a "success" or a "failure". This will return an internal
      /// representation of this instances state, suitable for debugging only.
      /// </summary>
      public override string ToString() =>
         IsSuccess //neither 'value' nor 'failure' is ever null!
         ? $"[EFailable<{typeof( TValue ).Name}>.Success: { _value }]"
         : $"[EFailable<{typeof( TValue ).Name}>.Failure: { _failure }]";


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
      /// - if <see cref="IsSuccess"/> being <see langword="false"/>: result is <see langword="true"/>
      /// if and only if both instances reference the same <see cref="Exception"/> internally
      /// </para>
      /// <para>Respects type, a <see cref="EFailable{A}"/> and a <see cref="EFailable{B}"/> are never equal!</para>
      /// <para>Supports cross-class checks with <see cref="Failable{TValue, Exception}"/> following the same semantics as above!</para>
      /// </summary>
      public override bool Equals( object obj ) =>
            ( ( obj is EFailable<TValue> otherEf )
            && ( IsSuccess == otherEf.IsSuccess )
            && ( IsFailure || _value.Equals( otherEf.OrThrow() ) )
            && ( IsSuccess || _failure == otherEf.FailureOrThrow() ) )
         || ( ( obj is Failable<TValue, Exception> otherF )
            && ( IsSuccess == otherF.IsSuccess )
            && ( IsFailure || _value.Equals( otherF.OrThrow() ) )
            && ( IsSuccess || _failure == otherF.FailureOrThrow() ) );

      /// <summary>
      /// Custom implementation of <see cref="object.GetHashCode()"/>, wrapping a call to this
      /// instance's value, be it a "success" or a "failure".
      /// </summary>
      public override int GetHashCode() => IsSuccess ? _value.GetHashCode() : _failure.GetHashCode();

      #endregion Object
   }
}