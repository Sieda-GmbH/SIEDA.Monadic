using System;

namespace SIEDA.Monadic
{
   /// <summary>
   /// An implementation of <see cref="IOption{TValue, TFail}"/>.
   /// </summary>
   public readonly struct Option<TValue, TFail> : IOption<TValue, TFail>
   {
      #region State

      private readonly TValue _value;

      private readonly TFail _failure;

      // Properties IsSome and IsFailure are also "State", and they are relevant for 'Equals(...)'.

      #endregion State

      #region Construction

      /// <summary>
      /// Empty instance, no value present, failure absent.
      /// </summary>
      public static readonly Option<TValue, TFail> None = new Option<TValue, TFail>(
         hasValue: false,
         hasError: false,
         default,
         default );

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

         return new Option<TValue, TFail>( hasValue: true, hasError: false, value, default );
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

         return new Option<TValue, TFail>( hasValue: true, hasError: false, nullableValue.Value, default );
      }

      /// <summary>
      /// <para>
      /// Creates and returns an <see cref="IOption{TValue, TFail}"/> with value <paramref
      /// name="value"/> if <paramref name="value"/> != <see langword="null"/>.
      /// </para>
      /// <para>Returns <see cref="None"/> if <paramref name="value"/> == <see langword="null"/>.</para>
      /// </summary>
      public static IOption<TValue, TFail> From( TValue value ) =>
         ReferenceEquals( value, null ) ? None : new Option<TValue, TFail>( hasValue: true, hasError: false, value, default );

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
      public static IOption<TValue, TFail> From<T>( T? nullableValue ) where T : struct, TValue =>
          nullableValue.HasValue ? new Option<TValue, TFail>( hasValue: true, hasError: false, nullableValue.Value, default ) : None;

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

         return new Option<TValue, TFail>( hasValue: false, hasError: true, default, failure );
      }

      private Option( bool hasValue, bool hasError, TValue value, TFail failure )
      {
         IsSome = hasValue;
         IsFailure = hasError;
         _value = value;
         _failure = failure;
      }

      #endregion Construction

      #region Properties

      ///<inheritdoc/>
      public bool IsSome { get; }

      ///<inheritdoc/>
      public bool IsNone => !IsSome && !IsFailure;

      ///<inheritdoc/>
      public bool IsFailure { get; }

      ///<inheritdoc/>
      public bool IsNotSome => !IsSome;

      ///<inheritdoc/>
      public bool IsNotFailure => !IsFailure;

      #endregion Properties

      #region Mapping


      ///<inheritdoc/>
      public IOption<TNewValue, TFail> Map<TNewValue>( Func<TValue, TNewValue> func ) =>
         IsSome
            ? Option<TNewValue, TFail>.From( func( _value ) )
            : IsNone
               ? Option<TNewValue, TFail>.None
               : Option<TNewValue, TFail>.Failure( _failure );


      ///<inheritdoc/>
      public IOption<TNewValue, TFail> FlatMap<TNewValue>( Func<TValue, IOption<TNewValue, TFail>> func ) =>
         IsSome
            ? func( _value )
            : IsNone
               ? Option<TNewValue, TFail>.None
               : Option<TNewValue, TFail>.Failure( _failure );


      ///<inheritdoc/>
      public bool Is( TValue value ) => IsSome && _value.Equals( value );


      ///<inheritdoc/>
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

      ///<inheritdoc/>
      public TValue Or( TValue otherwise ) => IsSome ? _value : otherwise;

      ///<inheritdoc/>
      public TValue OrThrow() => IsSome ? _value : ThrowNotSome();

      ///<inheritdoc/>
      public TValue OrThrow( string msg ) => IsSome ? _value : ThrowNotSome( msg );

      ///<inheritdoc/>
      public TValue OrThrow( string msg, params string[] args ) =>
         IsSome ? _value : ThrowNotSome( string.Format( msg, args ) );

      ///<inheritdoc/>
      public TValue OrThrow( Exception e ) => IsSome ? _value : throw e;

      ///<inheritdoc/>
      public bool TryGetValue( out TValue value )
      {
         value = IsSome ? _value : default;
         return IsSome;
      }

      #endregion Accessing Value

      #region Accessing Failure

      ///<inheritdoc/>
      public TFail FailureOrThrow() =>
         IsFailure ? _failure : throw new OptionNoFailureException( typeof( TValue ), typeof( TFail ) );

      ///<inheritdoc/>
      public bool TryGetFailure( out TFail failure )
      {
         failure = IsFailure ? _failure : default;
         return IsFailure;
      }

      #endregion Accessing Failure

      #region Converters

      ///<inheritdoc/>
      public Maybe<TValue> ToMaybe() => IsSome ? Maybe<TValue>.Some( _value ) : Maybe<TValue>.None;

      ///<inheritdoc/>
      public IFailable<TValue, TFail> ToFailable( TFail errorOnNone ) =>
         IsSome
            ? Failable<TValue, TFail>.Success( _value )
            : Failable<TValue, TFail>.Failure( IsFailure ? _failure : errorOnNone );

      ///<inheritdoc/>
      public IValidation<TFail> ToValidation() =>
         IsFailure
            ? Validation<TFail>.Failure( _failure )
            : Validation<TFail>.Success;

      ///<inheritdoc/>
      public IValidation<TFail> ToValidation( TFail errorOnNone ) =>
         IsSome
            ? Validation<TFail>.Success
            : Validation<TFail>.Failure(IsFailure? _failure : errorOnNone );

      #endregion Converters

      #region Object

      ///<inheritdoc/>
      public override string ToString() =>
              IsSome ? $"[Option<{typeof( TValue ).Name}, {typeof( TFail ).Name}>.Some: {_value}]"
         : IsFailure ? $"[Option<{typeof( TValue ).Name}, {typeof( TFail ).Name}>.Failure: {_failure}]"
                     : $"[Option<{typeof( TValue ).Name}, {typeof( TFail ).Name}>.None]";


      ///<inheritdoc/>
      public override bool Equals( object obj ) =>
         ( obj is IOption<TValue, TFail> other )
            && ( IsSome == other.IsSome )
            && ( IsNone == other.IsNone )
            && ( IsNotSome || _value.Equals( other.OrThrow() ) )
            && ( IsNotFailure || _failure.Equals( other.FailureOrThrow() ) );

      ///<inheritdoc/>
      public override int GetHashCode() =>
         IsSome
            ? _value.GetHashCode()
            : IsFailure ? _failure.GetHashCode() : int.MinValue;

      #endregion Object
   }
}