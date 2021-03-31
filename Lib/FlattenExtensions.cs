namespace SIEDA.Monadic
{
   /// <summary> Defines Flatten-Extensions for <see cref="Maybe{T}"/> and <see cref="Validation{TFail}"/>.</summary>
   public static class FlattenExtensions
   {
      /// <summary> Flattens nested instances, returing a <see cref="Validation{TFail}"/>. </summary>
      public static Validation<TFail> Flatten<TFail>( this Validation<Validation<TFail>> v ) => v.IsSuccess ? Validation<TFail>.Success : v.FailureOrThrow();

      /// <summary> Flattens nested instances, returing a <see cref="Maybe{TValue}"/>. </summary>
      public static Maybe<TValue> Flatten<TValue>( this Maybe<Maybe<TValue>> m ) => m.IsSome ? m.OrThrow() : Maybe<TValue>.None;
   }
}
