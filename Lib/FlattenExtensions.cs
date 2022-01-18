using System;
using Monadic.SwitchCase;

namespace SIEDA.Monadic
{
   /// <summary>
   /// Defines Flatten-Extensions for all monadic archetypes. Note that one can only flatten *IDENTICAL* left-hand side types.
   /// If you want to change such a type, use an appropriate mapping-function, as one usually does when working with Monadics.
   /// </summary>
   public static class FlattenExtensions
   {
      /// <summary> Flattens nested instances, returing a <see cref="Failable{TValue, TFail}"/>. </summary>
      public static Failable<TValue, TFail> Flatten<TValue, TFail>( this Failable<Failable<TValue, TFail>, TFail> f )
         => f.IsSuccess ? f.OrThrow() : Failable<TValue, TFail>.Failure( f.FailureOrThrow() );

      /// <summary> Flattens nested instances of different failure-type, returing a <see cref="Failable{TValue, TFail}"/>. </summary>
      public static Failable<TValue, TFail> Flatten<TValue, TFail, TFail2>( this Failable<Failable<TValue, TFail2>, TFail> f, Func<TFail2, TFail> failureConverter )
      {
         switch( f.Enum )
         {
            case FlbType.Success:
            {
               var inner = f.OrThrow();
               return inner.IsSuccess
                  ? Failable<TValue, TFail>.Success( inner.OrThrow() )
                  : Failable<TValue, TFail>.Failure( failureConverter( inner.FailureOrThrow() ) );
            }
            default: return Failable<TValue, TFail>.Failure( f.FailureOrThrow() );
         }
      }

      /// <summary> Flattens nested instances, returing a <see cref="EFailable{TValue}"/>. </summary>
      public static EFailable<TValue> Flatten<TValue>( this EFailable<EFailable<TValue>> f )
         => f.IsSuccess ? f.OrThrow() : EFailable<TValue>.Failure( f.FailureOrThrow() );

      /// <summary> Flattens nested instances, returing a <see cref="Option{TValue, TFail}"/>. </summary>
      public static Option<TValue, TFail> Flatten<TValue, TFail>( this Option<Option<TValue, TFail>, TFail> o )
      {
         switch( o.Enum )
         {
            case OptType.Failure: return Option<TValue, TFail>.Failure( o.FailureOrThrow() );
            case OptType.Some: return o.OrThrow();
            default: return Option<TValue, TFail>.None;
         }
      }

      /// <summary> Flattens nested instances of different failure-type, returing a <see cref="Option{TValue, TFail}"/>. </summary>
      public static Option<TValue, TFail> Flatten<TValue, TFail, TFail2>( this Option<Option<TValue, TFail2>, TFail> o, Func<TFail2, TFail> failureConverter )
      {
         switch( o.Enum )
         {
            case OptType.Some:
            {
               var inner = o.OrThrow();
               switch( inner.Enum )
               {
                  case OptType.Failure: return Option<TValue, TFail>.Failure( failureConverter( inner.FailureOrThrow() ) );
                  case OptType.Some: return Option<TValue, TFail>.Some( inner.OrThrow() );
                  default: return Option<TValue, TFail>.None;
               }
            }
            case OptType.Failure: return Option<TValue, TFail>.Failure( o.FailureOrThrow() );
            default: return Option<TValue, TFail>.None;
         }
      }

      /// <summary> Flattens nested instances, returing a <see cref="EOption{TValue}"/>. </summary>
      public static EOption<TValue> Flatten<TValue>( this EOption<EOption<TValue>> o )
      {
         switch( o.Enum )
         {
            case OptType.Failure: return EOption<TValue>.Failure( o.FailureOrThrow() );
            case OptType.Some: return o.OrThrow();
            default: return EOption<TValue>.None;
         }
      }

      /// <summary> Flattens nested instances, returing a <see cref="Validation{TFail}"/>. </summary>
      /// <remarks> An <see cref="EValidation"/> cannot contain another <see cref="EValidation"/>, hence this is the only method for that monadic archetype. </remarks>
      public static Validation<TFail> Flatten<TFail>( this Validation<Validation<TFail>> v ) => v.IsSuccess ? Validation<TFail>.Success : v.FailureOrThrow();

      /// <summary> Flattens nested instances, returing a <see cref="Maybe{TValue}"/>. </summary>
      public static Maybe<TValue> Flatten<TValue>( this Maybe<Maybe<TValue>> m ) => m.IsSome ? m.OrThrow() : Maybe<TValue>.None;
   }
}
