using System;
using System.Runtime.Serialization;

namespace SIEDA.Monadic
{
   /// <summary>Exception thrown exclusively by <see cref="Maybe{TValue}"/> on illegal "some value" instance construction.</summary>
   [Serializable]
   public class MaybeSomeConstructionException : AnyMaybeException
   {
      /// <summary>Allows <see cref="Maybe{TValue}"/> to construct and throw this exception.</summary>
      internal MaybeSomeConstructionException( Type typeValue )
         : base( $"Illegal 'some' given for Maybe<{typeValue.Name}>, the value must not be a 'null'-reference!" ) {}

      #region constructors for serialization
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public MaybeSomeConstructionException() : base() { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete( "DO NOT USE EXPLICITLY, exists only for serialization and similar usecases" )]
      public MaybeSomeConstructionException( string message ) : base( message ) { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public MaybeSomeConstructionException( string message, Exception innerException) : base(message, innerException) { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      protected MaybeSomeConstructionException( SerializationInfo info, StreamingContext context) : base(info, context) { }
      #endregion constructors for serialization
   }
}